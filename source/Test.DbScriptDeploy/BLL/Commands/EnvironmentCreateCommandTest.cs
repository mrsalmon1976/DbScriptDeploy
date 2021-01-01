using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Validators;
using DbScriptDeploy.BLL.Security;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Exceptions;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.Core.Security;

namespace Test.DbScriptDeploy.BLL.Commands
{
    [TestFixture]
    public class EnvironmentCreateCommandTest
    {
        private IEnvironmentCreateCommand _createEnvironmentCommand;

        private IDbContext _dbContext;
        private IEnvironmentValidator _environmentValidator;
        private IEncryptionProvider _encryptionProvider;

        [SetUp]
        public void EnvironmentCreateCommandTest_SetUp()
        {
            _dbContext = Substitute.For<IDbContext>();
            _environmentValidator = Substitute.For<IEnvironmentValidator>();
            _encryptionProvider = Substitute.For<IEncryptionProvider>();

            _createEnvironmentCommand = new EnvironmentCreateCommand(_dbContext, _environmentValidator, _encryptionProvider);
        }

        [TearDown]
        public void EnvironmentCreateCommandTest_TearDown()
        {
            // delete all .db files (in case previous tests have failed)
            TestHelper.DeleteTestFiles(AppContext.BaseDirectory, "*.dbtest");

        }

        [Test]
        public void Execute_ValidationFails_ThrowsException()
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();

            _environmentValidator.Validate(Arg.Any<EnvironmentModel>()).Returns(new ValidationResult("error"));

            // execute
            TestDelegate del = () => _createEnvironmentCommand.Execute(model);
            
            // assert
            Assert.Throws<ValidationException>(del);

            // we shouldn't have even tried to do the insert
            _dbContext.DidNotReceive().ExecuteNonQuery(Arg.Any<string>(), Arg.Any<object>());
            _encryptionProvider.DidNotReceive().SimpleEncrypt(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<byte[]>());
        }

        [Test]
        public void Execute_ValidationSucceeds_RecordInserted()
        {
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();

            _environmentValidator.Validate(Arg.Any<EnvironmentModel>()).Returns(new ValidationResult());

            // execute
            _createEnvironmentCommand.Execute(model);

            // assert
            _dbContext.Received(1).ExecuteScalar<int>(Arg.Any<string>(), Arg.Any<object>());
            _encryptionProvider.Received(1).SimpleEncrypt(model.Password, Arg.Any<byte[]>(), Arg.Any<byte[]>());
        }

        /// <summary>
        /// Tests that the insert actually works
        /// </summary>
        [Test]
        public void Execute_IntegrationTest_SQLite()
        {
            DateTime startTime = DateTime.UtcNow.AddSeconds(-1);
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName() + ".dbtest");
            EnvironmentModel model = DataHelper.CreateEnvironmentModel();

            using (SQLiteDbContext dbContext = new SQLiteDbContext(filePath))
            {
                dbContext.Initialise();
                dbContext.BeginTransaction();

                // create the user
                IEnvironmentRepository environmentRepo = new EnvironmentRepository(dbContext);
                IEnvironmentValidator environmentValidator = new EnvironmentValidator();
                IEncryptionProvider encryptionProvider = new AESGCM();

                IEnvironmentCreateCommand createEnvironmentCommand = new EnvironmentCreateCommand(dbContext, environmentValidator, encryptionProvider);
                EnvironmentModel environment = createEnvironmentCommand.Execute(model);

                EnvironmentModel savedEnvironment = environmentRepo.GetById(environment.Id);

                Assert.IsNotNull(savedEnvironment);
                Assert.AreEqual(environment.Name, savedEnvironment.Name);
                Assert.AreEqual(environment.ProjectId, savedEnvironment.ProjectId);
                Assert.AreEqual(environment.Host, savedEnvironment.Host);
                Assert.AreEqual(environment.DbType, savedEnvironment.DbType);
                Assert.AreEqual(environment.Database, savedEnvironment.Database);
                Assert.AreEqual(environment.Port, savedEnvironment.Port);
                Assert.AreEqual(environment.UserName, savedEnvironment.UserName);
                Assert.AreNotEqual(environment.Password, savedEnvironment.Password);
                Assert.IsNotEmpty(savedEnvironment.Password);
                Assert.AreEqual(environment.DisplayOrder, savedEnvironment.DisplayOrder);
                Assert.LessOrEqual(startTime, savedEnvironment.CreateDate);
                Assert.GreaterOrEqual(DateTime.UtcNow, savedEnvironment.CreateDate);

            }

        }



    }
}
