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
using DbScriptDeploy.UI.Resources;
using DbScriptDeploy.UI.Utils;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				this.Icon = ImageUtils.ImageSourceFromIcon(Images.app);
			}

        }

        public string Message
        {
            get
            {
                return lblMessage.Content.ToString();
            }
            set
            {
                lblMessage.Content = value;
            }
        }
    }
}
