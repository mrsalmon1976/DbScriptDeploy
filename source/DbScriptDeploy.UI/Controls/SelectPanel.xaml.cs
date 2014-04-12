using System;
using System.Collections.Generic;
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

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for SelectPanel.xaml
    /// </summary>
    public partial class SelectPanel : UserControl
    {
        private Brush _borderDefault;
        //public event EventHandler<MouseButtonEventArgs> MouseUp;
        //public event EventHandler<MouseButtonEventArgs> MouseDown;

        public SelectPanel()
        {
            InitializeComponent();

            this._borderDefault = this.BorderBrush;
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
            this.BorderBrush = Brushes.Blue;
            
            //this.BorderThickness.Top = this.BorderThickness.Top + 2;//
        }
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            txtText.TextDecorations.Clear();
            this.Cursor = Cursors.Arrow;
            this.BorderBrush = _borderDefault;
        }

    }
}
