using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DbScriptDeploy.UI.Views
{
    /// <summary>
    /// Interaction logic for ProjectDialog.xaml
    /// </summary>
    public partial class ProjectDialog : Window
    {
        private System.Windows.Forms.FolderBrowserDialog dlgFolder = null;
        private Brush _defaultBorderBrush;

        public ProjectDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _defaultBorderBrush = txtName.BorderBrush;
                this.Project = new Project();
                InitializeDialogs();
				tbScriptFolderInfo.FontSize = tbScriptFolderInfo.FontSize - 2;
            }

        }

        #region Properties

        public Project Project { get; set; }

        #endregion

        #region Private Methods

        private void InitializeDialogs()
        {
            dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            //dlgSaveFile.DefaultExt = Constants.ProjectFileExtension;
            //dlgSaveFile.Filter = String.Format("DbScriptDeploy.UI project files ({0})|*{0}", Constants.ProjectFileExtension); 
        }

        private void UpdateProject()
        {
            this.Project.Name = txtName.Text;
        }

        private void ValidateInput()
        {
            // validate input
            if (!ValidationUtils.ValidateMandatoryInput(txtName, btnSave)) return;

            if (!Directory.Exists(txtScriptFolder.Text))
            {
                txtScriptFolder.BorderBrush = Brushes.Red;
                btnSave.IsEnabled = false;
                return;
            }

            bool isChanged = false;
            
            if (this.Project.Name != txtName.Text)
            {
                isChanged = true;
            }

            btnSave.IsEnabled = isChanged;
        }

        #endregion

        #region Events

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateProject();
            this.DialogResult = true;
            this.Hide();

        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ValidateInput();
        }

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = dlgFolder.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.Project.ScriptFolder = dlgFolder.SelectedPath;
                this.txtScriptFolder.Text = this.Project.ScriptFolder;
            }
        }

        private void txtScriptFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ValidateInput();
        }



    }
}
