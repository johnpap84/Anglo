using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngloRota.Data;
using AngloRota.Data.Entities;
using AngloRota.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AngloRota.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AngloRota")]
    //[Authorize]
    public class JobTitleController : ControllerBase
    {
        private IRepository _repository;
        private IMapper _mapper;
        private ILogger<AngloRotaContext> _logger;

        public JobTitleController(IRepository repository, IMapper mapper, ILogger<AngloRotaContext> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobTitleModel>>> GetAllJobTitlesAsync()
        {
            try
            {
                var result = await _repository.GetAllJobTitlesAsync();
                if (result == null)
                {
                    _logger.LogWarning("GetAllJobTitles: No Job Titles found.");
                    return BadRequest("No Job Titles found.");
                }

                List<JobTitleModel> model = _mapper.Map<List<JobTitleModel>>(result);
                foreach (var job in model)
                {
                    job.numberOfEmployees = job.EmployeesInJob.Count();
                }

                _logger.LogInformation("Successfully loaded all Job Titles.");
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during loading all Job Titles: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during loading all Job Titles: {ex}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateJobTitle([FromBody] JobTitleModel newJobTitle)
        {
            try
            {
                newJobTitle.Id = 0;

                var jobTitleexists = await _repository.GetJobTitleByNameAsync(newJobTitle.JobTitleName);
                if (jobTitleexists != null)
                {
                    _logger.LogWarning($"Job Title exists: {newJobTitle.JobTitleName} during Create Job Title.");
                    return BadRequest($"Job Title with the name: '{newJobTitle.JobTitleName}' already exists.");
                }

                var inDepartment = await _repository.GetDepartmentByNameAsync(newJobTitle.InDepartment);
                if (inDepartment == null)
                {
                    _logger.LogWarning($"Department: {newJobTitle.InDepartment} not found during Create Job Title.");
                    return NotFound($"The department: '{newJobTitle.InDepartment}' doesn't exist.");
                }

                var addJobTitle = _mapper.Map<JobTitle>(newJobTitle);
                addJobTitle.Department = inDepartment;
                _repository.Add(addJobTitle);
                inDepartment.JobTitles.Add(addJobTitle);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully created new Job Title: {newJobTitle.JobTitleName}");
                    return Ok();
                }

                return BadRequest("Something went wrong, Job Title was not created.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during creating new Job Title by Name: {newJobTitle.JobTitleName}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during creating Job title: {ex}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTitle(int id)
        {
            try
            {
                var jobTitleToDelete = await _repository.GetJobTitleByIdAsync(id);
                if (jobTitleToDelete == null)
                {
                    _logger.LogWarning($"DeleteJobTitle: Job Title was not found with Id: {id}.");
                    return NotFound($"Job Title was not found with Id: {id}.");
                }

                if (jobTitleToDelete.EmployeesInJob.Count != 0)
                {
                    return BadRequest($"There are: '{jobTitleToDelete.EmployeesInJob.Count}' Employees asigned to the Job Title, please reasign them before deleting.");
                }

                _repository.Delete(jobTitleToDelete);
                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully deleted Job Title by Id: {id}");
                    return Ok();
                }
                return BadRequest("Something went wrong, Job Title has not been deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during deleting Job Title by Id: {id}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during deleting Job Title: {ex}");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateJobTitle([FromBody] JobTitleModel updateJobTitle)
        {
            try
            {
                JobTitle oldJobTitle = await _repository.GetJobTitleByIdAsync(updateJobTitle.Id);
                if (oldJobTitle == null)
                {
                    _logger.LogWarning($"UpdateJobTitle: Job Title was not found with Id: {updateJobTitle.Id}.");
                    return NotFound($"Job Title was not found with Id: {updateJobTitle.Id}.");
                }

                Department department = await _repository.GetDepartmentByNameAsync(updateJobTitle.InDepartment);
                if (department == null)
                {
                    return BadRequest($"The department: '{updateJobTitle.InDepartment}' doesn't exist.");
                }
                oldJobTitle.Department = department;

                _mapper.Map(updateJobTitle, oldJobTitle);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully updated Job Title with Id: {updateJobTitle.Id}.");
                    return Ok();
                }
                return BadRequest("Something went wrong, Job Title has not been updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during updating Job Title with Id: {updateJobTitle.Id}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during updating Job Title: {ex}");
            }
        }
    }
}