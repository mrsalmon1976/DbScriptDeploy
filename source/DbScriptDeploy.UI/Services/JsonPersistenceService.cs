using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DbScriptDeploy.UI.Services
{
	public interface IJsonPersistenceService
	{
		void WriteFile(string path, object contents);
	}

	public class JsonPersistenceService : IJsonPersistenceService
	{
		public void WriteFile(string path, object contents)
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			string projects = JsonConvert.SerializeObject(contents, settings);
			File.WriteAllText(path, projects);
		}
	}
}
