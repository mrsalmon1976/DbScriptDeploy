using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class DatabaseTypeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DefaultPort { get; set; }
    }
}
