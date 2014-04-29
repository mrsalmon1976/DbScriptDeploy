using DbScriptDeploy.UI.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;
using System.IO;
using DbScriptDeploy.UI.Utils;
using DbScriptDeploy.UI.Data;

namespace DbScriptDeploy.UI
{
    public class BootStrapper
    {
        public static void Boot()
        {
			ObjectFactory.Configure(x => x.For<IFileWrap>().Use<FileWrap>());
			ObjectFactory.Configure(x => x.For<IJsonPersistenceService>().Use<JsonPersistenceService>());
			ObjectFactory.Configure(x => x.For<IScriptExecutionService>().Use<ScriptExecutionService>());
            ObjectFactory.Configure(x => x.For<IDatabaseComparisonService>().Singleton().Use<DatabaseComparisonService>());

			// set up the project service - for now we'll default the project file path
            string projectFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DbScriptDeploy");
            if (!Directory.Exists(projectFolder)) Directory.CreateDirectory(projectFolder);
			string projectFilePath = Path.Combine(projectFolder, Constants.UserProjectFileName);
			ObjectFactory.Configure(x => x.For<IProjectService>().Singleton().Use(() => { 
				return new ProjectService(projectFilePath, ObjectFactory.GetInstance<IJsonPersistenceService>()); 
			}));

        
        }
    }
}
