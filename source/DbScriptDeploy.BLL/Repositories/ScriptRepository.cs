using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Repositories
{
    public interface IScriptRepository
    {
        ScriptModel GetById(int id);

    }

    public class ScriptRepository : IScriptRepository
    {

        private readonly IDbContext _dbContext;

        public ScriptRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ScriptModel GetById(int id)
        {
            const string sql = "SELECT * FROM Script WHERE Id = @Id";
            return _dbContext.Query<ScriptModel>(sql, new { Id = id }).SingleOrDefault();
        }

    }
}
