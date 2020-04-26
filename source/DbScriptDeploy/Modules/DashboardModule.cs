using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class DashboardModule : BaseSecureModule
    {

        public const string Route_Default_Get = "/dashboard";

        public DashboardModule()
        {
            Get(Route_Default_Get, x =>
            {
                return this.View["Index.html"];
            });
        }
    }
}
