using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels;
using Nancy;
using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class BaseModule : NancyModule
    {

        public BaseModule()
        {
            Before.AddItemToStartOfPipeline(ctx =>
            {
                ctx.ViewBag.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
                ctx.ViewBag.Data = new ViewBagData();
                return null;
            });
        }

        protected void AddScript(string script)
        {
            ((ViewBagData)Context.ViewBag.Data).Scripts.Add(script);
        }
    }
}
