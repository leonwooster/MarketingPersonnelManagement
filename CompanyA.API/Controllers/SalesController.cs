using Microsoft.AspNetCore.Mvc;
using CompanyA.BusinessComponents;
using CompanyA.BusinessEntity;

namespace CompanyA.API.Controllers
{
    /// <summary>
    /// Sales management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesService salesService, ILogger<SalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// Get sales records with optional filtering
        /// </summary>
        /// <param name="personnelId">Filter by personnel ID</param>
        /// <param name="from">Start date filter (inclusive)</param>
        /// <param name="to">End date filter (inclusive)</param>
        /// <returns>List of sales records matching the criteria</returns>
        /// <response code="200">Returns the list of sales</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SalesDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SalesDto>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<SalesDto>>>> GetSales(
            [FromQuery] int? personnelId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            try
            {
                var sales = await _salesService.GetSalesAsync(personnelId, from, to);
                return Ok(ApiResponse<IEnumerable<SalesDto>>.SuccessResult(sales));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales with filters - PersonnelId: {PersonnelId}, From: {From}, To: {To}", 
                    personnelId, from, to);
                return StatusCode(500, ApiResponse<IEnumerable<SalesDto>>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Get sales record by ID
        /// </summary>
        /// <param name="id">Sales record ID</param>
        /// <returns>Sales record details</returns>
        /// <response code="200">Returns the sales record</response>
        /// <response code="404">Sales record not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SalesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SalesDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<SalesDto>), 500)]
        public async Task<ActionResult<ApiResponse<SalesDto>>> GetSalesById(int id)
        {
            try
            {
                var sales = await _salesService.GetSalesByIdAsync(id);
                if (sales == null)
                {
                    return NotFound(ApiResponse<SalesDto>.ErrorResult($"Sales record with ID {id} not found"));
                }

                return Ok(ApiResponse<SalesDto>.SuccessResult(sales));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales record with ID {SalesId}", id);
                return StatusCode(500, ApiResponse<SalesDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Create new sales record
        /// </summary>
        /// <param name="salesDto">Sales data to create</param>
        /// <returns>Created sales record</returns>
        /// <response code="201">Sales record created successfully</response>
        /// <response code="400">Validation errors or business rule violations</response>
        /// <response code="500">Internal server error</response>
        /// <remarks>
        /// Report date cannot be in the future. Sales amount must be non-negative.
        /// Personnel ID must reference an existing personnel record.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SalesDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<SalesDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<SalesDto>), 500)]
        public async Task<ActionResult<ApiResponse<SalesDto>>> CreateSales([FromBody] SalesDto salesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<SalesDto>.ErrorResult("Validation failed", errors));
                }

                var createdSales = await _salesService.CreateSalesAsync(salesDto);
                return CreatedAtAction(nameof(GetSalesById), new { id = createdSales.Id }, 
                    ApiResponse<SalesDto>.SuccessResult(createdSales));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Business rule violation when creating sales record");
                return BadRequest(ApiResponse<SalesDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sales record");
                return StatusCode(500, ApiResponse<SalesDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Delete sales record
        /// </summary>
        /// <param name="id">Sales record ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Sales record deleted successfully</response>
        /// <response code="404">Sales record not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSales(int id)
        {
            try
            {
                var exists = await _salesService.SalesExistsAsync(id);
                if (!exists)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Sales record with ID {id} not found"));
                }

                var deleted = await _salesService.DeleteSalesAsync(id);
                if (!deleted)
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResult("Failed to delete sales record"));
                }

                return Ok(ApiResponse<object>.SuccessResult($"Sales record with ID {id} deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sales record with ID {SalesId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error"));
            }
        }
    }
}
