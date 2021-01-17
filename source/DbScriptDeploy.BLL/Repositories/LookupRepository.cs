using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.BLL.Repositories
{

    public interface ILookupRepository
    {
        IEnumerable<DatabaseTypeModel> GetAllDatabaseTypes();


        IEnumerable<DesignationModel> GetAllDesignations();

        DesignationModel GetDesignationById(int designationId);
    }

    public class LookupRepository : ILookupRepository
    {
        private readonly IDbContext _dbContext;
        private static List<DatabaseTypeModel> _databaseTypes = new List<DatabaseTypeModel>();

        public LookupRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<DatabaseTypeModel> GetAllDatabaseTypes()
        {
            if (_databaseTypes.Count() == 0)
            {
                _databaseTypes.Add(new DatabaseTypeModel() { Id = 1, Name = "Microsoft SQL Server" });
            }
            return _databaseTypes.OrderBy(x => x.Name);
        }
        public IEnumerable<DesignationModel> GetAllDesignations()
        {
            const string sql = "SELECT * FROM Designation ORDER BY Name";
            return _dbContext.Query<DesignationModel>(sql);
        }

        public DesignationModel GetDesignationById(int designationId)
        {
            const string sql = "SELECT * FROM Designation WHERE Id = @Id";
            return _dbContext.Query<DesignationModel>(sql, new { Id = designationId }).SingleOrDefault();
        }
    }
}
