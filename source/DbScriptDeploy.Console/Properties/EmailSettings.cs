using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console.Properties
{
    public interface IEmailSettings
    {
        string DefaultFromAddress { get; }
    }
    public class EmailSettings : IEmailSettings
    {
        public string DefaultFromAddress { get; set; }
    }
}
