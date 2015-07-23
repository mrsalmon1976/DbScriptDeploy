using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Utils
{
    public class ControlUtils
    {
        /// <summary>
        /// Utility method to ensure "hidden" or reserved characters get displayed correctly.
        /// </summary>
        /// <param name="underscores"></param>
        /// <returns></returns>
        public static string EscapeContent(string content, bool showUnderscores = true)
        {
            string result = content;
            if (showUnderscores)
            {
                result = (result ?? "").Replace("_", "__");
            }
            return result;
        }
    }
}
