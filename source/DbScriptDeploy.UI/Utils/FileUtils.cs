using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Utils
{
    public class FileUtils
    {
        public static bool IsValidFileName(string s)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (s.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
