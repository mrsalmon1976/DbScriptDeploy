using Nancy;
using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DbScriptDeploy.Security
{
    public class UserMapper : IUserMapper
    {
        /// <summary>
        /// Get the real username from an identifier
        /// </summary>
        /// <param name="identifier">User identifier</param>
        /// <param name="context">The current NancyFx context</param>
        /// <returns>Matching populated IUserIdentity object, or empty</returns>
        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new ClaimsPrincipal(new GenericIdentity("test"));
        }

    }
}
