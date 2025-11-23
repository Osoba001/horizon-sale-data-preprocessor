using System.Text.Json.Serialization;

namespace HorizonPreProcessor.Api.DTOs
{
    public class InputSalesRecord
    {
        [JsonPropertyName("billing_address")]
        public Address? BillingAddress { get; set; }

        [JsonPropertyName("completed_at")]
        public string? CompletedAt { get; set; }

        [JsonPropertyName("customer")]
        public Customer? Customer { get; set; }

        [JsonPropertyName("discounts")]
        public List<object> Discounts { get; set; } = [];

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("items")]
        public List<OrderItem> Items { get; set; } = [];

        [JsonPropertyName("notes")]
        public List<object> Notes { get; set; } = [];

        [JsonPropertyName("payment")]
        public Payment? Payment { get; set; }

        [JsonPropertyName("processor_response")]
        public ProcessorResponse? ProcessorResponse { get; set; }

        [JsonPropertyName("referral")]
        public Referral? Referral { get; set; }

        [JsonPropertyName("shipping_address")]
        public Address? ShippingAddress { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("started_at")]
        public string? StartedAt { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("address")]
        public string? Street { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("lat")]
        public string? Latitude { get; set; }

        [JsonPropertyName("lon")]
        public string? Longitude { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }

    public class Customer
    {
        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("first")]
        public string? FirstName { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("last")]
        public string? LastName { get; set; }
    }

    public class OrderItem
    {
        [JsonPropertyName("discounts")]
        public List<object> Discounts { get; set; } = new();

        [JsonPropertyName("fulfillment")]
        public string? Fulfillment { get; set; }

        [JsonPropertyName("gift_card")]
        public bool GiftCard { get; set; }

        [JsonPropertyName("grams")]
        public string? Grams { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("requires_shipping")]
        public bool RequiresShipping { get; set; }

        [JsonPropertyName("sku")]
        public string? Sku { get; set; }

        [JsonPropertyName("taxable")]
        public bool Taxable { get; set; }

        [JsonPropertyName("taxes")]
        public List<object> Taxes { get; set; } = new();

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("vendor")]
        public string? Vendor { get; set; }
    }

    public class Payment
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("authorization")]
        public string? Authorization { get; set; }

        [JsonPropertyName("gateway")]
        public string? Gateway { get; set; }

        [JsonPropertyName("last_four")]
        public string? LastFour { get; set; }
    }

    public class ProcessorResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("amount_refunded")]
        public decimal AmountRefunded { get; set; }

        [JsonPropertyName("application_fee")]
        public object? ApplicationFee { get; set; }

        [JsonPropertyName("balance_transaction")]
        public string? BalanceTransaction { get; set; }

        [JsonPropertyName("captured")]
        public bool Captured { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("customer")]
        public object? Customer { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("dispute")]
        public object? Dispute { get; set; }

        [JsonPropertyName("failure_code")]
        public object? FailureCode { get; set; }

        [JsonPropertyName("failure_message")]
        public object? FailureMessage { get; set; }

        [JsonPropertyName("fraud_details")]
        public FraudDetails FraudDetails { get; set; } = new();

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("invoice")]
        public object? Invoice { get; set; }

        [JsonPropertyName("livemode")]
        public bool Livemode { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("paid")]
        public bool Paid { get; set; }

        [JsonPropertyName("receipt_email")]
        public object? ReceiptEmail { get; set; }

        [JsonPropertyName("receipt_number")]
        public object? ReceiptNumber { get; set; }

        [JsonPropertyName("refunded")]
        public bool Refunded { get; set; }

        [JsonPropertyName("refunds")]
        public Refunds Refunds { get; set; } = new();

        [JsonPropertyName("shipping")]
        public object? Shipping { get; set; }

        [JsonPropertyName("source")]
        public Source Source { get; set; } = new();

        [JsonPropertyName("statement_descriptor")]
        public object? StatementDescriptor { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class FraudDetails
    {
        
    }

    public class Refunds
    {
        [JsonPropertyName("data")]
        public List<object> Data { get; set; } = new();

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class Source
    {
        [JsonPropertyName("brand")]
        public string? Brand { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("customer")]
        public object? Customer { get; set; }

        [JsonPropertyName("cvc_check")]
        public string? CvcCheck { get; set; }

        [JsonPropertyName("exp_month")]
        public int ExpMonth { get; set; }

        [JsonPropertyName("exp_year")]
        public int ExpYear { get; set; }

        [JsonPropertyName("funding")]
        public string? Funding { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("last4")]
        public string? Last4 { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }
    }

    public class Referral
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("landing_page")]
        public string? LandingPage { get; set; }

        [JsonPropertyName("site")]
        public string? Site { get; set; }
    }
}