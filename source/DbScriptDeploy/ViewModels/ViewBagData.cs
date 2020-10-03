using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.ViewModels
{
    public class ViewBagData
    {
        private readonly List<string> _scripts = new List<string>();

        public List<string> Scripts {  get { return _scripts; } }
    }
}
