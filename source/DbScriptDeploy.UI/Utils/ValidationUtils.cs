using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace DbScriptDeploy.UI.Utils
{
    public class ValidationUtils
    {

        public static bool ValidateMandatoryInput(PasswordBox passwordBox, params Control[] controlsToDisable) 
        {
            if (passwordBox.Password.Trim().Length == 0)
            {
                ToggleControlValidIndicator(passwordBox, false);
                foreach (Control control in controlsToDisable)
                {
                    control.IsEnabled = false;
                }
                return false;
            }

            ToggleControlValidIndicator(passwordBox, true);
            return true;
        }

        public static bool ValidateMandatoryInput(TextBox textBox, params Control[] controlsToDisable) 
        {
            if (textBox.Text.Trim().Length == 0)
            {
                ToggleControlValidIndicator(textBox, false);
                foreach (Control control in controlsToDisable)
                {
                    control.IsEnabled = false;
                }
                return false;
            }

            ToggleControlValidIndicator(textBox, true);
            return true;
        }

        public static void ToggleControlValidIndicator(Control control, bool isValid) 
        {
            if (!isValid)
            {
                control.BorderBrush = Brushes.Red;
            }
            else 
            {
                control.BorderBrush = new TextBox().BorderBrush;
            }
        }
    }
}
