using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ghumfir.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using UserManagementWithIdentity.Services;
using Ghumfir.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserContextConnection") ?? throw new InvalidOperationException("Connection string 'LoginContextConnection' not found.");

builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(connectionString));

string conn = builder.Configuration.GetConnectionString("connection");
builder.Services.AddDbContext<ghumfirContext>(options =>
{
    options.UseSqlServer(conn);
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<UserContext>();
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing.
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    //options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();
app.UseAuthentication(); ;
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();