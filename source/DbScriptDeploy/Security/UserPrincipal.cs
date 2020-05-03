using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DbScriptDeploy.Security
{
    public class UserPrincipal : ClaimsPrincipal
    {
        public UserPrincipal(Guid userId, IIdentity identity) : base(identity)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; set; }

        public string UserName { get; set; }
    }
}
