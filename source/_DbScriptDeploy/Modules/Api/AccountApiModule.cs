using Nancy;
using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class AccountApiModule : NancyModule
    {
        public static class Artefacts
        {
            public static class Routes
            {
                public const string Login = "/api/login";
            }
        }

        public AccountApiModule()
        {
            Post(Artefacts.Routes.Login, x =>
            {
                return this.Login(Guid.NewGuid(), DateTime.Now.AddDays(30));
            });
        }
    }
}
