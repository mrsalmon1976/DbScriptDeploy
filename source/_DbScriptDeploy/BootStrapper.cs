using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Diagnostics;
using DbScriptDeploy.Configuration;
using DbScriptDeploy.Security;

namespace DbScriptDeploy
{

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // register settings first as we will use that below
            IAppSettings settings = new AppSettings();
            container.Register<IAppSettings>(settings);

        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            IAppSettings settings = container.Resolve<IAppSettings>();

            // WebConsole classes and controllers
            container.Register<IUserMapper, UserMapper>();

        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            var formsAuthConfiguration = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "~/account/login",
                UserMapper = container.Resolve<IUserMapper>(),
                DisableRedirect = context.Request.IsAjaxRequest()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

            // set shared ViewBag details here
            context.ViewBag.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            if (Debugger.IsAttached)
            {
                context.ViewBag.AppVersion = DateTime.Now.ToString("yyyyMMddHHmmssttt");
            }
            context.ViewBag.Scripts = new List<string>();
            context.ViewBag.Claims = new List<string>();

            // before the request builds up, if there is a logged in user then set the user info
            pipelines.BeforeRequest.AddItemToEndOfPipeline((ctx) =>
            {
                if (ctx.CurrentUser != null && ctx.CurrentUser.Identity != null)
                {
                    ctx.ViewBag.CurrentUserName = ctx.CurrentUser.Identity.Name;
                    //    if (ctx.CurrentUser.Claims != null)
                    //    {
                    //        ctx.ViewBag.Claims = new List<string>(ctx.CurrentUser.Claims);
                    //    }
                }
                return null;
            });

        }

    }
}
