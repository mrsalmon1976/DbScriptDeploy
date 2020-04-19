using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DbScriptDeploy.Security
{
    public class UserPrincipal : ClaimsPrincipal
    {
        public UserPrincipal()
        {
        }

        public string UserName { get; set; }
    }
}
