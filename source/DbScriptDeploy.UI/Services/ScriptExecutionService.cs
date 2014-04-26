using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.UI.Data;
using DbScriptDeploy.UI.Models;
using StructureMap;

namespace DbScriptDeploy.UI.Services
{
	public interface IScriptExecutionService
	{
		void ExecuteScripts(string connString, string folder);
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

	}
}
