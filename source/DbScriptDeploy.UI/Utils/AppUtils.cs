using System;
using System.Collections.Generic;
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
        public static string BaseDirectory()
        {
            Uri uri = new Uri(Path.GetDirectoryName(Assembly.GetAssembly(typeof(AppUtils)).CodeBase));
            return uri.PathAndQuery.Replace("/", Path.DirectorySeparatorChar.ToString());
        }

        public static string CurrentWindowsIdentity()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            return (identity == null ? String.Empty : identity.Name);
        }
    }
}
