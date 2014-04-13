using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Services
{
    public interface IDatabaseComparisonService
    {
        event EventHandler<NotificationEventArgs> Notification;

        /// <summary>
        /// Compares two schemas, and returns the result of the comparison.
        /// </summary>
        /// <param name="schemaPath1">The schema1.</param>
        /// <param name="scheschemaPath2ma2">The schema2.</param>
        /// <returns></returns>
        SchemaComparisonResult CompareSchemas(string basePath, string schemaPath1, string schemaPath2);

        void ExtractSchema(string path, DbEnvironment dbInstance);
    }

    public class DatabaseComparisonService : IDatabaseComparisonService
    {
        public event EventHandler<NotificationEventArgs> Notification;

        /// <summary>
        /// Compares two schemas, and returns the result of the comparison.
        /// </summary>
        /// <param name="schemaPath1">The schema1.</param>
        /// <param name="schemaPath2">The schema2.</param>
        /// <returns></returns>
        public SchemaComparisonResult CompareSchemas(string basePath, string schemaPath1, string schemaPath2)
        {
            Notify("Reading schema path " + schemaPath1);
            List<string> files1 = Directory.EnumerateFiles(schemaPath1, "*.sql", SearchOption.AllDirectories).ToList();

            Notify("Reading schema path " + schemaPath2);
            List<string> files2 = Directory.EnumerateFiles(schemaPath2, "*.sql", SearchOption.AllDirectories).ToList();

            SchemaComparisonResult result = new SchemaComparisonResult();
            result.Path = basePath;

            // loop through the first file list
            while (files1.Any())
            {
                string file1 = files1[0];
                string file2 = files2.FirstOrDefault(x => x == file1.Replace(schemaPath1, schemaPath2));

                files1.RemoveAt(0);

                if (!String.IsNullOrEmpty(file2))
                {
                    files2.Remove(file2);
                }

                FileInfo f1 = new FileInfo(file1);
                string f1Key = GetSchemaObjectKey(f1);

                if (file2 == null || !File.Exists(file2))
                {
                    result.Items.Add(new SchemaComparisonResultItem(f1Key, SchemaDifference.LeftOnly));
                    continue;
                }

                FileInfo f2 = new FileInfo(file2);
                string f2Key = GetSchemaObjectKey(f2);

                if (f1.Length != f2.Length)
                {
                    result.Items.Add(new SchemaComparisonResultItem(f2Key, SchemaDifference.Differ));
                    continue;
                }
                else
                {
                    string t1 = File.ReadAllText(file1);
                    string t2 = File.ReadAllText(file2);
                    if (t1 != t2)
                    {
                        result.Items.Add(new SchemaComparisonResultItem(f2Key, SchemaDifference.Differ));
                        continue;
                    }
                }

                result.Items.Add(new SchemaComparisonResultItem(f1Key, SchemaDifference.None));

            }

            // anything left in f2 is also an issue
            foreach (string s in files2)
            {
                result.Items.Add(new SchemaComparisonResultItem(GetSchemaObjectKey(new FileInfo(s)), SchemaDifference.RightOnly));
            }
            
            return result;
        }

        public void ExtractSchema(string path, DbEnvironment dbInstance)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string args = String.Format("Script -s={0} -b={1} -d=\"{2}\" -o=1", dbInstance.Host, dbInstance.Catalog, path);

            ProcessStartInfo startInfo = new ProcessStartInfo("schemazen.exe");
            startInfo.Arguments = args;
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = AppUtils.BaseDirectory();

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private string GetSchemaObjectKey(FileInfo fi)
        {
            string dir = fi.Directory.Name;
            if (dir.StartsWith("1_") || dir.StartsWith("2_")) dir = "";
            return String.Format("{0}{1}{2}", dir, Path.DirectorySeparatorChar.ToString(), fi.Name);
        }

        private void Notify(string message)
        {
            if (this.Notification != null)
            {
                this.Notification(this, new NotificationEventArgs(message));
            }
        }
    }
}
