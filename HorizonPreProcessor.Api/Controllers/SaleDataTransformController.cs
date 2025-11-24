using HorizonPreProcessor.Api.DTOs;
using HorizonPreProcessor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HorizonPreProcessor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleDataTransformController : ControllerBase
    {
        private readonly ISalesDataTransformer _transformer;
        private readonly ILogger<SaleDataTransformController> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public SaleDataTransformController(ISalesDataTransformer transformer, ILogger<SaleDataTransformController> logger)
        {
            _transformer = transformer;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                AllowTrailingCommas = true
            };
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<List<OutputSalesRecord>>), 200)]
        public async Task<IActionResult> TransformStream()
        {

            var successful = new List<OutputSalesRecord>();
            var errors = new List<TransformationError>();
            int record= 0;
            try
            {
                await foreach (var input in JsonSerializer.DeserializeAsyncEnumerable<InputSalesRecord>(Request.Body, _jsonOptions, cancellationToken: HttpContext.RequestAborted))
                {
                    record++;
                    if (input == null) continue;

                    var result = _transformer.TransformSaleData(input);
                    if (result.Success)
                        successful.AddRange(result.Data);
                    else
                        errors.Add(new TransformationError { RecordIdentifier = input.Id ?? "Unknown", Error = result.Error ?? "Transform failed" });
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error");
                return BadRequest(CreateErrorResponse($"Invalid JSON format: {ex.Message}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                return StatusCode(500, CreateErrorResponse("Internal server error"));
            }

            return Ok(new { success = true, 
                summary = new { totalInputRecords = record, totalFailed = errors.Count, totalSalesRecords = successful.Count,  },
                errors,
                data = successful});
        }

        private ApiResponse<object> CreateErrorResponse(string error)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Error = error,
                RequestId = HttpContext.TraceIdentifier
            };
        }
    }
}