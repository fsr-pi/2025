using Microsoft.EntityFrameworkCore;

namespace MVC_EN.Models;

public partial class FirmContext
{
  public virtual DbSet<ViewPartner> vw_Partners { get; set; }
  
  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ViewPartner>(entity => {
      entity.HasNoKey();
      //entity.ToView("vw_Partners");
    });
  }
}