using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VCLWebAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity2 = await manager.CreateAsync(this, authenticationType);//CreateIdentityAsync(this, authenticationType);
            var userIdentity = await manager.GetClaimsAsync(this);
            // Add custom user claims here
            return (ClaimsIdentity)userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public static ApplicationDbContext Create()
        {
            // "LocalIdentConnection", throwIfV1Schema: false??
            DbContextOptions< ApplicationDbContext > options = new DbContextOptions< ApplicationDbContext >();
            return new ApplicationDbContext(options);
        }
    }
}