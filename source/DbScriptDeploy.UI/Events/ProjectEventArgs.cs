using DbScriptDeploy.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Events
{
    public class ProjectEventArgs : EventArgs
    {
        public ProjectEventArgs(Project project)
            : base()
        {
            this.Project = project;
        }

        public Project Project { get; set; }
    }
}
