using System.Collections.Generic;

using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views
{
  public partial class LogModellingImportDialog : DataAnalysisImportDialog
  {
    public new LogModellingImportType ImportType
    {
      get
      {
        return new LogModellingImportType()
        {
          Shuffle = this.ShuffleDataCheckbox.Checked,
          TrainingPercentage = this.TrainingTestTrackBar.Value,
          CaseIDVariable = (string)this.CaseIDVariableComboBox.SelectedValue,
          TimestampVariable = (string)this.TimestampVariableComboBox.SelectedValue,
          ActivityVariable = (string)this.ActivityVariableComboBox.SelectedValue
        };
      }
    }

    public LogModellingImportDialog() => InitializeComponent();

    protected override void CheckAdditionalConstraints(TableFileParser csvParser)
    {
      base.CheckAdditionalConstraints(csvParser);
      SetPossibleLogVariables();
    }

    protected void SetPossibleLogVariables()
    {
      var dataset = this.PreviewDatasetMatrix.Content as Dataset;
      if(dataset != null)
      {
        this.CaseIDVariableComboBox.DataSource = new List<string>(dataset.VariableNames);
        this.TimestampVariableComboBox.DataSource = new List<string>(dataset.VariableNames);
        this.ActivityVariableComboBox.DataSource = new List<string>(dataset.VariableNames);
      }
    }
  }
}
