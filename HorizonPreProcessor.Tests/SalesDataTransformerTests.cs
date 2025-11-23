using Microsoft.Extensions.Logging;
using Moq;
using HorizonPreProcessor.Api.DTOs;
using HorizonPreProcessor.Api.Services;
using static HorizonPreProcessor.Api.Services.Validator;

namespace HorizonPreProcessor.Tests
{
    public class SalesDataTransformerTests
    {
        private readonly Mock<IAddressFormatter> _mockAddressFormatter;
        private readonly Mock<IDataParser> _mockDateTimeParser;
        private readonly Mock<IValidator> _mockValidator;
        private readonly Mock<ILogger<SalesDataTransformer>> _mockLogger;
        private readonly SalesDataTransformer _transformer;

        public SalesDataTransformerTests()
        {
            _mockAddressFormatter = new Mock<IAddressFormatter>();
            _mockDateTimeParser = new Mock<IDataParser>();
            _mockValidator = new Mock<IValidator>();
            _mockLogger = new Mock<ILogger<SalesDataTransformer>>();

            // Default setups
            _mockAddressFormatter.Setup(x => x.FormatCustomerAddress(It.IsAny<Customer>(), It.IsAny<Address>()))
                .Returns("Formatted Customer Address");
            _mockAddressFormatter.Setup(x => x.FormatShippingAddress(It.IsAny<Address>()))
                .Returns("Formatted Shipping Address");

            var testDateTime = DateTime.UtcNow;
            var testDateOnly = DateOnly.FromDateTime(testDateTime);
            _mockDateTimeParser.Setup(x => x.ParseDateTime(It.IsAny<string>()))
                .Returns((testDateTime, testDateOnly));

            _mockValidator.Setup(x => x.ValidateInputRecord(It.IsAny<InputSalesRecord>()))
                .Returns(new ValidationResult { IsValid = true });

            _transformer = new SalesDataTransformer(
                _mockAddressFormatter.Object,
                _mockDateTimeParser.Object,
                _mockValidator.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void TransformSaleData_ValidInput_ReturnsSuccessWithTransformedRecords()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);

            var firstRecord = result.Data[0];
            Assert.Equal("test-order-123", firstRecord.Id);
            Assert.Equal("12345", firstRecord.CustomerId);
            Assert.Equal("john.doe@example.com", firstRecord.CustomerEmail);
            Assert.Equal("John", firstRecord.CustomerForenames);
            Assert.Equal("Doe", firstRecord.CustomerSurname);
            Assert.Equal("Formatted Customer Address", firstRecord.CustomerAddress);
            Assert.Equal("Test Product 1", firstRecord.ProductName);
            Assert.Equal("SKU-001", firstRecord.ProductSku);
            Assert.Equal(2, firstRecord.ProductQuantity);
            Assert.Equal(29.99m, firstRecord.ProductPrice);
            Assert.Equal("Formatted Shipping Address", firstRecord.ShippingAddress);
        }

