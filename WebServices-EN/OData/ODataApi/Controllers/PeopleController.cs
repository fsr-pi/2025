using EFModel;

namespace ODataApi.Controllers;

public class PeopleController : GenericController<Person, int>
{  
  public PeopleController(FirmContext ctx, ILogger<PeopleController> logger) : base(ctx, logger)
  {

  }
}