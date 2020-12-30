using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Utilities;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class EnvironmentApiModule : BaseSecureApiModule
    {
        public const string Route_Post_AddEnvironment = "/api/environment";

        private readonly IDbContext _dbContext;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectCreateCommand _projectCreateCommand;

        public EnvironmentApiModule(IDbContext dbContext, IProjectRepository projectRepo, IProjectCreateCommand projectCreateCommand)
        {
            _dbContext = dbContext;
            _projectRepo = projectRepo;
            _projectCreateCommand = projectCreateCommand;

            //Post(Route_Post_AddProject, x =>
            //{
            //    return AddProject();
            //});
            //Get(Route_Get_Api_Projects_User, x =>
            //{
            //    return LoadUserProjects();
            //});
            //Get(Route_Get_ProjectEnvironments, x =>
            //{
            //    var id = x.id;
            //    return LoadEnvironments(id);
            //});

        }

        public dynamic AddEnvironment()
        {
            //var projectName = Request.Form.ProjectName;
            //_dbContext.BeginTransaction();
            //ProjectModel result = _projectCreateCommand.Execute(projectName);
            //_dbContext.Commit();

            //// only return what we need in the response
            //var response = new ProjectViewModel()
            //{
            //    Id = UrlUtility.EncodeNumber(result.Id),
            //    Name = result.Name
            //};
            //return this.Response.AsJson(response);
            throw new NotImplementedException();
        }

    }
}
