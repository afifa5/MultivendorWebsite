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
using Microsoft.Owin.Security;
using System.Data.Entity.Validation;

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
        public User Register(RegisterViewModel register)
        {
            if (register == null) throw new ArgumentNullException("info");
            if (register == null) throw new ArgumentNullException("info.AuthenticationInformation");

            User user = GetUserByUsername(register.UserName);
            if (user == null)
            {
                var passwordhasher = PasswordLookup.Default;
                string newhashedpassword = passwordhasher.HashPassword(register.Password);
                
                var customer = new Customer();
                customer.FirstName = register.FirstName;
                customer.Address = register.Address;
                customer.CareOf = register.CareOf;
                customer.City = register.City;
                customer.Country = register.Country;
                customer.CustomerIdentity = register.CustomerIdentity;
                customer.Email = register.Email;
                customer.LastName = register.LastName;
                customer.PhoneNumber = register.PhoneNumber;
                customer.PostCode = register.PostCode;

                var newUser = new User();
                newUser.UserName = register.UserName;
                newUser.PassWord = newhashedpassword;
                newUser.CompanyName = register.CompanyName;
                newUser.UserRole = register.UserRole;
                newUser.CreatedBy = register.UserName;
                newUser.CreatedDate = DateTime.Now;
                newUser.IsActive = true;//should be true on email verification

                using (var context = ServerModelDatabaseContextManager.Default.Context())
                {
                    //use try catch 
                    try
                    {
                        //Save customer
                        if (customer != null)
                        {
                            Customer newCustomer = context.UpdateGraph<Customer>(customer);
                            context.SaveChanges();
                            newUser.CustomerId = newCustomer.Id;
                            //order.Customer = null;
                        }
                        if (newUser != null)
                        {
                            User newUserItem = context.UpdateGraph<User>(newUser);
                            context.SaveChanges();
                            CacheManager.Default.Remove(string.Concat("AllUser@", "MultivendorWeb"));
                            CacheManager.Default.Remove(string.Concat("AllCustomer@", "MultivendorWeb"));
                            return newUserItem;
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                   
                   
                }
            }
            
          return null;


        }
        public User FindUserByName(string username)
        {
            using (var context = ServerModelDatabaseContextManager.Default.Context())
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
            using (var context = ServerModelDatabaseContextManager.Default.Context())
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
                using (var context = ServerModelDatabaseContextManager.Default.Context())
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
                using (var context = ServerModelDatabaseContextManager.Default.Context())
                {
                    var allCustomers = context.Customers.ToLookup(t => t.Id);
                    return allCustomers;
                }
            });
        }

        public RegisterViewModel GetRegisterViewModel(User user) {
            var registerViewModel = new RegisterViewModel();
            if (user != null) {
                registerViewModel.UserName = user.UserName;
                registerViewModel.ModifiedBy = user.ModifiedBy;
                registerViewModel.CreatedBy = user.CreatedBy;
                registerViewModel.CompanyName = user.CompanyName;
                registerViewModel.IsActive = user.IsActive;
                registerViewModel.UserRole = user.UserRole;
                registerViewModel.CreatedDate = user.CreatedDate;
                registerViewModel.ModificationDate = user.ModificationDate;
                if (user.CustomerId.HasValue) {
                    var customer = GetCustomerById(user.CustomerId.Value);
                    if (customer != null) {
                        registerViewModel.FirstName = customer.FirstName;
                        registerViewModel.CustomerIdentity = customer.CustomerIdentity;
                        registerViewModel.LastName = customer.LastName;
                        registerViewModel.Email = customer.Email;
                        registerViewModel.PhoneNumber = customer.PhoneNumber;
                        registerViewModel.Address = customer.Address;
                        registerViewModel.PostCode = customer.PostCode;
                        registerViewModel.City = customer.City;
                        registerViewModel.CareOf = customer.CareOf;
                        registerViewModel.Country = customer.Country;
                    }
                }
            }
            return registerViewModel;
        }


    }
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager( IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Empty = new ApplicationUserManager(new ApplicationUserStore());

        // Solution from: http://stackoverflow.com/questions/21918000/mvc5-vs2012-identity-createidentityasync-value-cannot-be-null
        public override Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public override Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            var loginManager =  UserDBManager.Default;

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
            var loginManager =  UserDBManager.Default;

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
        public virtual ApplicationUser RegisterUser(RegisterViewModel registerItem)
        {

            var loginManager =  UserDBManager.Default;
            if (loginManager != null)
            {
                var user = loginManager.Register(registerItem);
                if (user != null)
                {
                    return new ApplicationUser(user);
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public async override Task<ApplicationUser> FindAsync(string userName, string password)
        {

            var loginManager = UserDBManager.Default;
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
            if (routeData == null) return new ApplicationUserManager( new ApplicationUserStore()); // ApplicationUserManager.Empty;
            ApplicationUserStore applicationUserStore = new ApplicationUserStore();
            var manager = Instance.Create< ApplicationUserStore, ApplicationUserManager>(applicationUserStore);

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
                var multivendorUser = UserDBManager.Default.FindUserByName(userId) ;
                return multivendorUser != null ? new ApplicationUser(multivendorUser) : null;
            });
        }

        public virtual Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var dbManager =  UserDBManager.Default;
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