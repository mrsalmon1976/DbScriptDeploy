using AutoMapper;
using DbScriptDeploy.BLL.Commands;
using DbScriptDeploy.BLL.Data;
using DbScriptDeploy.BLL.Models;
using DbScriptDeploy.BLL.Repositories;
using DbScriptDeploy.Console.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DbScriptDeploy.Console.Areas.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : BaseApiController
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProjectCreateCommand _projectCreateCommand;
        private readonly IProjectRepository _projectRepo;

        public ProjectController(ILogger<ProjectController> logger, IDbContext dbContext, IMapper mapper, IProjectCreateCommand projectCreateCommand, IProjectRepository projectRepo)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _projectCreateCommand = projectCreateCommand;
            _projectRepo = projectRepo;
        }

        [HttpGet]
        [Route("/api/project/user")]
        public IEnumerable<ProjectViewModel> GetUserProjects()
        {
            Guid userId = this.CurrentUser.UserId;
            IEnumerable<ProjectModel> projects;
            if (this.CurrentUser.IsAdmin)
            {
                projects = _projectRepo.GetAll().ToList();
            }
            else
            {
                throw new NotImplementedException();
            }
            var result = _mapper.Map<IEnumerable<ProjectViewModel>>(projects);
            return result; 

        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Route("/api/project")]
        public ProjectViewModel PostProject([FromBody] ProjectViewModel projectViewModel)
        {
            _logger.LogDebug("Project received via /api/project");

            //var projectName = Request.Form.ProjectName;
            _dbContext.BeginTransaction();
            ProjectModel project = _projectCreateCommand.Execute(projectViewModel.Name);
            _dbContext.Commit();
            _logger.LogInformation($"Project {project.Id}/{project.Name} created");

            var result = _mapper.Map<ProjectViewModel>(project);
            return result;

        }

    }
}
