using System.ComponentModel.DataAnnotations;

namespace ODataApi.Contract;

/// <summary>
/// Data transfer object (DTO) for cities. Used as input/output model in web api (OData) for cities
/// </summary>
public class CityDto
{
  /// <summary>
  /// Internal identifies (i.e. primary key for cities)
  /// </summary>
  [Key]
  public int CityId { get; set; }

  /// <summary>
  /// Postal code number
  /// </summary>
  [Required(ErrorMessage = "Postal code is required")]
  [Range(10, 99999, ErrorMessage = "Postal code should be in range: 10-99999")]
  public int PostalCode { get; set; }

  /// <summary>
  /// City name
  /// </summary>
  [Required(ErrorMessage = "City name is required")]
  public required string CityName { get; set; }

  /// <summary>
  /// Postal name
  /// </summary>
  public string? PostalName { get; set; }

  /// <summary>
  /// Country name
  /// </summary>
  public string CountryName { get; set; } = string.Empty;

  /// <summary>
  /// Country code
  /// </summary>
  [Required(ErrorMessage = "Country code is required")]    
  public required string CountryCode { get; set; }   
}