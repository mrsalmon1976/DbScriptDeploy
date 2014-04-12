using Newtonsoft.Json;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.UI.Events;

namespace DbScriptDeploy.UI.Services
{
    public interface IProjectService
    {
        event EventHandler<ProjectEventArgs> ProjectAdded;

        /// <summary>
        /// Loads all projects opened previously by the user.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Project> LoadProjects();

        /// <summary>
        /// Saves the project.
        /// </summary>
        /// <param name="project">The project.</param>
        void SaveProject(Project project);
    }

    public class ProjectService : IProjectService
    {
        private readonly string _projectFilePath = null;
        private List<Project> _projects = new List<Project>();

        public event EventHandler<ProjectEventArgs> ProjectAdded;
        public event EventHandler<ProjectEventArgs> ProjectUpdated;

        public ProjectService() 
        {
            _projectFilePath = Path.Combine(AppUtils.BaseDirectory(), Constants.UserProjectFileName);
        }


        /// <summary>
        /// Loads all projects opened previously by the user.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Project> LoadProjects()
        {
            if (_projects.Count > 0) 
            {
                return _projects;
            }

            if (!File.Exists(_projectFilePath))
            {
                return Enumerable.Empty<Project>();
            }
            string projects = File.ReadAllText(_projectFilePath);

            _projects = JsonConvert.DeserializeObject<IEnumerable<Project>>(projects)
                .OrderBy(x => x.Name)
                .ToList();
            return _projects;
        }

        public void SaveProject(Project project)
        {
            bool isNew = true;
            Project p = this._projects.FirstOrDefault(x => x.Id == project.Id);
            if (p != null)
            {
                _projects.Remove(p);
                isNew = false;
            }
            _projects.Add(project);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            string projects = JsonConvert.SerializeObject(_projects, settings);
            File.WriteAllText(_projectFilePath, projects);

            if (isNew && this.ProjectAdded != null)
            {
                this.ProjectAdded(this, new ProjectEventArgs(project));
            }
            if (!isNew && this.ProjectUpdated != null)
            {
                this.ProjectUpdated(this, new ProjectEventArgs(project));
            }

        }

    }
}
