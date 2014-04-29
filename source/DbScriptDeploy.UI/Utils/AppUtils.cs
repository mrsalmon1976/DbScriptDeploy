using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Utils
{
    public class AppUtils
    {
		public static string AppVersion()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
		}

        public static string BaseDirectory()
        {
            string loc = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(loc);
        }

        public static string CurrentWindowsIdentity()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            return (identity == null ? String.Empty : identity.Name);
        }

    }
}
