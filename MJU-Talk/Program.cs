using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MJU_Talk.DAL.Data;
using MJU_Talk.DAL.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<StudentDbContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration["ConnectionStrings:StudentDbContextConnection"],
        b => { b.MigrationsAssembly("MJU-Talk.DAL"); }
    )
);

builder.Services.AddDefaultIdentity<StudentUser>(options => {
    options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<StudentDbContext>();

builder.Services.Configure<IdentityOptions>(opts =>
    {
        opts.Password.RequiredLength = 6;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequireDigit = false;
        opts.User.RequireUniqueEmail = true;
    }
);

var app = builder.Build();

app.UseStaticFiles();

app.UseAuthentication();

app.MapControllers();
app.MapControllerRoute("controllers", "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();