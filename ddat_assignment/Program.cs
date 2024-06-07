using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Data;
using ddat_assignment.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ddat_assignmentContextConnection") ?? throw new InvalidOperationException("Connection string 'ddat_assignmentContextConnection' not found.");

builder.Services.AddDbContext<ddat_assignmentContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ddat_assignmentUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ddat_assignmentContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
