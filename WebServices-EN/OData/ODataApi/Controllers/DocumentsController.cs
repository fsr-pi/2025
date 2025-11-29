using EFModel;

namespace ODataApi.Controllers;

public class DocumentsController : GenericController<Document, int>
{
  public DocumentsController(FirmContext ctx, ILogger<DocumentsController> logger) : base(ctx, logger)
  {

  }
}