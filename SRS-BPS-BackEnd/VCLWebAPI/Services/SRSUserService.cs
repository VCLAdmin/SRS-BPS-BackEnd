using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
//using System.Web;
using VCLWebAPI.Mappers;
using VCLWebAPI.Models;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.SRS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Owin;
//using System.Web.Security;
using System.Threading.Tasks;

namespace VCLWebAPI.Services
{
    public class SRSUserService
    {
        private readonly VCLDesignDBEntities _db;
        private readonly BpsProjectService _bpSPS;
        private AccountService _accountService;

        public SRSUserService()
        {
            _db = new VCLDesignDBEntities();
            _bpSPS = new BpsProjectService();
            _accountService = new AccountService();
        }

        public async Task<List<SRSUserApiModel>> GetAll()
        {
           var dealers =  _db.Dealer.ToList();
            List<SRSUserApiModel> response = new List<SRSUserApiModel>();
            var projectMApper = new ProjectMapper();
            foreach (var dealer in dealers)
            {
                foreach (var usr in dealer.User)
                {
                    response.Add(await projectMApper.ProjectDbToApiModel(dealer, usr));
                }
            }
            var fabs = _db.Fabricator.ToList();
            foreach (var fab in fabs)
            {
                foreach (var usr in fab.User)
                {
                    response.Add(projectMApper.ProjectDbToApiModel(fab, usr));
                }
            }
            return response;
        }
        public void Create(SRSUserApiModel model)
        {

            try
            {
                string language = "en-US";
                string company = "Schuco";
                User user = new User
                {
                    UserName = model.NameFirst + " " + model.NameLast,
                    UserGuid = Guid.NewGuid(),
                    NameFirst = model.NameFirst,
                    NameLast = model.NameLast,
                    Email = model.Email,
                    Language = language,
                    Company = company
                };

                if (_db.User.Where(x => x.Email == user.Email).SingleOrDefault() == null)
                {
                    _db.User.Add(user);
                    _db.SaveChanges();
                    var password = model.Password;
                    byte[] salt;
                    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                    string savedPasswordHash = Convert.ToBase64String(hashBytes);
                    user.Hash = savedPasswordHash;
                    user.Salt = salt;
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();

                    string role = model.UserRole;
                    if (model.FabricatorId != 0)
                    {
                        var fab = _db.Fabricator.First(u => u.FabricatorId == model.FabricatorId);
                        fab.User.Add(user);
                        role = "Fabricator";
                    }
                    else if (model.DealerId != 0)
                    {
                        var dealer = _db.Dealer.First(u => u.DealerId == model.DealerId);
                        dealer.User.Add(user);
                        role = "Dealer-" + model.UserRole;
                    }
                    _db.SaveChanges();
                    AddUsersToAspNet(user.Email, role, password);
                }
               

            }
            catch(Exception ex)
            {

            }
        }

        public string GetEmail(Guid guid)
        {
            User updateUser = _db.User.Where(x => x.UserGuid == guid).SingleOrDefault();
            return updateUser.Email;

        }

        public bool IsEmailDuplicate(string email)
        {
            User updateUser = _db.User.Where(x => x.Email == email).FirstOrDefault();
            return updateUser == null ? true : false;

        }

        public async Task<string> ChangePassword(SRSUserApiModel model)
        {
            return await _accountService.UpdateContactPassword(model.Email, model.Password);
            
            //var db = new ApplicationDbContext();
            //var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            //var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("SRS");
            //manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
            //var identityUser = manager.FindByEmail(model.Email);
            //if (identityUser != null)
            //{
            //    var token = manager.GeneratePasswordResetToken(identityUser.Id);
            //    var result = manager.ResetPassword(identityUser.Id, token, model.Password);
            //    if (result.Succeeded)
            //    {
            //        return "";
            //    }
            //    else {
            //        return result.Errors.FirstOrDefault();
            //    }
            //}
            //else
            //    return "User not found";
        }

