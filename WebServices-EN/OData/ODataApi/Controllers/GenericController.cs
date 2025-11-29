using EFModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace ODataApi.Controllers;

/// <summary>
/// Generic controller to get data using OData methods
/// </summary>
/// <typeparam name="T">entity type</typeparam>
/// <typeparam name="K">primary key type (e.g. int, string, ...)</typeparam>
public abstract class GenericController<T, K> : ODataController where T:class
{
  protected readonly FirmContext ctx;
  protected readonly ILogger logger;

  protected GenericController(FirmContext ctx, ILogger<GenericController<T, K>> logger)
  {
    this.logger = logger;
    this.ctx = ctx;
  }

  // GET: odata/T
  /// <summary>
  /// Get up to 50 itemas based on OData criteria
  /// </summary>
  /// <returns>IQueryable<typeparamref name="T"/> which can further be filtered and used with OData methods</returns>
  [EnableQuery(PageSize = 50)]
  public virtual IQueryable<T> Get()
  {
    logger.LogTrace($"Get {Request.QueryString.Value}");
    var query = ctx.Set<T>().AsNoTracking();
    return query;
  }

  // GET: odata/T(key)
  /// <summary>
  /// Postupak za dohvat nekog zapisa po cjelobrojnom primarnom ključu. 
  /// </summary>
  /// <returns>podatak</returns>
  [EnableQuery]
  public virtual async Task<ActionResult<T>> Get(K key)
  {
    logger.LogTrace($"Get + key = {key} {Request.QueryString.Value}");
    var item = await ctx.Set<T>()
                        .FindAsync(key);                             
    if (item != null)
    {
      return item;        
    }
    else
    {
      return NotFound("No element with key: " + key);
    }
  }
}