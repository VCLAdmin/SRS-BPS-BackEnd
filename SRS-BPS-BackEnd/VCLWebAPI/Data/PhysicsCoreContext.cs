using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VCLWebAPI.Models;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace VCLWebAPIService.Data
{
    public partial class VCLWebAPIContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //services.AddDbContext<VCLWebAPIContext>(options =>
            //    options.UseSqlite(
            //        Configuration.GetConnectionString("DefaultConnection")));
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);
        }

        public VCLWebAPIContext()
        {
        }

        public VCLWebAPIContext(DbContextOptions<VCLWebAPIContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
