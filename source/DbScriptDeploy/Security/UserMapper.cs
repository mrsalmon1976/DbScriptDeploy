using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
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

        private readonly IUserRepository _userRepo;

        public UserMapper(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// Get the real username from an identifier
        /// </summary>
        /// <param name="identifier">User identifier</param>
        /// <param name="context">The current NancyFx context</param>
        /// <returns>Matching populated IUserIdentity object, or empty</returns>
        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            UserModel user = _userRepo.GetById(identifier);
            if (user == null)
            {
                return null;
            }
            return new UserPrincipal(identifier, new GenericIdentity(user.UserName));
        }

    }
}
