using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Encoding;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbScriptDeploy.Services;
using DbScriptDeploy.BLL.Exceptions;
using Nancy.ModelBinding;

namespace DbScriptDeploy.Modules.Api
{
    public class ProjectApiModule : BaseSecureApiModule
    {
        public const string Route_Post_AddProject = "/api/project";

        public const string Route_Get_Api_Projects_User = "/api/project/user";

        public const string Route_Get_ProjectEnvironments = "/api/project/{id}/environments";

        public const string Route_Post_Project_Script = "/api/project/{projectId}/script";

        public const string Route_Get_ProjectScripts = "/api/project/{id}/scripts";

        private readonly IDbContext _dbContext;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectViewService _projectViewService;
        private readonly IProjectCreateCommand _projectCreateCommand;
        private readonly IScriptCreateCommand _scriptCreateCommand;

        public ProjectApiModule(IDbContext dbContext, IProjectRepository projectRepo, IProjectViewService projectViewService, IProjectCreateCommand projectCreateCommand, IScriptCreateCommand scriptCreateCommand)
        {
            _dbContext = dbContext;
            _projectRepo = projectRepo;
            _projectViewService = projectViewService;
            _projectCreateCommand = projectCreateCommand;
            _scriptCreateCommand = scriptCreateCommand;

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
                return LoadProjectEnvironments(id);
            });
            Get(Route_Get_ProjectScripts, x =>
            {
                var id = x.id;
                return LoadProjectScripts(id);
            });
            Post(Route_Post_Project_Script, x =>
            {
                return AddScript(x.projectId);
            });

        }

        public dynamic AddProject()
        {
            var projectName = Request.Form.ProjectName;
            _dbContext.BeginTransaction();
            ProjectModel result = _projectCreateCommand.Execute(projectName);
            _dbContext.Commit();

            // only return what we need in the response
            var response = new ProjectViewModel()
            {
                Id = UrlUtility.EncodeNumber(result.Id),
                Name = result.Name
            };
            return this.Response.AsJson(response);
        }

        public dynamic AddScript(string projectId)
        {
            ScriptViewModel scriptViewModel = this.Bind<ScriptViewModel>();

            try
            {
                _dbContext.BeginTransaction();
                ScriptModel scriptModel = _scriptCreateCommand.Execute(scriptViewModel.ToScriptModel());
                ScriptViewModel result = ScriptViewModel.FromScriptModel(scriptModel);
                _dbContext.Commit();
                return Response.AsJson(result);
            }
            catch (ValidationException ve)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = ve.Message
                };
            }
        }

        public dynamic LoadUserProjects()
        {
            UserPrincipal userPrincipal = this.Context.CurrentUser as UserPrincipal;
            List<ProjectModel> projects = _projectRepo.GetAllByUserId(userPrincipal.UserId).ToList();
            IEnumerable<ProjectViewModel> result = projects.Select(x => ProjectViewModel.FromProjectModel(x));
            return this.Response.AsJson(result);

        }

        public dynamic LoadProjectEnvironments(string projectId)
        {
            var environments = _projectViewService.LoadEnvironments(projectId);
            return this.Response.AsJson(environments);
        }

        public dynamic LoadProjectScripts(string projectId)
        {
            var scripts = _projectViewService.LoadScripts(projectId);
            return this.Response.AsJson(scripts);
        }

    }
}
