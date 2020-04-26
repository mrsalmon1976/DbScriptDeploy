using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class UserClaimModel
    {
        public UserClaimModel()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public Guid? ProjectId { get; set; }

    }
}
