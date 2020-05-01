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

        public const string Route_Get_Api_Projects_User = "/api/projects/user";

        private readonly IProjectCreateCommand _projectCreateCommand;

        public ProjectApiModule(IProjectCreateCommand projectCreateCommand)
        {
            _projectCreateCommand = projectCreateCommand;

            Post(Route_Post, x =>
            {
                return AddProject();
            });
            //Get(Route_Get_Api_Projects_User, x =>
            //{
            //    ProjectModel result = projectCreateCommand.Execute(projectName);
            //    return this.Response.AsJson(result);
            //});

        }

        public dynamic AddProject()
        {
            var projectName = Request.Form.ProjectName;
            ProjectModel result = _projectCreateCommand.Execute(projectName);
            return this.Response.AsJson(result);
        }
    }
}
