using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.Core.Encoding;
using DbScriptDeploy.Modules.Api;
using DbScriptDeploy.Security;
using DbScriptDeploy.ViewModels.Api;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
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
    public class EnvironmentApiModuleTest
    {
        private IDbContext _dbContext;
        private IEnvironmentRepository _environmentRepo;
        private IEnvironmentCreateCommand _environmentCreateCommand;

        [SetUp]
        public void EnvironmentApiModuleTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _environmentRepo = Substitute.For<IEnvironmentRepository>();
            _environmentCreateCommand = Substitute.For<IEnvironmentCreateCommand>();
        }

        #region AddEnvironment Tests

        [Test]
        public void AddEnvironment_ValidData_ReturnsEnvironmentData()
        {
            // setup
            var currentUser = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("Joe Soap"), new string[] { }));
            var browser = CreateBrowser(currentUser);
            var user = DataHelper.CreateUserModel();


            EnvironmentModel environment = DataHelper.CreateEnvironmentModel();
            EnvironmentViewModel environmentViewModel = EnvironmentViewModel.FromEnvironmentModel(environment);
            environment.Id = new Random().Next(1, 1000);
            _environmentCreateCommand.Execute(Arg.Any<EnvironmentModel>()).Returns(environment);

            // execute
            var response = browser.Post(EnvironmentApiModule.Route_Post_AddEnvironment, (with) =>
            {
                with.HttpRequest();
                with.FormValue("name", environmentViewModel.Name);
                with.FormValue("projectId", environmentViewModel.ProjectId);
                with.FormValue("dbType", environmentViewModel.DbType.ToString());
                with.FormValue("host", environmentViewModel.Host);
                with.FormValue("port", environmentViewModel.Port.ToString());
                with.FormValue("database", environmentViewModel.Database);
                with.FormValue("userName", environmentViewModel.UserName);
                with.FormValue("password", environmentViewModel.Password);
                with.FormValue("designationId", environmentViewModel.DesignationId.ToString());

                with.FormsAuth(user.Id, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _environmentCreateCommand.Received(1).Execute(Arg.Any<EnvironmentModel>());

            EnvironmentViewModel result = JsonConvert.DeserializeObject<EnvironmentViewModel>(response.Body.AsString());
            string safeEnvironmentId = UrlUtility.EncodeNumber(environment.Id);
            Assert.AreEqual(safeEnvironmentId, result.Id);
            Assert.AreEqual(environmentViewModel.Name, result.Name);
            Assert.AreEqual(environmentViewModel.ProjectId, result.ProjectId);
            Assert.AreEqual(environmentViewModel.DbType, result.DbType);
            Assert.AreEqual(environmentViewModel.Host, result.Host);
            Assert.AreEqual(environmentViewModel.Port, result.Port);
            Assert.AreEqual(environmentViewModel.Database, result.Database);
            Assert.AreEqual(environmentViewModel.UserName, result.UserName);
            Assert.AreEqual(environmentViewModel.Password, result.Password);
            Assert.AreEqual(environmentViewModel.DesignationId, result.DesignationId);

            _dbContext.Received(1).BeginTransaction();
            _dbContext.Received(1).Commit();
        }


        #endregion

        
        #region Private Methods

        private Browser CreateBrowser(ClaimsPrincipal currentUser)
        {

            var browser = new Browser((bootstrapper) =>
                            bootstrapper.Module(new EnvironmentApiModule(_dbContext, _environmentRepo, _environmentCreateCommand))
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
