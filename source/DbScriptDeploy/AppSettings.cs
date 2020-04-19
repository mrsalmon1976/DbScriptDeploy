using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy
{
    public interface IAppSettings
    {
        int Port { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public int Port { get; set; }
    }
}
