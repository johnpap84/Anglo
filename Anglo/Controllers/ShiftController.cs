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
    public class ShiftController : ControllerBase
    {
        private IRepository _repository;
        private IMapper _mapper;
        private ILogger _logger;

        public ShiftController(IRepository repository, IMapper mapper, ILogger logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShiftModel>>> GetAllShiftsAsync()
        {
            try
            {
                IEnumerable<Shift> result = await _repository.GetAllShiftsAsync();
                if (result == null)
                {
                    _logger.LogWarning("GetAllShifts: No Shifts found.");
                    return NotFound("No Shifts found.");
                }

                _logger.LogInformation("GetAllShifts: All Shifts loaded.");
                return _mapper.Map<List<ShiftModel>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAllShifts: Error during loading all Shifts: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during loading all Shifts: {ex}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift([FromBody] ShiftModel newShift)
        {
            try
            {
                newShift.ShiftId = 0;
                Shift shiftNameExists = await _repository.GetShiftByNameAsync(newShift.ShiftName);
                if (shiftNameExists != null)
                {
                    _logger.LogWarning($"CreateShift: Shift with name: '{newShift.ShiftName}' already exists.");
                    return BadRequest($"Shift with name: '{newShift.ShiftName}' already exists. Choose unique name!");
                }
                
                _repository.Add(_mapper.Map<Shift>(newShift));

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"CreateShift: Shift successfully created with name: {newShift.ShiftName}");
                    return Ok();
                }

                return BadRequest("Something went wrong, Shift was not created.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateShift: Error during creating a new Shift with name: {newShift.ShiftName}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during creating a new Shift: {ex}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            try
            {
                Shift shiftExists = await _repository.GetShiftByIdAsync(id);
                if (shiftExists == null)
                {
                    _logger.LogWarning($"DeleteShift: Cannot find this Shift with Id: {id}");
                    return NotFound($"Cannot find this Shift with Id: {id}");
                }

                Shift shiftInRota = _mapper.Map<Shift>(shiftExists);

                if (await _repository.GetRotaByShiftAsync(shiftInRota))
                {
                    return BadRequest($"Cannot delete the Shift: {shiftExists.ShiftName}, because it's been used in the Rota.");
                }

                _repository.Delete(shiftExists);
                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"DeleteShift: Shift with Id: {id} successfully deleted");
                    return Ok();
                }
                return BadRequest("Something went wrong, Shift was not deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteShift: Error during deleting Shift with Id: {id}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during deleting Shift with Id: {id}: {ex}");
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateShift([FromBody] ShiftModel updateShift)
        {
            try
            {
                Shift oldShift = await _repository.GetShiftByIdAsync(updateShift.ShiftId);
                if (oldShift == null)
                {
                    _logger.LogWarning($"UpdateShift: Shift with Id: {updateShift.ShiftId} was not found.");
                    return NotFound($"Shift with Id: {updateShift.ShiftId} was not found.");
                }

                Shift shiftNameExists = await _repository.GetShiftByNameAsync(updateShift.ShiftName);
                if (shiftNameExists != null)
                {
                    return BadRequest($"Shift with name: '{updateShift.ShiftName}' already exists. Choose unique name!");
                }

                _mapper.Map(updateShift, oldShift);

                if (await _repository.SaveChangesAsync())
                {
                    _logger.LogInformation($"UpdateShift: Shift with Id: {updateShift.ShiftId} and Name: {updateShift.ShiftName} successfully created.");
                    return Ok();
                }
                return BadRequest("Something went wrong, Shift has not been updated.");
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateShift: Error during updating Shift with Id: {updateShift.ShiftId} and Name: {updateShift.ShiftName}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during updating the Shift: {ex}");
            }
        }
    }
}
