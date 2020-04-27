using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class ProjectApiModule : BaseSecureApiModule
    {
        public const string Route_Post = "/api/projects";

        public ProjectApiModule()
        {
            Post(Route_Post, x =>
            {
                var projectName = Request.Form.ProjectName;//["Name"];
                //this.bin
                var result = new { Result = "Ok" };
                return this.Response.AsJson(result);
            });

        }
    }
}
