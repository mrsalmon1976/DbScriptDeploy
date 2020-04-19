using DbScriptDeploy.Security;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules
{
    public class BaseSecureModule : BaseModule
    {
        public BaseSecureModule()
        {
            After.AddItemToEndOfPipeline(ctx =>
            {
                if (ctx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //ctx.Response = Response.AsRedirect(AccountModule.Artefacts.Routes.Login);
                }
            });

            FormsAuthenticationConfiguration config = new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "/account/login",
                UserMapper = new UserMapper()
            };
            FormsAuthentication.Enable(this, config);


            this.RequiresAuthentication();
        }

    }
}
