using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DbScriptDeploy.UI.Controls
{
    public class WatermarkTextBox : TextBox
    {
        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox),
                   new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }
    }
}
