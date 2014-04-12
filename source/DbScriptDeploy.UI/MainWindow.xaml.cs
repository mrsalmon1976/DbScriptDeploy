using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Services;
using DbScriptDeploy.UI.Views;
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
using DbScriptDeploy.UI.Utils;
using DbScriptDeploy.UI.Resources;

namespace DbScriptDeploy.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<UserControl> panels = new List<UserControl>();
        private IProjectService _projectService = null;

        internal static MainWindow Instance = null; 

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _projectService = ObjectFactory.GetInstance<IProjectService>();
				this.Title = this.Title + AppUtils.AppVersion();
				this.Icon = ImageUtils.ImageSourceFromIcon(Images.app);
            }

        }

        public void AddProject()
        {
            ProjectDialog dlg = new ProjectDialog();
            bool result = dlg.ShowDialog() ?? false;
            if (result)
            {
                Project prg = dlg.Project;
                _projectService.SaveProject(prg);
            }
            dlg.Close();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnuFile_NewProject_Click(object sender, RoutedEventArgs e)
        {
            AddProject();
        }


    }
}
