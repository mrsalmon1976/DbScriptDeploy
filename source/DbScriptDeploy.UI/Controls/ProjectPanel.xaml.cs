using DbScriptDeploy.UI.Data;
using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Services;
using DbScriptDeploy.UI.Utils;
using StructureMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Diagnostics;
using DbScriptDeploy.UI.Services.UI;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for ProjectPanel.xaml
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private IProjectService _projectService;
        private Project _project;
        private BackgroundWorker _bgWorkerExecute;
        private BackgroundWorker _bgWorkerLoad;
        private BackgroundWorker _bgWorkerCompare;
        private ProgressDialog _progressDlg;

        public event EventHandler<CompareReportEventArgs> ComparisonResultCompleted;

        private const string Untagged = "Untagged";

        public ProjectPanel()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _projectService = ObjectFactory.GetInstance<IProjectService>();

                _projectService.ProjectUpdated += OnProjectServiceProjectUpdated;

            }

        }

        void OnProjectServiceProjectUpdated(object sender, ProjectEventArgs e)
        {
            if (e.Project.Id == this._project.Id)
            {
                this.Project = e.Project;
            }
        }

        public Project Project
        {
            get { return _project; }
            set
            {
                _project = value;
                ReloadDatabaseInstances();
                lblHeader.Content = "Project: " + value.Name;
            }
        }

        private DbEnvironment CurrentDbInstance
        {
            get
            {
                return (cbDatabaseInstances.SelectedItem as DbEnvironment);
            }
        }

        private void UpdateScriptVisibility(ScriptCheckBox scb)
        {
            string filter = txtFilter.Text;
            if (filter.Length == 0) return;

            scb.Visibility = (scb.ScriptLog.Name.Contains(filter) ? Visibility.Visible : Visibility.Collapsed);
        }

        private void AddScript(Script script)
        {
            if (lstScripts.Items.Count > 0 && lstScripts.Items[0] is Label && ((Label)lstScripts.Items[0]).Tag == null)
			{
				lstScripts.Items.Clear();
			}

            ScriptCheckBox scb = new ScriptCheckBox();
            scb.ScriptLog = script;
            scb.CheckedChanged += scb_CheckedChanged;
            scb.Tag = script.Tag ?? Untagged;
			btnSelectAll.IsEnabled = true;

            UpdateScriptVisibility(scb);
            lstScripts.Items.Add(scb);

        }

        private void AddTagHeader(string tag)
        {
            tag = (tag ?? Untagged);

            Label lblTag = new Label();
            lblTag.Content = tag;
            lblTag.Tag = tag;
            lblTag.FontSize = this.FontSize + 2;

            if (lstScripts.Items.Count > 0)
            {
                lblTag.Margin = new Thickness(0, 5, 0, 0);
            }

            lstScripts.Items.Add(lblTag);
        }

        void scb_CheckedChanged(object sender, Events.CheckedEventArgs e)
        {
			bool isExecuteEnabled = false;
			lstScripts.SelectionChanged -= lstScripts_SelectionChanged;

            for (int i=0; i<lstScripts.Items.Count; i++)
            {

                ScriptCheckBox sbc = lstScripts.Items[i] as ScriptCheckBox;
                if (sbc == null) continue;

				if (sbc.IsChecked)
				{
					lstScripts.SelectedItems.Add(sbc);
					isExecuteEnabled = true;
				}
				else
				{
					lstScripts.SelectedItems.Remove(sbc);
				}
            }
			btnExecuteScripts.IsEnabled = isExecuteEnabled;
			lstScripts.SelectionChanged += lstScripts_SelectionChanged;
			
        }

        private void ReloadDatabaseInstances()
        {
            IEnumerable<DbEnvironment> instances = this.Project.DatabaseInstances.OrderBy(x => x.Name);
            cbDatabaseInstances.Items.Clear();
            cbDatabaseInstances.Items.Add("");
            foreach (DbEnvironment dbInstance in instances)
            {
                cbDatabaseInstances.Items.Add(dbInstance);
            }
        }


        private void btnAddDbInstance_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentDialog dlg = new EnvironmentDialog();
            if (dlg.ShowDialog() == true)
            {
                DbEnvironment dbInstance = dlg.DbEnvironment;
                this.Project.DatabaseInstances.Add(dbInstance);
                _projectService.SaveProject(this.Project);
                this.ReloadDatabaseInstances();
            }
            dlg.Close();
        }

        private void cbDatabaseInstances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			ReloadScripts();
		}

		private void ReloadScripts()
		{
            // clear the project pane
			lstScripts.Items.Clear();
            btnCompare.IsEnabled = false;
            btnArchive.IsEnabled = false;
            btnEditDbInstance.IsEnabled = false;
            btnAddScript.IsEnabled = false;
            btnExecuteScripts.IsEnabled = false;
            btnSelectAll.IsEnabled = false;
            btnRefresh.IsEnabled = false;

            DbEnvironment dbInstance = CurrentDbInstance;
            if (dbInstance == null)
            {
                return;
            }

            _progressDlg = new ProgressDialog();
            _progressDlg.Message = "Initializing...";
            _progressDlg.Owner = MainWindow.Instance;

            _bgWorkerLoad = new BackgroundWorker();
            _bgWorkerLoad.WorkerReportsProgress = true;
            _bgWorkerLoad.DoWork += _bgWorkerLoad_DoWork;
            _bgWorkerLoad.RunWorkerCompleted += _bgWorkerLoad_RunWorkerCompleted;
            _bgWorkerLoad.ProgressChanged += _bgWorkerLoad_ProgressChanged;
            _bgWorkerLoad.RunWorkerAsync(new WorkerInfo(dbInstance, this.Project, txtFilter.Text));

            _progressDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _progressDlg.ShowDialog();

        }

        void _bgWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<Script> scripts = (List<Script>)e.Result;
            string tag = Guid.NewGuid().ToString();

            foreach (Script script in scripts.OrderBy(x => (x.Tag ?? String.Empty).ToUpper()))
            {
                string scriptTag = (script.Tag ?? String.Empty).ToUpper();
                tag = (tag ?? String.Empty).ToUpper();
                if (scriptTag != tag)
                {
                    this.AddTagHeader(script.Tag);
                    tag = scriptTag;
                }
                this.AddScript(script);
            }

            _progressDlg.Close();
            btnRefresh.IsEnabled = true;
            btnAddScript.IsEnabled = true;
            btnCompare.IsEnabled = (cbDatabaseInstances.Items.Count > 2);
            btnEditDbInstance.IsEnabled = (this.CurrentDbInstance != null);
            btnArchive.IsEnabled = btnEditDbInstance.IsEnabled;

            if (scripts.Count == 0)
            {
                Label lbl = new Label();
                lbl.Content = "No scripts to run";
				lstScripts.Items.Add(lbl);
            }
            else
            {
                btnSelectAll.IsEnabled = true;
            }

            ApplyFilter();
        }

        void _bgWorkerLoad_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            if (message != null)
            {
                _progressDlg.Message = message;
                return;
            }

            Exception ex = e.UserState as Exception;
            if (ex != null)
            {
                _progressDlg.Hide();
                MessageBox.Show(MainWindow.Instance, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void _bgWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            WorkerInfo workerInfo = (WorkerInfo)e.Argument;
            IScriptExecutionService scriptExecutionService = ObjectFactory.GetInstance<IScriptExecutionService>();
            List<Script> loadedScripts = new List<Script>();

            try
            {
                // initialise the table
                bgw.ReportProgress(-1, "Initialising script logging table in database...");
                using (DbHelper dbHelper = new DbHelper(workerInfo.DbInstance))
                {
                    dbHelper.InitScriptLogTable();

                    //System.Threading.Thread.Sleep(1500);

                    // read the files in the folder
                    bgw.ReportProgress(-1, "Reading script files...");

                    if (!Directory.Exists(workerInfo.Project.ScriptFolder))
                    {
                        throw new Exception("Project folder no longer exists.");
                    }

                    //string filter = String.Format("{0}*.sql" workerInfo.Filter
                    List<string> scripts = Directory.EnumerateFiles(workerInfo.Project.ScriptFolder, "*.sql")
                        .OrderBy(x => x)
                        .ToList();
                    //System.Threading.Thread.Sleep(1500);

                    // load all the scripts that haven't been archived
                    bgw.ReportProgress(-1, "Loading executed scripts...");
                    IEnumerable<Script> executedScripts = dbHelper.GetExecutedScripts();

                    foreach (Script log in executedScripts)
                    {
                        string script = scripts.FirstOrDefault(x => x.EndsWith("\\" + log.Name));
                        if (script != null)
                        {
                            // this script has been run - we can skip it
                            scripts.Remove(script);
                        }
                    }

                    // create controls to display the scripts that can still be run
                    bgw.ReportProgress(-1, "Loading scripts that have not been executed...");
                    foreach (string file in scripts)
                    {
                        // load the script up and add for processing at a later stage
                        Script log = scriptExecutionService.LoadScriptFromFile(file);
                        loadedScripts.Add(log);
                        bgw.ReportProgress(-1, log);
                    }

                }


            }
            catch (Exception ex)
            {
                bgw.ReportProgress(-1, ex);
            }

            e.Result = loadedScripts;

        }

        private class WorkerInfo
        {
            public WorkerInfo(DbEnvironment dbInstance, Project project, string filter)
            {
                this.DbInstance = dbInstance;
                this.Project = project;
                this.Scripts = new List<Script>();
                this.Filter = filter;
            }

            public DbEnvironment DbInstance { get; set; }

            public DbEnvironment DbInstance2 { get; set; }

            public Project Project { get; set; }

            public List<Script> Scripts { get; private set; }

            public string Filter { get; set; }
        }

        private void btnAddScript_Click(object sender, RoutedEventArgs e)
        {
            ScriptDialog dlg = new ScriptDialog();
            dlg.Owner = MainWindow.Instance;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowInTaskbar = false;
            dlg.DbEnvironment = this.CurrentDbInstance;
            dlg.Project = this.Project;
            dlg.IsReadOnly = false;

            if ((dlg.ShowDialog() ?? false) == true)
            {
                this.ReloadScripts();
            }

            dlg.Close();
        }

        private void btnExecuteScripts_Click(object sender, RoutedEventArgs e)
        {
            btnExecuteScripts.IsEnabled = false;
            btnSelectAll.IsEnabled = false;
            WorkerInfo workerInfo = new WorkerInfo(this.CurrentDbInstance, this.Project, txtFilter.Text);

			foreach (Control c in lstScripts.Items)
            {
                ScriptCheckBox scb = c as ScriptCheckBox;
                if (scb != null && scb.IsChecked) workerInfo.Scripts.Add(scb.ScriptLog);
            }


            _progressDlg = new ProgressDialog();
            _progressDlg.Message = "Initializing...";
            _progressDlg.Owner = MainWindow.Instance;

            _bgWorkerExecute = new BackgroundWorker();
            _bgWorkerExecute.WorkerReportsProgress = true;
            _bgWorkerExecute.DoWork += _bgWorkerExecute_DoWork;
            _bgWorkerExecute.RunWorkerCompleted += _bgWorkerExecute_RunWorkerCompleted;
            _bgWorkerExecute.ProgressChanged += _bgWorkerLoad_ProgressChanged;
            _bgWorkerExecute.RunWorkerAsync(workerInfo);

            _progressDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _progressDlg.ShowDialog();



        }

        void _bgWorkerExecute_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			_progressDlg.Close();

			Exception ex = e.Result as Exception;
			if (ex != null)
			{
				MessageBox.Show(MainWindow.Instance, ex.Message, "Execution failed", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			
			ReloadScripts();
        }

        void _bgWorkerExecute_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            WorkerInfo workerInfo = (WorkerInfo)e.Argument;
			string scriptFolder = workerInfo.Project.ScriptFolder;

            using (DbHelper dbHelper = new DbHelper(workerInfo.DbInstance))
            {
                foreach (Script script in workerInfo.Scripts)
                {
                    try
                    {
						string scriptPath = Path.Combine(scriptFolder, script.Name);
                        bgw.ReportProgress(-1, String.Format("Executing {0}...", script.Name));
						// reload script in case it's been changed on disk
						script.ScriptText = File.ReadAllText(scriptPath);
                        dbHelper.ExecuteScript(script);
                    }
                    catch (Exception ex)
                    {
                        string msg = String.Format("Failed to execute script '{0}':{1}{1}{2}.", script.Name, Environment.NewLine, ex.Message);
						e.Result = new Exception(msg, ex);
                        return;
                    }
                }
            }
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {

			foreach (Control c in lstScripts.Items)
            {
                ScriptCheckBox scb = c as ScriptCheckBox;
                if (scb != null)
                {
                    scb.IsChecked = true;
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.ReloadScripts();
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            CompareDialog dlg = new CompareDialog();
            dlg.SourceInstance = this.CurrentDbInstance;
            dlg.Project = this.Project;
            if (dlg.ShowDialog() ?? false == true)
            {
                CompareInstances(dlg.SourceInstance, dlg.TargetInstance);
            }
            dlg.Close();
        }

        private void CompareInstances(DbEnvironment databaseInstance1, DbEnvironment databaseInstance2)
        {
            _progressDlg = new ProgressDialog();
            _progressDlg.Message = String.Format("Comparing {0} and {1}...", databaseInstance1.Name, databaseInstance2.Name);
            _progressDlg.Owner = MainWindow.Instance;

            WorkerInfo workerInfo = new WorkerInfo(databaseInstance1, this.Project, txtFilter.Text);
            workerInfo.DbInstance2 = databaseInstance2;

            _bgWorkerCompare = new BackgroundWorker();
            _bgWorkerCompare.WorkerReportsProgress = true;
            _bgWorkerCompare.RunWorkerCompleted += _bgWorkerCompare_RunWorkerCompleted;
            _bgWorkerCompare.DoWork += _bgWorkerCompare_DoWork;
            _bgWorkerCompare.ProgressChanged += _bgWorkerLoad_ProgressChanged;
            _bgWorkerCompare.RunWorkerAsync(workerInfo);

            _progressDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _progressDlg.ShowDialog();
        }

        void _bgWorkerCompare_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SchemaComparisonResult result = (SchemaComparisonResult)e.Result;

            _progressDlg.Close();

            if (this.ComparisonResultCompleted != null)
            {
                this.ComparisonResultCompleted(this, new CompareReportEventArgs(result));
            }
            

        }

        void _bgWorkerCompare_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            WorkerInfo workerInfo = (WorkerInfo)e.Argument;
            string timeStampFolder = Path.Combine(AppUtils.BaseDirectory(), "Comparisons", DateTime.Now.ToString("yyyyMMdd-HHmmss"));

            EventHandler<NotificationEventArgs> notificationEventHandler = delegate(object nSender, NotificationEventArgs nea)
            {
                bgw.ReportProgress(-1, nea.Message);
            };


            IDatabaseComparisonService dbcService = ObjectFactory.GetInstance<IDatabaseComparisonService>();
            dbcService.Notification += notificationEventHandler;
            try
            {
                bgw.ReportProgress(-1, String.Format("Running Schema Zen on {0}...", workerInfo.DbInstance.Name));
                string path1 = Path.Combine(timeStampFolder, "1_" + workerInfo.DbInstance.Name);
                dbcService.ExtractSchema(path1, workerInfo.DbInstance);
                
                bgw.ReportProgress(-1, String.Format("Running Schema Zen on {0}...", workerInfo.DbInstance2.Name));
                string path2 = Path.Combine(timeStampFolder, "2_" + workerInfo.DbInstance2.Name);
                dbcService.ExtractSchema(path2, workerInfo.DbInstance2);

                SchemaComparisonResult result = dbcService.CompareSchemas(timeStampFolder, path1, path2);
                result.Source = workerInfo.DbInstance.Name;
                result.Target = workerInfo.DbInstance2.Name;
                e.Result = result;
            }
            finally
            {
                dbcService.Notification -= notificationEventHandler;
            }

        }

		private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
		{
			string scriptFolder = this.Project.ScriptFolder;
			if (Directory.Exists(scriptFolder))
			{
				Process.Start(scriptFolder);
			}
			else
			{
				MessageBox.Show(MainWindow.Instance, String.Format("The folder '{0}' does not exist.", scriptFolder), "Invalid Folder", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void lstScripts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (object c in e.RemovedItems)
			{
                ScriptCheckBox scb = c as ScriptCheckBox;
                if (scb != null)
                {
                    scb.IsChecked = false;
                }
			}
			foreach (object c in e.AddedItems)
			{
                ScriptCheckBox scb = c as ScriptCheckBox;
                if (scb != null)
                {
                    scb.IsChecked = true;
                }
			}
		}

        private void ApplyFilter()
        {
            string filter = txtFilter.Text;
            if (filter.Length == 0) return;

            foreach (object item in lstScripts.Items)
            {
                ScriptCheckBox scb = item as ScriptCheckBox;
                if (scb != null)
                {
                    UpdateScriptVisibility(scb);
                }
            }
        }

        private void OnTxtFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void btnEditDbInstance_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentDialog dlg = new EnvironmentDialog();
            dlg.DbEnvironment = this.CurrentDbInstance;
            if (dlg.ShowDialog() == true)
            {
                DbEnvironment dbInstance = dlg.DbEnvironment;
                this.Project.DatabaseInstances.Remove(this.CurrentDbInstance);
                this.Project.DatabaseInstances.Add(dbInstance);
                _projectService.SaveProject(this.Project);
                this.ReloadDatabaseInstances();
            }
            dlg.Close();
        }

        private void btnArchive_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(MainWindow.Instance, "Are you sure you want to archive executed scripts?", "Archive Scripts", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Action<int> callback = (cnt) =>
                {
                    string info = "No scripts archived.";
                    if (cnt > 0)
                    {
                        info = "{0} scripts were archived - please make sure you commit the changes if your scripts are under source control.";
                    }
                    MessageBox.Show(MainWindow.Instance, String.Format(info, cnt), "Archive Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                };
                ScriptArchiveService sas = new ScriptArchiveService(this.Project, this.CurrentDbInstance, callback);
                sas.Execute();
            }
        }

    }
}
