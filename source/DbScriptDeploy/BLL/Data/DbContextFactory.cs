using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Data
{
    public interface IDbContextFactory
    {
        IDbContext GetDbContext();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private string _databasePath;

        public DbContextFactory(string dbPath)
        {
            _databasePath = dbPath;

        }

        public IDbContext GetDbContext()
        {
            return new SQLiteDbContext(_databasePath);
        }
    }
}
