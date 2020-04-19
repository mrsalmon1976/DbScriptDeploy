using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class ProjectApiModule : BaseSecureApiModule
    {
        public const string Route_Item = "/api/projects";

        public ProjectApiModule()
        {
            Post(Route_Item, x =>
            {
                var result = new { Result = "Ok" };
                return this.Response.AsJson(result);
            });

        }
    }
}
