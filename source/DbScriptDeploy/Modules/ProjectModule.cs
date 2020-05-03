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
            Get(Route_Default_Get, x =>
            {
                return this.View["Index.html"];
            });
        }
    }
}