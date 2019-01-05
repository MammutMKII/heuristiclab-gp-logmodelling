#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views
{
  partial class LogModellingImportDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if(disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.TimestampVariableComboBox = new System.Windows.Forms.ComboBox();
      this.TimestampVariableLabel = new System.Windows.Forms.Label();
      this.ActivityVariableLabel = new System.Windows.Forms.Label();
      this.ActivityVariableComboBox = new System.Windows.Forms.ComboBox();
      this.CaseIDVariableLabel = new System.Windows.Forms.Label();
      this.CaseIDVariableComboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).BeginInit();
      this.CSVSettingsGroupBox.SuspendLayout();
      this.ProblemDataSettingsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // OkButton
      // 
      this.OkButton.Location = new System.Drawing.Point(303, 451);
      // 
      // TrainingTestTrackBar
      // 
      this.TrainingTestTrackBar.Location = new System.Drawing.Point(6, 73);
      // 
      // TestLabel
      // 
      this.TestLabel.Location = new System.Drawing.Point(303, 98);
      // 
      // TrainingLabel
      // 
      this.TrainingLabel.Location = new System.Drawing.Point(76, 98);
      // 
      // CancellationButton
      // 
      this.CancellationButton.Location = new System.Drawing.Point(384, 451);
      // 
      // ProblemDataSettingsGroupBox
      // 
      this.ProblemDataSettingsGroupBox.Controls.Add(this.CaseIDVariableLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.CaseIDVariableComboBox);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.ActivityVariableLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.ActivityVariableComboBox);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TimestampVariableLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TimestampVariableComboBox);
      this.ProblemDataSettingsGroupBox.Size = new System.Drawing.Size(447, 255);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TimestampVariableComboBox, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TimestampVariableLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ActivityVariableComboBox, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ActivityVariableLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.CaseIDVariableComboBox, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.CaseIDVariableLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.PreviewLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ShuffelInfoLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ShuffleDataCheckbox, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TrainingTestTrackBar, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TrainingLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.PreviewDatasetMatrix, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TestLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ErrorTextBox, 0);
      // 
      // ErrorTextBox
      // TODO: remove error text box; for this, make the importdialog independent of dataanalysis
      // 
      this.ErrorTextBox.Size = new System.Drawing.Size(435, 103);
      // 
      // PreviewDatasetMatrix
      // 
      this.PreviewDatasetMatrix.Location = new System.Drawing.Point(6, 138);
      // 
      // PreviewLabel
      // 
      this.PreviewLabel.Location = new System.Drawing.Point(9, 119);
      // 
      // SeparatorInfoLabel
      // 
      this.ToolTip.SetToolTip(this.SeparatorInfoLabel, "Select the separator used to separate columns in the csv file.");
      // 
      // DateTimeFormatInfoLabel
      // 
      this.ToolTip.SetToolTip(this.DateTimeFormatInfoLabel, "Select the date time format used in the csv file");
      // 
      // DecimalSeparatorInfoLabel
      // 
      this.ToolTip.SetToolTip(this.DecimalSeparatorInfoLabel, "Select the decimal separator used to for double values");
      // 
      // ShuffelInfoLabel
      // 
      this.ToolTip.SetToolTip(this.ShuffelInfoLabel, "Check, if the imported data should be shuffled");
      // 
      // TimestampVariableComboBox
      // 
      this.TimestampVariableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TimestampVariableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.TimestampVariableComboBox.FormattingEnabled = true;
      this.TimestampVariableComboBox.Location = new System.Drawing.Point(76, 41);
      this.TimestampVariableComboBox.Name = "TimestampVariableComboBox";
      this.TimestampVariableComboBox.Size = new System.Drawing.Size(141, 21);
      this.TimestampVariableComboBox.TabIndex = 10;
      // 
      // TimestampVariableLabel
      // 
      this.TimestampVariableLabel.AutoSize = true;
      this.TimestampVariableLabel.Location = new System.Drawing.Point(9, 44);
      this.TimestampVariableLabel.Name = "TimestampVariableLabel";
      this.TimestampVariableLabel.Size = new System.Drawing.Size(61, 13);
      this.TimestampVariableLabel.TabIndex = 20;
      this.TimestampVariableLabel.Text = "Timestamp:";
      // 
      // ActivityVariableLabel
      // 
      this.ActivityVariableLabel.AutoSize = true;
      this.ActivityVariableLabel.Location = new System.Drawing.Point(229, 44);
      this.ActivityVariableLabel.Name = "ActivityVariableLabel";
      this.ActivityVariableLabel.Size = new System.Drawing.Size(44, 13);
      this.ActivityVariableLabel.TabIndex = 22;
      this.ActivityVariableLabel.Text = "Activity:";
      // 
      // ActivityVariableComboBox
      // 
      this.ActivityVariableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ActivityVariableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ActivityVariableComboBox.FormattingEnabled = true;
      this.ActivityVariableComboBox.Location = new System.Drawing.Point(280, 41);
      this.ActivityVariableComboBox.Name = "ActivityVariableComboBox";
      this.ActivityVariableComboBox.Size = new System.Drawing.Size(157, 21);
      this.ActivityVariableComboBox.TabIndex = 21;
      // 
      // CaseIDVariableLabel
      // 
      this.CaseIDVariableLabel.AutoSize = true;
      this.CaseIDVariableLabel.Location = new System.Drawing.Point(229, 20);
      this.CaseIDVariableLabel.Name = "CaseIDVariableLabel";
      this.CaseIDVariableLabel.Size = new System.Drawing.Size(45, 13);
      this.CaseIDVariableLabel.TabIndex = 24;
      this.CaseIDVariableLabel.Text = "CaseID:";
      // 
      // CaseIDVariableComboBox
      // 
      this.CaseIDVariableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.CaseIDVariableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.CaseIDVariableComboBox.FormattingEnabled = true;
      this.CaseIDVariableComboBox.Location = new System.Drawing.Point(280, 17);
      this.CaseIDVariableComboBox.Name = "CaseIDVariableComboBox";
      this.CaseIDVariableComboBox.Size = new System.Drawing.Size(157, 21);
      this.CaseIDVariableComboBox.TabIndex = 23;
      // 
      // LogModellingImportDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(471, 486);
      this.Name = "LogModellingImportDialog";
      this.Text = "Log Modelling CSV Import";
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
      this.CSVSettingsGroupBox.ResumeLayout(false);
      this.CSVSettingsGroupBox.PerformLayout();
      this.ProblemDataSettingsGroupBox.ResumeLayout(false);
      this.ProblemDataSettingsGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.ComboBox TimestampVariableComboBox;
    protected System.Windows.Forms.Label TimestampVariableLabel;
    protected System.Windows.Forms.Label CaseIDVariableLabel;
    protected System.Windows.Forms.ComboBox CaseIDVariableComboBox;
    protected System.Windows.Forms.Label ActivityVariableLabel;
    protected System.Windows.Forms.ComboBox ActivityVariableComboBox;
  }
}