using EFModel;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using ODataApi;

var builder = WebApplication.CreateBuilder(args);

#region Configure services

// add/configure services

//Note: Cannot run in parallel, see https://chillicream.com/docs/hotchocolate/integrations/entity-framework for workaround
builder.Services.AddDbContext<FirmContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firm")));
builder.Services.AddControllers()
              .AddOData(opt => opt.AddRouteComponents("odata", FirmaODataModelBuilder.GetEdmModel()));
#endregion

var app = builder.Build();

#region Configure middleware pipeline.
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order-1

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});

#endregion

app.Run();