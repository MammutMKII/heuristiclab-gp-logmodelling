using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views
{
  [View("LogModelling InstanceProvider View")]
  [Content(typeof(LogModellingInstanceProvider), IsDefaultView = true)]
  public partial class LogModellingInstanceProviderView : DataAnalysisInstanceProviderView<ILogModellingProblemData>
  {

    public new LogModellingInstanceProvider Content
    {
      get => (LogModellingInstanceProvider)base.Content;
      set => base.Content = value;
    }

    public LogModellingInstanceProviderView() => InitializeComponent();

    protected override void importButton_Click(object sender, EventArgs e)
    {
      var importTypeDialog = new LogModellingImportDialog();
      if(importTypeDialog.ShowDialog() == DialogResult.OK)
      {
        ILogModellingProblemData instance = null;

        Task.Factory.StartNew(() =>
        {
          var mainForm = (MainForm.WindowsForms.MainForm)MainFormManager.MainForm;
          // lock active view and show progress bar
          var activeView = (IContentView)MainFormManager.MainForm.ActiveView;

          try
          {
            IProgress progress = mainForm.AddOperationProgressToContent(activeView.Content,
              "Loading problem instance.");

            Content.ProgressChanged +=
              (o, args) => { progress.ProgressValue = args.ProgressPercentage / 100.0; };

            instance = Content.ImportData(importTypeDialog.Path, importTypeDialog.ImportType,
              importTypeDialog.CSVFormat);
          }
          catch(Exception ex)
          {
            ErrorWhileParsing(ex);
            return;
          }
          finally
          {
            mainForm.RemoveOperationProgressFromContent(activeView.Content);
          }

          try
          {
            GenericConsumer.Load(instance);
          }
          catch(Exception ex)
          {
            ErrorWhileLoading(ex, importTypeDialog.Path);
          }
          finally
          {
            Invoke((Action)(() => this.instancesComboBox.SelectedIndex = -1));
          }
        });
      }
    }
  }
}
