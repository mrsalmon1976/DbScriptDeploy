using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Validators;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Models;

namespace Test.DbScriptDeploy.BLL.Repositories
{
    [TestFixture]
    public class ProjectRepositoryTest
    {
        private IDbContext _dbContext;


        [SetUp]
        public void ProjectRepositoryTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
        }

        [TearDown]
        public void ProjectRepositoryTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }


        #region GetById 

        /// <summary>
        /// Tests that the GetById method fetches a project by its id
        /// </summary>
        [Test]
        public void GetById_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IProjectRepository projectRepo = new ProjectRepository(dbContext);

                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());

                // create the user
                ProjectModel project = createProjectCommand.Execute(name);

                ProjectModel result = projectRepo.GetById(project.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(name, result.Name);
            }

        }

        #endregion

        #region GetAllByUserId 

        /// <summary>
        /// Tests that the GetAllByUserId method fetches all projects for an administrator
        /// </summary>
        [Test]
        public void GetAllByUserId_Administrator_Integration_ReturnsData()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IProjectRepository projectRepo = new ProjectRepository(dbContext);
                IUserRepository userRepo = new UserRepository(dbContext);
                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbContext);

                IUserCreateCommand createUserCommand = new UserCreateCommand(dbContext, new UserValidator(userRepo), new PasswordProvider());
                IUserClaimCreateCommand createUserClaimCommand = new UserClaimCreateCommand(dbContext, new UserClaimValidator(userClaimRepo));
                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());

                // create the user
                UserModel user = createUserCommand.Execute("test", "test");

                // create the projects
                ProjectModel projectNotMember = createProjectCommand.Execute("projectNotMember");
                ProjectModel projectOwner = createProjectCommand.Execute("projectOwner");
                ProjectModel projectUser = createProjectCommand.Execute("projectUser");

                //create the user claims - linked to only one project
                createUserClaimCommand.Execute(user.Id, Claims.Administrator, null);
                createUserClaimCommand.Execute(user.Id, Claims.ProjectOwner, projectOwner.Id);
                createUserClaimCommand.Execute(user.Id, Claims.ProjectUser, projectUser.Id);


                List<ProjectModel> result = projectRepo.GetAllByUserId(user.Id).ToList();

                Assert.AreEqual(3, result.Count);
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == projectNotMember.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == projectOwner.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == projectUser.Id));
            }

        }

        /// <summary>
        /// Tests that the GetAllByUserId method only fetches the appropriate projects for project owners and project users
        /// </summary>
        [TestCase(Claims.ProjectOwner)]
        [TestCase(Claims.ProjectUser)]
        public void GetAllByUserId_ProjectOwnerOrUser_Integration_ReturnsData(string claimName)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            string name = Path.GetRandomFileName();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                IProjectRepository projectRepo = new ProjectRepository(dbContext);
                IUserRepository userRepo = new UserRepository(dbContext);
                IUserClaimRepository userClaimRepo = new UserClaimRepository(dbContext);

                IUserCreateCommand createUserCommand = new UserCreateCommand(dbContext, new UserValidator(userRepo), new PasswordProvider());
                IUserClaimCreateCommand createUserClaimCommand = new UserClaimCreateCommand(dbContext, new UserClaimValidator(userClaimRepo));
                IProjectCreateCommand createProjectCommand = new ProjectCreateCommand(dbContext, new ProjectValidator());

                // create the user
                UserModel user = createUserCommand.Execute("test", "test");

                // create the projects
                ProjectModel projectNotMember = createProjectCommand.Execute("projectNotMember");
                ProjectModel projectMember1 = createProjectCommand.Execute("member1");
                ProjectModel projectMember2 = createProjectCommand.Execute("member2");

                //create the user claims - linked to only two projects
                createUserClaimCommand.Execute(user.Id, claimName, projectMember1.Id);
                createUserClaimCommand.Execute(user.Id, claimName, projectMember2.Id);


                List<ProjectModel> result = projectRepo.GetAllByUserId(user.Id).ToList();

                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result.FirstOrDefault(x => x.Id == projectNotMember.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == projectMember1.Id));
                Assert.IsNotNull(result.FirstOrDefault(x => x.Id == projectMember2.Id));
            }

        }

        #endregion


    }
}
