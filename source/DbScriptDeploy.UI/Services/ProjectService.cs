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
using SystemWrapper.IO;
using StructureMap;

namespace DbScriptDeploy.UI.Services
{
    public interface IProjectService
    {
        event EventHandler<ProjectEventArgs> ProjectAdded;
		event EventHandler<ProjectEventArgs> ProjectDeleted;
		event EventHandler<ProjectEventArgs> ProjectUpdated;

		/// <summary>
		/// Deletes a project.
		/// </summary>
		/// <param name="project"></param>
		void DeleteProject(Project project);

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
		private readonly IJsonPersistenceService _jsonPersistenceService = null;
        private List<Project> _projects = new List<Project>();

        public event EventHandler<ProjectEventArgs> ProjectAdded;
		public event EventHandler<ProjectEventArgs> ProjectDeleted;
        public event EventHandler<ProjectEventArgs> ProjectUpdated;

        public ProjectService(string projectFilePath, IJsonPersistenceService persistenceService) 
        {
			_jsonPersistenceService = persistenceService;
			_projectFilePath = projectFilePath;
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

			IFileWrap fileWrap = ObjectFactory.GetInstance<IFileWrap>();
            if (!fileWrap.Exists(_projectFilePath))
            {
                return Enumerable.Empty<Project>();
            }
            string projects = fileWrap.ReadAllText(_projectFilePath);

            _projects = JsonConvert.DeserializeObject<IEnumerable<Project>>(projects)
                .OrderBy(x => x.Name)
                .ToList();
            return _projects;
        }

		public void DeleteProject(Project project)
		{
			Project p = this._projects.FirstOrDefault(x => x.Id == project.Id);
			if (p == null)
			{
				return;
			}
			_projects.Remove(p);
			_jsonPersistenceService.WriteFile(_projectFilePath, _projects);

			if (this.ProjectDeleted != null)
			{
				this.ProjectDeleted(this, new ProjectEventArgs(project));
			}
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
			_jsonPersistenceService.WriteFile(_projectFilePath, _projects);

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
