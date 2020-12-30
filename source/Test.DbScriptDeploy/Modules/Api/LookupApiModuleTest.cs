using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Utilities;
using DbScriptDeploy.Modules.Api;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
//using Nancy.Testing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Test.DbScriptDeploy.Modules.Api
{
    [TestFixture]
    public class LookupApiModuleTest
    {
        private IDbContext _dbContext;
        private ILookupRepository _lookupRepo;

        [SetUp]
        public void LookupApiModuleTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _lookupRepo = Substitute.For<ILookupRepository>();
        }

        

        #region LoadUserProjects Tests

        [Test]
        public void GetDatabaseTypes_LoadsAlLDatabaseTypes()
        {
            // setup
            Guid userId = Guid.NewGuid();
            var currentUser = new UserPrincipal(userId, new GenericIdentity("Joe Soap"));
            var browser = CreateBrowser(currentUser);

            List<DatabaseTypeModel> databaseTypes = new List<DatabaseTypeModel>();
            databaseTypes.Add(new DatabaseTypeModel() { Id = 1, Name = "DbType1", DefaultPort = 1111 });
            databaseTypes.Add(new DatabaseTypeModel() { Id = 2, Name = "DbType2", DefaultPort = 2222 });
            _lookupRepo.GetAllDatabaseTypes().Returns(databaseTypes);

            // execute
            var response = browser.Get(LookupApiModule.Route_Get_DatabaseTypes, (with) =>
            {
                with.HttpRequest();
                with.FormsAuth(userId, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _lookupRepo.Received(1).GetAllDatabaseTypes();

            List<DatabaseTypeModel> result = JsonConvert.DeserializeObject<List<DatabaseTypeModel>>(response.Body.AsString());
            Assert.AreEqual(databaseTypes.Count, result.Count);

            DatabaseTypeModel dtm = result.First();
            Assert.AreEqual(databaseTypes[0].Id, dtm.Id);
            Assert.AreEqual(databaseTypes[0].Name, dtm.Name);
        }


        #endregion


        #region Private Methods

        private Browser CreateBrowser(ClaimsPrincipal currentUser)
        {

            var browser = new Browser((bootstrapper) =>
                            bootstrapper.Module(new LookupApiModule(_dbContext, _lookupRepo))
                                .RootPathProvider(new TestRootPathProvider())
                                .RequestStartup((container, pipelines, context) => {
                                    context.CurrentUser = currentUser;
                                })
                            );
            return browser;
        }

        #endregion


    }
}
