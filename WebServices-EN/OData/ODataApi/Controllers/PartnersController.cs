using EFModel;

namespace ODataApi.Controllers;

public class PartnersController : GenericController<Partner, int>
{
  public PartnersController(FirmContext ctx, ILogger<PartnersController> logger) : base(ctx, logger)
  {

  }
}