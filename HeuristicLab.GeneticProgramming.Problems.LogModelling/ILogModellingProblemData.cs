using System;
using System.Collections.Generic;

namespace HeuristicLab.Problems.DataAnalysis
{
  public interface ILogModellingProblemData : IDataAnalysisProblemData
  {
    string CaseIDVariable { get; set; }
    string TimestampVariable { get; set; }
    string ActivityVariable { get; set; }

    IEnumerable<string> CaseIDVariableValues { get; }
    IEnumerable<string> CaseIDVariableTrainingValues { get; }
    IEnumerable<string> CaseIDVariableTestValues { get; }

    IEnumerable<DateTime> TimestampVariableValues { get; }
    IEnumerable<DateTime> TimestampVariableTrainingValues { get; }
    IEnumerable<DateTime> TimestampVariableTestValues { get; }

    IEnumerable<string> ActivityVariableValues { get; }
    IEnumerable<string> ActivityVariableTrainingValues { get; }
    IEnumerable<string> ActivityVariableTestValues { get; }
  }
}

