using DbScriptDeploy.BLL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Models
{
    public class EnvironmentModel
    {
        public EnvironmentModel()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }

        public string HostName { get; set; }

        public DatabaseType DbType { get; set; }

        public string DbName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
