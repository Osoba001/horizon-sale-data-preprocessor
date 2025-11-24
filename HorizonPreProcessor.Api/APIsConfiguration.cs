using HorizonPreProcessor.Api.DTOs;
using HorizonPreProcessor.Api.Services;
using System.Text.Json;

namespace HorizonPreProcessor.Api
{
    public static class APIsConfiguration
    {
        public static WebApplication MapSaleDataTransformApis(this WebApplication app)
        {
            app.MapPost("/api/SaleDataTransform", TransformStream);
            app.MapGet("/", () => Results.Ok(new
            {
                status = "Horizon PreProcessor API is running ✔️",
                instructions = "Please use the HorizonPreProcessor.Api.http file on the project or test from command line (curl or Postman) to POST data to /api/SaleDataTransform.",
                httpFile = "HorizonPreProcessor.Api.http",
                examplePost = "POST https://localhost:7085/api/SaleDataTransform",
                description = "Send your JSON array data in the POST body."
            }));

            return app;
        }

        private static async Task<IResult> TransformStream(HttpRequest request, ISalesDataTransformer transformer)
        {

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };

            var successful = new List<OutputSalesRecord>();
            var errors = new List<TransformationError>();
            int record = 0;

            try
            {
                await foreach (var input in JsonSerializer.DeserializeAsyncEnumerable<InputSalesRecord>(
                    request.Body, jsonOptions, cancellationToken: request.HttpContext.RequestAborted))
                {
                    record++;
                    if (input == null) continue;

                    var result = transformer.TransformSaleData(input);
                    if (result.Success)
                        successful.AddRange(result.Data);
                    else
                        errors.Add(new TransformationError
                        {
                            RecordIdentifier = input.Id ?? "Unknown",
                            Error = result.Error ?? "Transform failed"
                        });
                }
            }
            catch (JsonException ex)
            {
                request.HttpContext.RequestServices
                    .GetService<ILoggerFactory>()?
                    .CreateLogger("TransformStream")?
                    .LogError(ex, "JSON parsing error");

                return Results.BadRequest(CreateErrorResponse(request, $"Invalid JSON format: {ex.Message}"));
            }
            catch (Exception ex)
            {
                request.HttpContext.RequestServices
                    .GetService<ILoggerFactory>()?
                    .CreateLogger("TransformStream")?
                    .LogError(ex, "Unexpected error");

                return Results.StatusCode(500);
            }

            return Results.Ok(new
            {
                success = true,
                summary = new
                {
                    totalInputRecords = record,
                    totalFailed = errors.Count,
                    totalSalesRecords = successful.Count
                },
                errors,
                data = successful
            });
        }

        private static ApiResponse<object> CreateErrorResponse(HttpRequest request, string error)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Error = error,
                RequestId = request.HttpContext.TraceIdentifier
            };
        }
    }
}
