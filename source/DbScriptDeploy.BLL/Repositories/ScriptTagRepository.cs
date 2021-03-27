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

        /// <summary>
        /// Gets all tags for scripts for a specific project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        IEnumerable<ScriptTagModel> GetByProjectId(int projectId);
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

        public IEnumerable<ScriptTagModel> GetByProjectId(int projectId)
        {

            string sql = @"SELECT st.* 
                    FROM ScriptTag st
                    INNER JOIN Script s on st.ScriptId = s.Id
                    WHERE s.ProjectId = @ProjectId
                    ORDER BY st.Tag";
            return _dbContext.Query<ScriptTagModel>(sql, new { ProjectId = projectId });
        }


    }
}
