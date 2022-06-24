// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VCLWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using VCLWebAPI.Utils;
using System.Reflection;
using VCLWebAPI.ServiceConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IO;
using System;
using Newtonsoft.Json;
using VCLWebAPI.Models.Edmx;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace VCLWebAPI
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetGlobal();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            Globals.StartupAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // 1.Add Cors
            services.AddCors(o => o.AddPolicy("VCL_Policy", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddMvc(options =>
            {
                // using Microsoft.AspNetCore.Mvc.Formatters;
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            }).AddMvcOptions(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.UseMemberCasing();
            }).AddWebApiConventions();

            EntityFrameworkServiceConfiguration.ConfigureEntityFramework(services, Configuration);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders()
                    .AddRoles<IdentityRole>();

            // Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VCL Design", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Join(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
                c.CustomSchemaIds(x => x.FullName);
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });


            #region commented code


            // 4. Add EF services to the services container.

            //// 2. AddAuthentication
            //services.AddAuthentication("BasicAuthentication")
            ////    .AddOAuth2Introspection("token", options =>
            ////    {
            ////        options.Authority = Configuration.GetSection(@"Authority").Value;

            ////        options.ClientId = "CMACS_API";
            ////        options.ClientSecret = "secret";
            ////    });
            //.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


            //// 3. addAuthorization
            //services.AddAuthorization();

            //services.AddControllersWithViews(ConfigureMvcOptions)
            //    // Newtonsoft.Json is added for compatibility reasons
            //    // The recommended approach is to use System.Text.Json for serialization
            //    // Visit the following link for more guidance about moving away from Newtonsoft.Json to System.Text.Json
            //    // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
            //    .AddNewtonsoftJson(options =>
            //    {
            //        options.UseMemberCasing();
            //    });

            ////7. physicsCore
            //services.AddMemoryCache();

            ////services.AddScoped<ICachingService, CachingService>();

            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            //});


            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();



            // 5. identity config

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    // Password settings.
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequireNonAlphanumeric = true;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequiredLength = 6;
            //    //options.Password.RequiredUniqueChars = 1;

            //    // Lockout settings.
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.AllowedForNewUsers = true;

            //    // User settings.
            //    options.User.AllowedUserNameCharacters =
            //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            //    options.User.RequireUniqueEmail = true;
            //});

            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.HttpOnly = true;
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            //    options.LoginPath = "/Identity/Account/Login";
            //    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            //    options.SlidingExpiration = true;
            //});

            //// 6. add IISserver configure
            //services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestBodySize = int.MaxValue;
            //});

            //services.Configure<FormOptions>(options =>
            //{
            //    options.ValueLengthLimit = int.MaxValue;
            //    options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
            //    options.MultipartHeadersLengthLimit = int.MaxValue;
            //});

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VCL Design Services v1"));

            //7 physicsCore
            app.UseHttpsRedirection();

            //app.MapControllers();
            app.UseCors("VCL_Policy");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");

            //    //endpoints.MapControllerRoute(
            //    //    name: "VCLWebAPI",
            //    //    pattern: "api/{controller}/{action}/{id?}",
            //    //    defaults: new { controller = "Home", action = "Index" });
            //});
            app.UseMvc(ConfigureRoute);
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        {
        }

        private void ConfigureRoute(IRouteBuilder routeBuilder)
        {
            //Home/Index 
            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
        }

        private void SetGlobal()
        {
            Globals.VCLDesignDBConnection = Configuration.GetConnectionString("VCLDesignDBEntities");
            Globals.ApplicationDBConnection = Configuration.GetConnectionString("LocalIdentConnection");
            Globals.accessKey = Configuration.GetSection(@"DE_AWSAccessKey").Value;
            Globals.secretKey = Configuration.GetSection(@"DE_AWSSecretKey").Value;
            Globals.service_url = Configuration.GetSection(@"DES3ServiceUrl").Value;
            Globals.bucket_name = Configuration.GetSection(@"DEAWSBucket").Value;
        }
    }
}
