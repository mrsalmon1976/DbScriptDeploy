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
    public interface IScriptTagRepository
    {
        IEnumerable<ScriptTagModel> GetByScriptId(int scriptId);

    }

    public class ScriptTagRepository : IScriptTagRepository
    {

        private readonly IDbContext _dbContext;

        public ScriptTagRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ScriptTagModel> GetByScriptId(int scriptId)
        {
            const string sql = "SELECT * FROM ScriptTag WHERE ScriptId = @ScriptId ORDER BY Tag";
            return _dbContext.Query<ScriptTagModel>(sql, new { ScriptId = scriptId });
        }

    }
}
