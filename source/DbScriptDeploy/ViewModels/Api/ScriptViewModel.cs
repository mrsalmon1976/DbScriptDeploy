using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class ScriptViewModel
    {
        public string Name { get; set; }

        public string Tags { get; set; }
                        
        public string ScriptUp { get; set; }

        public string ScriptDown { get; set; }

    }
}
