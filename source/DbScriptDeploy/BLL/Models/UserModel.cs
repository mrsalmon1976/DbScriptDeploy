using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class UserModel
    {
        public UserModel()
        {
            this.Id = Guid.NewGuid();
            this.Claims = new List<UserClaimModel>();
        }

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool PasswordExpired { get; set; }

        public List<UserClaimModel> Claims { get; }
    }
}
