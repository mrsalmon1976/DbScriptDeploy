using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbScriptDeploy.Modules.Api
{
    public class LookupApiModule : BaseSecureApiModule
    {
        public const string Route_Get_DatabaseTypes = "/api/lookup/databasetypes";
        public const string Route_Get_Designations = "/api/lookup/designations";

        private readonly IDbContext _dbContext;
        private readonly ILookupRepository _lookupRepo;

        public LookupApiModule(IDbContext dbContext, ILookupRepository lookupRepo)
        {
            _dbContext = dbContext;
            _lookupRepo = lookupRepo;

            Get(Route_Get_DatabaseTypes, x =>
            {
                return GetDatabaseTypes();
            });
            Get(Route_Get_Designations, x =>
            {
                return GetDesignations();
            });

        }


        public dynamic GetDatabaseTypes()
        {
            IEnumerable<DatabaseTypeModel> dbTypes = _lookupRepo.GetAllDatabaseTypes();
            return this.Response.AsJson(dbTypes);
        }

        public dynamic GetDesignations()
        {
            IEnumerable<DesignationModel> designations = _lookupRepo.GetAllDesignations();
            return this.Response.AsJson(designations);
        }
    }
}
