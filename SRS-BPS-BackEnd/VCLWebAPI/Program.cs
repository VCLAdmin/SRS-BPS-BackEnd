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
using System.Text.Json;

// gettoken
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
//using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using VCLWebAPI.Models.Account;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VCLWebAPI.Utils;
using VCLWebAPI.Models.Edmx;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Globals.ConnectionString = builder.Configuration.GetConnectionString("VCLDesignDBEntities");
//Globals.DE_AWSSecretKey = builder.Configuration.GetSection(@"DE_AWSSecretKey").Value;
//Globals.DE_AWSSecretKey = builder.Configuration.GetSection(@"DE_AWSSecretKey").Value;
//Globals.DE_AWSSecretKey = builder.Configuration.GetSection(@"DE_AWSSecretKey").Value;

var connectionString = builder.Configuration.GetConnectionString("LocalIdentConnection");
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));

string signingkey = builder.Configuration["DE_AWSSecretKey"];
string issuer = builder.Configuration["Issuer"];
string audience = builder.Configuration["Audience"];

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["DE_AWSSecretKey"]))
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

builder.Services.AddAntiforgery(options =>
{
    // Set Cookie properties using CookieBuilder properties†.
    options.FormFieldName = "AntiforgeryFieldname";
    options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
    options.SuppressXFrameOptionsHeader = false;
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

app.MapPost("/Token", GetToken);

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

var antiforgery = app.Services.GetRequiredService<IAntiforgery>();

app.Use((context, next) =>
{
    var requestPath = context.Request.Path.Value;

    if (string.Equals(requestPath, "/", StringComparison.OrdinalIgnoreCase)
        || string.Equals(requestPath, "/index.html", StringComparison.OrdinalIgnoreCase))
    {
        var tokenSet = antiforgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
            new CookieOptions { HttpOnly = false });
    }

    return next(context);
});

//app.MapControllers();

app.UseMvc(
    routes =>
    {
        routes.MapRoute("areaRoute", "{area:exists}/{controller=Api/Controller}/{action=Index}/{id?}");
        routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
    });

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


//async Task<string> ReadStringDataManual()
//{
//    using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
//    {
//        return await reader.ReadToEndAsync();
//    }
//}
Boolean ValidateHash(string username, string password, User user)
{
    // User user = new User();
    byte[] salt = new byte[16];
    string hashedPassword = string.Empty;
    byte[] hashBytes = new byte[] { };
    byte[] hash = new byte[] { };
    Rfc2898DeriveBytes pbkdf2;
    Boolean valid = true;

    // user = _db.User.SingleOrDefault(x => x.UserName.Equals(username));

    if (user != null)
    {
        salt = user.Salt;
        hashedPassword = user.Hash;
        hashBytes = Convert.FromBase64String(hashedPassword);
        Array.Copy(hashBytes, 0, salt, 0, 16);
        pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                valid = false;
                break;
            }
        }
    }
    else
    {
        valid = false;
    }

    return valid;
}
async Task GetToken(HttpContext http)
{
  //  private readonly VCLDesignDBEntities _dbContext = new VCLDesignDBEntities();
    var dbContext = http.RequestServices.GetService<VCLDesignDBEntities>();
    dbContext = dbContext != null? dbContext : new VCLDesignDBEntities();
    //var request = http.Request;
    //request.Headers.ContentType = "application/json";
    //var jsonModel = new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.UTF8, "application/json");
    //var inputUser = await jsonModel.ReadFromJsonAsync<AccountApiModel>();
    var inputUser = await http.Request.ReadFromJsonAsync<AccountApiModel>();
    if (!string.IsNullOrEmpty(inputUser.UserName) &&
        !string.IsNullOrEmpty(inputUser.Password))
    {
        var user = dbContext.User
            .SingleOrDefault(x => x.Email.Equals(inputUser.UserName));
        //
        // var loggedInUser = null;
        Boolean valid = true;
        if (user != null)
        {
            byte[] salt = new byte[16];
            string hashedPassword = string.Empty;
            byte[] hashBytes = new byte[] { };
            byte[] hash = new byte[] { };
            Rfc2898DeriveBytes pbkdf2;
            //Boolean valid = true;

            // user = _db.User.SingleOrDefault(x => x.UserName.Equals(username));

            // if (user != null)
            //{
            salt = user.Salt;
            hashedPassword = user.Hash;
            hashBytes = Convert.FromBase64String(hashedPassword);
            Array.Copy(hashBytes, 0, salt, 0, 16);
            pbkdf2 = new Rfc2898DeriveBytes(inputUser.Password, salt, 10000);
            hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    valid = false;
                    break;
                }
            }
        }
        else
        {

            http.Response.StatusCode = 401;
            return;

            valid = false;
        }
        //
        if (valid)
        {


            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, inputUser.UserName),
            new Claim(JwtRegisteredClaimNames.Name, inputUser.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

            var token = new JwtSecurityToken
            (
                issuer: builder.Configuration["Issuer"],
                audience: builder.Configuration["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["DE_AWSSecretKey"])),
                    SecurityAlgorithms.HmacSha256)
            );

            await http.Response.WriteAsJsonAsync(new { access_token = new JwtSecurityTokenHandler().WriteToken(token), expires_in = TimeSpan.FromHours(8).TotalSeconds, refresh_token = "" });
            return;
        }

    }

    http.Response.StatusCode = 400;
    // await  V();
}

//async Task V()
//{

//}