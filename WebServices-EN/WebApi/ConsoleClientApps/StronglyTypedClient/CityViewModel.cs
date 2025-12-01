namespace FirmOpenApiClients;

public partial class CityViewModel
{
  public override string ToString()
  {
    return $"{CityId}. {PostalCode} {CityName} {PostalName} ({CountryName})";
  }
}
