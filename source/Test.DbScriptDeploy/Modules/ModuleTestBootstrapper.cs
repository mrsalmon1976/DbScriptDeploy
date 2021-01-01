using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.IO;
using NSubstitute;
using System.Security.Principal;
using Nancy.Security;
using Nancy.ViewEngines.Razor;
using NUnit.Framework;
using System.Security.Claims;
using DbScriptDeploy.Security;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Validators;
using DbScriptDeploy.BLL.Commands;

namespace Test.DbScriptDeploy.Modules
{

    /// <summary>
    /// Bootstrapper for module unit tests.
    /// </summary>
    public class ModuleTestBootstrapper : DefaultNancyBootstrapper
    {
        private IRootPathProvider _rootPathProvider;

        public ModuleTestBootstrapper()
        {
            _rootPathProvider = new TestRootPathProvider();
        }

        public Action<TinyIoCContainer> ApplicationStartupCallback { get; set; }

        public Action<TinyIoCContainer> ConfigureRequestContainerCallback { get; set; }

        public Action<TinyIoCContainer, IPipelines, NancyContext> ConfigureRequestStartupCallback { get; set; }

        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                return _rootPathProvider;
            }
        }
        /// <summary>
        /// Gets/sets the current user - set this to null if you want to simulate no auth.
        /// </summary>
        public ClaimsPrincipal CurrentUser { get; set; }

        /// <summary>
        /// Simulates a login and returns the user created.
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal Login()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal(new GenericIdentity("test"));
            this.CurrentUser = principal;
            return this.CurrentUser;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            if (this.ApplicationStartupCallback != null)
            {
                this.ApplicationStartupCallback(container);
            }

            this.Conventions.ViewLocationConventions.Add((viewName, model, context) =>
            {
                return string.Concat("Views/Shared/", viewName);
            });
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, UserMapper>();
            container.Register<IUserValidator>(Substitute.For<IUserValidator>());

            // register database context per request
            container.Register<IDbContext>(Substitute.For<IDbContext>());
            container.Register<IEnvironmentRepository>(Substitute.For<IEnvironmentRepository>());
            container.Register<IProjectRepository>(Substitute.For<IProjectRepository>());
            container.Register<IUserRepository>(Substitute.For<IUserRepository>());

            container.Register<IEnvironmentCreateCommand>(Substitute.For<EnvironmentCreateCommand>());

            if (this.ConfigureRequestContainerCallback != null)
            {
                this.ConfigureRequestContainerCallback(container);
            }
            
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            if (this.ConfigureRequestStartupCallback != null)
            {
                this.ConfigureRequestStartupCallback(container, pipelines, context);
            }
            //var formsAuthConfiguration = new FormsAuthenticationConfiguration()
            //{
            //    RedirectUrl = "~/login",
            //    UserMapper = container.Resolve<IUserMapper>(),
            //};
            //FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
            context.ViewBag.Scripts = new List<string>();
            context.ViewBag.Claims = new List<string>();
            context.CurrentUser = this.CurrentUser;
            //context.ViewBag.Projects = new List<ProjectModel>();
            if (this.CurrentUser != null)
            {
                context.ViewBag.CurrentUserName = this.CurrentUser.Identity.Name;
            }

        }

    }
}
