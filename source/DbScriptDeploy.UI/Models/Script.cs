using System;
using System.Collections.Generic;
using System.IO;
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
        
        /// <summary>
        /// Gets/sets the tag applied to the script.  This will be a comment on line one of the file, starting with "--- "
        /// </summary>
        public string Tag { get; set; }
	}
}
