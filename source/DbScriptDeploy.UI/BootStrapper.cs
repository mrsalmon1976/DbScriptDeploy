using DbScriptDeploy.UI.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI
{
    public class BootStrapper
    {
        public static void Boot()
        {
			ObjectFactory.Configure(x => x.For<IScriptExecutionService>().Use<ScriptExecutionService>());
			ObjectFactory.Configure(x => x.For<IProjectService>().Singleton().Use<ProjectService>());
            ObjectFactory.Configure(x => x.For<IDatabaseComparisonService>().Singleton().Use<DatabaseComparisonService>());
        
        }
    }
}
