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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DbScriptDeploy.UI.Resources;
using DbScriptDeploy.UI.Utils;

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for SelectPanel.xaml
    /// </summary>
    public partial class SelectPanel : UserControl
    {
		private static Brush _borderDefault = Brushes.LightGray;
		public event EventHandler<MouseButtonEventArgs> DeleteButtonMouseUp;
        public event EventHandler<MouseButtonEventArgs> EditButtonMouseUp;

        public SelectPanel()
        {
            InitializeComponent();

			this.BorderBrush = _borderDefault;

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				imgIcon.Source = ImageUtils.ImageSourceFromIcon(Images.db); ;
				imgR.Source = ImageUtils.ImageSourceFromIcon(Images.delete);
                imgEdit.Source = ImageUtils.ImageSourceFromIcon(Images.edit);
			}

        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text 
        { 
            get { return txtText.Text; } 
            set { txtText.Text = value; } 
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            txtText.TextDecorations.Add(TextDecorations.Underline);
            this.Cursor = Cursors.Hand;
            this.BorderBrush = Brushes.SlateBlue;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            txtText.TextDecorations.Clear();
            this.Cursor = Cursors.Arrow;
            this.BorderBrush = _borderDefault;
        }

		private void imgR_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.DeleteButtonMouseUp != null) {
				DeleteButtonMouseUp(this, e);
			}
		}

        private void imgEdit_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.EditButtonMouseUp != null)
            {
                EditButtonMouseUp(this, e);
            }
        }

    }
}
