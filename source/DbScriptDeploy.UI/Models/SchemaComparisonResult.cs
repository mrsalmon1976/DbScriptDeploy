using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.UI.Models
{
    public enum SchemaObject
    {
        None = 0,
        ForeignKey,
        Function,
        StoredProcedure,
        Table,
        View,
        Trigger
    }

    public enum SchemaDifference
    {
        None = 0,
        LeftOnly = 1,
        Differ = 2,
        RightOnly = 3
    }

    public class SchemaComparisonResultItem
    {
        public SchemaComparisonResultItem(string objectName, SchemaDifference difference) 
        {
            this.ObjectName = objectName;
            this.Difference = difference;
        }

        public SchemaDifference Difference { get; set; }
        public string ObjectName { get; set; }
    }

    public class SchemaComparisonResult
    {
        public SchemaComparisonResult()
        {
            this.Items = new List<SchemaComparisonResultItem>();
        }

        public string Source { get; set; }

        public string Target { get; set; }

        public string Path { get; set; }

        public List<SchemaComparisonResultItem> Items { get; private set; }
    }
}
