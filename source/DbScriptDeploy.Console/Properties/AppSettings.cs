using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbScriptDeploy.Console;
using static DbScriptDeploy.Console.AppConstants;

namespace DbScriptDeploy.Console.Properties
{
    public interface IAppSettings
    {
        int Port { get; set; }

        IEmailSettings EmailSettings { get; set; }

        EmailProvider EmailProvider { get; set; }

    }

    public class AppSettings : IAppSettings
    {
        public int Port { get; set; }

        public IEmailSettings EmailSettings { get; set; }

        public EmailProvider EmailProvider { get; set; }
    }
}
