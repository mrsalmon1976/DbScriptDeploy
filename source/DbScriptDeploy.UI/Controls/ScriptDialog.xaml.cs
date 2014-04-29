using DbScriptDeploy.UI.Data;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;
using DbScriptDeploy.UI.Resources;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for ScriptDialog.xaml
    /// </summary>
    public partial class ScriptDialog : Window
    {
        private Scintilla _scintilla = null;
        private BackgroundWorker _bgWorkerParse;
        private ProgressDialog _progressDlg;

        public ScriptDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _scintilla = ScintillaUtils.InitSqlEditor(this.FontSize);
                scintillaHost.Child = _scintilla;
				this.Icon = ImageUtils.ImageSourceFromIcon(Images.app);
			}

            this.Script = new Script();
        }

        public Script Script { get; set; }

        public DbEnvironment DbEnvironment { get; set; }

        public Project Project { get; set; }

        public bool IsEdit { get; set; }

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            _progressDlg = new ProgressDialog();
            _progressDlg.Message = "Initializing...";
            _progressDlg.Owner = MainWindow.Instance;

            _bgWorkerParse = new BackgroundWorker();
            _bgWorkerParse.WorkerReportsProgress = true;
            _bgWorkerParse.DoWork += _bgWorkerParse_DoWork;
            _bgWorkerParse.RunWorkerCompleted += _bgWorkerParse_RunWorkerCompleted;
            _bgWorkerParse.RunWorkerAsync(_scintilla.Text);

            _progressDlg.Message = "Parsing script...";
            _progressDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _progressDlg.ShowDialog();


        }

        void _bgWorkerParse_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressDlg.Close();

            IEnumerable<string> errors = e.Result as IEnumerable<string>;
            if (errors.Any())
            {
                MessageBox.Show(this, String.Join(Environment.NewLine + Environment.NewLine, errors), "Parse Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(this, "SQL parsed without any errors.", "Parse Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        void _bgWorkerParse_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            string script = (string)e.Argument;

            using (DbHelper dbHelper = new DbHelper(this.DbEnvironment))
            {
                IEnumerable<string> errors = dbHelper.ParseScript(script);
                e.Result = errors;
            }


        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text) || String.IsNullOrWhiteSpace(_scintilla.Text))
            {
                MessageBox.Show(this, "A name and script must be provided.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!FileUtils.IsValidFileName(txtName.Text))
            {
                MessageBox.Show(this, String.Format("'{0}' is not a valid file name.", txtName.Text), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string fileName = String.Format("{0}_{1}.sql", DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"), txtName.Text);
                string fullPath = System.IO.Path.Combine(this.Project.ScriptFolder, fileName);

				this.Script.Name = fileName;

                File.WriteAllText(fullPath, _scintilla.Text);
				this.DialogResult = true;
				this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
