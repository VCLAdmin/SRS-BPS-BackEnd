using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCLWebAPI.Models;
using VCLWebAPI.Utils;

namespace VCLWebAPI.ServiceConfiguration
{
    public static class EntityFrameworkServiceConfiguration
    {
        public static void ConfigureEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            // registers ASP >NET Core Identity DB Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Globals.ApplicationDBConnection,
                ServerVersion.AutoDetect(Globals.ApplicationDBConnection),
                mySql => mySql.MigrationsAssembly(Globals.StartupAssembly)));


        }
    }
}
