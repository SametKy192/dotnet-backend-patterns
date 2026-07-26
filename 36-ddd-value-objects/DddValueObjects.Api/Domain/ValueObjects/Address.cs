namespace DddValueObjects.Api.Domain.ValueObjects;

public record Address
{
    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }
    public string Country { get; }

    public Address(string street, string city, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street is required.");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.");
        if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException("ZipCode is required.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.");

        Street = street.Trim();
        City = city.Trim();
        ZipCode = zipCode.Trim();
        Country = country.Trim();
    }

    public override string ToString() => $"{Street}, {City}, {ZipCode}, {Country}";
}
