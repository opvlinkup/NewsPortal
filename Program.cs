using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsPortal.Data;
using NewsPortal.Seeds;
using NewsPortal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NPDbContext>(
    (sp, options) =>
    {
        options
            .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
);

builder
    .Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<NPDbContext>()
    .AddDefaultTokenProviders();

var supportedCultures = new[] { "ru", "en" };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider()
    };
});

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Admin/Account/Login";
    opt.LogoutPath = "/Admin/Account/Logout";
    opt.AccessDeniedPath = "/Admin/Account/Login";
});

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IIdentitySetupService, IdentitySetupService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var setup = scope.ServiceProvider.GetRequiredService<IIdentitySetupService>();
    await setup.EnsureRolesAsync();
    var dbContext = scope.ServiceProvider.GetRequiredService<NPDbContext>();
    await ArticleSeeder.SeedAsync(dbContext);
}

var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
app.UseRequestLocalization(locOptions!);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/News/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=News}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
