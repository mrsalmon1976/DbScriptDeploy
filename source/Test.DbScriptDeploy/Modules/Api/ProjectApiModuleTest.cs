using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.Modules.Api;
using DbScriptDeploy.Security;
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
    public class ProjectApiModuleTest
    {
        private IDbContext _dbContext;
        private IProjectRepository _projectRepo;
        private IProjectCreateCommand _projectCreateCommand;

        [SetUp]
        public void ProjectApiModuleTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _projectRepo = Substitute.For<IProjectRepository>();
            _projectCreateCommand = Substitute.For<IProjectCreateCommand>();
        }

        #region AddProject Tests

        [Test]
        public void AddProject_ValidData_ReturnsProjectData()
        {
            // setup
            var currentUser = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("Joe Soap"), new string[] { }));
            var browser = CreateBrowser(currentUser);
            var user = DataHelper.CreateUserModel();


            ProjectModel project = DataHelper.CreateProjectModel();
            _projectCreateCommand.Execute(project.Name).Returns(project);

            // execute
            var response = browser.Post(ProjectApiModule.Route_Post, (with) =>
            {
                with.HttpRequest();
                with.FormValue("projectName", project.Name);
                with.FormsAuth(user.Id, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _projectCreateCommand.Received(1).Execute(project.Name);

            ProjectModel result = JsonConvert.DeserializeObject<ProjectModel>(response.Body.AsString());
            Assert.AreEqual(project.Id, result.Id);

            _dbContext.Received(1).BeginTransaction();
            _dbContext.Received(1).Commit();
        }


        #endregion

        #region AddProject Tests

        [Test]
        public void LoadUserProjects_LoadsDataForCurrentUser()
        {
            // setup
            Guid userId = Guid.NewGuid();
            var currentUser = new UserPrincipal(userId, new GenericIdentity("Joe Soap"));
            var browser = CreateBrowser(currentUser);

            List<ProjectModel> userProjects = new List<ProjectModel>();
            userProjects.Add(DataHelper.CreateProjectModel());
            userProjects.Add(DataHelper.CreateProjectModel());
            userProjects.Add(DataHelper.CreateProjectModel());
            _projectRepo.GetAllByUserId(userId).Returns(userProjects);

            // execute
            var response = browser.Get(ProjectApiModule.Route_Get_Api_Projects_User, (with) =>
            {
                with.HttpRequest();
                with.FormsAuth(userId, new FormsAuthenticationConfiguration());
            }).Result;

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _projectRepo.Received(1).GetAllByUserId(userId);

            List<ProjectModel> result = JsonConvert.DeserializeObject<List<ProjectModel>>(response.Body.AsString());
            Assert.AreEqual(userProjects.Count, result.Count);
        }


        #endregion

        #region Private Methods

        private Browser CreateBrowser(ClaimsPrincipal currentUser)
        {

            var browser = new Browser((bootstrapper) =>
                            bootstrapper.Module(new ProjectApiModule(_dbContext, _projectRepo, _projectCreateCommand))
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
