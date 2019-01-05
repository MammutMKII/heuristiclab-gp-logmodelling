using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.LogModelling
{
  [Plugin("HeuristicLab.Problems.DataAnalysis.Symbolic.LogModelling", "Provides classes to represent and model event logs as process trees", "1.0.0")]
  [PluginFile("HeuristicLab.Problems.DataAnalysis.Symbolic.LogModelling-1.0.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Data.Views", "3.3")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
  [PluginDependency("HeuristicLab.Operators", "3.3")]
  [PluginDependency("HeuristicLab.Optimization", "3.3")]
  [PluginDependency("HeuristicLab.Optimization.Operators", "3.3")]
  [PluginDependency("HeuristicLab.Parameters", "3.3")]
  [PluginDependency("HeuristicLab.Persistence", "3.3")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]
  [PluginDependency("HeuristicLab.Problems.Instances", "3.3")]
  [PluginDependency("HeuristicLab.Problems.Instances.DataAnalysis", "3.3")]
  [PluginDependency("HeuristicLab.Problems.Instances.DataAnalysis.Views", "3.3")]
  [PluginDependency("HeuristicLab.Problems.Instances.Views", "3.3")]
  [PluginDependency("HeuristicLab.Random", "3.3")]
  public class Plugin : PluginBase
  {
  }
}