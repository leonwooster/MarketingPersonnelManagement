using Microsoft.AspNetCore.Mvc;
using CompanyA.BusinessEntity;
using CompanyA.BusinessComponents;
using System.ComponentModel.DataAnnotations;

namespace CompanyA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelController : ControllerBase
    {
        private readonly IPersonnelService _personnelService;
        private readonly ILogger<PersonnelController> _logger;

        public PersonnelController(IPersonnelService personnelService, ILogger<PersonnelController> logger)
        {
            _personnelService = personnelService;
            _logger = logger;
        }

        /// <summary>
        /// Get all personnel records
        /// </summary>
        /// <returns>List of all personnel with their details</returns>
        /// <response code="200">Returns the list of personnel</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PersonnelDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PersonnelDto>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonnelDto>>>> GetAllPersonnel()
        {
            try
            {
                var personnel = await _personnelService.GetAllPersonnelAsync();
                return Ok(ApiResponse<IEnumerable<PersonnelDto>>.SuccessResult(personnel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all personnel");
                return StatusCode(500, ApiResponse<IEnumerable<PersonnelDto>>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Get personnel by ID
        /// </summary>
        /// <param name="id">Personnel ID</param>
        /// <returns>Personnel details for the specified ID</returns>
        /// <response code="200">Returns the personnel details</response>
        /// <response code="404">Personnel not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 500)]
        public async Task<ActionResult<ApiResponse<PersonnelDto>>> GetPersonnelById(int id)
        {
            try
            {
                var personnel = await _personnelService.GetPersonnelByIdAsync(id);
                if (personnel == null)
                {
                    return NotFound(ApiResponse<PersonnelDto>.ErrorResult($"Personnel with ID {id} not found"));
                }

                return Ok(ApiResponse<PersonnelDto>.SuccessResult(personnel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving personnel with ID {PersonnelId}", id);
                return StatusCode(500, ApiResponse<PersonnelDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Create new personnel
        /// </summary>
        /// <param name="personnelDto">Personnel data to create</param>
        /// <returns>Created personnel details</returns>
        /// <response code="201">Personnel created successfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 500)]
        public async Task<ActionResult<ApiResponse<PersonnelDto>>> CreatePersonnel([FromBody] PersonnelDto personnelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<PersonnelDto>.ErrorResult("Validation failed", errors));
                }

                // Additional business validation
                var validationErrors = await ValidatePersonnelAsync(personnelDto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponse<PersonnelDto>.ErrorResult("Validation failed", validationErrors));
                }

                var createdPersonnel = await _personnelService.CreatePersonnelAsync(personnelDto);
                return CreatedAtAction(nameof(GetPersonnelById), 
                    new { id = createdPersonnel.Id }, 
                    ApiResponse<PersonnelDto>.SuccessResult(createdPersonnel, "Personnel created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating personnel");
                return StatusCode(500, ApiResponse<PersonnelDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Update existing personnel
        /// </summary>
        /// <param name="id">Personnel ID to update</param>
        /// <param name="personnelDto">Updated personnel data</param>
        /// <returns>Updated personnel details</returns>
        /// <response code="200">Personnel updated successfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="404">Personnel not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<PersonnelDto>), 500)]
        public async Task<ActionResult<ApiResponse<PersonnelDto>>> UpdatePersonnel(int id, [FromBody] PersonnelDto personnelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<PersonnelDto>.ErrorResult("Validation failed", errors));
                }

                // Additional business validation
                var validationErrors = await ValidatePersonnelAsync(personnelDto);
                if (validationErrors.Any())
                {
                    return BadRequest(ApiResponse<PersonnelDto>.ErrorResult("Validation failed", validationErrors));
                }

                var updatedPersonnel = await _personnelService.UpdatePersonnelAsync(id, personnelDto);
                if (updatedPersonnel == null)
                {
                    return NotFound(ApiResponse<PersonnelDto>.ErrorResult($"Personnel with ID {id} not found"));
                }

                return Ok(ApiResponse<PersonnelDto>.SuccessResult(updatedPersonnel, "Personnel updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating personnel with ID {PersonnelId}", id);
                return StatusCode(500, ApiResponse<PersonnelDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Delete personnel (requires confirmation)
        /// </summary>
        /// <param name="id">Personnel ID to delete</param>
        /// <param name="confirm">Confirmation flag (must be true)</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Personnel deleted successfully</response>
        /// <response code="400">Confirmation required or deletion failed</response>
        /// <response code="404">Personnel not found</response>
        /// <response code="500">Internal server error</response>
        /// <remarks>
        /// Deleting personnel will also delete all associated sales records (cascade delete).
        /// You must set confirm=true to proceed with deletion.
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeletePersonnel(int id, [FromQuery] bool confirm = false)
        {
            try
            {
                if (!confirm)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Delete confirmation required. Add ?confirm=true to the request."));
                }

                var exists = await _personnelService.PersonnelExistsAsync(id);
                if (!exists)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Personnel with ID {id} not found"));
                }

                var deleted = await _personnelService.DeletePersonnelAsync(id, confirm);
                if (!deleted)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Failed to delete personnel"));
                }

                return Ok(ApiResponse<object>.SuccessResult(null, "Personnel deleted successfully. Associated sales records have been removed."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting personnel with ID {PersonnelId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error"));
            }
        }

        private async Task<List<string>> ValidatePersonnelAsync(PersonnelDto personnelDto)
        {
            var errors = new List<string>();

            // Validate name is not empty after trimming
            if (string.IsNullOrWhiteSpace(personnelDto.Name))
            {
                errors.Add("Name cannot be empty or whitespace only");
            }

            // Validate phone is not empty after trimming
            if (string.IsNullOrWhiteSpace(personnelDto.Phone))
            {
                errors.Add("Phone cannot be empty or whitespace only");
            }

            // Validate age is 19 or older
            if (personnelDto.Age < 19)
            {
                errors.Add("Age must be 19 or older");
            }

            // Validate commission profile exists
            if (personnelDto.CommissionProfileId > 0)
            {
                var profileExists = await _personnelService.CommissionProfileExistsAsync(personnelDto.CommissionProfileId);
                if (!profileExists)
                {
                    errors.Add($"Commission profile with ID {personnelDto.CommissionProfileId} does not exist");
                }
            }
            else
            {
                errors.Add("Commission profile ID must be provided");
            }

            return errors;
        }
    }
}