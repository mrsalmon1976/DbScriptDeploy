using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class DashboardModule : BaseSecureModule
    {

        public static class Artefacts
        {
            public static class Routes
            {
                public const string Default = "/dashboard";
            }
        }

        public DashboardModule()
        {
            Get(Artefacts.Routes.Default, x =>
            {
                return this.View["Login.html"];
            });
        }
    }
}
