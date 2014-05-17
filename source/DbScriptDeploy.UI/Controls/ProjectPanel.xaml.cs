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

        public ProjectPanel()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _projectService = ObjectFactory.GetInstance<IProjectService>();

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

        private void AddScript(Script script)
        {
			if (lstScripts.Items.Count > 0 && lstScripts.Items[0] is Label)
			{
				lstScripts.Items.Clear();
			}

            ScriptCheckBox scb = new ScriptCheckBox();
            scb.ScriptLog = script;
            scb.CheckedChanged += scb_CheckedChanged;
			lstScripts.Items.Add(scb);
			btnSelectAll.IsEnabled = true;

        }

        void scb_CheckedChanged(object sender, Events.CheckedEventArgs e)
        {
			bool isExecuteEnabled = false;
			lstScripts.SelectionChanged -= lstScripts_SelectionChanged;

            for (int i=0; i<lstScripts.Items.Count; i++)
            {
				
				ScriptCheckBox sbc = (ScriptCheckBox)lstScripts.Items[i];
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
            btnAddScript.IsEnabled = false;
            btnExecuteScripts.IsEnabled = false;
            btnSelectAll.IsEnabled = false;

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
            _bgWorkerLoad.RunWorkerAsync(new WorkerInfo(dbInstance, this.Project));

            _progressDlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _progressDlg.ShowDialog();

        }

        void _bgWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressDlg.Close();
            btnAddScript.IsEnabled = true;
            btnCompare.IsEnabled = (cbDatabaseInstances.Items.Count > 2);

			if (lstScripts.Items.Count == 0)
            {
                Label lbl = new Label();
                lbl.Content = "No scripts to run";
				lstScripts.Items.Add(lbl);
            }
            else
            {
                btnSelectAll.IsEnabled = true;
            }

        }

        void _bgWorkerLoad_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            if (message != null)
            {
                _progressDlg.Message = message;
                return;
            }

            Script script = e.UserState as Script;
            if (script != null)
            {
                this.AddScript(script);
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
                    List<Guid> scriptsToArchive = new List<Guid>();
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
                        if (script == null)
                        {
                            // script is no longer in the folder, archive it
                            scriptsToArchive.Add(log.Id);
                        }
                        else
                        {
                            // this script has been run - we can skip it
                            scripts.Remove(script);
                        }
                    }

                    // create controls to display the scripts that can still be run
                    bgw.ReportProgress(-1, "Loading scripts that have not been executed...");
                    foreach (string file in scripts)
                    {
                        Script log = new Script();
                        log.Id = Guid.NewGuid();
                        log.Name = new FileInfo(file).Name;
                        log.ScriptText = File.ReadAllText(file);

                        bgw.ReportProgress(-1, log);
                    }

                    // archive the logs that have been removed
					if (scriptsToArchive.Any())
					{
						bgw.ReportProgress(-1, "Marking removed scripts as archived...");
						dbHelper.ArchiveLogs(scriptsToArchive);
					}
                }

            }
            catch (Exception ex)
            {
                bgw.ReportProgress(-1, ex);
            }
        }

        private class WorkerInfo
        {
            public WorkerInfo(DbEnvironment dbInstance, Project project)
            {
                this.DbInstance = dbInstance;
                this.Project = project;
                this.Scripts = new List<Script>();
            }

            public DbEnvironment DbInstance { get; set; }

            public DbEnvironment DbInstance2 { get; set; }

            public Project Project { get; set; }

            public List<Script> Scripts { get; private set; }
        }

        private void btnAddScript_Click(object sender, RoutedEventArgs e)
        {
            ScriptDialog dlg = new ScriptDialog();
            dlg.Owner = MainWindow.Instance;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowInTaskbar = false;
            dlg.DbEnvironment = this.CurrentDbInstance;
            dlg.Project = this.Project;

            if ((dlg.ShowDialog() ?? false) == true)
            {
                this.AddScript(dlg.Script);
            }

            dlg.Close();
        }

        private void btnExecuteScripts_Click(object sender, RoutedEventArgs e)
        {
            btnExecuteScripts.IsEnabled = false;
            btnSelectAll.IsEnabled = false;
            WorkerInfo workerInfo = new WorkerInfo(this.CurrentDbInstance, this.Project);

			foreach (ScriptCheckBox sbc in lstScripts.Items)
            {
                if (sbc.IsChecked) workerInfo.Scripts.Add(sbc.ScriptLog);
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
			foreach (ScriptCheckBox sbc in lstScripts.Items)
            {
                sbc.IsChecked = true;
            }
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

            WorkerInfo workerInfo = new WorkerInfo(databaseInstance1, this.Project);
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
			foreach (ScriptCheckBox scb in e.RemovedItems)
			{
				scb.IsChecked = false;
			}
			foreach (ScriptCheckBox scb in e.AddedItems)
			{
				scb.IsChecked = true;
			}
		}

    }
}
