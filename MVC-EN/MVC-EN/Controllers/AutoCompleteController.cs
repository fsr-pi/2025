using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC_EN.Models;
using MVC_EN.ViewModels;

namespace MVC_EN.Controllers;

public class AutoCompleteController : Controller
{
  private readonly FirmContext ctx;
  private readonly AppSettings appData;

  public AutoCompleteController(FirmContext ctx, IOptionsSnapshot<AppSettings> options)
  {
    this.ctx = ctx;
    appData = options.Value;
  }

  public async Task<List<IdLabel>> Cities(string term)
  {
    var query = ctx.Cities
                    .Select(c => new IdLabel
                    {
                      Id = c.CityId,
                      Label = c.PostalCode + " " + c.CityName
                    })
                    .Where(l => l.Label.Contains(term));

    var list = await query.OrderBy(l => l.Label)
                          .ThenBy(l => l.Id)
                          .Take(appData.AutoCompleteCount)
                          .ToListAsync();
    return list;
  }
}
