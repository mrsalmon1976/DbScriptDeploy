using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.UI.Data;
using DbScriptDeploy.UI.Models;
using StructureMap;
using SystemWrapper.IO;
using StructureMap.Pipeline;

namespace DbScriptDeploy.UI.Services
{
	public interface IScriptExecutionService
	{
		void ExecuteScripts(string connString, string folder);

        Script LoadScriptFromFile(string filePath);
	}


	public class ScriptExecutionService : IScriptExecutionService
	{
		public void ExecuteScripts(string connString, string folder)
		{
			DbHelper dbHelper = new DbHelper(connString);
			
			// make sure the folder exists
			if (!Directory.Exists(folder))
			{
				throw new ArgumentException(String.Format("Folder '{0}' does not exist", folder));
			}

			IEnumerable<string> scripts = Directory.GetFiles(folder).OrderBy(x => x);
			IEnumerable<Script> executedScripts = dbHelper.GetExecutedScripts();
			List<Script> scriptsToRun = new List<Script>();
			foreach (string script in scripts)
			{
				string fileName = new FileInfo(script).Name;
				Script s = executedScripts.FirstOrDefault(x => x.Name == fileName);
				if (s == null)
				{
					s = new Script();
					s.Id = Guid.NewGuid();
					s.Name = fileName;
					s.ScriptText = File.ReadAllText(script);
					scriptsToRun.Add(s);
				}
			}

			dbHelper.ExecuteScripts(scriptsToRun);
	    }

        public Script LoadScriptFromFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string scriptText = File.ReadAllText(filePath);
            
            Script log = new Script();
            log.Id = Guid.NewGuid();
            log.Name = fileInfo.Name;
            log.ScriptText = scriptText;

            // if line 1 starts with "--- " then it's a tag, so set it
            if (scriptText.StartsWith("--- "))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    log.Tag = reader.ReadLine().Replace("--- ", "").Trim();
                }
            }

            return log;
        }

	}
}
