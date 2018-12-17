using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis
{
  [Item("LogModellingProblemData", "Represents an item containing all data defining a log modelling problem.")]
  [StorableClass]
  public class LogModellingProblemData : DataAnalysisProblemData, ILogModellingProblemData, IStorableContent
  {
    protected const string CaseIDVariableParameterName = "CaseIDVariable";
    protected const string TimestampVariableParameterName = "TimestampVariable";
    protected const string ActivityVariableParameterName = "ActivityVariable";
    public string Filename { get; set; }

    #region default data
    private static readonly IEnumerable<IList> sampleLog = new List<IList> {
      new List<string> {"1", "1", "1", "2", "2", "2", "2", "2", "3", "3"},
      new List<DateTime> {
        new DateTime(2018,10,04,10,30,00), new DateTime(2018,10,04,10,30,00), new DateTime(2018,10,04,10,50,00),
        new DateTime(2018,10,05,10,30,00), new DateTime(2018,10,05,10,40,00), new DateTime(2018,10,05,10,50,00),
        new DateTime(2018,10,05,11,00,00), new DateTime(2018,10,05,11,10,00), new DateTime(2018,10,03,10,30,00),
        new DateTime(2018,10,06,10,50,00)
      },
      new List<string> {"X", "Y", "Z", "X", "Y", "X", "Y", "Z", "X", "Z"}
    };

    private static readonly Dataset defaultDataset;
    private static readonly string defaultCaseIDVariable;
    private static readonly string defaultTimestampVariable;
    private static readonly string defaultActivityVariable;

    private static readonly LogModellingProblemData emptyProblemData;
    public static LogModellingProblemData EmptyProblemData => emptyProblemData;

    static LogModellingProblemData()
    {

      defaultDataset = new Dataset(new string[] { "CaseID", "Timestamp", "Activity" }, sampleLog)
      {
        Name = "Sample Log Dataset",
        Description = "Sample log consisting of three activities and cases"
      };
      defaultCaseIDVariable = "CaseID";
      defaultTimestampVariable = "Timestamp";
      defaultActivityVariable = "Activity";

      var problemData = new LogModellingProblemData();
      problemData.Parameters.Clear();
      problemData.Name = "Empty Log Modelling ProblemData";
      problemData.Description = "This ProblemData acts as place holder before the correct problem data is loaded.";
      problemData.isEmpty = true;

      problemData.Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", new Dataset()));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(CaseIDVariableParameterName, new ItemSet<StringValue>()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(TimestampVariableParameterName, new ItemSet<StringValue>()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(ActivityVariableParameterName, new ItemSet<StringValue>()));
      emptyProblemData = problemData;
    }
    #endregion

    public IConstrainedValueParameter<StringValue> CaseIDVariableParameter => (IConstrainedValueParameter<StringValue>)Parameters[CaseIDVariableParameterName];
    public string CaseIDVariable
    {
      get => CaseIDVariableParameter.Value.Value;
      set
      {
        if(value == null)
        {
          throw new ArgumentNullException("caseIDVariable", "The provided value for the caseIDVariable is null.");
        }

        if(value == CaseIDVariable)
        {
          return;
        }

        StringValue matchingParameterValue = CaseIDVariableParameter.ValidValues.FirstOrDefault(v => v.Value == value);
        CaseIDVariableParameter.Value = matchingParameterValue ?? throw new ArgumentException("The provided value is not valid as the caseIDVariable.", "caseIDVariable");
      }
    }
    public IEnumerable<string> CaseIDVariableValues => Dataset.GetStringValues(CaseIDVariable);
    public IEnumerable<string> CaseIDVariableTrainingValues => Dataset.GetStringValues(CaseIDVariable, TrainingIndices);
    public IEnumerable<string> CaseIDVariableTestValues => Dataset.GetStringValues(CaseIDVariable, TestIndices);

    public IConstrainedValueParameter<StringValue> TimestampVariableParameter => (IConstrainedValueParameter<StringValue>)Parameters[TimestampVariableParameterName];
    public string TimestampVariable
    {
      get => TimestampVariableParameter.Value.Value;
      set
      {
        if(value == null)
        {
          throw new ArgumentNullException("timestampVariable", "The provided value for the timestampVariable is null.");
        }

        if(value == TimestampVariable)
        {
          return;
        }

        StringValue matchingParameterValue = TimestampVariableParameter.ValidValues.FirstOrDefault(v => v.Value == value);
        if(matchingParameterValue == null)
        {
          throw new ArgumentException("The provided value is not valid as the timestampVariable.", "timestampVariable");
        }

        TimestampVariableParameter.Value = matchingParameterValue;
      }
    }
    public IEnumerable<DateTime> TimestampVariableValues => Dataset.GetDateTimeValues(TimestampVariable);
    public IEnumerable<DateTime> TimestampVariableTrainingValues => Dataset.GetDateTimeValues(TimestampVariable, TrainingIndices);
    public IEnumerable<DateTime> TimestampVariableTestValues => Dataset.GetDateTimeValues(TimestampVariable, TestIndices);

    public IConstrainedValueParameter<StringValue> ActivityVariableParameter => (IConstrainedValueParameter<StringValue>)Parameters[ActivityVariableParameterName];
    public string ActivityVariable
    {
      get => ActivityVariableParameter.Value.Value;
      set
      {
        if(value == null)
        {
          throw new ArgumentNullException("activityVariable", "The provided value for the activityVariable is null.");
        }

        if(value == ActivityVariable)
        {
          return;
        }

        StringValue matchingParameterValue = ActivityVariableParameter.ValidValues.FirstOrDefault(v => v.Value == value);
        if(matchingParameterValue == null)
        {
          throw new ArgumentException("The provided value is not valid as the activityVariable.", "activityVariable");
        }

        ActivityVariableParameter.Value = matchingParameterValue;
      }
    }
    public IEnumerable<string> ActivityVariableValues => Dataset.GetStringValues(ActivityVariable);
    public IEnumerable<string> ActivityVariableTrainingValues => Dataset.GetStringValues(ActivityVariable, TrainingIndices);
    public IEnumerable<string> ActivityVariableTestValues => Dataset.GetStringValues(ActivityVariable, TestIndices);


    [StorableConstructor]
    protected LogModellingProblemData(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() => RegisterParameterEvents();

    protected LogModellingProblemData(LogModellingProblemData original, Cloner cloner)
      : base(original, cloner) => RegisterParameterEvents();
    public override IDeepCloneable Clone(Cloner cloner)
    {
      if(this == emptyProblemData)
      {
        return emptyProblemData;
      }

      return new LogModellingProblemData(this, cloner);
    }

    public LogModellingProblemData()
      : this(defaultDataset, defaultCaseIDVariable, defaultTimestampVariable, defaultActivityVariable)
    {
    }
    public LogModellingProblemData(ILogModellingProblemData logModellingProblemData)
      : this(logModellingProblemData.Dataset, logModellingProblemData.CaseIDVariable, logModellingProblemData.TimestampVariable, logModellingProblemData.ActivityVariable)
    {
      TrainingPartition.Start = logModellingProblemData.TrainingPartition.Start;
      TrainingPartition.End = logModellingProblemData.TrainingPartition.End;
      TestPartition.Start = logModellingProblemData.TestPartition.Start;
      TestPartition.End = logModellingProblemData.TestPartition.End;
    }

    public LogModellingProblemData(IDataset dataset, string caseIDVariable, string timestampVariable, string activityVariable, IEnumerable<ITransformation> transformations = null)
      : base(dataset, Enumerable.Empty<string>(), transformations ?? Enumerable.Empty<ITransformation>())
    {
      IEnumerable<StringValue> values = dataset.VariableNames.Select(x => new StringValue(x));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(CaseIDVariableParameterName, new ItemSet<StringValue>(values), values.First()));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(TimestampVariableParameterName, new ItemSet<StringValue>(values), values.First()));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(ActivityVariableParameterName, new ItemSet<StringValue>(values), values.First()));
      RegisterParameterEvents();
    }

    private void RegisterParameterEvents()
    {
      CaseIDVariableParameter.ValueChanged += new EventHandler(CaseIDVariableParameter_ValueChanged);
      TimestampVariableParameter.ValueChanged += new EventHandler(TimestampVariableParameter_ValueChanged);
      ActivityVariableParameter.ValueChanged += new EventHandler(ActivityVariableParameter_ValueChanged);
    }
    private void CaseIDVariableParameter_ValueChanged(object sender, EventArgs e) => OnChanged();
    private void TimestampVariableParameter_ValueChanged(object sender, EventArgs e) => OnChanged();
    private void ActivityVariableParameter_ValueChanged(object sender, EventArgs e) => OnChanged();

    protected override bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage)
    {
      if(problemData == null)
      {
        throw new ArgumentNullException("problemData", "The provided problemData is null.");
      }

      var logModellingProblemData = problemData as ILogModellingProblemData;
      if(logModellingProblemData == null)
      {
        throw new ArgumentException("The problem data is not a log modelling problem data. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
      }

      bool returnValue = base.IsProblemDataCompatible(problemData, out errorMessage);
      return returnValue;
    }

    public override void AdjustProblemDataProperties(IDataAnalysisProblemData problemData)
    {
      if(problemData == null)
      {
        throw new ArgumentNullException("problemData", "The provided problemData is null.");
      }

      var logModellingProblemData = problemData as LogModellingProblemData;
      if(logModellingProblemData == null)
      {
        throw new ArgumentException("The problem data is not a log modelling problem data. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
      }

      base.AdjustProblemDataProperties(problemData);
    }
  }
}
