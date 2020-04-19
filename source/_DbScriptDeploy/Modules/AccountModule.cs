using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class AccountModule : BaseModule
    {

        public static class Artefacts
        {
            public static class Routes
            {
                public const string Default = "/";
                public const string Login = "/account/login";
            }
        }

        public AccountModule()
        {
            Get(Artefacts.Routes.Default, x =>
            {
                return this.Response.AsRedirect(Artefacts.Routes.Login);
            });

            Get(Artefacts.Routes.Login, x =>
            {
                return this.View["Login.html"];
            });
        }
    }
}
