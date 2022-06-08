//// This application entry point is based on ASP.NET Core new project templates and is included
//// as a starting point for app host configuration.
//// This file may need updated according to the specific scenario of the application being upgraded.
//// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;

//namespace VCLWebAPI
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder.UseStartup<Startup>();
//                });
//    }
//}


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Text.Json.Serialization;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VCLWebAPI.Services;
using VCLWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using VCLWebAPI.Services.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// gettoken
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
//using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using VCLWebAPI.Models.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("LocalIdentConnection");
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlite(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Issuer"],
        ValidAudience = builder.Configuration["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]))
    };
});
builder.Services.AddAuthorization();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
    options.MultipartHeadersLengthLimit = int.MaxValue;
});


builder.Services.AddMemoryCache();

//builder.Services.AddScoped<ICachingService, CachingService>();

builder.Services.AddControllersWithViews()
                // Newtonsoft.Json is added for compatibility reasons
                // The recommended approach is to use System.Text.Json for serialization
                // Visit the following link for more guidance about moving away from Newtonsoft.Json to System.Text.Json
                // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                })
                .AddMvcOptions(options =>
                {
                    options.EnableEndpointRouting = false;
                });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddPolicy("VCL_Policy", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    //options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});


IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();


var app = builder.Build();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Configure the HTTP request pipeline.

app.UseCors("VCL_Policy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

//app.UseMvc(
//    routes =>
//    {
//        routes.MapRoute("areaRoute", "{area:exists}/{controller=Api/Controller}/{action=Index}/{id?}");
//        routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
//    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapPost("/Token", GetToken);

app.Run();




async Task GetToken(HttpContext http)
{
    var dbContext = http.RequestServices.GetService<ApplicationDbContext>();
    var inputUser = await http.Request.ReadFromJsonAsync<AccountApiModel>();
    if (!string.IsNullOrEmpty(inputUser.UserName) &&
        !string.IsNullOrEmpty(inputUser.Password))
    {
        var loggedInUser = await dbContext.Users
            .FirstOrDefaultAsync(user => user.UserName == inputUser.UserName
            && user.PasswordHash == inputUser.Password);
        if (loggedInUser == null)
        {
            http.Response.StatusCode = 401;
            return;
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, inputUser.UserName),
            new Claim(JwtRegisteredClaimNames.Name, inputUser.UserName),
            new Claim(JwtRegisteredClaimNames.Email, loggedInUser.Email)
        };

        var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Issuer"],
            audience: builder.Configuration["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"])),
                SecurityAlgorithms.HmacSha256)
        );

        await http.Response.WriteAsJsonAsync(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        return;
    }

    http.Response.StatusCode = 400;
}