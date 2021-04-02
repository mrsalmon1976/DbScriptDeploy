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
    public interface IScriptExecutionRepository
    {
        ScriptExecutionModel GetById(int scriptExecutionId);

        IEnumerable<ScriptExecutionModel> GetByScriptId(int scriptId);

        /// <summary>
        /// Gets all tags for scripts for a specific project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        IEnumerable<ScriptExecutionModel> GetByProjectId(int projectId);
    }

    public class ScriptExecutionRepository : IScriptExecutionRepository
    {

        private readonly IDbContext _dbContext;

        public ScriptExecutionRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ScriptExecutionModel GetById(int scriptExecutionId)
        {
            const string sql = "SELECT * FROM ScriptExecution WHERE Id = @Id";
            return _dbContext.Query<ScriptExecutionModel>(sql, new { Id = scriptExecutionId }).SingleOrDefault();
        }

        public IEnumerable<ScriptExecutionModel> GetByScriptId(int scriptId)
        {
            const string sql = "SELECT * FROM ScriptExecution WHERE ScriptId = @ScriptId ORDER BY Tag";
            return _dbContext.Query<ScriptExecutionModel>(sql, new { ScriptId = scriptId });
        }

        public IEnumerable<ScriptExecutionModel> GetByProjectId(int projectId)
        {

            string sql = @"SELECT se.* 
                    FROM ScriptExecution se
                    INNER JOIN Script s on se.ScriptId = s.Id
                    WHERE s.ProjectId = @ProjectId";
            return _dbContext.Query<ScriptExecutionModel>(sql, new { ProjectId = projectId });
        }


    }
}
