using Microsoft.AspNetCore.Mvc;
using CompanyA.BusinessComponents;
using CompanyA.BusinessEntity;

namespace CompanyA.API.Controllers
{
    /// <summary>
    /// Commission profile management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommissionProfileController : ControllerBase
    {
        private readonly ICommissionProfileService _commissionProfileService;
        private readonly ILogger<CommissionProfileController> _logger;

        public CommissionProfileController(ICommissionProfileService commissionProfileService, ILogger<CommissionProfileController> logger)
        {
            _commissionProfileService = commissionProfileService;
            _logger = logger;
        }

        /// <summary>
        /// Get all commission profiles
        /// </summary>
        /// <returns>List of all commission profiles</returns>
        /// <response code="200">Returns the list of commission profiles</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CommissionProfileDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CommissionProfileDto>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommissionProfileDto>>>> GetAllCommissionProfiles()
        {
            try
            {
                var profiles = await _commissionProfileService.GetAllCommissionProfilesAsync();
                return Ok(ApiResponse<IEnumerable<CommissionProfileDto>>.SuccessResult(profiles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all commission profiles");
                return StatusCode(500, ApiResponse<IEnumerable<CommissionProfileDto>>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Get commission profile by ID
        /// </summary>
        /// <param name="id">Commission profile ID</param>
        /// <returns>Commission profile details</returns>
        /// <response code="200">Returns the commission profile</response>
        /// <response code="404">Commission profile not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 500)]
        public async Task<ActionResult<ApiResponse<CommissionProfileDto>>> GetCommissionProfileById(int id)
        {
            try
            {
                var profile = await _commissionProfileService.GetCommissionProfileByIdAsync(id);
                if (profile == null)
                {
                    return NotFound(ApiResponse<CommissionProfileDto>.ErrorResult($"Commission profile with ID {id} not found"));
                }

                return Ok(ApiResponse<CommissionProfileDto>.SuccessResult(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving commission profile with ID {ProfileId}", id);
                return StatusCode(500, ApiResponse<CommissionProfileDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Create new commission profile
        /// </summary>
        /// <param name="profileDto">Commission profile data to create</param>
        /// <returns>Created commission profile</returns>
        /// <response code="201">Commission profile created successfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="500">Internal server error</response>
        /// <remarks>
        /// Profile name must be an integer. Commission fixed uses decimal(10,2) precision.
        /// Commission percentage uses decimal(10,6) precision.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 500)]
        public async Task<ActionResult<ApiResponse<CommissionProfileDto>>> CreateCommissionProfile([FromBody] CommissionProfileDto profileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<CommissionProfileDto>.ErrorResult("Validation failed", errors));
                }

                var createdProfile = await _commissionProfileService.CreateCommissionProfileAsync(profileDto);
                return CreatedAtAction(nameof(GetCommissionProfileById), new { id = createdProfile.Id }, 
                    ApiResponse<CommissionProfileDto>.SuccessResult(createdProfile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating commission profile");
                return StatusCode(500, ApiResponse<CommissionProfileDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Update existing commission profile
        /// </summary>
        /// <param name="id">Commission profile ID to update</param>
        /// <param name="profileDto">Updated commission profile data</param>
        /// <returns>Updated commission profile</returns>
        /// <response code="200">Commission profile updated successfully</response>
        /// <response code="400">Validation errors</response>
        /// <response code="404">Commission profile not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<CommissionProfileDto>), 500)]
        public async Task<ActionResult<ApiResponse<CommissionProfileDto>>> UpdateCommissionProfile(int id, [FromBody] CommissionProfileDto profileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<CommissionProfileDto>.ErrorResult("Validation failed", errors));
                }

                profileDto.Id = id;
                var updatedProfile = await _commissionProfileService.UpdateCommissionProfileAsync(id, profileDto);
                if (updatedProfile == null)
                {
                    return NotFound(ApiResponse<CommissionProfileDto>.ErrorResult($"Commission profile with ID {id} not found"));
                }

                return Ok(ApiResponse<CommissionProfileDto>.SuccessResult(updatedProfile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating commission profile with ID {ProfileId}", id);
                return StatusCode(500, ApiResponse<CommissionProfileDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Delete commission profile
        /// </summary>
        /// <param name="id">Commission profile ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Commission profile deleted successfully</response>
        /// <response code="400">Cannot delete profile referenced by personnel</response>
        /// <response code="404">Commission profile not found</response>
        /// <response code="500">Internal server error</response>
        /// <remarks>
        /// Cannot delete commission profiles that are referenced by personnel records.
        /// Remove or reassign personnel first before deleting the profile.
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCommissionProfile(int id)
        {
            try
            {
                var exists = await _commissionProfileService.CommissionProfileExistsAsync(id);
                if (!exists)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Commission profile with ID {id} not found"));
                }

                var deleted = await _commissionProfileService.DeleteCommissionProfileAsync(id);
                if (!deleted)
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResult("Failed to delete commission profile"));
                }

                return Ok(ApiResponse<object>.SuccessResult($"Commission profile with ID {id} deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Attempt to delete commission profile with personnel references - ID {ProfileId}", id);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting commission profile with ID {ProfileId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error"));
            }
        }
    }
}
