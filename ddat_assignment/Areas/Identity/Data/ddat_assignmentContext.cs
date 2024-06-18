using ddat_assignment.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Models;

namespace ddat_assignment.Data;

public class ddat_assignmentContext : IdentityDbContext<ddat_assignmentUser>
{
    public ddat_assignmentContext(DbContextOptions<ddat_assignmentContext> options)
        : base(options)
    {
    }

    public DbSet<WarehouseModel> WarehouseModel { get; set; }

    public DbSet<ParcelModel> ParcelModel { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
