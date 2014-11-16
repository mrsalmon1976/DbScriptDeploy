using DbScriptDeploy.UI.Data;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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

namespace DbScriptDeploy.UI.Controls
{
    /// <summary>
    /// Interaction logic for EnvironmentDialog.xaml
    /// </summary>
    public partial class EnvironmentDialog : Window
    {
        private BackgroundWorker bgWorker;
        private DbEnvironment _environment;

        public EnvironmentDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
				this.Icon = ImageUtils.ImageSourceFromIcon(Images.app);
				this.DbEnvironment = new DbEnvironment();

                lblMessage.Content = String.Empty;
            }
        }

        public DbEnvironment DbEnvironment 
        {
            get
            {
                return this._environment;
            }
            set
            {
                this._environment = value;
                txtName.Text = _environment.Name;
                txtServer.Text = _environment.Host;
                txtPort.Text = _environment.Port.ToString();
                txtCatalog.Text = _environment.Catalog;
                radAuthTypeSqlServer.IsChecked = (_environment.AuthType == AuthType.SqlServer);
                radAuthTypeWindows.IsChecked = (_environment.AuthType == AuthType.Windows);
                txtUserName.Text = _environment.UserName;
                txtPassword.Password = _environment.Password;
            }
        }

        private void ToggleCredentials(bool enabled)
        {
            if (lblUserName == null) return;

            lblUserName.IsEnabled = enabled;
            txtUserName.IsEnabled = enabled;
            lblPassword.IsEnabled = enabled;
            txtPassword.IsEnabled = enabled;

            if (!enabled)
            {
                ValidationUtils.ToggleControlValidIndicator(txtUserName, true);
                ValidationUtils.ToggleControlValidIndicator(txtPassword, true);
            }
        }

        private void ValidateInput()
        {
            if (!ValidationUtils.ValidateMandatoryInput(txtName, btnSave)) return;
            if (!ValidationUtils.ValidateMandatoryInput(txtServer, btnSave)) return;
            if (!ValidationUtils.ValidateMandatoryInput(txtPort, btnSave)) return;
            if (!ValidationUtils.ValidateMandatoryInput(txtCatalog, btnSave)) return;

            int port;
            if (!Int32.TryParse(txtPort.Text, out port))
            {
                ValidationUtils.ToggleControlValidIndicator(txtPort, false);
                return;
            }

            if (radAuthTypeSqlServer.IsChecked == true)
            {
                if (!ValidationUtils.ValidateMandatoryInput(txtUserName, btnSave)) return;
                if (!ValidationUtils.ValidateMandatoryInput(txtPassword, btnSave)) return;
            }

            bool isChanged = false;
            if (
                this.DbEnvironment.AuthType != GetSelectedAuthType()
                || this.DbEnvironment.Catalog != txtCatalog.Text
                || this.DbEnvironment.Host != txtServer.Text
                || this.DbEnvironment.Name != txtName.Text
                || this.DbEnvironment.Password != txtPassword.Password
                || this.DbEnvironment.Port.ToString() != txtPort.Text
                || this.DbEnvironment.UserName != txtUserName.Text
                )
            {
                isChanged = true;
            }

            btnSave.IsEnabled = isChanged;
        }

        private AuthType GetSelectedAuthType()
        {
            if (radAuthTypeWindows.IsChecked == true) return AuthType.Windows;
            return AuthType.SqlServer;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pnlMain.IsEnabled = false;

            this.DbEnvironment.Name = txtName.Text;
            this.DbEnvironment.Host = txtServer.Text;
            this.DbEnvironment.Port = Convert.ToInt32(txtPort.Text);
            this.DbEnvironment.Catalog = txtCatalog.Text;

            if (radAuthTypeWindows.IsChecked == true)
            {
                this.DbEnvironment.AuthType = AuthType.Windows;
                this.DbEnvironment.UserName = String.Empty;
                this.DbEnvironment.Password = String.Empty;
            }
            else
            {
                this.DbEnvironment.AuthType = AuthType.SqlServer;
                this.DbEnvironment.UserName = txtUserName.Text;
                this.DbEnvironment.Password = txtPassword.Password;
            }

            lblMessage.Content = "Testing connection...";

            bgWorker = new BackgroundWorker();
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerAsync();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (IDbConnection conn = DbHelper.GetDbConnection(this.DbEnvironment))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Exception ex = e.Result as Exception;

            lblMessage.Content = String.Empty;
            pnlMain.IsEnabled = true;
            bgWorker.Dispose();
            bgWorker = null;

            if (ex != null)
            {
                string msg = String.Format("Unable to connect using the captured values.{0}{0}{1}", Environment.NewLine, ex.Message);
                MessageBox.Show(this, msg, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.DialogResult = true;
                this.Hide();
            }

        }

        private void txtServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateInput();
        }

        private void radAuthTypeWindows_Checked(object sender, RoutedEventArgs e)
        {
            ToggleCredentials(false);
            ValidateInput();
        }

        private void radAuthTypeSqlServer_Checked(object sender, RoutedEventArgs e)
        {
            ToggleCredentials(true);
            ValidateInput();
        }

        private void txtCatalog_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

    }
}
