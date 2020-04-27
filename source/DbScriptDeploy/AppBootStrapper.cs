using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Services;
using DbScriptDeploy.BLL.Validators;
using Microsoft.AspNetCore.Hosting;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Extensions;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy
{
    public class AppBootStrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider { get; }

        private readonly IAppSettings _appSettings;

        public AppBootStrapper(IWebHostEnvironment environment, IAppSettings appSettings)
        {
            RootPathProvider = new AppRootPathProvider(environment);
            _appSettings = appSettings;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Initialise the database only on application start
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DbScriptDeploy.db");

            // make sure we initialise the database and an admin user if one does not exist
            IDbContextFactory dbContextFactory = new DbContextFactory(dbPath);
            container.Register<IDbContextFactory>(dbContextFactory);
            container.Register<IDbContext>(container.Resolve<IDbContextFactory>().GetDbContext());

            this.InitialiseDatabase(dbContextFactory);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            // register database context per request
            IDbContextFactory dbContextFactory = container.Resolve<IDbContextFactory>();
            container.Register<IDbContext>(dbContextFactory.GetDbContext());

            var formsAuthConfiguration = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "/account/login",
                UserMapper = container.Resolve<IUserMapper>(),
                DisableRedirect = context.Request.IsAjaxRequest()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

        }

        private void InitialiseDatabase(IDbContextFactory dbContextFactory)
        {
            using (IDbContext dbc = dbContextFactory.GetDbContext())
            {
                dbc.Initialise();

                IUserRepository userRepo = new UserRepository(dbc);
                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbc);
                IUserCreateCommand createUserCmd = new UserCreateCommand(dbc, new UserValidator(userRepo), new PasswordProvider());
                IUserClaimCreateCommand createUserClaimCmd = new UserClaimCreateCommand(dbc, new UserClaimValidator(userClaimRepo));

                // make sure an administrator exists
                IUserService userService = new UserService(dbc, userRepo, createUserCmd, createUserClaimCmd);
                userService.InitialiseAdminUser();
            }

        }


    }
}
