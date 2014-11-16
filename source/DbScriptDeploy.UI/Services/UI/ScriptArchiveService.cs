using DbScriptDeploy.UI.Controls;
using DbScriptDeploy.UI.Data;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using StructureMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SystemWrapper.IO;

namespace DbScriptDeploy.UI.Services.UI
{
    public class ScriptArchiveService
    {
        private Action<int> _callback;
        private DbEnvironment _environment;
        private Project _project;

        private ProgressDialog _progressDialog;

        public ScriptArchiveService(Project project, DbEnvironment environment, Action<int> callback)
        {
            _project = project;
            _environment = environment;
            _callback = callback;
        }

        public void Execute()
        {
            WorkerInfo workerInfo = new WorkerInfo();
            workerInfo.Environment = _environment;
            workerInfo.Project = _project;

            _progressDialog = new ProgressDialog();
            _progressDialog.Message = "Initializing...";
            _progressDialog.Owner = MainWindow.Instance;

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.DoWork += OnWorkerDoWork;
            bgWorker.RunWorkerCompleted += OnWorkerCompleted;
            bgWorker.ProgressChanged += OnWorkerProgressChanged;
            bgWorker.RunWorkerAsync(workerInfo);

            _progressDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _progressDialog.ShowDialog();

        }

        void OnWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressDialog.Close();

            Exception ex = e.Result as Exception;
            if (ex != null)
            {
                MessageBox.Show(MainWindow.Instance, ex.Message, "Execution failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Execute the callback
            _callback(Convert.ToInt32(e.Result));
        }

        void OnWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            WorkerInfo workerInfo = (WorkerInfo)e.Argument;
            //string scriptFolder = workerInfo.Project.ScriptFolder;
            int archiveCount = 0;

            using (DbHelper dbHelper = new DbHelper(workerInfo.Environment))
            {
                bgw.ReportProgress(-1, "Creating archive folder");
                string archiveFolder = String.Format("Archive\\{0}\\", DateTime.Now.ToString("yyyyMMdd"));
                string archivePath = Path.Combine(workerInfo.Project.ScriptFolder, archiveFolder);
                if (!Directory.Exists(archivePath))
                {
                    Directory.CreateDirectory(archivePath);
                }

                bgw.ReportProgress(-1, "Retrieving list of executed scripts");
                IEnumerable<Script> scripts = dbHelper.GetExecutedScripts();
                IFileWrap fileWrap = ObjectFactory.GetInstance<IFileWrap>();

                foreach (Script script in scripts)
                {
                    string filePath = Path.Combine(workerInfo.Project.ScriptFolder, script.Name);
                    bgw.ReportProgress(-1, String.Format("Archiving script {0}", script.Name));

                    if (fileWrap.Exists(filePath))
                    {
                        string target = Path.Combine(archivePath, script.Name);
                        fileWrap.Move(filePath, target);
                        archiveCount++;
                    }
                }
            }

            e.Result = archiveCount;
        }

        void OnWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            if (message != null)
            {
                _progressDialog.Message = message;
                return;
            }

            Exception ex = e.UserState as Exception;
            if (ex != null)
            {
                _progressDialog.Hide();
                MessageBox.Show(MainWindow.Instance, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private class WorkerInfo
        {
            public DbEnvironment Environment { get; set; }

            public Project Project { get; set; }
        }


    }
}
