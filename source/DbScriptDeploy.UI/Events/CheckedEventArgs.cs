using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Events
{
    public class CheckedEventArgs : EventArgs
    {
        public CheckedEventArgs()
            : base()
        {
        }

        public CheckedEventArgs(bool isChecked)
            : this()
        {
            this.IsChecked = IsChecked;
        }

        public bool IsChecked { get; set; }
    }
}
