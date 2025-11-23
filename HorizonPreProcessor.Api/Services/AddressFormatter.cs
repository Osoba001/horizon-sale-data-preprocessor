using HorizonPreProcessor.Api.DTOs;

namespace HorizonPreProcessor.Api.Services
{
    public interface IAddressFormatter
    {
        string FormatBillingAddress(Address? address);
        string FormatShippingAddress(Address? address);
        public string FormatCustomerAddress(Customer? customer, Address? billingAddress);
    }

    public class AddressFormatter : IAddressFormatter
    {
        public string FormatBillingAddress(Address? address)
        {
            if (address == null) return string.Empty;

            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(address.Name))
                parts.Add(address.Name.Trim());

            if (!string.IsNullOrWhiteSpace(address.Street))
                parts.Add(address.Street.Trim());

            if (!string.IsNullOrWhiteSpace(address.City))
                parts.Add(address.City.Trim());

            if (!string.IsNullOrWhiteSpace(address.State))
                parts.Add(address.State.Trim());

            if (!string.IsNullOrWhiteSpace(address.Country))
                parts.Add(address.Country.Trim());

            return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
        }

        public string FormatShippingAddress(Address? address)
        {
            if (address == null) return string.Empty;

            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(address.Name))
                parts.Add(address.Name.Trim());
            else
                parts.Add("Recipient Name Not Provided");

            if (!string.IsNullOrWhiteSpace(address.Street))
                parts.Add(address.Street.Trim());

            if (!string.IsNullOrWhiteSpace(address.City))
                parts.Add(address.City.Trim());

            if (!string.IsNullOrWhiteSpace(address.State))
                parts.Add(address.State.Trim());

            if (!string.IsNullOrWhiteSpace(address.Country))
                parts.Add(address.Country.Trim());

            var formattedAddress = string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));

            return formattedAddress == "Recipient Name Not Provided" ? string.Empty : formattedAddress;
        }

        public string FormatCustomerAddress(Customer? customer, Address? billingAddress)
        {
            if (billingAddress != null)
            {
                return FormatBillingAddress(billingAddress);
            }

            if (customer != null)
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(customer.FirstName) || !string.IsNullOrWhiteSpace(customer.LastName))
                    parts.Add($"{customer.FirstName} {customer.LastName}".Trim());

                if (!string.IsNullOrWhiteSpace(customer.Company))
                    parts.Add(customer.Company);

                if (!string.IsNullOrWhiteSpace(customer.Country))
                    parts.Add(customer.Country);

                return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
            }

            return string.Empty;
        }
    }
}
