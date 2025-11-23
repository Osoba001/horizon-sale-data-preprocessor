using HorizonPreProcessor.Api.DTOs;

namespace HorizonPreProcessor.Api.Services
{
    public interface ISalesDataTransformer
    {
        TransformationResult TransformSaleData(InputSalesRecord inputRecord);
    }

    public class SalesDataTransformer(
        IAddressFormatter addressFormatter,
        IDataParser dateTimeParser,
        IValidator validator,
        ILogger<SalesDataTransformer> logger) : ISalesDataTransformer
    {
        private readonly IAddressFormatter _addressFormatter = addressFormatter;
        private readonly IDataParser _dateTimeParser = dateTimeParser;
        private readonly IValidator _validator = validator;
        private readonly ILogger<SalesDataTransformer> _logger = logger;

        public TransformationResult TransformSaleData(InputSalesRecord input)
        {
            var validationResult = _validator.ValidateInputRecord(input);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for record {RecordId}: {Error}",
                    input.Id, validationResult.ErrorMessage);
                return TransformationResult.FailureResult(validationResult.ErrorMessage!, input.Id);
            }

            var outputRecords = new List<OutputSalesRecord>();

            foreach (var item in input.Items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Sku) || string.IsNullOrWhiteSpace(item.Title))
                {
                    _logger.LogWarning("Skipping invalid item in order {OrderId}", input.Id);
                    continue;
                }

                var (purchasedOn, purchasedOnDate) = _dateTimeParser.ParseDateTime(
                        input.CompletedAt ?? input.StartedAt ?? DateTime.UtcNow.ToString("O"));

                var customerAddress = _addressFormatter.FormatCustomerAddress(input.Customer, input.BillingAddress);
                var shippingAddress = _addressFormatter.FormatShippingAddress(input.ShippingAddress);

                var outputRecord = new OutputSalesRecord
                {
                    Id = input.Id ?? Guid.NewGuid().ToString("N"),
                    CustomerId = input.Customer?.Id.ToString() ?? "Unknown",
                    CustomerEmail = input.Customer?.Email ?? string.Empty,
                    CustomerForenames = input.Customer?.FirstName ?? string.Empty,
                    CustomerSurname = input.Customer?.LastName ?? string.Empty,
                    CustomerAddress = customerAddress,
                    ProductName = item.Title,
                    ProductSku = item.Sku,
                    ProductQuantity = item.Quantity,
                    ProductPrice = item.Price,
                    ShippingAddress = shippingAddress,
                    PurchasedOn = purchasedOn,
                    PurchasedOnDate = purchasedOnDate
                };

                outputRecords.Add(outputRecord);
            }
            return TransformationResult.SuccessResult(outputRecords);
        }


       
    }
}