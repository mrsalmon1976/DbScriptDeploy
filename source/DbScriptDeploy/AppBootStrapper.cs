using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Validators;
using DbScriptDeploy.BLL.Security;
using Microsoft.AspNetCore.Hosting;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Cryptography;
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
        private static CryptographyConfiguration _cryptographyConfiguration;


        public AppBootStrapper(IWebHostEnvironment environment, IAppSettings appSettings)
        {
            RootPathProvider = new AppRootPathProvider(environment);
            _appSettings = appSettings;

        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // do not call base, we don't want auto registration to happen
            base.ApplicationStartup(container, pipelines);

            // set up the crypto config
            _cryptographyConfiguration = new CryptographyConfiguration(
                new AesEncryptionProvider(new PassphraseKeyGenerator($"AES_{_appSettings.SecureKey}", new byte[] { 101, 2, 103, 4, 105, 6, 107, 8 })),
                new DefaultHmacProvider(new PassphraseKeyGenerator($"HMAC_{_appSettings.SecureKey}", new byte[] { 101, 2, 103, 4, 105, 6, 107, 8 })));

            // Initialise the database only on application start
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DbScriptDeploy.db");

            // make sure we initialise the database and an admin user if one does not exist
            IDbContextFactory dbContextFactory = new DbContextFactory(dbPath);
            container.Register<IDbContextFactory>(dbContextFactory);
            container.Register<IDbContext>(dbContextFactory.GetDbContext());

            // validators
            container.Register<IEnvironmentValidator, EnvironmentValidator>();
            container.Register<IProjectValidator, ProjectValidator>();
            container.Register<IScriptValidator, ScriptValidator>();
            container.Register<IUserClaimValidator, UserClaimValidator>();
            container.Register<IUserValidator, UserValidator>();

            container.Register<IPasswordProvider, PasswordProvider>();
            container.Register<DbScriptDeploy.BLL.Security.IEncryptionProvider, AESGCM>();

            this.RegisterDbDependencies(container);

            this.InitialiseDatabase(dbContextFactory);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            // register database context per request
            IDbContextFactory dbContextFactory = container.Resolve<IDbContextFactory>();
            IDbContext dbContext = dbContextFactory.GetDbContext();
            container.Register<IDbContext>(dbContext);
            this.RegisterDbDependencies(container);

            var formsAuthConfiguration = new FormsAuthenticationConfiguration()
            {
                CryptographyConfiguration = _cryptographyConfiguration,
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
                // initialise the database
                dbc.Initialise();

                ILookupRepository lookupRepo = new LookupRepository(dbc);
                IUserRepository userRepo = new UserRepository(dbc);

                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbc);
                
                IUserCreateCommand createUserCmd = new UserCreateCommand(dbc, new UserValidator(userRepo), new PasswordProvider());
                IUserClaimCreateCommand createUserClaimCmd = new UserClaimCreateCommand(dbc, new UserClaimValidator(userClaimRepo));
                IDesignationCreateCommand createDesignationCmd = new DesignationCreateCommand(dbc, new DesignationValidator());

                dbc.BeginTransaction();

                // make sure an administrator exists
                IUserInitialiseAdminCommand cmd = new UserInitialiseAdminCommand(dbc, userRepo, createUserCmd, createUserClaimCmd);
                cmd.Execute();

                // initialise designation info
                if (lookupRepo.GetAllDesignations().Count() == 0)
                {
                    createDesignationCmd.Execute("Development");
                    createDesignationCmd.Execute("Staging");
                    createDesignationCmd.Execute("Production");
                }

                dbc.Commit();
            }

        }

        private void RegisterDbDependencies(TinyIoCContainer container)
        {
            // repositories
            container.Register<IEnvironmentRepository, EnvironmentRepository>();
            container.Register<ILookupRepository, LookupRepository>();
            container.Register<IProjectRepository, ProjectRepository>();
            container.Register<IScriptRepository, ScriptRepository>();
            container.Register<IUserClaimRepository, UserClaimRepository>();
            container.Register<IUserRepository, UserRepository>();

            // commands
            container.Register<IEnvironmentCreateCommand, EnvironmentCreateCommand>();
            container.Register<IProjectCreateCommand, ProjectCreateCommand>();
            container.Register<IScriptCreateCommand, ScriptCreateCommand>();
            container.Register<IUserClaimCreateCommand, UserClaimCreateCommand>();
            container.Register<IUserCreateCommand, UserCreateCommand>();

        }


    }
}
