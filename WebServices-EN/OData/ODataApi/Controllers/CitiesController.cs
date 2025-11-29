using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataApi.Contract;
using System.Text.Json;

namespace ODataApi.Controllers;


/// <summary>
/// A slightly different example than other, as it used CityDto instead of City directly 
/// from EF model
/// </summary>
public class CitiesController : ODataController
{
  private readonly EFModel.FirmContext ctx;
  private readonly ILogger logger;
  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="ctx">FirmContext to connect to the database</param>
  /// <param name="logger">Logger for logging errors</param>
  public CitiesController(EFModel.FirmContext ctx, ILogger<CitiesController> logger)
  {
    this.logger = logger;
    this.ctx = ctx;
  }

  // GET: odata/Cities
  /// <summary>
  /// Get all cities (up to 50 cities per request). 
  /// </summary>
  /// <returns>IQueryable of cities. Use OData for further manipulation</returns>    
  [EnableQuery(PageSize = 50)]
  public IQueryable<CityDto> Get()
  {
    logger.LogTrace("Get: " + Request.QueryString.Value);
    var query = ctx.Cities
                   .Select(m => new CityDto
                   {
                     CityId = m.CityId,
                     CityName = m.CityName,
                     PostalCode = m.PostalCode,
                     PostalName = m.PostalName,
                     CountryCode = m.CountryCode,
                     CountryName = m.CountryCodeNavigation.CountryName
                   });
    return query;
  }

  // GET: odata/Cities(key)
  /// <summary>
  /// Get city based on primary key. 
  /// </summary>
  /// <returns>CityDto, i.e. information about the city</returns>
  public async Task<ActionResult<CityDto>> Get(int key)
  {
    logger.LogTrace($"Get + key = {key} {Request.QueryString.Value}");
    var city = await ctx.Cities
                          .Where(m => m.CityId == key)
                          .Select(m => new CityDto
                          {
                            CityId = m.CityId,
                            CityName = m.CityName,
                            PostalCode = m.PostalCode,
                            PostalName = m.PostalName,
                            CountryCode = m.CountryCode,
                            CountryName = m.CountryCodeNavigation.CountryName
                          })
                          .FirstOrDefaultAsync();
    if (city == null)
    {
      return NotFound($"There is no city with id {key}");
    }
    else
    {
      return city;
    }
  }

  // POST /odata/Cities
  [HttpPost]
  public async Task<IActionResult> Post([FromBody] CityDto model)
  {     
    logger.LogTrace(JsonSerializer.Serialize(model));

    if (model != null && ModelState.IsValid)
    {
      var city = new EFModel.City
      {
        CityName = model.CityName,
        PostalCode = model.PostalCode,
        PostalName = model.PostalName,
        CountryCode = model.CountryCode
      };
     
      ctx.Add(city);
      await ctx.SaveChangesAsync();
      model.CityId = city.CityId;

      return Created(model);
    }
    else
    {
      return BadRequest(ModelState);
    }
  }

  // PUT /odata/Cities(key)
  [HttpPut]
  public async Task<IActionResult> Put(int key, [FromBody] CityDto model)
  {    
    logger.LogTrace(JsonSerializer.Serialize(model));

    if (model == null || model.CityId != key || !ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }
    else
    {
      var city = await ctx.Cities.FindAsync(key);
      if (city == null)
      {
        return NotFound("There is no city with id: " + key);
      }
      else
      {
        city.CityName = model.CityName;
        city.PostalCode = model.PostalCode;
        city.PostalName = model.PostalName;
        city.CountryCode = model.CountryCode;

        await ctx.SaveChangesAsync();
        return Updated(model);
      };
    }
  }

  // PATCH /odata/Cities(key)
  [HttpPatch]
  public async Task<IActionResult> Patch(int key, [FromBody] Delta<CityDto> model)
  {
    foreach (var changedProp in model.GetChangedPropertyNames())
    {
      if (model.TryGetPropertyValue(changedProp, out var value))
      {
        logger.LogTrace($"Changing {changedProp} to {value}");
      }
    }

    if (model == null || !ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }
    else
    {
      var city = await ctx.Cities.FindAsync(key);
      if (city == null)
      {
        return NotFound("No such city");
      }
      else
      {                    
        var viewmodel = new CityDto
        {
          CityId = city.CityId,
          CityName = city.CityName,
          PostalCode = city.PostalCode,
          PostalName = city.PostalName,
          CountryCode = city.CountryCode,
        };

        model.Patch(viewmodel);

        city.CityName = viewmodel.CityName;
        city.PostalCode = viewmodel.PostalCode;
        city.PostalName = viewmodel.PostalName;
        city.CountryCode = viewmodel.CountryCode;

        await ctx.SaveChangesAsync();
        return Updated(viewmodel);
      };
    }
  }

  // DELETE /odata/Cities(key)   
  [HttpDelete]
  public async Task<IActionResult> Delete(int key)
  {
    var city = await ctx.Cities.FindAsync(key);
    if (city == null)
    {
      return NotFound("No such city");
    }
    else
    {
      ctx.Remove(city);
      await ctx.SaveChangesAsync();
      return NoContent();
    }
  }
}
