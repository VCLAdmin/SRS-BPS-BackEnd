using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using VCLWebAPI.Mappers;
using VCLWebAPI.Models;
using VCLWebAPI.Models.Account;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI;

namespace VCLWebAPI.Services
{
    public class AccountService
    {
        private readonly VCLDesignDBEntities _db;
        private ApplicationUserManager _userManager;
        public AccountService()
        {
            _db = new VCLDesignDBEntities();
        }

        public AccountService(VCLDesignDBEntities dbContext)
        {
            _db = dbContext;
        }

        public User GetUser(int userId)
        {
            User user = _db.User.Where(x => x.UserId == userId).SingleOrDefault();
            return user;
        }
        public User GetCurrentUser() {
            return _db.User.Where(e => e.Email == HttpContextHelper.Current.User.Identity.Name).FirstOrDefault();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContextHelper.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public async Task<UserApiModel> SignIn(AccountApiModel accountApiModel)
        {
            try
            {
                List<User> user1 = _db.User.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            User user = _db.User.Where(a => a.Email.Equals(accountApiModel.UserName)).FirstOrDefault();
            UserApiModel userApiModel = new UserApiModel();

            if (user != null)
            {
                Thread.CurrentThread.CurrentCulture = !String.IsNullOrEmpty(user.Language) ? new CultureInfo(user.Language) : new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = !String.IsNullOrEmpty(user.Language) ? new CultureInfo(user.Language) : new CultureInfo("en-US");
                userApiModel = new UserMapper().MapUserDbModelToApiModel(user);
                // var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var aspUser = await UserManager.FindByNameAsync(user.Email);
                var rolesForUser = UserManager.GetRoles(aspUser.Id);
                if (rolesForUser.Contains("Dealer-Full") || rolesForUser.Contains("Dealer-Restricted"))
                {
                    user.Language = SetLanguage(accountApiModel, user);
                    _db.SaveChanges();
                }
                foreach (var role in rolesForUser)
                {
                    userApiModel.AccessRoles.Add(role);
                    userApiModel.Features = this.GetFeauturePermissionsByUser(role);
                }
            }

            return userApiModel;
        }

        public async Task<string> GetRoleForSRS(string email)
        {
            var aspUser = await UserManager.FindByNameAsync(email);
            var rolesForUser = UserManager.GetRoles(aspUser.Id);
            return rolesForUser[0] != null ? rolesForUser[0] : string.Empty;
        }
        public string GetUserRole(string email)
        {
            var aspUser = UserManager.FindByName(email);
            var rolesForUser = UserManager.GetRoles(aspUser.Id);
            return rolesForUser[0] != null ? rolesForUser[0] : string.Empty;
        }

        public List<FeatureApiModel> GetFeauturePermissionsByUser(string role)
        {
            List<FeatureApiModel> featureApiModelList = new List<FeatureApiModel>();
            var userRole = this._db.AspNetRoles.Where(a => a.Name == role).FirstOrDefault();
            if (userRole != null && userRole.Permission_Role != null)
            {
                var permissionRoles = userRole.Permission_Role.ToList();
                foreach (var perRole in permissionRoles)
                {
                    foreach (var feature in perRole.Feature)
                    {
                        featureApiModelList.Add(this.ConvertFeatureToApiModel(feature, perRole.PermissionId));
                    }
                }
            }
            return featureApiModelList;
        }

        private FeatureApiModel ConvertFeatureToApiModel(Feature feature, int permissionId)
        {
            var featureApiModel = new FeatureApiModel();
            featureApiModel.FeatureId = feature.Id;
            featureApiModel.FeatureGuid = feature.FeatureGuid.ToString();
            featureApiModel.Feature = feature.Feature1;
            featureApiModel.ParentId = Convert.ToInt32(feature.ParentId);
            featureApiModel.Permission = (Models.Permission)permissionId;
            return featureApiModel;
        }

        public string GetLanguage(string userName)
        {
            User user = _db.User.Where(a => a.UserName.Equals(userName)).FirstOrDefault();
            return user != null ? user.Language : String.Empty;
        }

        public string GetCurrentUserLanguage()
        {
            User currentUser = GetCurrentUser();
            var user = _db.User.Where(a => a.UserGuid == currentUser.UserGuid).FirstOrDefault();
            return user != null ? user.Language : String.Empty;
        }

        public string SetLanguage(AccountApiModel accountApiModel, User user = null)
        {
            if (user == null)
                user = _db.User.Where(a => a.UserName.Equals(accountApiModel.UserName)).FirstOrDefault();
            var language = "en-US";
            if (user != null)
            {
                language = !String.IsNullOrEmpty(accountApiModel.Language) ? accountApiModel.Language : "en-US";
                //_db.SaveChanges();
            }

            return language;
        }

        public string SetCurrentUserLanguage(string language = null)
        {
            User currentUser = GetCurrentUser();
            var user = _db.User.Where(a => a.Email == currentUser.Email).FirstOrDefault();
            if (user != null)
            {
                user.Language = language == null ? "en-US" : language;
                _db.SaveChanges();
            }
            return language;
        }

        internal Boolean ValidateHash(UserApiModel user, string password)
        {
            string hashedPassword = string.Empty;
            byte[] hashBytes = new byte[] { };
            byte[] hash = new byte[] { };
            byte[] salt = new byte[16];
            Rfc2898DeriveBytes pbkdf2;
            Boolean valid = true;

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

            valid = true; // for debugging user login only; remember to comment!

            return valid;
        }

        internal Boolean ValidateHash(string username, string password)
        {
            User user = new User();
            byte[] salt = new byte[16];
            string hashedPassword = string.Empty;
            byte[] hashBytes = new byte[] { };
            byte[] hash = new byte[] { };
            Rfc2898DeriveBytes pbkdf2;
            Boolean valid = true;

            user = _db.User.SingleOrDefault(x => x.UserName.Equals(username));

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

        internal void SaltAndHashAllUsers()
        {
            List<User> users = _db.User.ToList();

            foreach (User user in users)
            {
                if (user.Hash != null)
                {
                    continue;
                }

                var password = "SchucoUSA";

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
            }
        }

        internal void SaltAndHashUser(string username, string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            var user = _db.User.SingleOrDefault(x => x.UserName.Equals(username));
            user.Hash = savedPasswordHash;
            user.Salt = salt;
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public int SaltAndHashNewUsers()
        {
            List<User> users = _db.User.Where(x => x.Hash == null && x.Salt == null).ToList();

            foreach (User user in users)
            {
                var password = "bps2019!";

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
            }

            return 1;
        }

        public List<CreateContactBindingModel> ContactList()
        {
            List<CreateContactBindingModel> contactList = new List<CreateContactBindingModel>();
            foreach (User user in _db.User.Where(e => e.Email != null).ToList()) {
                var userRoles = GetUserRole(user.Email);
                if (userRoles != null && (userRoles != "SRSAdministrator" && userRoles != "Dealer-Full" && userRoles != "Dealer-Restricted"))
                {
                    CreateContactBindingModel contact = new CreateContactBindingModel();
                    contact.Id = user.UserId;
                    contact.UserGuid = user.UserGuid;
                    contact.Email = user.Email;
                    contact.UserName = user.UserName;
                    contact.FirstName = user.NameFirst;
                    contact.LastName = user.NameLast;
                    contact.Language = user.Language;
                    contact.Company = user.Company;
                    contactList.Add(contact);
                }
            }
            return contactList;
        }
        public CreateContactBindingModel GetContact(int id)
        {
            CreateContactBindingModel contact = new CreateContactBindingModel();
            if(id != 0)
            {
                User user = _db.User.Where(s => s.UserId == id).FirstOrDefault();
                if (user == null) { }
                else
                {
                    contact.Id = user.UserId;
                    contact.UserGuid = user.UserGuid;
                    contact.Email = user.Email;
                    contact.UserName = user.UserName;
                    contact.FirstName = user.NameFirst;
                    contact.LastName = user.NameLast;
                    contact.Language = user.Language;
                    contact.Company = user.Company;
                }
            }
            return contact;
        }
        
        public async Task<string> UpdateContactPassword(string email, string password) {

            User updateUser = _db.User.Where(x => x.Email == email).SingleOrDefault(); //x.Email == email && 

            if (updateUser != null)
            {
                if (password != null && password != string.Empty) { 
                    byte[] salt;
                    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                    string savedPasswordHash = Convert.ToBase64String(hashBytes);
                    updateUser.Hash = savedPasswordHash;
                    updateUser.Salt = salt;
                    _db.Entry(updateUser).State = EntityState.Modified;
                    _db.SaveChanges();
                    return await PasswordReset(updateUser.Email, password);
                }
            }
            return "User not found";
        }
        public async Task<CreateContactBindingModel> UpdateContact(CreateContactBindingModel model)
        {
            string language = model.Language == null || model.Language == string.Empty ? "en-US" : model.Language;
            string company = model.Company == null || model.Company == string.Empty ? "Schuco" : model.Company;

            User updateUser = _db.User.Where(x => x.UserId == model.Id).SingleOrDefault(); //x.Email == model.Email && 

            if (updateUser != null)
            {
                updateUser.UserName = model.UserName;
                updateUser.NameFirst = model.FirstName;
                updateUser.NameLast = model.LastName;
                updateUser.Email = model.Email;
                updateUser.Language = language;
                updateUser.Company = company;
                _db.Entry(updateUser).State = EntityState.Modified;
                _db.SaveChanges();

                if (model.Password != null && model.Password != string.Empty)
                {
                    byte[] salt;
                    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                    var pbkdf2 = new Rfc2898DeriveBytes(model.Password, salt, 10000);
                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                    string savedPasswordHash = Convert.ToBase64String(hashBytes);
                    updateUser.Hash = savedPasswordHash;
                    updateUser.Salt = salt;
                    _db.Entry(updateUser).State = EntityState.Modified;
                    _db.SaveChanges();
                    await PasswordReset(updateUser.Email, model.Password);
                }
            }
            return model;
        }
        public CreateContactBindingModel CreateContact(CreateContactBindingModel model) {
            string language = "en-US";
            string company = "Schuco";
            User user = new User { 
                UserName = model.FirstName + " " + model.LastName, UserGuid = Guid.NewGuid(), 
                NameFirst = model.FirstName, NameLast = model.LastName, 
                Email = model.Email, Language = language, Company = company };

            if (_db.User.Where(x => x.Email == user.Email).SingleOrDefault() == null)
            {
                _db.User.Add(user);
                _db.SaveChanges();
                var password = "bps2019!";
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
            }
            AddUsersToAspNet();
            return model;
        }

        public void AddNewUsers()
        {
            string language = "en-US";
            string company = "Schuco";
            List<User> currentUsers = _db.User.ToList();
            List<User> users = new List<User>();
            
            users.Add(new User { UserName = "Jdelacalle", UserGuid = Guid.NewGuid(), NameFirst = "Javier", NameLast = "de la Calle", Email = "jdelacalle@schuco-usa.com", Language = language, Company = company });
            users.Add(new User { UserName = "Maximilian Scherer", UserGuid = Guid.NewGuid(), NameFirst = "Maximilian", NameLast = "Scherer", Email = "mscherer@schueco.com", Language = language, Company = company });
            users.Add(new User { UserName = "Chris Newman", UserGuid = Guid.NewGuid(), NameFirst = "Chris", NameLast = "Newman", Email = "technicalservices@schueco.com", Language = language, Company = company });
            users.Add(new User { UserName = "Nikita Mattar", UserGuid = Guid.NewGuid(), NameFirst = "Nikita", NameLast = "Mattar", Email = "nikita.mattar@plan.one", Language = language, Company = company });
            users.Add(new User { UserName = "Johannes Wienke", UserGuid = Guid.NewGuid(), NameFirst = "Johannes", NameLast = "Wienke", Email = "johannes.wienke@plan.one", Language = language, Company = company });
            users.Add(new User { UserName = "Bjoern Siekmann", UserGuid = Guid.NewGuid(), NameFirst = "Bjoern", NameLast = "Siekmann", Email = "BSiekmann@schueco.com", Language = language, Company = company });
            users.Add(new User { UserName = "Marina Schaeffer", UserGuid = Guid.NewGuid(), NameFirst = "Marina", NameLast = "Schaeffer", Email = "mschaeffer@schueco.com", Language = language, Company = company });
            users.Add(new User { UserName = "Julian Einhaus", UserGuid = Guid.NewGuid(), NameFirst = "Julian", NameLast = "Einhaus", Email = "julian.einhaus@plan.one", Language = language, Company = company });
            users.Add(new User { UserName = "Thomas Schwarze", UserGuid = Guid.NewGuid(), NameFirst = "Thomas", NameLast = "Schwarze", Email = "tschwarze@schueco.com", Language = language, Company = company });
            users.Add(new User { UserName = "Tianzhen Ren", UserGuid = Guid.NewGuid(), NameFirst = "Tianzhen", NameLast = "Ren", Email = "Tren@schuco-usa.com", Language = language, Company = company });
            users.Add(new User { UserName = "Lwin Khaing", UserGuid = Guid.NewGuid(), NameFirst = "Lwin", NameLast = "Khaing", Email = "Lkhaing@schuco-usa.com", Language = language, Company = company });
            users.Add(new User { UserName = "Federico Gerasmo", UserGuid = Guid.NewGuid(), NameFirst = "Federico", NameLast = "Gerasmo", Email = "fgerasmo@schuco-usa.com", Language = language, Company = company });
            users.Add(new User { UserName = "Ekaterina Svitasheva", UserGuid = Guid.NewGuid(), NameFirst = "Ekaterina", NameLast = "Svitasheva", Email = "esvitasheva@schueco.ru", Language = language, Company = company });


            foreach (User user in users)
            {
                if (currentUsers.Where(x => x.Email == user.Email).FirstOrDefault() == null)
                {
                    _db.User.Add(user);
                    _db.SaveChanges();
                    var password = "bps2019!";
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
                }
            }
            AddUsersToAspNet();
        }
        public async Task<string> PasswordReset(string email, string password)
        {
            try
            {
                var aspUser = await UserManager.FindByNameAsync(email);
                if (aspUser != null)
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(aspUser.Id);
                    var result = await UserManager.ResetPasswordAsync(aspUser.Id, token, password);
                    if (result.Succeeded)
                    {
                        return "";
                    }
                    else
                    {
                        return result.Errors.FirstOrDefault();
                    }
                }
                else
                    return "User not found";

            }
            catch (Exception e)
            {
                throw;
            }
            return email;
        }

        public void AddUsersToAspNet()
        {
            List<string> aspNetUsers = _db.AspNetUsers.Select(s => s.Email).ToList();
            List<User> newUserList = _db.User.Where(x => x.Email.Trim() != "" && x.Email != null && !aspNetUsers.Contains(x.Email)).ToList();
            foreach (User newUser in newUserList.Distinct())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                var user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email };
                var result = userManager.Create(user, "bps2019!");
                result = userManager.SetLockoutEnabled(user.Id, false);
                // Add user admin to Role Admin if not already added
                var rolesForUser = userManager.GetRoles(user.Id);
                if (!rolesForUser.Contains(newUser.Email))
                {
                    result = userManager.AddToRole(user.Id, "Internal");
                }
            }
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void AddAspNetUsers()
        {
            //Seed Database here for now
            // var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            if (!RoleManager.RoleExists("SRSAdministrator"))
            {
                string[] roleNames = { "SRSAdministrator", "Fabricator", "Dealer-Full","Dealer-Restricted","Administrator", "Internal", "Designer", "DigitalProposal", "Acoustics", "ProductConfigurator", "External" };
                IdentityResult roleResult;
                foreach (var roleName in roleNames)
                {
                    if (!RoleManager.RoleExists(roleName))
                    {
                        roleResult = RoleManager.Create(new IdentityRole(roleName));
                    }
                }

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                //AWSUtils awsUtil = new AWSUtils();
                string[] userNames = { "SRSAdministrator","Administrator", "Internal", "Designer", "DigitalProposal", "Acoustics", "ProductConfigurator" };
                //var userManager = GetUserManager<ApplicationUserManager>();
                foreach (var userName in userNames)
                {
                    string name = userName + "@vcldesign.com";
                    var user = userManager.FindByName(name);

                    if (user == null)
                    {
                        user = new ApplicationUser { UserName = name, Email = name };
                        var result = userManager.Create(user, "bps2019!");
                        result = userManager.SetLockoutEnabled(user.Id, false);
                        try
                        {
                            User appUser = new User
                            {
                                UserName = userName,
                                UserGuid = Guid.NewGuid(),
                                NameFirst = userName,
                                NameLast = userName,
                                Email = name,
                                Language = "en-US",
                                Company = "Schuco"
                            };
                            if (_db.User.Where(x => x.UserName == userName).SingleOrDefault() == null)
                            {
                                _db.User.Add(appUser);
                                _db.SaveChanges();
                            }
                            SaltAndHashNewUsers();
                        }
                        catch (Exception ex)
                        {
                            var ex1 = ex;
                        }
                    }

                    // Add user admin to Role Admin if not already added
                    var rolesForUser = userManager.GetRoles(user.Id);
                    if (!rolesForUser.Contains(userName))
                    {
                        var result = userManager.AddToRole(user.Id, userName);
                    }
                }

                try
                {
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            AddUsersToAspNet();
        }
        public void AddSRSData() {
            AddFabricator();
            AddSRSAdmin();
        }
        public void AddFabricator()
        {
            User srsAdmin = _db.User.Where(e => e.Email.Trim().ToLower() == "SRSAdministrator@vcldesign.com".Trim().ToLower()).FirstOrDefault();
            List<Fabricator> fabricatorList = _db.Fabricator.ToList();
            if (fabricatorList.Count > 0) { }
            else
            {
                Address add = BuildAddress("8051 NW 79th Pl", "Medley", "FL", "33166", srsAdmin.UserId);
                Fabricator fab1 = BuildFabricator("Mr Glass Doors & Windows Inc", "Jose Garcia", "(305) 764-3963", "jGacia@mrglass.com", add.AddressId);
                AddSRSUsers("Dacio", "Lorenzo", "dLorenzo@mail.com", "Fabricator", 0, fab1.FabricatorId);
                //AddSRSUsers("Guzman", "Delgado", "gDelgado@mail.com", "Fabricator", 0, fab1.FabricatorId);
                //AddSRSUsers("Encarnacion", "Marquez", "eMarquez@mail.com", "Fabricator", 0, fab1.FabricatorId);
                //AddSRSUsers("Gregorio", "Reyes", "gReyes@mail.com", "Fabricator", 0, fab1.FabricatorId);
                //AddSRSUsers("Adam", "Bush", "aBush@mail.com", "Fabricator", 0, fab1.FabricatorId);
                //AddSRSUsers("Nieves", "Pascual", "nPascual@mail.com", "Fabricator", 0, fab1.FabricatorId);

                Address add2 = BuildAddress("Accesso 2 #5 Bodegas 15 y 16 Parque Industrial Benito Juarez Queretaro", "Queretaro Arteaga", "Mexico", "76120", srsAdmin.UserId);
                Fabricator fab2 = BuildFabricator("IAVA", "Juan Valdez", "+52 442 209 5000", "juanvaldez@IAVA.com", add.AddressId);
                AddSRSUsers("Alonso", "Lopez", "aLopez@mail.com", "Fabricator", 0, fab2.FabricatorId);
                //AddSRSUsers("Lourdes", "Vidal", "lVidal@mail.com", "Fabricator", 0, fab2.FabricatorId);
                //AddSRSUsers("Gema", "Martinez", "gMartinez@mail.com", "Fabricator", 0, fab2.FabricatorId);
                //AddSRSUsers("Teodosia", "Gonzalez", "tGonzalez@mail.com", "Fabricator", 0, fab2.FabricatorId);
                //AddSRSUsers("Gertrudis", "Mendez", "gMendez@mail.com", "Fabricator", 0, fab2.FabricatorId);
                //AddSRSUsers("Inmaculada", "Ibanez", "iIbanez@mail.com", "Fabricator", 0, fab2.FabricatorId);
            }
            AddDealer();
        }
        public void AddDealer()
        {
            User srsAdmin = _db.User.Where(e => e.Email.Trim().ToLower() == "SRSAdministrator@vcldesign.com".Trim().ToLower()).FirstOrDefault();
            List<Dealer> dealerList = _db.Dealer.ToList();
            if (dealerList.Count > 0) { }
            else
            {
                List<Fabricator> fabricators = _db.Fabricator.ToList();

                var fab1 = fabricators.Where(e => e.PrimaryContactEmail == "jGacia@mrglass.com").FirstOrDefault();
                Address add = BuildAddress("1182 Hawthorne Blvd.", "Carson", "CA", "90112", srsAdmin.UserId);
                Dealer del = BuildDealer("Authentic Design", "David Willow", "(310)-657-1121", "dwillow@Authdesign.com", 100000, fab1.FabricatorId, add.AddressId);
                AddSRSUsers("Jakobe", "Lucas", "lLucas@mail.com", "Full", del.DealerId, 0);
                //AddSRSUsers("Deegan", "Duke", "dDuke@mail.com", "Full", del.DealerId, 0);
                AddSRSUsers("Erin", "Vaughan", "vVaughan@mail.com", "Restricted", del.DealerId, 0);

                add = BuildAddress("9000 Royal Road", "Arlington", "Texas", "71101", srsAdmin.UserId);
                del = BuildDealer("Admiral Doors and Windows", "Duke Wellington", "(463)-447-9183", "duke@AdmiralDoorsandGlass.com", 100000, fab1.FabricatorId, add.AddressId);
                AddSRSUsers("Ava", "Page", "pPage@mail.com", "Full", del.DealerId, 0);
                AddSRSUsers("Tyree", "Kidd", "kKidd@mail.com", "Restricted", del.DealerId, 0);

                //fab1 = fabricators.Where(e => e.PrimaryContactEmail == "juanvaldez@IAVA.com").FirstOrDefault();
                //add = BuildAddress("386 west 14 Street", "New York", "NY", "10067", srsAdmin.UserId);
                //del = BuildDealer("Euro Windows", "Franz Hammil", "(212)-447-9183", "Hammil@EuroWindowNY.com", 100000, fab1.FabricatorId, add.AddressId);
                //AddSRSUsers("Louis", "Duarte", "dDuarte@mail.com", "Full", del.DealerId, 0);
                //AddSRSUsers("Tommy", "Prince", "pPrince@mail.com", "Full", del.DealerId, 0);
                //AddSRSUsers("Dwayne", "Conway", "cConway@mail.com", "Restricted", del.DealerId, 0);
                //AddSRSUsers("Sincere", "Long", "lLong@mail.com", "Restricted", del.DealerId, 0);

                //add = BuildAddress("111-475 Liberty Pkwy.", "Denver", "CO", "21121", srsAdmin.UserId);
                //del = BuildDealer("Colorado Glass", "Chip Wallas", "(645)-732-4711", "chipwallas@ColoradoGlass.com", 100000, fab1.FabricatorId, add.AddressId);
                //AddSRSUsers("Yusuf", "Duncan", "dDuncan@mail.com", "Full", del.DealerId, 0);
                //AddSRSUsers("Charles", "Brooks", "bBrooks@mail.com", "Full", del.DealerId, 0);
                //AddSRSUsers("Orion", "Cook", "cCook@mail.com", "Restricted", del.DealerId, 0);
            }
            UpdateFinancials();
        }
        public void AddSRSAdmin()
        {
            User srsAdmin = _db.User.Where(e => e.Email.Trim().ToLower() == "SRSAdmin@vcldesign.com".Trim().ToLower()).FirstOrDefault();
            if (srsAdmin == null) {
                AddSRSUsers("SRS", "Admin", "SRSAdmin@vcldesign.com", "SRSAdministrator", 0, 0);
            }
        }
        public void AddSRSUsers(string FirstName, string LastName, string Email, string role, int DealerId, int FabricatorId)
        {
            CreateSRSUsers(BuildUser(FirstName, LastName, Email, "bps2019!", role, DealerId, FabricatorId));
        }
        public SRSUserApiModel BuildUser(string NameFirst, string NameLast, string Email, string Password, string UserRole, int DealerId, int FabricatorId)
        {
            return new SRSUserApiModel
            {
                NameFirst = NameFirst,
                NameLast = NameLast,
                Email = Email,
                Password = Password,
                UserRole = UserRole,
                FabricatorId = FabricatorId,
                DealerId = DealerId
            };
        }
        public Address BuildAddress(string Line1, string Line2, string City, string PostalCode, int currentUserId)
        {
            Address add = new Address
            {
                AddressExternalId = Guid.NewGuid(),
                Line1 = Line1,
                Line2 = Line2,
                City = City,
                PostalCode = PostalCode,
                CreatedBy = currentUserId,
                CreatedOn = DateTime.Now,
                Longitude = 0,
                Latitude = 0,
            };
            _db.Address.Add(add);
            _db.SaveChanges();
            return add;
        }
        public Fabricator BuildFabricator(string Name, string PrimaryContactName, string PrimaryContactPhone, string PrimaryContactEmail, int AddressId)
        {
            Fabricator fab = new Fabricator
            {
                FabricatorExternalId = Guid.NewGuid(),
                Name = Name,
                PrimaryContactName = PrimaryContactName,
                PrimaryContactPhone = PrimaryContactPhone,
                PrimaryContactEmail = PrimaryContactEmail,
                SupportsADS = 1,
                SupportsASS = 1,
                SupportsAWS = 1,
                AddressId = AddressId
            };
            _db.Fabricator.Add(fab);
            _db.SaveChanges();
            return fab;
        }
        public Dealer BuildDealer(string Name, string PrimaryContactName, string PrimaryContactPhone, string PrimaryContactEmail, int CreditLine, int FabricatorId, int AddressId)
        {

            Dealer del = new Dealer
            {
                DealerExternalId = Guid.NewGuid(),
                Name = Name,
                PrimaryContactName = PrimaryContactName,
                PrimaryContactPhone = PrimaryContactPhone,
                PrimaryContactEmail = PrimaryContactEmail,
                ADSFabricatorId = FabricatorId,
                AWSFabricatorId = FabricatorId,
                ASSFabricatorId = FabricatorId,
                CreditLine = CreditLine,
                AddressId = AddressId,
            };
            _db.Dealer.Add(del);
            _db.SaveChanges();
            return del;
        }
        public void UpdateFinancials()
        {
            User srsAdmin = _db.User.Where(e => e.Email.Trim().ToLower() == "SRSAdministrator@vcldesign.com".Trim().ToLower()).FirstOrDefault();
            List<Dealer> allDealer = _db.Dealer.ToList();
            List<Financial> allFinancial = _db.Financial.ToList();
            var updateDealerInfoList = allDealer.Where(w => !allFinancial.Any(e => e.DealerId == w.DealerId)).ToList();
            foreach (var item in updateDealerInfoList)
            {
                if (item != null)
                {
                    var fin = new Financial();
                    fin.FinancialExternalId = Guid.NewGuid();

                    fin.DealerId = item.DealerId;
                    fin.LineOfCredit = item.CreditLine;
                    fin.OrdersToDate = 0;
                    fin.PaidToDate = 0;
                    fin.CurrentBalance = item.CreditLine;

                    fin.CreatedBy = srsAdmin.UserId;
                    fin.CreatedOn = DateTime.Now;

                    _db.Financial.Add(fin);
                    _db.SaveChanges();
                }
            }
        }

        public void CreateSRSUsers(SRSUserApiModel model)
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
                    AddUsersToAspNet(user.Email, role);
                }


            }
            catch (Exception ex)
            {

            }
        }
        public void AddUsersToAspNet(string email, string role)
        {
            User newUser = _db.User.First(x => x.Email == email);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
            var user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email };
            var result = userManager.Create(user, "bps2019!");
            result = userManager.SetLockoutEnabled(user.Id, false);
            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(newUser.Email))
            {
                result = userManager.AddToRole(user.Id, role);
            }

            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}