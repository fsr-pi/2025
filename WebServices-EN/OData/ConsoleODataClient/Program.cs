using EFModel; //referenced for the sake of simplicity - a separate set of classes should be created or generate using scheme
using DTOs = ODataApi.Contract; //shared project, just simple DTO objects
using Simple.OData.Client;
using System;
using System.Threading.Tasks;

namespace ConsoleODataClient;

class Program
{
  const string url = "https://localhost:59226/odata";
  static async Task Main(string[] args)
  {
    ODataClientSettings settings = new ODataClientSettings(new Uri(url));
    settings.IgnoreResourceNotFoundException = true;
    settings.OnTrace = (x, y) => Console.WriteLine(string.Format(x, y));

    var client = new ODataClient(settings);

    await SearchForCity(client);
    await AddNewCity(client);
    await GetDocuments(client);
  }

  private static async Task SearchForCity(ODataClient client)
  {
    Console.WriteLine("Number of cities: " + await client.For<DTOs.CityDto>("Cities").Count().FindScalarAsync<int>());

    Console.WriteLine("City with id: 25");
    await PrintCity(client, 25);

    Console.WriteLine("all cities in Croatia with postal number between 10000 and 10500 that does not contain word Zagreb:");   
    var cities = await client.For<DTOs.CityDto>("Cities")
                             .Filter(m => m.CountryCode == "HR")
                             .Filter(m => m.PostalCode >= 10000 && m.PostalCode < 10500)
                             .Filter(m => !m.CityName.Contains("Zagreb"))
                             .Select(m => new { m.PostalCode, m.CityName })
                             .OrderBy(m => m.PostalCode)
                             .FindEntriesAsync();

    foreach (var city in cities)
    {
      Console.WriteLine($"{city.PostalCode} {city.CityName}");
    }
  }

  private static async Task AddNewCity(ODataClient client)
  {
    var newcity = new DTOs.CityDto
    {
      PostalCode = 59999,
      CityName = "Test city",
      CountryCode = "_A",
      PostalName = "Test postal name"
    };

    try
    {
      var city = await client.For<DTOs.CityDto>("Cities")
                                .Set(newcity)
                                .InsertEntryAsync();
      Console.WriteLine("City added with id: " + city.CityId);
      await PrintCity(client, city.CityId);

      city.CityName += "_";
      city = await client.For<DTOs.CityDto>("Cities")
                           .Key(city.CityId)                           
                           .Set(new { CityName = "_New TEST name_" })
                           .UpdateEntryAsync();
      Console.WriteLine("City updated");
      await PrintCity(client, city.CityId);



      await client.For<DTOs.CityDto>("Cities")
                  .Key(city.CityId)
                  .DeleteEntryAsync();

      Console.WriteLine("City deleted");
    }
    catch (WebRequestException exc)
    {
      Console.WriteLine(exc.Message);
      Console.WriteLine(exc.Response);
    }
  }

  private static async Task GetDocuments(ODataClient client)
  {
    Console.WriteLine("Number of documents: " + await client.For<Document>().Count().FindScalarAsync<int>());

    //First 3 document of a person which first name starts with Jeff
    //and where created since 2016.
    //with amount larger than 10000
    var documents = await client.For<Document>()
                                .Filter(d => d.Partner.Person.FirstName.StartsWith("Jeff"))
                                .Filter(d => d.DocumentDate > new DateTime(2016, 1, 1))
                                .Filter(d => d.Amount > 10000)
                                .OrderByDescending(d => d.Amount)
                                .Expand(d => d.Partner.Person)                                  
                                .Select(d => new {d.DocumentId, d.DocumentDate, d.Amount,
                                                  d.Partner.Person.FirstName,
                                                  d.Partner.Person.LastName,
                                                  })                                  
                                .Top(3)                                  
                                .FindEntriesAsync();
    foreach (var d in documents)
    {        
      Console.WriteLine($"{d.DocumentId}/{d.DocumentDate:d.M.yyyy} {d.Partner.Person.FirstName} {d.Partner.Person.LastName}");        
    }

    documents = await client.For<Document>()
                            .Filter(d => d.Partner.Person.FirstName.StartsWith("Jeff"))
                            .Filter(d => d.DocumentDate > new DateTime(2016, 1, 1))
                            .Filter(d => d.Amount > 10000)
                            .Expand(d => d.Items)
                            .OrderByDescending(d => d.Amount)
                            .Select(d => new {
                              d.DocumentId,
                              d.DocumentDate,  
                              d.Items
                            })
                            .Top(3)
                            .FindEntriesAsync();
    foreach (var d in documents)
    {
      Console.WriteLine($"{d.DocumentId}/{d.DocumentDate:d.M.yyyy}");
      foreach (var item in d.Items)
      {
        Console.WriteLine($"\t {item.Quantity:C2} x {item.ProductNumber} {item.UnitPrice:N2}");
      }
    }
  }

  private static async Task PrintCity(ODataClient client, int key)
  {
    var city = await client.For<DTOs.CityDto>("Cities")
                           .Key(key)
                           .FindEntryAsync();
    if (city != null)
    {
      Console.WriteLine($"CityId: {city.CityId} {city.CountryCode}-{city.PostalCode} {city.CityName}");
    }
    else
    {
      Console.WriteLine($"There is no city with id: {key}");
    }
  }      
}
