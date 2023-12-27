using CodeScanning.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
if (Environment.GetEnvironmentVariable("env") == "prod")
{
    connectionString = builder.Configuration.GetConnectionString("ProdConnection");
    connectionString += ";User Id=" + Environment.GetEnvironmentVariable("DB_USER") + ";Password=" + Environment.GetEnvironmentVariable("DB_PASSWORD");
}
if (Environment.GetEnvironmentVariable("env") == "dev")
{
    connectionString = builder.Configuration.GetConnectionString("DevConnection");
    connectionString += ";User Id=" + Environment.GetEnvironmentVariable("DB_USER") + ";Password=" + Environment.GetEnvironmentVariable("DB_PASSWORD");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
