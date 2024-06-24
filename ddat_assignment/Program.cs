using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ddat_assignment.Data;
using ddat_assignment.Areas.Identity.Data;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("ddat_assignmentContextConnection") ?? throw new InvalidOperationException("Connection string 'ddat_assignmentContextConnection' not found.");

        builder.Services.AddDbContext<ddat_assignmentContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<ddat_assignmentUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<ddat_assignmentContext>();

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

        app.MapControllerRoute(
            name: "Admin",
            pattern: "Admin/{controller}/{action=Index}/{id?}");

        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Customer", "Warehouse", "Driver" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ddat_assignmentUser>>();

            string email = "admin@okbossexpress.com";
            string password = "Admin1234@";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ddat_assignmentUser();
                user.UserName = email;
                user.Email = email;
                user.Role = "Warehouse";

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Warehouse");
            }
        }

        app.Run();
    }
}