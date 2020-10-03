﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class ProjectModule : BaseSecureModule
    {
        public const string Route_Default_Get = "/project/{id}";

        public ProjectModule()
        {
            Before.AddItemToEndOfPipeline(ctx =>
            {
                this.AddScript("/Content/js/pages/project.js");
                return null;
            });

            Get(Route_Default_Get, x =>
            {
                var model = new { ProjectId = x.id };
                return this.View["Index.html", model];
            });
        }
    }
}
