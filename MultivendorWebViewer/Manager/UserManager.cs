using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Threading.Tasks;
using System.IO;
using RefactorThis.GraphDiff;
using MultivendorWebViewer.Server.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace MultivendorWebViewer.Manager
{
    public class UserDBManager:SingletonBase<UserDBManager>
    {
        public  User Login(SignInInformation info)
        {
            if (info == null) throw new ArgumentNullException("info");
            if (info== null) throw new ArgumentNullException("info.AuthenticationInformation");

           User user = FindUserByName(info.UserName);

            if (user == null) return user; // throw new NotFoundException();

            // Check if this is a plain password
            if (info.PassWord == user.PassWord) return user;

            // Check for a hashed password
            var passwordhasher = PasswordLookup.Default;
            PasswordVerificationResult result = passwordhasher.VerifyHashedPassword(user.PassWord, info.PassWord);

            if (result == PasswordVerificationResult.Success) return user;

            return null;
        }

        public User FindUserByName(string username)
        {
            using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
            {
                var user = context.Users.Where(p=>p.UserName == username && p.IsActive == true).Include(p=>p.Customer).FirstOrDefault();
                return user;
            }
        }
        public User GetUserByUsername(string userName)
        {
            var allUsers = GetAllUsers();
            return allUsers[userName].FirstOrDefault();

        }
        public User GetUserByUserId(int id)
        {
            using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
            {
                var user = context.Users.Where(p => p.Id == id).FirstOrDefault();
                return user;
            }

        }
        public Customer GetCustomerById(int id)
        {
            var allCustomer = GetAllCustomer();
            return allCustomer[id].FirstOrDefault();

        }
        public ILookup<string, User> GetAllUsers()
        {
            return CacheManager.Default.Get<ILookup<string, User>>(string.Concat("AllUser@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var allUsers = context.Users.ToLookup(t => t.UserName);
                    return allUsers;
                }
            });
        }
        public ILookup<int, Customer> GetAllCustomer()
        {
            return CacheManager.Default.Get<ILookup<int, Customer>>(string.Concat("AllCustomer@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var allCustomers = context.Customers.ToLookup(t => t.Id);
                    return allCustomers;
                }
            });
        }



    }
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(ApplicationRequestContext requestContext, IUserStore<ApplicationUser> store)
            : base(store)
        {
            ApplicationRequestContext = requestContext;
        }

        public static ApplicationUserManager Empty = new ApplicationUserManager(null, new ApplicationUserStore(null));
        public ApplicationRequestContext ApplicationRequestContext { get; set; }

        // Solution from: http://stackoverflow.com/questions/21918000/mvc5-vs2012-identity-createidentityasync-value-cannot-be-null
        public override Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public override Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            var loginManager = ApplicationRequestContext != null ? ApplicationRequestContext.UserDBManager : UserDBManager.Default;

            if (loginManager != null)
            {
                var user = loginManager.FindUserByName(userId);
                if (user != null)
                {
                    IList<Claim> result = new List<Claim>();
                    if (String.IsNullOrEmpty(user.UserName) == false) result.Add(new Claim(ClaimTypes.Name, user.UserName));
                    if (String.IsNullOrEmpty(user.Customer.LastName) == false) result.Add(new Claim(ClaimTypes.Surname, user.Customer.LastName));
                    if (String.IsNullOrEmpty(user.Customer.FirstName) == false) result.Add(new Claim(ClaimTypes.GivenName, user.Customer.FirstName));
                    if (String.IsNullOrEmpty(user.Customer.Email) == false) result.Add(new Claim(ClaimTypes.Email, user.Customer.Email));
                    if (String.IsNullOrEmpty(user.Customer.PhoneNumber) == false) result.Add(new Claim(ClaimTypes.HomePhone, user.Customer.PhoneNumber));

                    return Task.FromResult<IList<Claim>>(result);
                }
            }

            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public override Task<IdentityResult> RemoveClaimAsync(string userId, Claim claim)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public async override Task<IList<string>> GetRolesAsync(string userId)
        {
            var loginManager = ApplicationRequestContext != null ? ApplicationRequestContext.UserDBManager : UserDBManager.Default;

            if (loginManager != null)
            {
                var user = loginManager.FindUserByName(userId);
                if (user != null)
                {
                    IList<string> result = new List<string>() { user.UserRole};
                    return result;
                }
                return null;
            }
            else
            {
                return await base.GetRolesAsync(userId);
            }
        }

        public async override Task<ApplicationUser> FindAsync(string userName, string password)
        {

            var loginManager = ApplicationRequestContext!=null ? ApplicationRequestContext.UserDBManager :UserDBManager.Default;
            if (loginManager != null)
            {
                var user = loginManager.Login(new SignInInformation { UserName = userName, PassWord = password  });
                if (user != null)
                {
                    return new ApplicationUser(user);
                }
                return null;
            }
            else
            {
                return await base.FindAsync(userName, password);
            }
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            
            var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(System.Web.HttpContext.Current.Request.RequestContext.HttpContext);
            if (routeData == null) return new ApplicationUserManager(null, new ApplicationUserStore(null)); // ApplicationUserManager.Empty;
            var applicationRequestContext = ApplicationRequestContext.GetContext(HttpContext.Current);

            if (applicationRequestContext == null) return new ApplicationUserManager(null, new ApplicationUserStore(null)); // ApplicationUserManager.Empty;

            ApplicationUserStore applicationUserStore = new ApplicationUserStore(applicationRequestContext);
            var manager = Instance.Create<ApplicationRequestContext, ApplicationUserStore, ApplicationUserManager>(applicationRequestContext, applicationUserStore);

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true,
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            //// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            //// You can write your own provider and plug in here.
            //manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = Strings.Strings.YourSecurityCodeIs
            //});
            //manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = Strings.Strings.SecurityCode,
            //    BodyFormat = Strings.Strings.YourSecurityCodeIs
            //});
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }


    }
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserClaimStore<ApplicationUser>
    {
        public ApplicationUserStore(ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;

        }

        public ApplicationRequestContext ApplicationRequestContext { get; set; }

       
        public virtual Task CreateAsync(ApplicationUser user)
        {
            throw new System.NotSupportedException();
        }

        public virtual Task DeleteAsync(ApplicationUser user)
        {
            throw new System.NotSupportedException();
        }

        public virtual Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return Task.Run(() =>
            {
                var multivendorUser = ApplicationRequestContext != null ? ApplicationRequestContext.UserDBManager.FindUserByName(userId) : null;
                return multivendorUser != null ? new ApplicationUser(multivendorUser) : null;
            });
        }

        public virtual Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var dbManager = ApplicationRequestContext != null ? ApplicationRequestContext.UserDBManager : UserDBManager.Default;
            return Task.Run(() =>
            {
                var multivendorUser = dbManager.FindUserByName(userName) ;
                return multivendorUser != null ? new ApplicationUser(multivendorUser) : null;
            });
        }

        public virtual Task UpdateAsync(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.Run(() => user.MultivendorUser.PassWord);
        }

        public virtual Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.Run(() =>string.IsNullOrEmpty( user.MultivendorUser.PassWord) == false);
        }

        public virtual Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            throw new System.NotSupportedException();
        }

        public virtual Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            throw new System.NotSupportedException();
        }

        //public virtual Task<IList<string>> GetRolesAsync(ApplicationUser user)
        //{
        //    return Task.Run(() => (user.Roles == null) ? new List<string>() : user.Roles.Select(r => r.RoleId).ToList<string>() as IList<string>);
        //}

        //public virtual Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        //{
        //    return Task.Run(() => user.MultivendorUser.UserGroups.Any(g => g.GroupName == roleName));
        //}

        //public virtual Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        //{
        //    throw new System.NotSupportedException();
        //}

        public void Dispose()
        {
        }

        // Solution from: http://stackoverflow.com/questions/21918000/mvc5-vs2012-identity-createidentityasync-value-cannot-be-null
        public virtual Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            return Task.FromResult<int>(0);
        }

        public virtual Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public virtual Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            return Task.FromResult<int>(0);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            throw new NotImplementedException();
        }
    }

}