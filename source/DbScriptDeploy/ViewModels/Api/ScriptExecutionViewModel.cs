using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels.Api
{
    public class ScriptExecutionViewModel
    {
        public string Id { get; set; }

        public string ScriptId { get; set; }

        public string EnvironmentId { get; set; }

        public DateTime? ExecutionStartDate { get; set; }

        public DateTime? ExecutionCompleteDate { get; set; }

        public string Error { get; set; }

    }
}
