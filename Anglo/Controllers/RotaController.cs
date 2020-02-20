using AngloRota.Data;
using AngloRota.Data.Entities;
using AngloRota.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngloRota.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AngloRota")]
    [Authorize]
    public class RotaController : ControllerBase
    {
        private IRepository _repository;
        private IMapper _mapper;
        private ILogger _logger;

        public RotaController(IRepository repository, IMapper mapper, ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{all}")]
        public async Task<ActionResult<List<RotaDataModel>>> GetAllRota()
        {
            try
            {
                var allRotaData = await _repository.GetAllRotaAsync();
                if (allRotaData == null)
                {
                    _logger.LogWarning("GetAllRota: No Rota entries found");
                    return NotFound("No Rota entries found.");
                }

                _logger.LogInformation("GetAllRota: Successfully loaded all Rota Data.");
                return _mapper.Map<List<RotaDataModel>>(allRotaData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllRota: Error during loading all Rota Data: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during loading all Rota Data: {ex}");
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<RotaDataModel>> GetRotaById(int employeeId, DateTime date)
        {
            try
            {
                var rotaData = await _repository.GetRotaAsync(employeeId, date);
                if (rotaData == null)
                {
                    _logger.LogWarning($"GetRotaById: Rota data was not found for Employee Id: {employeeId} and for Date: {date}");
                    return NotFound("Rota data has not been found.");
                }

                _logger.LogInformation($"GetRotaById: Successfully loaded Rota Data for Employee Id: {employeeId} and for Date: {date}");
                return _mapper.Map<RotaDataModel>(rotaData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetRotaById: Error during loading Rota Data for Employee Id: {employeeId} and for Date: {date}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during loading Rota Data: {ex}");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateRotaData(RotaDataModel newRota)
        {
            try
            {
                Employee employeeForRota = await _repository.GetEmployeeByIdAsync(newRota.EmployeeId);
                if (employeeForRota == null)
                {
                    _logger.LogWarning($"CreateRotaData: Employee was not found with Id: {newRota.EmployeeId}");
                    return NotFound($"Employee was not found with Id: {newRota.EmployeeId}");
                }

                Shift shiftForRota = await _repository.GetShiftByNameAsync(newRota.ShiftName);
                if (shiftForRota == null)
                {
                    _logger.LogWarning($"CreateRotaData: Shift was not found with name: {newRota.ShiftName}");
                    return NotFound($"Shift was not found with name: {newRota.ShiftName}");
                }

                RotaData addRotaData = _mapper.Map<RotaData>(newRota);

                if (newRota.EmployeeName == null || newRota.EmployeeName.Equals(employeeForRota.EmployeeName))
                {
                    addRotaData.RotaForEmployee = employeeForRota;
                }
                else
                {
                    _logger.LogWarning($"CreateRotaData: Submitted Employee Id: {newRota.EmployeeId} and Name: {newRota.EmployeeName} pair not matching the database.");
                    return BadRequest("The submitted Employee Id and Name are not matching each other.");
                }

                if (newRota.DurationInMins == 0 || newRota.DurationInMins == shiftForRota.DurationInMins)
                {
                    addRotaData.Shift = shiftForRota;
                }
                else
                {
                    _logger.LogWarning($"CreateRotaData: Submitted Shift Name: {newRota.ShiftName} and Duration: {newRota.DurationInMins} pair not matching the database.");
                    return BadRequest("The submitted Shift Name and Duration in minutes are not matching each other.");
                }

                _repository.Add(addRotaData);
                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"CreateRotaData: Successfully created Rota Data on: {newRota.Date} for Employee: {newRota.EmployeeId}");
                    return Ok();
                }

                return BadRequest("Something went wrong, Rota entry was not created.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateRotaData: Error during creating Rota Data on: {newRota.Date} for Employee: {newRota.EmployeeId}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during creating Rota Data: {ex}");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateRotaData([FromBody] RotaDataModel updateRotaData)
        {
            try
            {
                RotaData oldRotaData = await _repository.GetRotaAsync(updateRotaData.EmployeeId, updateRotaData.Date);
                if (oldRotaData == null)
                {
                    await CreateRotaData(updateRotaData);
                }

                if (updateRotaData.ShiftName == null)
                {
                    await DeleteRotaData(updateRotaData);
                }

                Shift updateRotaShift = await _repository.GetShiftByNameAsync(updateRotaData.ShiftName);
                if (updateRotaShift == null)
                {
                    _logger.LogWarning($"UpdateRotaData: Shift was not found with name: {updateRotaData.ShiftName}");
                    return NotFound($"Shift was not found with name: {updateRotaData.ShiftName}");
                }
                oldRotaData.Shift = updateRotaShift;

                Employee updateRotaForEmployee = await _repository.GetEmployeeByIdAsync(updateRotaData.EmployeeId);
                if (updateRotaForEmployee == null)
                {
                    _logger.LogWarning($"UpdateRotaData: Employee was not found with Id: {updateRotaData.EmployeeId}");
                    return NotFound($"Employee was not found with Id: {updateRotaData.EmployeeId}");
                }
                oldRotaData.RotaForEmployee = updateRotaForEmployee;

                DateTime startDate = new DateTime(2019, 01, 01);
                DateTime endDate = new DateTime(2019, 12, 31);
                if (updateRotaData.Date < startDate || updateRotaData.Date > endDate || updateRotaData.Date == null)
                {
                    _logger.LogWarning($"UpdateRotaData: Date was null or not between range: {updateRotaData.Date} for Employee: {updateRotaData.EmployeeId}");
                    return BadRequest($"Date was not set or not between the range: {updateRotaData.Date}");
                }

                _mapper.Map(updateRotaData, oldRotaData);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Successfully updated Rota Data on: {updateRotaData.Date} for Employee: {updateRotaData.EmployeeId}.");
                    return Ok();
                }

                return BadRequest("Something went wrong, Rota entry was not updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateRotaData: Error during updating Rota for Employee: {updateRotaData.EmployeeId} on: {updateRotaData.Date}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during updating Rota: {ex}");
            }
        }

        
        public async Task<IActionResult> DeleteRotaData(RotaDataModel deleteRotaData)
        {
            try
            {
                RotaData rotaToDelete = _mapper.Map<RotaData>(deleteRotaData);
                _repository.Delete(rotaToDelete);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"DeleteRotaData: Rota was deleted successfully: Date: {rotaToDelete.Date} for Employee: {rotaToDelete.RotaForEmployee.EmployeeName}");
                    return Ok();
                }
                return BadRequest("Something went wrong, Rota entry was not deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteRotaData: Error during deleting Rota for Employee: {deleteRotaData.EmployeeId} on: {deleteRotaData.Date}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during updating Rota: {ex}");
            }
        }
    }
}
