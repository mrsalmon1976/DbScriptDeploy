using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.UI.Events;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using StructureMap;
using SystemWrapper.IO;

namespace DbScriptDeploy.UI.Tests.Services
{
	[TestFixture]
	public class ProjectServiceTests
	{
		private string _testFilePath;
		private JsonPersistenceService _jsonPersistenceService;
		private ProjectService _projectService;

		#region SetUp and TearDown

		[TestFixtureSetUp]
		public void ProjectServiceTestsSetUp()
		{
			_testFilePath = Path.Combine(Environment.CurrentDirectory, "ProjectServiceTests.json");
			_jsonPersistenceService = new JsonPersistenceService();

			// make sure there is no test file!
			if (File.Exists(_testFilePath))
			{
				Assert.Fail("Unable to start test as a test file already exists: {0}", _testFilePath);
			}
		}

		[TestFixtureTearDown]
		public void ProjectServiceTestsTearDown()
		{

			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}
		}

		#endregion

		#region LoadProjects Tests

		[Test]
		public void LoadProjects_AlreadyLoaded_LoadsFromMemory()
		{
			// Setup 
			CreateProjectFile(2);
			IFileWrap fileWrap = Substitute.For<IFileWrap>();
			fileWrap.Exists(_testFilePath).Returns(true);
			fileWrap.ReadAllText(_testFilePath).Returns(File.ReadAllText(_testFilePath));
			ObjectFactory.Configure(x => x.For<IFileWrap>().Use(fileWrap));

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			_projectService.LoadProjects();

			_projectService.LoadProjects();

			// Assert
			fileWrap.Received(1).ReadAllText(_testFilePath);
		}

		[Test]
		public void LoadProjects_ProjectFileDoesNotExist_ReturnsEmptyCollection()
		{
			// Setup 
			IFileWrap fileWrap = Substitute.For<IFileWrap>();
			fileWrap.Exists(_testFilePath).Returns(false);
			ObjectFactory.Configure(x => x.For<IFileWrap>().Use(fileWrap));

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			IEnumerable<Project> projects = _projectService.LoadProjects();

			// Assert
			fileWrap.Received(0).ReadAllText(Arg.Any<string>());
			Assert.IsEmpty(projects);
		}

		[Test]
		public void LoadProjects_ProjectFileExists_ReturnsExpectedProjects()
		{
			// Setup 
			List<Project> projects = CreateProjectFile(3);
			ObjectFactory.Configure(x => x.For<IFileWrap>().Use(new FileWrap()));

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			List<Project> result = _projectService.LoadProjects().ToList();

			// Assert
			Assert.AreEqual(3, result.Count());
			foreach (Project p in projects) {
				Project pComp = result.SingleOrDefault(x => x.Id == p.Id);
				Assert.IsNotNull(pComp);
			}
		}

		#endregion

		#region DeleteProject Tests

		[Test]
		public void DeleteProject_ProjectDoesNotExist_Exits()
		{
			// Setup
			IJsonPersistenceService jsonPersistenceService = Substitute.For<IJsonPersistenceService>();
			_projectService = new ProjectService(_testFilePath, jsonPersistenceService);

			// Execute
			Project p = new Project();
			_projectService.DeleteProject(p);

			// Assert
			jsonPersistenceService.Received(0).WriteFile(Arg.Any<string>(), Arg.Any<object>());

		}

		[Test]
		public void DeleteProject_ProjectDeleted_WritesToProjectFile()
		{
			// Setup
			ObjectFactory.Configure(x => x.For<IFileWrap>().Use<FileWrap>());
			List<Project> projects = this.CreateProjectFile(1);

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			_projectService.LoadProjects();
			_projectService.DeleteProject(projects[0]);

			// Assert
			string projectFileText = File.ReadAllText(_testFilePath);
			IEnumerable<Project> projectsAfterDelete = JsonConvert.DeserializeObject<IEnumerable<Project>>(projectFileText);
			Assert.AreEqual(0, projectsAfterDelete.Count());
		}

		[Test]
		public void DeleteProject_ProjectDeleted_RaisesEvent()
		{
			// Setup
			ObjectFactory.Configure(x => x.For<IFileWrap>().Use<FileWrap>());
			List<Project> projects = this.CreateProjectFile(1);
			bool eventRaised = false;

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			_projectService.ProjectDeleted += delegate(object sender, ProjectEventArgs eventArgs)
			{
				eventRaised = true;
			};
			_projectService.LoadProjects();
			_projectService.DeleteProject(projects[0]);

			// Assert
			Assert.IsTrue(eventRaised);
		}

		#endregion

		#region SaveProject Tests

		[Test]
		public void SaveProject_NewProjectAdded_WritesToFile()
		{
			// Setup
			Project p = new Project();
			IJsonPersistenceService jsonPersistenceService = Substitute.For<IJsonPersistenceService>();

			// Execute
			_projectService = new ProjectService(_testFilePath, jsonPersistenceService);
			_projectService.SaveProject(p);

			// Assert
			jsonPersistenceService.Received(1).WriteFile(_testFilePath, Arg.Any<object>());
		}

		[Test]
		public void SaveProject_NewProjectAdded_RaisesAddEvent()
		{
			// Setup
			Project p = new Project();
			bool addEventRaised = false;
			bool updateEventRaised = false;

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			_projectService.ProjectAdded += delegate(object sender, ProjectEventArgs eventArgs)
			{
				addEventRaised = true;
			};
			_projectService.ProjectUpdated += delegate(object sender, ProjectEventArgs eventArgs)
			{
				updateEventRaised = true;
			};
			_projectService.SaveProject(p);

			// Assert
			Assert.IsTrue(addEventRaised);
			Assert.IsFalse(updateEventRaised);
		}

		[Test]
		public void SaveProject_ExistingProjectModified_RaisesUpdatedEvent()
		{
			// Setup
			List<Project> projects = CreateProjectFile(2);
			Project p = projects[0];
			bool addEventRaised = false;
			bool updateEventRaised = false;

			// Execute
			_projectService = new ProjectService(_testFilePath, _jsonPersistenceService);
			_projectService.LoadProjects();
			_projectService.ProjectAdded += delegate(object sender, ProjectEventArgs eventArgs)
			{
				addEventRaised = true;
			};
			_projectService.ProjectUpdated += delegate(object sender, ProjectEventArgs eventArgs)
			{
				updateEventRaised = true;
			};
			_projectService.SaveProject(p);

			// Assert
			Assert.IsFalse(addEventRaised);
			Assert.IsTrue(updateEventRaised);
		}

		#endregion
		#region Private Helper Methods

		private List<Project> CreateProjectFile(int projectCount = 1)
		{
			List<Project> projects = new List<Project>();
			for (int i = 0; i < projectCount; i++)
			{
				Project p = new Project();
				p.Id = Guid.NewGuid();
				p.Name = p.Id.ToString();
				p.ScriptFolder = Environment.CurrentDirectory;
				projects.Add(p);
			}
			_jsonPersistenceService.WriteFile(_testFilePath, projects);
			return projects;
		}

		#endregion


	}
}
