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
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace AngloRota.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AngloRota")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EmployeesController(IRepository repository, IMapper mapper, ILogger<AngloRotaContext> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<List<EmployeeModel>>> GetAllEmployees()
        {
            try
            {
                var result = await _repository.GetAllEmployeesAsync();

                return _mapper.Map<List<EmployeeModel>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllEmployees: Error during loading all Employees: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to load from database: {ex}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeModel>> GetEmployeeById(int id)
        {
            try
            {
                var result = await _repository.GetEmployeeByIdAsync(id);

                if (result == null)
                {
                    _logger.LogError($"GetEmployeeById: Employee with Id: {id} was not found.");
                    return NotFound($"Employee by Id: {id} was not found.");
                }

                _logger.LogInformation($"Successfully loaded Employee with Id: {id}");
                return _mapper.Map<EmployeeModel>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetEmployeeById: Error during loading Employee with Id: {id}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to load from database: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeModel>> CreateNewEmployee([FromBody] EmployeeModel employee)
        {
            try
            {
                Employee employeeIdExists = await _repository.GetEmployeeByIdAsync(employee.EmployeeId);
                if (employeeIdExists != null)
                {
                    return BadRequest($"This Employee Id: {employee.EmployeeId} is already in use");
                }

                Department addDepartment = await _repository.GetDepartmentByNameAsync(employee.Department);
                if (addDepartment == null)
                {
                    return NotFound($"Department received doesn't exist: {employee.Department}");
                }

                JobTitle addJobTitle = await _repository.GetJobTitleByNameAsync(employee.JobTitle);
                if (addJobTitle == null)
                {
                    return NotFound($"Job Title received doesn't exist: {employee.JobTitle}");
                }
                if (!addJobTitle.Department.Name.Equals(employee.Department))
                {
                    return BadRequest($"Job title: {employee.JobTitle} doesn't exists in Department: {employee.Department}");
                }

                Employee addEmployee = _mapper.Map<Employee>(employee);
                addEmployee.Department = addDepartment;
                addEmployee.JobTitle = addJobTitle;
                _repository.Add(addEmployee);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"SuccessFully created new Employee with Id: {addEmployee.EmployeeId} and Name: {addEmployee.EmployeeName}");
                    return Ok(_mapper.Map<EmployeeModel>(addEmployee));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during creating new Employee: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create new Employee: {ex}");
            }
            return BadRequest();
        }

        [HttpPatch]
        public async Task<ActionResult<EmployeeModel>> UpdateEmployee([FromBody] EmployeeModel employee)
        {
            try
            {
                var oldEmployee = await _repository.GetEmployeeByIdAsync(employee.EmployeeId);
                if (oldEmployee == null)
                {
                    return NotFound($"Could not find the employee ID: {employee.EmployeeId}");
                }
                
                Department updateDepartment = await _repository.GetDepartmentByNameAsync(employee.Department);
                if (updateDepartment == null)
                {
                    return NotFound($"Department received doesn't exist: {employee.Department}");
                }
                oldEmployee.Department = updateDepartment;

                JobTitle updateJobTitle = await _repository.GetJobTitleByNameAsync(employee.JobTitle);
                if (updateJobTitle == null)
                {
                    return NotFound($"Job Title received doesn't exist: {employee.JobTitle}");
                }
                oldEmployee.JobTitle = updateJobTitle;

                if (!updateJobTitle.Department.Name.Equals(employee.Department))
                {
                    return NotFound($"Job title: {employee.JobTitle} doesn't exists in Department: {employee.Department}");
                }

                _mapper.Map(employee, oldEmployee);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully updated Employee with Id: {employee.EmployeeId}");
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during updating Employee with Id: {employee.EmployeeId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to update the Employee: {ex}");
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employeeToDelete = await _repository.GetEmployeeByIdAsync(id);
                if (employeeToDelete == null)
                {
                    _logger.LogWarning($"Employee with Id: {id} was not found in the database.");
                    return NotFound($"Employee with Id: {id} was not found in the database.");
                }

                _repository.Delete(employeeToDelete);
                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully deleted Employee with Id: {id}");
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during deleting Employee with id: {id}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database Failure: {ex}");
            }
            return BadRequest($"Failed to delete the employee with Id: {id}.");
        }

    }
}
