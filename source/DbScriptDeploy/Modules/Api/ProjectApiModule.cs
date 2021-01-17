﻿using DbScriptDeploy.BLL.Commands;
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

namespace DbScriptDeploy.Modules.Api
{
    public class ProjectApiModule : BaseSecureApiModule
    {
        public const string Route_Post_AddProject = "/api/project";

        public const string Route_Get_Api_Projects_User = "/api/project/user";

        public const string Route_Get_ProjectEnvironments = "/api/project/{id}/environments";

        public const string Route_Post_Project_Script = "/api/project/{projectId}/script";

        private readonly IDbContext _dbContext;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectViewService _projectViewService;
        private readonly IProjectCreateCommand _projectCreateCommand;

        public ProjectApiModule(IDbContext dbContext, IProjectRepository projectRepo, IProjectViewService projectViewService, IProjectCreateCommand projectCreateCommand)
        {
            _dbContext = dbContext;
            _projectRepo = projectRepo;
            _projectViewService = projectViewService;
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
                return LoadProjectEnvironments(id);
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
            //EnvironmentViewModel environmentModel = this.Bind<EnvironmentViewModel>();

            try
            {
                _dbContext.BeginTransaction();
                //EnvironmentModel model = _environmentCreateCommand.Execute(environmentModel.ToEnvironmentModel());
                //EnvironmentViewModel result = EnvironmentViewModel.FromEnvironmentModel(model);
                string result = "ok";
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
    }
}
