using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using DTOs = ODataApi.Contract;

namespace ODataApi
{
  public static class FirmaODataModelBuilder
  {
    public static IEdmModel GetEdmModel()
    {
      //definirati sve one na koje ide upit (bilo direktno ili kroz expand)
      //za one na koje se radi expand, samo definirati tip i ključ

      var builder = new ODataConventionModelBuilder();

      builder.EntitySet<DTOs.CityDto>("Cities")
                        .EntityType
                        .Filter() // Allow for the $filter Command
                        .Count() // Allow for the $count Command                     
                        .OrderBy() // Allow for the $orderby Command
                        .Page() // Allow for the $top and $skip Commands
                        .Select();// Allow for the $select Command;

      builder.EntitySet<EFModel.Document>("Documents")                      
                      .EntityType.HasKey(d => d.DocumentId)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command                                                                  
                      .HasMany(x => x.Items);

      builder.EntitySet<EFModel.Product>("Products")
                      .EntityType.HasKey(d => d.ProductNumber)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command                                                                  
                      .HasMany(x => x.Items);

      builder.EntityType<EFModel.Item>()
             .HasKey(s => s.ItemId);


      var partnerBuilder = builder.EntitySet<EFModel.Partner>("Partners")
                                  .EntityType.HasKey(d => d.PartnerId)
                                  .Filter() // Allow for the $filter Command
                                  .Count() // Allow for the $count Command
                                  .Expand() // Allow for the $expand Command
                                  .OrderBy() // Allow for the $orderby Command
                                  .Page() // Allow for the $top and $skip Commands
                                  .Select(); // Allow for the $select Command


      partnerBuilder.HasOptional(p => p.Company);
      partnerBuilder.HasOptional(p => p.Person);


      builder.EntitySet<EFModel.Person>("People")
                      .EntityType.HasKey(d => d.PersonId)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command                      
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select(); // Allow for the $select Command
                                 // 
      builder.EntitySet<EFModel.Company>("Companies")
                      .EntityType.HasKey(d => d.CompanyId)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command                      
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select(); // Allow for the $select Command 

      return builder.GetEdmModel();
    }
  }
}
