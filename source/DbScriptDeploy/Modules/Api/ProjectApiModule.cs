using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Models;
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

        public ProjectApiModule(IProjectCreateCommand projectCreateCommand)
        {
            Post(Route_Post, x =>
            {
                var projectName = Request.Form.ProjectName;
                ProjectModel result = projectCreateCommand.Execute(projectName);
                return this.Response.AsJson(result);
            });

        }
    }
}
