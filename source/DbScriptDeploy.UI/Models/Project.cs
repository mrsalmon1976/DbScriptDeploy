using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Models
{
	/// <summary>
	/// Represents a project.
	/// </summary>
    public class Project
    {
        public Project()
        {
            this.Id = Guid.NewGuid();
            this.DatabaseInstances = new List<DbEnvironment>();
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
        public List<DbEnvironment> DatabaseInstances { get; set; }

    }
}