        public void Update(Guid guid, SRSUserApiModel model)
        {
            //string language = model.Language == null || model.Language == string.Empty ? "en-US" : model.Language;
            //string company = model.Company == null || model.Company == string.Empty ? "Schuco" : model.Company;

            User updateUser = _db.User.Where(x => x.UserGuid == guid).SingleOrDefault(); //x.Email == model.Email && 

            if (updateUser != null)
            {
                updateUser.NameFirst = model.NameFirst;
                updateUser.NameLast = model.NameLast;
                updateUser.Email = model.Email;
                _db.Entry(updateUser).State = EntityState.Modified;
                _db.SaveChanges();

                //if (model.Password != null && model.Password != string.Empty)
                //{
                //    byte[] salt;
                //    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                //    var pbkdf2 = new Rfc2898DeriveBytes(model.Password, salt, 10000);
                //    byte[] hash = pbkdf2.GetBytes(20);
                //    byte[] hashBytes = new byte[36];
                //    Array.Copy(salt, 0, hashBytes, 0, 16);
                //    Array.Copy(hash, 0, hashBytes, 16, 20);
                //    string savedPasswordHash = Convert.ToBase64String(hashBytes);
                //    updateUser.Hash = savedPasswordHash;
                //    updateUser.Salt = salt;
                //    _db.Entry(updateUser).State = EntityState.Modified;
                //    _db.SaveChanges();
                //    await PasswordReset(updateUser.Email, model.Password);
                //}

                if (model.FabricatorId != 0)
                {
                    updateUser.Fabricator.Clear();
                    var fab = _db.Fabricator.First(u => u.FabricatorId == model.FabricatorId);
                    updateUser.Fabricator.Add(fab);

                }
                else if (model.DealerId != 0)
                {
                    updateUser.Dealer.Clear();
                    var dealer = _db.Dealer.First(u => u.DealerId == model.DealerId);
                    updateUser.Dealer.Add(dealer);
                    string role = "Dealer-" + model.UserRole;
                    ChangeRolesAspTable(updateUser.Email, role);

                }
                _db.SaveChanges();
            }
        }

        public void ChangeRolesAspTable(string email, string role)
        {
            //// User existingUser = _db.User.First(x => x.Email == email);
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            
            //var aspUser =  userManager.FindByEmail(email);
            //var rolesForUser = userManager.GetRoles(aspUser.Id);
            
            //if (!rolesForUser.Contains(role))
            //{
            //    userManager.RemoveFromRoles(aspUser.Id, rolesForUser.ToArray());
            //    userManager.AddToRole(aspUser.Id, role);
            //}

            //try
            //{
            //    _db.SaveChanges();
            //}
            //catch (Exception e)
            //{
            //    throw;
            //}
        }
        public void AddUsersToAspNet(string email, string role, string password = "bps2019!")
        {
            //User newUser = _db.User.First(x => x.Email == email);
            //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //    userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            //    {
            //        AllowOnlyAlphanumericUserNames = false
            //    };
            //    var user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email };
            //    var result = userManager.Create(user, password);
            //    result = userManager.SetLockoutEnabled(user.Id, false);
            //    // Add user admin to Role Admin if not already added
            //    var rolesForUser = userManager.GetRoles(user.Id);
            //    if (!rolesForUser.Contains(newUser.Email))
            //    {
            //        result = userManager.AddToRole(user.Id, role);
            //    }
            
            //try
            //{
            //    _db.SaveChanges();
            //}
            //catch (Exception e)
            //{
            //    throw;
            //}
        }
        public void DeleteUsersFromAspNet(string email)
        {
            //try
            //{
            //    Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            //    var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //    userMgr.Delete(userMgr.FindByName(email));
            //}
            //catch (Exception e)
            //{
            //    throw;
            //}
        }

        //public DealerApiModel Get(Guid guid)
        //{
        //    var dealer = _db.Dealer.Where(u => u.DealerExternalId == guid).FirstOrDefault();
        //    DealerApiModel response = new DealerApiModel();
        //    var projectMApper = new ProjectMapper();
        //    Fabricator secondaryFab = null;
        //    if (dealer.SecondaryFabricatorId != null)
        //    {
        //        secondaryFab = _db.Fabricator.FirstOrDefault(u => u.FabricatorId == dealer.SecondaryFabricatorId);
        //    }
        //    response = projectMApper.ProjectDbToApiModel(dealer, secondaryFab);
        //    return response;
        //}

        public bool CanDelete(Guid guid)
        {
            var srsUser = _db.User.Where(u => u.UserGuid == guid).FirstOrDefault();
            if (srsUser != null && (srsUser.BpsProject.ToList().Count > 0 || srsUser.Order.Count > 0 || srsUser.Order1.Count > 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void Delete(Guid guid)
        {
            try
            {
                var usr = _db.User.Where(u => u.UserGuid == guid).FirstOrDefault();
                usr.Financial.Clear();
                usr.Financial1.Clear();
                usr.BpsProject.Clear();
                usr.Fabricator.Clear();
                usr.Dealer.Clear();
                usr.Address.Clear();
                usr.Address1.Clear();
                _db.User.Remove(usr);
                _db.SaveChanges();
                DeleteUsersFromAspNet(usr.Email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}