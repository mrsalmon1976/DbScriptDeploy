using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DbScriptDeploy.UI.Resources;

namespace DbScriptDeploy.UI.Views
{
    /// <summary>
    /// Interaction logic for ProjectDialog.xaml
    /// </summary>
    public partial class ProjectDialog : Window
    {
        private System.Windows.Forms.FolderBrowserDialog dlgFolder = null;
        private Brush _defaultBorderBrush;
		private Project _project = null;

        public ProjectDialog()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
				this.Icon = ImageUtils.ImageSourceFromIcon(Images.app);
				_defaultBorderBrush = txtName.BorderBrush;
                this.Project = new Project();
                InitializeDialogs();
				tbScriptFolderInfo.FontSize = tbScriptFolderInfo.FontSize - 2;
            }

        }

        #region Properties

        /// <summary>
        /// Gets/sets the project object associated with the dialog.
        /// </summary>
		public Project Project
		{
			get
			{
				return _project;
			}
			set
			{
				_project = value;
				txtName.Text = _project.Name;
				txtScriptFolder.Text = _project.ScriptFolder;
			}
		}

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
            this.Project.ScriptFolder = txtScriptFolder.Text;
        }

        private void HandleInputChanges()
        {
            bool isValid = this.ValidateInput();
            if (!isValid)
            {
                btnSave.IsEnabled = false;
                return;
            }

            bool isChanged = false;
            if (this.Project.Name != txtName.Text
                || this.Project.ScriptFolder != txtScriptFolder.Text)
            {
                isChanged = true;
            }

            btnSave.IsEnabled = isChanged;
        }

        private bool ValidateInput()
        {
            // validate input
            if (!ValidationUtils.ValidateMandatoryInput(txtName)) return false;
            if (!ValidationUtils.ValidateMandatoryInput(txtScriptFolder)) return false;

            bool isDirectoryValid = Directory.Exists(txtScriptFolder.Text);
            ValidationUtils.ToggleControlValidIndicator(txtScriptFolder, isDirectoryValid);
            return isDirectoryValid;
        }

        #endregion

        #region Events

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateProject();
            this.DialogResult = true;
            this.Hide();

        }

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            this.HandleInputChanges();
        }

        #endregion

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Hide();
        }

        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            var result = dlgFolder.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.Project.ScriptFolder = dlgFolder.SelectedPath;
                this.txtScriptFolder.Text = this.Project.ScriptFolder;
            }
        }

        private void OnScriptFolderTextChanged(object sender, TextChangedEventArgs e)
        {
            this.HandleInputChanges();
        }



    }
}
