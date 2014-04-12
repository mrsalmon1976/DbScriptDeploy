using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for CompareReport.xaml
    /// </summary>
    public partial class CompareReport : UserControl
    {
        public CompareReport()
        {
            InitializeComponent();
        }

        public void SetReport(SchemaComparisonResult result)
        {
            IEnumerable<SchemaComparisonResultItem> left = result.Items.Where(x => x.Difference == SchemaDifference.LeftOnly);
            IEnumerable<SchemaComparisonResultItem> right = result.Items.Where(x => x.Difference == SchemaDifference.RightOnly);
            IEnumerable<SchemaComparisonResultItem> differ = result.Items.Where(x => x.Difference == SchemaDifference.Differ);
            IEnumerable<SchemaComparisonResultItem> same = result.Items.Where(x => x.Difference == SchemaDifference.None);

            string html = Scripts.ComparisonReport;

            html = html.Replace("{SOURCE}", result.Source);
            html = html.Replace("{TARGET}", result.Target);
            html = html.Replace("{SOURCE_NOT_TARGET}", GenerateList(left));
            html = html.Replace("{TARGET_NOT_SOURCE}", GenerateList(right));
            html = html.Replace("{DIFFER}", GenerateList(differ));
            html = html.Replace("{SAME}", GenerateList(same));

            string path = Path.Combine(result.Path, "Result.html");
            File.WriteAllText(path, html);

            webBrowser.Navigate(new Uri(path));
            
        }

        private string GenerateList(IEnumerable<SchemaComparisonResultItem> items)
        {
            if (!items.Any())
            {
                return "<li>None</li>";
            }

            IEnumerable<string> sItems = items.OrderBy(x => x.ObjectName).Select(x => "<li>" + x.ObjectName + "</li>");
            return String.Join("", sItems);

        }
    }
}
