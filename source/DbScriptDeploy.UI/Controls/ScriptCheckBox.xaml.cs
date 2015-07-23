using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
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
    /// Interaction logic for ScriptCheckBox.xaml
    /// </summary>
    public partial class ScriptCheckBox : UserControl
    {

        public event EventHandler<CheckedEventArgs> CheckedChanged;
		private Script _scriptLog;

        public ScriptCheckBox()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.cbScript.Unchecked += OnCheckedChanged;
                this.cbScript.Checked += OnCheckedChanged;
            }

        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (this.CheckedChanged != null)
            {
                this.CheckedChanged(sender, new CheckedEventArgs(this.IsChecked));
            }
        }

        void cbScript_Unchecked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public bool IsChecked
        {
            get
            {
                return (cbScript.IsChecked ?? false);
            }
            set
            {
                cbScript.IsChecked = value;
            }
        }

        public Script ScriptLog 
        {
            get
            {
                return _scriptLog;
            }
            set
            {
                _scriptLog = value;
                lblScript.Content = ControlUtils.EscapeContent(value.Name);
            }
        }

        private void lblScript_MouseEnter(object sender, MouseEventArgs e)
        {
            lblScript.Foreground = Brushes.Blue;
        }

        private void lblScript_MouseLeave(object sender, MouseEventArgs e)
        {
            lblScript.Foreground = Brushes.Black;
        }

        private void lblScript_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ScriptDialog dlg = new ScriptDialog();
            dlg.Owner = MainWindow.Instance;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowInTaskbar = false;
            dlg.IsReadOnly = true;
            dlg.Script = this.ScriptLog;
            //dlg.Project = this.Project;

            if ((dlg.ShowDialog() ?? false) == true)
            {
                //this.ReloadScripts();
            }

            dlg.Close();

        }

    }
}
