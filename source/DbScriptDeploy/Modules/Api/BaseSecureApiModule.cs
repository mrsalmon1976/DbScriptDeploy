using DbScriptDeploy.Security;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class BaseSecureApiModule : NancyModule
    {
        public BaseSecureApiModule()
        {
            this.RequiresAuthentication();
        }

    }
}
