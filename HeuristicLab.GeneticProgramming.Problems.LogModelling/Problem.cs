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
      return modelStats.ValidatingCases / (double)modelStats.TotalCases;
    }

    private LogModelStatistics Interpret(ISymbolicExpressionTree tree, IEnumerable<string> caseIDs, IEnumerable<DateTime> timestamps, IEnumerable<string> activities)
    {
      var modelStats = new LogModelStatistics();

      //reorder from column-major to row-major
      IEnumerable<(string caseID, DateTime timestamp, string activity)> rows = caseIDs.Zip(timestamps, Tuple.Create).Zip(activities, (tuple, activity) => (tuple.Item1, tuple.Item2, activity));
      var cases = rows.GroupBy(tuple => tuple.caseID, (key, g) => g.OrderBy(ev => ev.timestamp).Select(ev => ev.activity));

      modelStats.TotalCases = cases.Count();
      var validatedCases = cases.Select(c => ValidateCase(tree, c));
      modelStats.ValidatingCases = validatedCases.Count(Extension.Identity);

      return modelStats;
    }

    private bool ValidateCase(ISymbolicExpressionTree tree, IEnumerable<string> orderedActivities)
    {
      // skip programRoot and startSymbol
      var (valid, remainingActivities) = ValidateRec(tree.Root.GetSubtree(0).GetSubtree(0), orderedActivities);
      return valid && !remainingActivities.Any();
    }

    private (bool valid, IEnumerable<string> remainingActivities) ValidateRec(ISymbolicExpressionTreeNode node, IEnumerable<string> orderedActivities)
    {
      //validation fails if end of case is reached but more are expected (at this point every possible node leads to an expected activity)
      if(!orderedActivities.Any()) return (false, orderedActivities);

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
          return (currentActivity == node.Symbol.Name, orderedActivities.Skip(1));
        }
      }
    }

    private (bool valid, IEnumerable<string> remainingActivities) ValidateSeq(IEnumerable<ISymbolicExpressionTreeNode> subtreeSequence, IEnumerable<string> orderedActivities)
    {
      //allow previous subtrees to "remove" matched activities
      var remainingActivities = orderedActivities;
      bool valid = true;
      foreach(var n in subtreeSequence)
      {
        (valid, remainingActivities) = ValidateRec(n, remainingActivities);
        if(!valid) break;
      }
      return (valid, remainingActivities);
    }

    private (bool valid, IEnumerable<string> remainingActivities) ValidateLoop(IEnumerable<ISymbolicExpressionTreeNode> subtreeSequence, IEnumerable<string> orderedActivities)
    {
      var remainingActivities = orderedActivities;

      int LIMIT = 20;
      int i = 0;
      (bool intermediateValid, IEnumerable<string> intermediateRemainingActivities) result;
      while(i++ < LIMIT && (result = ValidateSeq(subtreeSequence, remainingActivities)).intermediateValid)
        remainingActivities = result.intermediateRemainingActivities;

      return (true, remainingActivities);
    }

    private (bool valid, IEnumerable<string> remainingActivities) ValidateOptional(ISymbolicExpressionTreeNode subtree, IEnumerable<string> orderedActivities)
    {
      var result = ValidateRec(subtree, orderedActivities);
      if(result.valid)
        return result;
      else
        return (true, orderedActivities);
    }

    private (bool valid, IEnumerable<string> remainingActivities) ValidateXor(IEnumerable<ISymbolicExpressionTreeNode> subtreeSet, IEnumerable<string> orderedActivities)
    {
      //do not allow subtrees to remove activities
      var validatingSubtrees = subtreeSet.Select(n => ValidateRec(n, orderedActivities)).Where(r => r.valid);
      if(validatingSubtrees.Count() == 1)
        return validatingSubtrees.Single();
      else
        return (false, orderedActivities);
    }

    private (bool valid, IEnumerable<string> remainingActivities) ValidateAnd(IEnumerable<ISymbolicExpressionTreeNode> subtreeBag, IEnumerable<string> orderedActivities)
    {
      //TODO: permute runtime complexity severely limits the feasible maximumArity of "and"
      //TODO: instead of reevaluating for all permutations, reuse evaluation of common subsequences, e.g. by evaluating at permutation generation
      var subtreeSequencePermutations = subtreeBag.Permute();
      //not sure if there is any better expression to evaluate only to the first success and only once
      var firstPositiveResult = subtreeSequencePermutations.Select(ssp => ValidateSeq(ssp, orderedActivities)).SkipWhile(result => !result.valid).Take(1);
      if(firstPositiveResult.Any())
        return firstPositiveResult.Single();
      else
        return (false, orderedActivities);
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
