using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Utils
{
    public class ScintillaUtils
    {
        public static Scintilla InitSqlEditor()
        {
            Scintilla scintilla = new Scintilla();
            scintilla.ConfigurationManager.Language = "mssql";
            scintilla.Margins[0].Width = 20;
            //scintilla.Font = new System.Windows.Controls.TextBox().Font
            return scintilla;
        }
    }
}
