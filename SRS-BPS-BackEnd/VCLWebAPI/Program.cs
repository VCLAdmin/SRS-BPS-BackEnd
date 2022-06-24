// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;

namespace VCLWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //log4net configuration initial setup
            var log4netRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(log4netRepository, new FileInfo("log4net.config"));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System.Configuration;
//using System.Text.Json.Serialization;


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using VCLWebAPI.Services;
//using VCLWebAPI.Models;
//using Microsoft.AspNetCore.Identity;
//using VCLWebAPI.Services.Handlers;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using System.Text.Json;

//// gettoken
//using Microsoft.AspNetCore.Http;
//using System.Security.Claims;
////using Microsoft.IdentityModel.JsonWebTokens;
//using System.IdentityModel.Tokens.Jwt;
//using VCLWebAPI.Models.Account;
//using System.Net.Http;
//using Newtonsoft.Json;
//using System.Net.Http.Json;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using VCLWebAPI.Utils;
//using VCLWebAPI.Models.Edmx;
//using System.Security.Cryptography;
//using Microsoft.AspNetCore.Antiforgery;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//Globals.ConnectionString = builder.Configuration.GetConnectionString("VCLDesignDBEntities");
//Globals.ConnectionStringApp = builder.Configuration.GetConnectionString("LocalIdentConnection");
//Globals.Issuer = builder.Configuration["Issuer"];
//Globals.Audience = builder.Configuration["Audience"];
//Globals.Secret = builder.Configuration["DE_AWSSecretKey"];
////Globals.DE_AWSSecretKey = builder.Configuration.GetSection(@"DE_AWSSecretKey").Value;
////Globals.DE_AWSSecretKey = builder.Configuration.GetSection(@"DE_AWSSecretKey").Value;
////Globals.DE_AWSSecretKey = builder.Configuration.GetSection(@"DE_AWSSecretKey").Value;


//builder.Services.AddCors(p => p.AddPolicy("VCL_Policy", builder =>
//{
//    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
//}));

//builder.Services.AddMvc().AddMvcOptions(options =>
//{
//    options.EnableEndpointRouting = false;
//});

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.SaveToken = true;
//    options.RequireHttpsMetadata = false;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidAudience = builder.Configuration["Issuer"],
//        ValidIssuer = builder.Configuration["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["DE_AWSSecretKey"]))
//    };
//});


////builder.Services.AddAuthorization();

//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();


//var app = builder.Build();

//System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

//// Configure the HTTP request pipeline.

//app.UseCors("VCL_Policy");

////app.UseHttpsRedirection();

////app.MapPost("/Token", GetToken);

//app.UseMvc(
//    routes =>
//    {
//        routes.MapRoute("areaRoute", "{area:exists}/{controller=Api/Controller}/{action=Index}/{id?}");
//        routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
//    });

////app.UseRouting();

//app.UseAuthentication();

//app.UseAuthorization();

//app.Run();

//Boolean ValidateHash(string username, string password, User user)
//{
//    // User user = new User();
//    byte[] salt = new byte[16];
//    string hashedPassword = string.Empty;
//    byte[] hashBytes = new byte[] { };
//    byte[] hash = new byte[] { };
//    Rfc2898DeriveBytes pbkdf2;
//    Boolean valid = true;

//    // user = _db.User.SingleOrDefault(x => x.UserName.Equals(username));

//    if (user != null)
//    {
//        salt = user.Salt;
//        hashedPassword = user.Hash;
//        hashBytes = Convert.FromBase64String(hashedPassword);
//        Array.Copy(hashBytes, 0, salt, 0, 16);
//        pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
//        hash = pbkdf2.GetBytes(20);

//        for (int i = 0; i < 20; i++)
//        {
//            if (hashBytes[i + 16] != hash[i])
//            {
//                valid = false;
//                break;
//            }
//        }
//    }
//    else
//    {
//        valid = false;
//    }

//    return valid;
//}

//async Task GetToken(HttpContext http)
//{
//    //  private readonly VCLDesignDBEntities _dbContext = new VCLDesignDBEntities();
//    var dbContext = http.RequestServices.GetService<VCLDesignDBEntities>();
//    dbContext = dbContext != null ? dbContext : new VCLDesignDBEntities();
//    //var request = http.Request;
//    //request.Headers.ContentType = "application/json";
//    //var jsonModel = new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.UTF8, "application/json");
//    //var inputUser = await jsonModel.ReadFromJsonAsync<AccountApiModel>();
//    var inputUser = await http.Request.ReadFromJsonAsync<AccountApiModel>();
//    if (!string.IsNullOrEmpty(inputUser.UserName) &&
//        !string.IsNullOrEmpty(inputUser.Password))
//    {
//        var user = dbContext.User
//            .SingleOrDefault(x => x.Email.Equals(inputUser.UserName));
//        //
//        // var loggedInUser = null;
//        Boolean valid = true;
//        if (user != null)
//        {
//            byte[] salt = new byte[16];
//            string hashedPassword = string.Empty;
//            byte[] hashBytes = new byte[] { };
//            byte[] hash = new byte[] { };
//            Rfc2898DeriveBytes pbkdf2;
//            //Boolean valid = true;

//            // user = _db.User.SingleOrDefault(x => x.UserName.Equals(username));

//            // if (user != null)
//            //{
//            salt = user.Salt;
//            hashedPassword = user.Hash;
//            hashBytes = Convert.FromBase64String(hashedPassword);
//            Array.Copy(hashBytes, 0, salt, 0, 16);
//            pbkdf2 = new Rfc2898DeriveBytes(inputUser.Password, salt, 10000);
//            hash = pbkdf2.GetBytes(20);

//            for (int i = 0; i < 20; i++)
//            {
//                if (hashBytes[i + 16] != hash[i])
//                {
//                    valid = false;
//                    break;
//                }
//            }
//        }
//        else
//        {

//            http.Response.StatusCode = 401;
//            return;

//            valid = false;
//        }
//        //
//        if (valid)
//        {


//            var claims = new[]
//            {
//            new Claim(JwtRegisteredClaimNames.Sub, inputUser.UserName),
//            new Claim(JwtRegisteredClaimNames.Name, inputUser.UserName),
//            new Claim(JwtRegisteredClaimNames.Email, user.Email)
//        };

//            var token = new JwtSecurityToken
//            (
//                issuer: builder.Configuration["Issuer"],
//                audience: builder.Configuration["Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddDays(60),
//                notBefore: DateTime.UtcNow,
//                signingCredentials: new SigningCredentials(
//                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["DE_AWSSecretKey"])),
//                    SecurityAlgorithms.HmacSha256)
//            );

//            await http.Response.WriteAsJsonAsync(new { access_token = new JwtSecurityTokenHandler().WriteToken(token), expires_in = TimeSpan.FromHours(8).TotalSeconds, refresh_token = "" });
//            return;
//        }

//    }

//    http.Response.StatusCode = 400;
//    // await  V();
//}
