using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class ScriptViewModel
    {
        public ScriptViewModel()
        {
            this.Tags = new List<ScriptTagViewModel>();
            this.Executions = new List<ScriptExecutionViewModel>();
        }

        public string Id { get; set; }

        public string ProjectId { get; set; }

        public string Name { get; set; }

        public List<ScriptTagViewModel> Tags { get; private set; }

        public List<ScriptExecutionViewModel> Executions { get; private set; }

        public string ScriptUp { get; set; }

        public string ScriptDown { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
