using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for MainPanel.xaml
    /// </summary>
    public partial class MainPanel : UserControl
    {
        private IProjectService _projectService;

        public MainPanel()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _projectService = ObjectFactory.GetInstance<IProjectService>();

                InitializeControls();
                InitializeEvents();
            }

        }

        private void InitializeControls()
        {
            ProjectsPanel projectsPanel = new ProjectsPanel();
            projectsPanel.ProjectPanelClick += projectsPanel_ProjectPanelClick;

            TabItem tabItem = new TabItem();
            tabItem.Name = "Projects";
            tabItem.Header = "Projects";
            tabItem.Content = projectsPanel;

            tabMain.Items.Add(tabItem);
            tabMain.SelectedItem = tabItem;
        }

        void projectsPanel_ProjectPanelClick(object sender, ProjectEventArgs e)
        {
            Project project = e.Project;

            foreach (TabItem ti in tabMain.Items)
            {
                Project p = ti.Tag as Project;
                if (p != null && p.Id == project.Id)
                {
                    ti.IsSelected = true;
                    return;
                }
            }

            TabItem tabItem = new TabItem();
            tabItem.Name = project.Name;
            tabItem.Header = project.Name;
            tabItem.Tag = project;

            ProjectPanel pnl = new ProjectPanel();
            pnl.Project = project;
            pnl.ComparisonResultCompleted += pnl_ComparisonResultCompleted;
            tabItem.Content = pnl;

            tabMain.Items.Add(tabItem);
            tabMain.SelectedItem = tabItem;
        }

        void pnl_ComparisonResultCompleted(object sender, CompareReportEventArgs e)
        {
            SchemaComparisonResult result = e.ComparisonResult;

            CompareReport report = new CompareReport();
            report.SetReport(result);

            TabItem tabItem = new TabItem();
            tabItem.Name = "ComparisonResult" + tabMain.Items.Count.ToString();
            tabItem.Header = "Comparison: " + result.Source + " / " + result.Target;
            tabItem.Content = report;
            tabMain.Items.Add(tabItem);
            tabMain.SelectedItem = tabItem;
        
        }

        private void InitializeEvents()
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
