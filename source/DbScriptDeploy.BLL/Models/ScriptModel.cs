using DbScriptDeploy.BLL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class ScriptModel
    {
        public ScriptModel()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ProjectId { get; set; }

        public string[] Tags { get; set; }

        public string ScriptUp { get; set; }

        public string ScriptDown { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
