using System;
using System.Collections.Generic;
using System.Linq;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.LogModelling
{
  [Item("Log Modelling", "Modelling of event logs using process trees")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 900)]
  [StorableClass]
  public sealed class Problem : SymbolicExpressionTreeProblem, ILogModellingProblem, IProblemInstanceConsumer<ILogModellingProblemData>, IProblemInstanceExporter<ILogModellingProblemData>
  {

    #region parameter names
    private const string ProblemDataParameterName = "ProblemData";
    #endregion

    #region Parameter Properties
    IParameter IDataAnalysisProblem.ProblemDataParameter => ProblemDataParameter;

    public IValueParameter<ILogModellingProblemData> ProblemDataParameter => (IValueParameter<ILogModellingProblemData>)Parameters[ProblemDataParameterName];
    #endregion

    #region Properties
    public ILogModellingProblemData ProblemData
    {
      get => ProblemDataParameter.Value;
      set => ProblemDataParameter.Value = value;
    }
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData => ProblemData;
    #endregion

    public event EventHandler ProblemDataChanged;

    public override bool Maximization => true;

    #region item cloning and persistence
    // persistence
    [StorableConstructor]
    private Problem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() => RegisterEventHandlers();

    // cloning 
    private Problem(Problem original, Cloner cloner)
      : base(original, cloner) => RegisterEventHandlers();
    public override IDeepCloneable Clone(Cloner cloner) => new Problem(this, cloner);
    #endregion

    public Problem()
          : base()
    {
      Parameters.Add(new ValueParameter<ILogModellingProblemData>(ProblemDataParameterName, "The data for the log modelling problem", new LogModellingProblemData()));

      var g = new SimpleSymbolicExpressionGrammar(); // empty grammar is replaced in UpdateGrammar()
      base.Encoding = new SymbolicExpressionTreeEncoding(g, 100, 17);

      UpdateGrammar();
      RegisterEventHandlers();
    }


    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random)
    {
      ILogModellingProblemData problemData = ProblemData;
      int[] rows = ProblemData.TrainingIndices.ToArray();
      var caseIDs = problemData.Dataset.GetStringValues(problemData.CaseIDVariable, rows);
      var timestamps = problemData.Dataset.GetDateTimeValues(problemData.TimestampVariable, rows);
      var activities = problemData.Dataset.GetStringValues(problemData.ActivityVariable, rows);

      var modelStats = Interpret(tree, caseIDs, timestamps, activities);

      //TODO: improve evaluation by adding additional optimization objectives
      return modelStats.ValidatingCases / (double)modelStats.TotalCases + 0.01 / modelStats.NumberOfProcessedNodes;
    }

    private LogModelStatistics Interpret(ISymbolicExpressionTree tree, IEnumerable<string> caseIDs, IEnumerable<DateTime> timestamps, IEnumerable<string> activities)
    {
      var modelStats = new LogModelStatistics();

      //reorder from column-major to row-major
      IEnumerable<(string caseID, DateTime timestamp, string activity)> rows = caseIDs.Zip(timestamps, Tuple.Create).Zip(activities, (tuple, activity) => (tuple.Item1, tuple.Item2, activity));
      var cases = rows.GroupBy(tuple => tuple.caseID, (key, g) => g.OrderBy(ev => ev.timestamp).Select(ev => ev.activity));

      modelStats.TotalCases = cases.Count();
      var validatedCases = cases.Select(c => ValidateCase(tree, c));
      modelStats.ValidatingCases = validatedCases.Count(r => r.valid);
      modelStats.NumberOfProcessedNodes = validatedCases.Sum(r => r.processedNodes);

      return modelStats;
    }

    private (bool valid, int processedNodes) ValidateCase(ISymbolicExpressionTree tree, IEnumerable<string> orderedActivities)
    {
      // skip programRoot and startSymbol
      var (valid, remainingActivities, processedNodes) = ValidateRec(tree.Root.GetSubtree(0).GetSubtree(0), orderedActivities);
      return (valid && !remainingActivities.Any(), processedNodes);
    }

    private (bool valid, IEnumerable<string> remainingActivities, int processedNodes) ValidateRec(ISymbolicExpressionTreeNode node, IEnumerable<string> orderedActivities)
    {
      //validation fails if end of case is reached but more are expected (at this point every possible node leads to an expected activity)
      if(!orderedActivities.Any()) return (false, orderedActivities, 1);

      switch(node.Symbol.Name)
      {
        case "seq":
        {
          return ValidateSeq(node.Subtrees, orderedActivities);
        }
        case "xor":
        {
          return ValidateXor(node.Subtrees, orderedActivities);
        }
        case "and":
        {
          return ValidateAnd(node.Subtrees, orderedActivities);
        }
        case "loop":
        {
          return ValidateLoop(node.Subtrees, orderedActivities);
        }
        case "optional":
        {
          return ValidateOptional(node.GetSubtree(0), orderedActivities);
        }
        default:
        {
          var currentActivity = orderedActivities.First();
          return (currentActivity == node.Symbol.Name, orderedActivities.Skip(1), 1);
        }
      }
    }

    private (bool valid, IEnumerable<string> remainingActivities, int processedNodes) ValidateSeq(IEnumerable<ISymbolicExpressionTreeNode> subtreeSequence, IEnumerable<string> orderedActivities)
    {
      //allow previous subtrees to "remove" matched activities
      var remainingActivities = orderedActivities;
      int processedNodes = 1;
      bool valid = true;
      foreach(var n in subtreeSequence)
      {
        int nodes;
        (valid, remainingActivities, nodes) = ValidateRec(n, remainingActivities);
        processedNodes += nodes;
        if(!valid) break;
      }
      return (valid, remainingActivities, processedNodes);
    }

    private (bool valid, IEnumerable<string> remainingActivities, int processedNodes) ValidateLoop(IEnumerable<ISymbolicExpressionTreeNode> subtreeSequence, IEnumerable<string> orderedActivities)
    {
      var remainingActivities = orderedActivities;
      var processedNodes = 1;

      const int LIMIT = 20;
      int i = 0;
      (bool intermediateValid, IEnumerable<string> intermediateRemainingActivities, int nodes) result;
      while(i++ < LIMIT && (result = ValidateSeq(subtreeSequence, remainingActivities)).intermediateValid)
      {
        remainingActivities = result.intermediateRemainingActivities;
        processedNodes += result.nodes;
      }

      return (true, remainingActivities, processedNodes);
    }

    private (bool valid, IEnumerable<string> remainingActivities, int processedNodes) ValidateOptional(ISymbolicExpressionTreeNode subtree, IEnumerable<string> orderedActivities)
    {
      var result = ValidateRec(subtree, orderedActivities);
      if(result.valid)
      {
        result.processedNodes++;
        return result;
      }
      else
        return (true, orderedActivities, result.processedNodes+1);
    }

    private (bool valid, IEnumerable<string> remainingActivities, int processedNodes) ValidateXor(IEnumerable<ISymbolicExpressionTreeNode> subtreeSet, IEnumerable<string> orderedActivities)
    {
      //do not allow subtrees to remove activities
      var validationResults = subtreeSet.Select(n => ValidateRec(n, orderedActivities));
      var positiveResults = validationResults.Where(r => r.valid);
      int processedNodes = validationResults.Select(r => r.processedNodes).Sum() + 1;
      if(positiveResults.Count() == 1)
      {
        var subtree = positiveResults.Single();
        return (subtree.valid, subtree.remainingActivities, processedNodes);
      }
      else
        return (false, orderedActivities, processedNodes);
    }

    private (bool valid, IEnumerable<string> remainingActivities, int processedNodes) ValidateAnd(IEnumerable<ISymbolicExpressionTreeNode> subtreeBag, IEnumerable<string> orderedActivities)
    {
      //TODO: permute runtime complexity severely limits the feasible maximumArity of "and"
      //TODO: instead of reevaluating for all permutations, reuse evaluation of common subsequences, e.g. by evaluating at permutation generation
      var subtreeSequencePermutations = subtreeBag.Permute();

      var processedNodes = 1;
      foreach(var ssp in subtreeSequencePermutations)
      {
        var validationResult = ValidateSeq(ssp, orderedActivities);
        processedNodes += validationResult.processedNodes;
        if(validationResult.valid)
          return (true, validationResult.remainingActivities, processedNodes);
      }
      return (false, orderedActivities, processedNodes);
    }

    #region events
    private void RegisterEventHandlers()
    {
      ProblemDataParameter.ValueChanged += new EventHandler(ProblemDataParameter_ValueChanged);
      if(ProblemDataParameter.Value != null)
      {
        ProblemDataParameter.Value.Changed += new EventHandler(ProblemData_Changed);
      }
    }

    private void ProblemDataParameter_ValueChanged(object sender, EventArgs e)
    {
      ProblemDataParameter.Value.Changed += new EventHandler(ProblemData_Changed);
      OnProblemDataChanged();
      OnReset();
    }

    private void ProblemData_Changed(object sender, EventArgs e)
    {
      OnProblemDataChanged();
      OnReset();
    }

    private void OnProblemDataChanged()
    {
      UpdateGrammar();

      ProblemDataChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateGrammar()
    {
      //TODO: ensuring that xor receives a set could be beneficial
      // whenever ProblemData is changed we create a new grammar with the necessary symbols
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new[] { "seq", "xor" }, 2, 5);
      g.AddSymbol("and", 2, 2); //TODO: arity is currently limited by performance
      //loop validates at least one and then as many repetitions of the sequence as possible (greedy)
      g.AddSymbol("loop", 1, 5);
      //optional validates its child node
      g.AddSymbol("optional", 1, 1);
      //TODO: currently not able to overfit for relatively simple logs, the model is missing something or is too large

      var activities = ProblemData.ActivityVariableTrainingValues.Distinct();
      //TODO get possible activity names into HL, this doesn't work
      foreach(string variableName in activities)
      {
        g.AddTerminalSymbol(variableName);
      }

      Encoding.Grammar = g;
    }
    #endregion

    #region Import & Export
    public void Load(ILogModellingProblemData data)
    {
      Name = data.Name;
      Description = data.Description;
      ProblemData = data;
    }

    public ILogModellingProblemData Export() => ProblemData;
    #endregion
  }
}
