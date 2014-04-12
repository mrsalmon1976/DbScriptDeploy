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
    /// Interaction logic for ProjectsPanel.xaml
    /// </summary>
    public partial class ProjectsPanel : UserControl
    {
        private IProjectService _projectService;

        public event EventHandler<ProjectEventArgs> ProjectPanelClick;

        public ProjectsPanel()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _projectService = ObjectFactory.GetInstance<IProjectService>();

                this.InitializeEvents();

                this.ReloadProjects();
            }
        }

        private void AddProject(Project prj)
        {
            SelectPanel pnl = new SelectPanel();
            pnl.Text = prj.Name;
            pnl.HorizontalAlignment = HorizontalAlignment.Stretch;
            DockPanel.SetDock(pnl, Dock.Top);
            pnl.Background = Brushes.BlueViolet;
            pnl.Tag = prj;
            this.pnlMain.Children.Add(pnl);

            pnl.MouseUp += pnl_MouseUp;
        }

        void pnl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SelectPanel panel = (SelectPanel)sender;
            Project project = (Project)panel.Tag;

            if (this.ProjectPanelClick != null)
            {
                this.ProjectPanelClick(panel, new ProjectEventArgs(project));
            }
        }

        private void InitializeEvents()
        {
            _projectService.ProjectAdded += _projectService_ProjectAdded;
        }

        void _projectService_ProjectAdded(object sender, ProjectEventArgs e)
        {
            ReloadProjects();
        }

        private void ReloadProjects()
        {
            IEnumerable<Project> projects = _projectService.LoadProjects().OrderBy(x => x.Name);

            this.pnlMain.Children.Clear();

            foreach (Project p in projects)
            {
                AddProject(p);
            }

            this.pnlMain.Children.Add(new Label());

        }

        private void OnMainWindowProjectAdded(object sender, ProjectEventArgs ea)
        {
            _projectService.SaveProject(ea.Project);
            ReloadProjects();
        }

    }
}
