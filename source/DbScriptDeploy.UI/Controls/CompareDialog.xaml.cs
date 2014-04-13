using DbScriptDeploy.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using DbScriptDeploy.UI.Utils;
using DbScriptDeploy.UI.Resources;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for CompareDialog.xaml
    /// </summary>
    public partial class CompareDialog : Window
    {
        public CompareDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {

				this.Icon = ImageUtils.ImageSourceFromIcon(Images.app);
				this.Loaded += CompareDialog_Loaded;
            }
        }

        void CompareDialog_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<DbEnvironment> instances = this.Project.DatabaseInstances.OrderBy(x => x.Name);
            cbDatabaseInstances.Items.Add("");
            foreach (DbEnvironment dbInstance in instances)
            {
                // TODO: Need to add an ID rather than this
                if (dbInstance.Host == this.SourceInstance.Host && dbInstance.Catalog == this.SourceInstance.Catalog)
                    continue;

                cbDatabaseInstances.Items.Add(dbInstance);
            }

        }

        public DbEnvironment SourceInstance { get; set; }

        public DbEnvironment TargetInstance { get; set; }

        public Project Project { get; set; }

        

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            this.TargetInstance = (DbEnvironment)cbDatabaseInstances.SelectedItem;
            this.DialogResult = true;
        }

        private void cbDatabaseInstances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = cbDatabaseInstances.SelectedValue.ToString();
            btnCompare.IsEnabled = (item != String.Empty);
        }
    }
}
