using HorizonPreProcessor.Api.DTOs;

namespace HorizonPreProcessor.Api.Services
{
    public interface IValidator
    {
        Validator.ValidationResult ValidateInputRecord(InputSalesRecord record);
    }
    public class Validator : IValidator
    {
        public ValidationResult ValidateInputRecord(InputSalesRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.Id))
                return ValidationResult.Failure("Order ID is required");

            if (record.Customer == null)
                return ValidationResult.Failure("Customer information is required");

            if (string.IsNullOrWhiteSpace(record.Customer.Email))
                return ValidationResult.Failure("Customer email is required");

            if (record.Items == null || record.Items.Count==0)
                return ValidationResult.Failure("At least one order item is required");

            foreach (var item in record.Items)
            {
                if (item == null)
                    return ValidationResult.Failure("Order item cannot be null");

                if (string.IsNullOrWhiteSpace(item.Sku))
                    return ValidationResult.Failure("Product SKU is required for all items");

                if (string.IsNullOrWhiteSpace(item.Title))
                    return ValidationResult.Failure("Product title is required for all items");

                if (item.Quantity <= 0)
                    return ValidationResult.Failure("Product quantity must be greater than 0");

                if (item.Price < 0)
                    return ValidationResult.Failure("Product price cannot be negative");
            }

            return ValidationResult.Success();
        }



        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string? ErrorMessage { get; set; }

            public static ValidationResult Success() => new() { IsValid = true };
            public static ValidationResult Failure(string error) => new() { IsValid = false, ErrorMessage = error };
        }
    }
}
