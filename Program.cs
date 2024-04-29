// using Glimpse.Migrations;
using Glimpse.Models;
using Glimpse.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEmailService, EmailService>();

var connectionString = builder.Configuration.GetConnectionString("default");


builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();

builder.Services.AddDbContext<GlimpseContext>(
    options => options.UseSqlServer(connectionString)
);
builder.Services.AddIdentity<User, IdentityRole>(
    Options =>
    {
        Options.Password.RequiredUniqueChars = 0;
        Options.Password.RequireUppercase = true;
        Options.Password.RequiredLength = 8;
        Options.Password.RequireDigit = true;
        Options.Password.RequireNonAlphanumeric = true;
        Options.Password.RequireLowercase = true;

        Options.User.RequireUniqueEmail = true;
        // Options.SignIn.RequireConfirmedEmail = true;
    }
    )
    .AddEntityFrameworkStores<GlimpseContext>().AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
