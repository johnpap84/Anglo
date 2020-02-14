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
    public class DepartmentController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DepartmentController(IRepository repository, IMapper mapper, ILogger<AngloRotaContext> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentModel>>> GetAllDepartments()
        {
            try
            {
                IEnumerable<Department> result = await _repository.GetAllDepartmentsAsync();
                if (result == null)
                {
                    _logger.LogWarning("Get all departments: No departments found.");
                    return NotFound("No Departments found.");
                }

                List<DepartmentModel> model = _mapper.Map<List<DepartmentModel>>(result);
                foreach (var dep in model)
                {
                    dep.NumberOfJobTitles = dep.JobTitles.Count();
                    dep.NumberOfEmployees = dep.EmployeesInDepartment.Count();
                }

                _logger.LogInformation("Successfully loaded All Departments:");
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllDepartments: Error during loading all Departments: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during loading all Departments: {ex}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewDepartment([FromBody] DepartmentModel newDepartment)
        {
            try
            {
                newDepartment.DepartmentId = 0;
                Department departmentNameExists = await _repository.GetDepartmentByNameAsync(newDepartment.DepartmentName);
                if (departmentNameExists != null)
                {
                    _logger.LogWarning("CreateNewDepartment: Department with name {0} already exists.", newDepartment.DepartmentName);
                    return BadRequest($"Department with the name: {newDepartment.DepartmentName} already exists. Choose unique name!");
                }

                Department addDepartment = _mapper.Map<Department>(newDepartment);
                _repository.Add(addDepartment);
                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"CreateNewDepartment: Department created: Name: {addDepartment.Name}, Id: {addDepartment.DepartmentId}");
                    return Ok();
                }
                return BadRequest("Something went wrong, Department was not created.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateNewDepartment: Error during creating the department with Name: {newDepartment.DepartmentName}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during creating the department: {ex}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                Department departmentToDelete = await _repository.GetDepartmentByIdAsync(id);
                if (departmentToDelete == null)
                {
                    _logger.LogWarning($"DeleteDepartment: Department with Id: {id}: was not found.");
                    return NotFound($"Department with id: {id} was not found.");
                }

                if (departmentToDelete.JobTitles.Count() > 0)
                {
                    return BadRequest($"There are {departmentToDelete.JobTitles.Count()} Job Title(s) linked to this department: {departmentToDelete.Name}. Remove the Job titles before deleting!");
                }

                if (departmentToDelete.EmployeesInDepartment.Count() > 0)
                {
                    return BadRequest($"There are {departmentToDelete.EmployeesInDepartment.Count()} Employee(s) belong to this department: {departmentToDelete.Name}. Remove the Employees before deleteing!");
                }

                _repository.Delete(departmentToDelete);
                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully deleted Department By Id: {id}");
                    return Ok();
                }
                return BadRequest("Something went wrong, department was not deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteDepartment: Error during deleting Department with Id: {0}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during deleting Department: {ex}");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentModel updateDepartment)
        {
            try
            {
                Department oldDepartment = await _repository.GetDepartmentByIdAsync(updateDepartment.DepartmentId);
                if (oldDepartment == null)
                {
                    _logger.LogWarning($"UpdateDepartment: Department with Id: {updateDepartment.DepartmentId} was not found.");
                    return NotFound($"Department with Id: {updateDepartment.DepartmentId} was not found.");
                }
                oldDepartment.Name = updateDepartment.DepartmentName;

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully updated Department with Id: {updateDepartment.DepartmentId}");
                    return Ok();
                }
                return BadRequest("Something went wrong, department was not updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateDepartment: Error during updating Department with id: {updateDepartment.DepartmentId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during updating Department: {ex}");
            }
        }

    }
}