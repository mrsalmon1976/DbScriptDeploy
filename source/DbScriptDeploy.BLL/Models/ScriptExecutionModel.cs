using DbScriptDeploy.BLL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class ScriptExecutionModel
    {
        public ScriptExecutionModel()
        {
        }

        public int Id { get; set; }

        public int ScriptId { get; set; }

        public int EnvironmentId { get; set; }

        public DateTime ExecutionStartDate { get; set; }

        public DateTime ExecutionCompleteDate { get; set; }
    }
}
