using Nancy;
using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class AccountModule : BaseModule
    {

        public const string Route_Default = "/";
        public const string Route_Login = "/account/login";
        public const string Route_Logout = "/account/logout";

        public AccountModule()
        {
            Get(Route_Default, x =>
            {
                return this.Response.AsRedirect(Route_Login);
            });

            Get(Route_Login, x =>
            {
                return this.View["Login.html"];
            });
            Get(Route_Logout, x =>
            {
                return this.LogoutAndRedirect(Route_Login);
            });
        }
    }
}
