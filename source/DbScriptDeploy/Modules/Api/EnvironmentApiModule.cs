using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Routing;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbScriptDeploy.BLL.Exceptions;

namespace DbScriptDeploy.Modules.Api
{
    public class EnvironmentApiModule : BaseSecureApiModule
    {
        public const string Route_Post_AddEnvironment = "/api/environment";

        private readonly IDbContext _dbContext;
        private readonly IEnvironmentRepository _environmentRepo;
        private readonly IEnvironmentCreateCommand _environmentCreateCommand;

        public EnvironmentApiModule(IDbContext dbContext, IEnvironmentRepository projectRepo, IEnvironmentCreateCommand projectCreateCommand)
        {
            _dbContext = dbContext;
            _environmentRepo = projectRepo;
            _environmentCreateCommand = projectCreateCommand;

            Post(Route_Post_AddEnvironment, x =>
            {
                return AddEnvironment();
            });
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
            EnvironmentViewModel environmentModel = this.Bind<EnvironmentViewModel>();

            try
            {
                _dbContext.BeginTransaction();
                EnvironmentModel model = _environmentCreateCommand.Execute(environmentModel.ToEnvironmentModel());
                EnvironmentViewModel result = EnvironmentViewModel.FromEnvironmentModel(model);
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

    }
}
