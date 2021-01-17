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
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ProjectId { get; set; }

        public string Host { get; set; }

        public Lookups.DatabaseType DbType { get; set; }

        public string Database { get; set; }

        public int Port { get; set; }
            
        public string UserName { get; set; }

        public string Password { get; set; }

        public int DisplayOrder { get; set; }

        public int DesignationId { get; set; }

        public DateTime CreateDate { get; set; }

    }
}
