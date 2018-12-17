using System;
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
  [Item("Log Modelling", "TODO")]
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
      var caseID = problemData.Dataset.GetStringValues(problemData.CaseIDVariable, rows);
      var timestamp = problemData.Dataset.GetDateTimeValues(problemData.TimestampVariable, rows);
      var activity = problemData.Dataset.GetStringValues(problemData.ActivityVariable, rows);

      //TODO interpretation & evalutation
      //var predicted = Interpret(tree, problemData.Dataset, rows);

      //OnlineCalculatorError errorState;
      //var r = OnlinePearsonsRCalculator.Calculate(target, predicted, out errorState);
      //if (errorState != OnlineCalculatorError.None) r = 0;
      //return r * r;
      return random.NextDouble();
    }

    //private IEnumerable<double> Interpret(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows)
    //{
    //    // skip programRoot and startSymbol
    //    return InterpretRec(tree.Root.GetSubtree(0).GetSubtree(0), dataset, rows);
    //}

    //private IEnumerable<double> InterpretRec(ISymbolicExpressionTreeNode node, IDataset dataset, IEnumerable<int> rows)
    //{
    //    Func<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode, Func<double, double, double>, IEnumerable<double>> binaryEval =
    //      (left, right, f) => InterpretRec(left, dataset, rows).Zip(InterpretRec(right, dataset, rows), f);

    //    switch (node.Symbol.Name)
    //    {
    //        case "+": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x + y);
    //        case "*": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x * y);
    //        case "-": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x - y);
    //        case "%": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => y.IsAlmost(0.0) ? 0.0 : x / y); // protected division
    //        default:
    //            {
    //                double erc;
    //                if (double.TryParse(node.Symbol.Name, out erc))
    //                {
    //                    return rows.Select(_ => erc);
    //                }
    //                else
    //                {
    //                    // assume that this is a variable name
    //                    return dataset.GetDoubleValues(node.Symbol.Name, rows);
    //                }
    //            }
    //    }
    //}


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

    private void ProblemData_Changed(object sender, EventArgs e) => OnReset();

    private void OnProblemDataChanged()
    {
      UpdateGrammar();

      EventHandler handler = ProblemDataChanged;
      if(handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void UpdateGrammar()
    {
      // whenever ProblemData is changed we create a new grammar with the necessary symbols
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new[] { "+", "*", "%", "-" }, 2, 2); // % is protected division 1/0 := 0

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
