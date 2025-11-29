using EFModel;

namespace ODataApi.Controllers;

public class ProductsController : GenericController<Product, int>
{
  public ProductsController(FirmContext ctx, ILogger<ProductsController> logger) : base(ctx, logger)
  {

  }
}

