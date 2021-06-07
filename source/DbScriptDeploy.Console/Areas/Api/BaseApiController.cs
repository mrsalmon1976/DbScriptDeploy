using DbScriptDeploy.Console.AppCode.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console.Areas.Api
{
    public class BaseApiController : ControllerBase
    {
        private IAppUser _appUser;

        public BaseApiController()
        {
        }

        public IAppUser CurrentUser 
        {
            get
            {
                if (_appUser == null && this.User != null)
                {
                    _appUser = new AppUser(this.User);
                }
                return _appUser;
            }
            set
            {
                _appUser = value;
            }
            
        }
    }
}
