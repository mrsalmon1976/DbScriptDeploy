using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.Security;
using Nancy;
using Nancy.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class ProjectApiModule : BaseSecureApiModule
    {
        public const string Route_Post_AddProject = "/api/projects";

        public const string Route_Get_Api_Projects_User = "/api/projects/user";

        public const string Route_Get_ProjectEnvironments = "/api/projects/environments/{id}";

        private readonly IDbContext _dbContext;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectCreateCommand _projectCreateCommand;

        public ProjectApiModule(IDbContext dbContext, IProjectRepository projectRepo, IProjectCreateCommand projectCreateCommand)
        {
            _dbContext = dbContext;
            _projectRepo = projectRepo;
            _projectCreateCommand = projectCreateCommand;

            Post(Route_Post_AddProject, x =>
            {
                return AddProject();
            });
            Get(Route_Get_Api_Projects_User, x =>
            {
                return LoadUserProjects();
            });
            Get(Route_Get_ProjectEnvironments, x =>
            {
                var id = x.id;
                return LoadEnvironments(id);
            });

        }

        public dynamic AddProject()
        {
            var projectName = Request.Form.ProjectName;
            _dbContext.BeginTransaction();
            ProjectModel result = _projectCreateCommand.Execute(projectName);
            _dbContext.Commit();
            return this.Response.AsJson(result);
        }

        public dynamic LoadUserProjects()
        {
            UserPrincipal userPrincipal = this.Context.CurrentUser as UserPrincipal;
            List<ProjectModel> projects = _projectRepo.GetAllByUserId(userPrincipal.UserId).ToList();
            return this.Response.AsJson(projects);

        }

        public dynamic LoadEnvironments(Guid projectId)
        {
            List<EnvironmentModel> environments = new List<EnvironmentModel>();
            for (int i=0; i< 10; i++)
            {
                environments.Add(new EnvironmentModel() { Name = $"Env {i}", HostName = $"Host {i}" });
            }
            return this.Response.AsJson(environments);
        }
    }
}
