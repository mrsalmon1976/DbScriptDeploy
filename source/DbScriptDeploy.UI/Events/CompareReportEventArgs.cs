using DbScriptDeploy.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Events
{
    public class CompareReportEventArgs : EventArgs
    {
        public CompareReportEventArgs(SchemaComparisonResult comparisonResult)
            : base()
        {
            this.ComparisonResult = comparisonResult;
        }

        public SchemaComparisonResult ComparisonResult { get; set; }
    }
}
