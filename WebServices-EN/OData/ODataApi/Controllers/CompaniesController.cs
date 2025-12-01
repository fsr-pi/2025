using EFModel;

namespace ODataApi.Controllers;

public class CompaniesController : GenericController<Company, int>
{  
  public CompaniesController(FirmContext ctx, ILogger<CompaniesController> logger) : base(ctx, logger)
  {

  }
}