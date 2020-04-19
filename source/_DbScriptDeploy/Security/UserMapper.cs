using Nancy;
using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.Security
{
    public class UserMapper : IUserMapper
    {
        public UserMapper()
        {
        }

        public virtual ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new ClaimsPrincipal(new GenericIdentity("test"));
        }
    }
}
