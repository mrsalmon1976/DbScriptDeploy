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
			pnl.Margin = new Thickness(0,5,0,0);
			pnl.FontSize = this.FontSize + 3;
            DockPanel.SetDock(pnl, Dock.Top);
            pnl.Tag = prj;
            this.pnlMain.Children.Add(pnl);

			pnl.MouseUp += delegate(object sender, MouseButtonEventArgs e)
			{
				SelectPanel panel = (SelectPanel)sender;
				Project project = (Project)panel.Tag;

				if (this.ProjectPanelClick != null)
				{
					this.ProjectPanelClick(panel, new ProjectEventArgs(project));
				}
			};

			pnl.DeleteButtonMouseUp += delegate(object sender, MouseButtonEventArgs e)
			{
				if (MessageBox.Show(MainWindow.Instance, "Are you sure you want to delete this project?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
				{
					SelectPanel sp = (SelectPanel)sender;
					Project p = (Project)sp.Tag;
					_projectService.DeleteProject(p);
					this.pnlMain.Children.Remove(sp);
				}
				e.Handled = true;
			};
        }

        private void InitializeEvents()
        {
            _projectService.ProjectAdded += OnProjectAdded;
        }

        void OnProjectAdded(object sender, ProjectEventArgs e)
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

    }
}
