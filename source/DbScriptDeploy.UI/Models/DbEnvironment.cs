using DbScriptDeploy.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Models
{
    public enum AuthType 
    {
        Windows = 0,
        SqlServer = 1
    }

    public class DbEnvironment
    {
        public DbEnvironment()
        {
            this.AuthType = Models.AuthType.Windows;
        }

        public string Name { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Catalog { get; set; }

        public AuthType AuthType { get; set; } 

        public string UserName { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            string result = Name;
            if (Port != DbHelper.DefaultPort) 
            {
                result = String.Format("{0}, {1}", result, Port);
            }

            string userName = (this.AuthType == Models.AuthType.Windows ? "Windows Auth" : this.UserName);
            return String.Format("{0} ({1})", result, userName);
        }
    }
}
