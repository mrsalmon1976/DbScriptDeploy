using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Models
{
    public class Script
    {
        public Guid Id { get; set; }
		public string Name { get; set; }
		public string ScriptText { get; set; }
		public DateTime CreatedOn { get; set; }
		public string CreatedUser { get; set; }
		public string CreatedAccount { get; set; }
        public bool Archived { get; set; }
    }
}
