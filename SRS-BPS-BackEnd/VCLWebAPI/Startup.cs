// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VCLWebAPI.Services;
using VCLWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using VCLWebAPI.Services.Handlers;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json.Serialization;

namespace VCLWebAPI
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetConfiguration();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1.Add Cors
            services.AddCors(o => o.AddPolicy("VCL_Policy", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            // 2. AddAuthentication
            services.AddAuthentication("BasicAuthentication")
            //    .AddOAuth2Introspection("token", options =>
            //    {
            //        options.Authority = Configuration.GetSection(@"Authority").Value;

            //        options.ClientId = "CMACS_API";
            //        options.ClientSecret = "secret";
            //    });
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddControllersWithViews(ConfigureMvcOptions)
                // Newtonsoft.Json is added for compatibility reasons
                // The recommended approach is to use System.Text.Json for serialization
                // Visit the following link for more guidance about moving away from Newtonsoft.Json to System.Text.Json
                // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                });

            // 3. addAuthorization
            services.AddAuthorization();

            // 4. Add EF services to the services container.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LocalIdentConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddMvc().AddMvcOptions(options =>
            //{
            //    options.EnableEndpointRouting = false;
            //});

            // 5. identity config

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
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

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // 6. add IISserver configure
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            //7. physicsCore
            services.AddMemoryCache();

            //services.AddScoped<ICachingService, CachingService>();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //7 physicsCore
            app.UseHttpsRedirection();

            //app.MapControllers();
            app.UseCors("VCL_Policy");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapControllerRoute(
                //    name: "VCLWebAPI",
                //    pattern: "api/{controller}/{action}/{id?}",
                //    defaults: new { controller = "Home", action = "Index" });
            });
            //app.UseMvc(
            //    routes =>
            //    {
            //        routes.MapRoute("areaRoute", "{area:exists}/{controller=Api/Controller}/{action=Index}/{id?}");
            //        routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            //    });
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        {
        }

        public void SetConfiguration()
        {
            //Can be removed later once the UI is ready with adding of new user.
            AccountService accountService = new AccountService();
            accountService.AddAspNetUsers();
            //accountService.AddNewUsers();
            //accountService.AddSRSData();
            //VCLDesignDB.Util.Globals.DBConnectionString = VCLDesignDB.Util.Constants.Local_DbConnectionString;
        }
    }
}
