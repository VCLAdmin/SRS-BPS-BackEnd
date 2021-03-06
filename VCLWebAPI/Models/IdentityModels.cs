using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VCLWebAPI.Models.Account;
using VCLWebAPI.Utils;

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

        //public Boolean ValidateHash(string username, string password)
        //{
        //    var user = this;
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
        //        hashedPassword = user.PasswordHash;
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
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            
        }
    }
}