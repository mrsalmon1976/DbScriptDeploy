using DbScriptDeploy.BLL.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console.AppCode.Security
{
    public interface IAppUser
    {
        bool IsAdmin { get; }

        Guid UserId { get; }
    }

    public class AppUser : IAppUser
    {
        private readonly ClaimsPrincipal _principal;

        public AppUser(ClaimsPrincipal principal)
        {
            this._principal = principal;
        }

        public bool IsAdmin
        {
            get
            {
                return _principal.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == Claims.Administrator);
            }
        }

        public Guid UserId
        {
            get
            {
                Guid currentUserId = Guid.Empty;
                if (_principal != null)
                {
                    string claimValue = _principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    if (Guid.TryParse(claimValue, out currentUserId))
                    {
                        return currentUserId;
                    }
                }
                return Guid.Empty;
            }
        }
    }
}
