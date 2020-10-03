using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class ProjectModule : BaseSecureModule
    {
        public const string Route_Default_Get = "/project/{id}";

        public const string Route_Editor_Get = "/project/{id}/editor";

        public ProjectModule()
        {
            Before.AddItemToEndOfPipeline(ctx =>
            {
                return null;
            });

            Get(Route_Default_Get, x =>
            {
                this.AddScript("/Content/js/pages/project.js");
                var model = new { ProjectId = x.id };
                return this.View["Index.html", model];
            });

            Get(Route_Editor_Get, x =>
            {
                this.AddScript("/Content/js/pages/project/editor.js");
                var model = new { ProjectId = x.id };
                return this.View["Editor/Index.html", model];
            });
        }
    }
}
