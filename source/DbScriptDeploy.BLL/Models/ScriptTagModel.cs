using DbScriptDeploy.BLL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class ScriptTagModel
    {
        public ScriptTagModel()
        {
        }

        public int Id { get; set; }

        public string Tag { get; set; }

        public int ScriptId { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
