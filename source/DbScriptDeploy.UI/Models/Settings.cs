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
    public class Settings
    {
        public Settings()
        {
        }

        public string ArchiveCommand { get; set; }

        public static Settings DefaultSettings()
        {
            Settings settings = new Settings();
            settings.ArchiveCommand = "git";
            return settings;
        }
    }
}