        [Fact]
        public void TransformSaleData_ValidationFails_ReturnsFailure()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            _mockValidator.Setup(x => x.ValidateInputRecord(It.IsAny<InputSalesRecord>()))
                .Returns(new ValidationResult { IsValid = false, ErrorMessage = "Validation failed" });

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Data);
            Assert.Equal("Validation failed", result.Error);
            Assert.Equal("test-order-123", result.OriginalId);
        }

        [Fact]
        public void TransformSaleData_NoItems_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.Items = new List<OrderItem>();

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public void TransformSaleData_ItemWithMissingSku_LogsWarningAndSkipsItem()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.Items[0].Sku = null; // Invalid item

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            Assert.Single(result.Data); // Only the second valid item should be processed
            Assert.Equal("SKU-002", result.Data[0].ProductSku);
        }

        [Fact]
        public void TransformSaleData_ItemWithMissingTitle_LogsWarningAndSkipsItem()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.Items[1].Title = null; // Invalid item

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            Assert.Single(result.Data); // Only the first valid item should be processed
            Assert.Equal("SKU-001", result.Data[0].ProductSku);
        }

        [Fact]
        public void TransformSaleData_NullItem_LogsWarningAndSkipsItem()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.Items.Add(null); // Add a null item

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count); // Only valid items should be processed
        }


        [Fact]
        public void TransformSaleData_NullCustomer_UsesDefaultValues()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.Customer = null;

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            var outputRecord = result.Data[0];
            Assert.Equal("Unknown", outputRecord.CustomerId);
            Assert.Equal(string.Empty, outputRecord.CustomerEmail);
            Assert.Equal(string.Empty, outputRecord.CustomerForenames);
            Assert.Equal(string.Empty, outputRecord.CustomerSurname);
        }

        [Fact]
        public void TransformSaleData_NullOrderId_GeneratesNewId()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.Id = null;

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            var outputRecord = result.Data[0];
            Assert.NotNull(outputRecord.Id);
            Assert.NotEmpty(outputRecord.Id);
            Assert.Equal(32, outputRecord.Id.Length);
        }

        [Fact]
        public void TransformSaleData_UsesCompletedAtDate_WhenAvailable()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            var expectedDateTime = DateTime.UtcNow.AddDays(-1);
            var expectedDateOnly = DateOnly.FromDateTime(expectedDateTime);

            _mockDateTimeParser.Setup(x => x.ParseDateTime("2023-11-28T15:17:17.410Z"))
                .Returns((expectedDateTime, expectedDateOnly));

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            var outputRecord = result.Data[0];
            Assert.Equal(expectedDateTime, outputRecord.PurchasedOn);
            Assert.Equal(expectedDateOnly, outputRecord.PurchasedOnDate);
        }

        [Fact]
        public void TransformSaleData_UsesStartedAtDate_WhenCompletedAtIsNull()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.CompletedAt = null;
            inputRecord.StartedAt = "2023-11-28T14:17:17.410Z";

            var expectedDateTime = DateTime.UtcNow.AddDays(-1);
            var expectedDateOnly = DateOnly.FromDateTime(expectedDateTime);

            _mockDateTimeParser.Setup(x => x.ParseDateTime("2023-11-28T14:17:17.410Z"))
                .Returns((expectedDateTime, expectedDateOnly));

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            var outputRecord = result.Data[0];
            Assert.Equal(expectedDateTime, outputRecord.PurchasedOn);
            Assert.Equal(expectedDateOnly, outputRecord.PurchasedOnDate);
        }

        [Fact]
        public void TransformSaleData_UsesCurrentDate_WhenBothDatesAreNull()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();
            inputRecord.CompletedAt = null;
            inputRecord.StartedAt = null;

            var currentDateTime = DateTime.UtcNow;
            var currentDateOnly = DateOnly.FromDateTime(currentDateTime);

            _mockDateTimeParser.Setup(x => x.ParseDateTime(It.IsAny<string>()))
                .Returns((currentDateTime, currentDateOnly));

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            Assert.True(result.Success);
            var outputRecord = result.Data[0];
            Assert.Equal(currentDateTime, outputRecord.PurchasedOn);
            Assert.Equal(currentDateOnly, outputRecord.PurchasedOnDate);
        }

        [Fact]
        public void TransformSaleData_CallsAddressFormatter_WithCorrectParameters()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            _mockAddressFormatter.Verify(x => x.FormatCustomerAddress(
                It.Is<Customer>(c => c.Id == 12345),
                It.Is<Address>(a => a.Street == "123 Main St")),
                Times.Exactly(2)); // Called for each item

            _mockAddressFormatter.Verify(x => x.FormatShippingAddress(
                It.Is<Address>(a => a.Street == "456 Shipping St")),
                Times.Exactly(2)); // Called for each item
        }

        [Fact]
        public void TransformSaleData_CallsDateTimeParser_WithCorrectParameter()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            _mockDateTimeParser.Verify(x => x.ParseDateTime("2023-11-28T15:17:17.410Z"),
                Times.Exactly(2)); // Called for each item
        }

        [Fact]
        public void TransformSaleData_CallsValidator_WithCorrectParameter()
        {
            // Arrange
            var inputRecord = CreateValidInputRecord();

            // Act
            var result = _transformer.TransformSaleData(inputRecord);

            // Assert
            _mockValidator.Verify(x => x.ValidateInputRecord(
                It.Is<InputSalesRecord>(r => r.Id == "test-order-123")),
                Times.Once);
        }

        private InputSalesRecord CreateValidInputRecord()
        {
            return new InputSalesRecord
            {
                Id = "test-order-123",
                BillingAddress = new Address
                {
                    Street = "123 Main St",
                    City = "Test City",
                    State = "Test State",
                    Country = "Test Country"
                },
                CompletedAt = "2023-11-28T15:17:17.410Z",
                StartedAt = "2023-11-28T14:17:17.410Z",
                Customer = new Customer
                {
                    Id = 12345,
                    Email = "john.doe@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Company = "Test Company",
                    Country = "Test Country"
                },
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Sku = "SKU-001",
                        Title = "Test Product 1",
                        Quantity = 2,
                        Price = 29.99m,
                        Vendor = "Test Vendor 1"
                    },
                    new OrderItem
                    {
                        Sku = "SKU-002",
                        Title = "Test Product 2",
                        Quantity = 1,
                        Price = 49.99m,
                        Vendor = "Test Vendor 2"
                    }
                },
                ShippingAddress = new Address
                {
                    Street = "456 Shipping St",
                    City = "Shipping City",
                    State = "Shipping State",
                    Country = "Shipping Country"
                },
                Status = "complete",
                Source = "Web"
            };
        }
    }
}