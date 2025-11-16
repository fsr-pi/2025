using MVC_EN.Models;
using Microsoft.EntityFrameworkCore;
using MVC_EN;

var builder = WebApplication.CreateBuilder(args);

  #region Configure services
  var appSection = builder.Configuration.GetSection("AppSettings");
  builder.Services.Configure<AppSettings>(appSection);

  builder.Services.AddDbContext<FirmContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firm")));

  builder.Services.AddControllersWithViews();
  #endregion

  var app = builder.Build();

  #region Configure middleware pipeline
  //middleware order https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order

  if (app.Environment.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
  }

  app.UseStaticFiles();
  app.MapDefaultControllerRoute();

#endregion


app.Run();

// in order to test the app
public partial class Program { }