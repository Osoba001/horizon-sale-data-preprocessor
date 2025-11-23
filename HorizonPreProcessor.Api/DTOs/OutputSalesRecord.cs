namespace HorizonPreProcessor.Api.DTOs
{
    public class OutputSalesRecord
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerForenames { get; set; } = string.Empty;
        public string CustomerSurname { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime PurchasedOn { get; set; }
        public DateOnly PurchasedOnDate { get; set; }
    }

    public class TransformationResult
    {
        public bool Success { get; set; }
        public List<OutputSalesRecord> Data { get; set; } = [];
        public string? Error { get; set; }
        public string? OriginalId { get; set; }

        public static TransformationResult SuccessResult(List<OutputSalesRecord> data)
            => new() { Success = true, Data = data };

        public static TransformationResult FailureResult(string error, string? originalId = null)
            => new() { Success = false, Error = error, OriginalId = originalId };
    }

    public class BatchTransformationResult
    {
        public List<OutputSalesRecord> SuccessfulRecords { get; set; } = new();
        public List<TransformationError> FailedRecords { get; set; } = new();
        public int TotalProcessed => SuccessfulRecords.Count + FailedRecords.Count;
        public int SuccessCount => SuccessfulRecords.Count;
        public int FailureCount => FailedRecords.Count;
    }

    public class TransformationError
    {
        public string? RecordIdentifier { get; set; }
        public string Error { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public string? RequestId { get; set; }
    }
}