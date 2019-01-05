using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis
{
  public class LogModellingCSVInstanceProvider : LogModellingInstanceProvider
  {
    public override string Name => "CSV File";
    public override string Description => "";
    public override Uri WebLink => new Uri("http://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/FAQ#DataAnalysisImportFileFormat");
    public override string ReferencePublication => "";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() => new List<IDataDescriptor>();
    public override ILogModellingProblemData LoadData(IDataDescriptor descriptor) => throw new NotImplementedException();

    public override bool CanImportData => true;
    public override ILogModellingProblemData ImportData(string path)
    {
      var csvFileParser = new TableFileParser();
      csvFileParser.Parse(path, csvFileParser.AreColumnNamesInFirstLine(path));

      var dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);

      string caseIDVar = dataset.VariableNames.First();
      string timestampVar = dataset.VariableNames.First();
      string activityVar = dataset.VariableNames.First();


      ILogModellingProblemData logData = new LogModellingProblemData(dataset, caseIDVar, timestampVar, activityVar);

      IEnumerable<int> trainingIndizes = Enumerable.Range(0, (csvFileParser.Rows * 2) / 3);
      int trainingPartEnd = trainingIndizes.Last();
      //TODO: when (if not removed) separating test and training, group by caseid
      logData.TrainingPartition.Start = trainingIndizes.First();
      logData.TrainingPartition.End = trainingPartEnd;
      logData.TestPartition.Start = trainingPartEnd;
      logData.TestPartition.End = csvFileParser.Rows;

      logData.Name = Path.GetFileName(path);

      return logData;
    }

    protected override ILogModellingProblemData ImportData(string path, LogModellingImportType type, TableFileParser csvFileParser)
    {
      List<IList> values = csvFileParser.Values;
      if(type.Shuffle)
      {
        values = Shuffle(values);
      }
      var dataset = new Dataset(csvFileParser.VariableNames, values);

      string caseIDVar = type.CaseIDVariable;
      string timestampVar = type.TimestampVariable;
      string activityVar = type.ActivityVariable;

      var logData = new LogModellingProblemData(dataset, caseIDVar, timestampVar, activityVar);

      int trainingPartEnd = (csvFileParser.Rows * type.TrainingPercentage) / 100;
      trainingPartEnd = trainingPartEnd > 0 ? trainingPartEnd : 1;
      logData.TrainingPartition.Start = 0;
      logData.TrainingPartition.End = trainingPartEnd;
      logData.TestPartition.Start = trainingPartEnd;
      logData.TestPartition.End = csvFileParser.Rows;

      logData.Name = Path.GetFileName(path);

      return logData;
    }
  }
}
