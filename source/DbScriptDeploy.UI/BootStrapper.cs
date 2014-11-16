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
            ObjectFactory.Configure(x => x.For<IFileInfoWrap>().Use<FileInfoWrap>());
			ObjectFactory.Configure(x => x.For<IJsonPersistenceService>().Use<JsonPersistenceService>());
			ObjectFactory.Configure(x => x.For<IScriptExecutionService>().Use<ScriptExecutionService>());
            ObjectFactory.Configure(x => x.For<IDatabaseComparisonService>().Singleton().Use<DatabaseComparisonService>());

			// set up the project service - for now we'll default the project file path
			ObjectFactory.Configure(x => x.For<IProjectService>().Singleton().Use(() => {
                string projectFilePath = Path.Combine(AppUtils.BaseDirectory(), Constants.UserProjectFileName);
                return new ProjectService(projectFilePath, ObjectFactory.GetInstance<IJsonPersistenceService>()); 
			}));

            // set up the settings service - for now we'll default the project file path
            ObjectFactory.Configure(x => x.For<ISettingsService>().Singleton().Use(() =>
            {
                string settingsFilePath = Path.Combine(AppUtils.BaseDirectory(), Constants.UserSettingsFileName);
                return new SettingsService(settingsFilePath, ObjectFactory.GetInstance<IJsonPersistenceService>());
            }));

        
        }
    }
}
