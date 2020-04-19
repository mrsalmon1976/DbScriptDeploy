using DbScriptDeploy.BLL.Data;
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
            IDbContextFactory dbContextFactory = new DbContextFactory(dbPath);
            container.Register<IDbContextFactory>(dbContextFactory);
            using (IDbContext dbc = dbContextFactory.GetDbContext())
            {
                dbc.Initialise();

                // make sure an administrator exists
                //IUserRepository userRepo = new UserRepository(dbc);
                //IUserValidator userValidator = new UserValidator(userRepo);
                //IUserService userService = new UserService(userRepo, new CreateUserCommand(dbc, userValidator, new PasswordProvider()));
                //userService.InitialiseAdminUser();
            }

        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var formsAuthConfiguration = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "/account/login",
                UserMapper = container.Resolve<IUserMapper>(),
                DisableRedirect = context.Request.IsAjaxRequest()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

        }
    }
}
