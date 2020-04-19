using DbScriptDeploy.Security;
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
                return null;
            });
        }

        protected void AddScript(string script)
        {
            if (Context.ViewBag.Scripts == null)
            {
                Context.ViewBag.Scripts = new List<string>();
            }
            ((List<string>)Context.ViewBag.Scripts).Add(script);
        }
    }
}
