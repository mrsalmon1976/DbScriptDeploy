using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Models
{
    public class Project
    {
        public Project()
        {
            this.Id = Guid.NewGuid();
            this.DatabaseInstances = new List<DatabaseInstance>();
        }

        /// <summary>
        /// Gets/sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the script folder.
        /// </summary>
        public string ScriptFolder { get; set; }

        /// <summary>
        /// Gets or sets the database environments associated with the project.
        /// </summary>
        public List<DatabaseInstance> DatabaseInstances { get; set; }

    }
}
