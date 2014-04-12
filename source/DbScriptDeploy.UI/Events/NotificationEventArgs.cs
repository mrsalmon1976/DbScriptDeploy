using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Events
{
    public class NotificationEventArgs
    {
        public NotificationEventArgs(string message)
            : base()
        {
            this.Message = message;
        }

        public string Message { get; set; }
    }
}
