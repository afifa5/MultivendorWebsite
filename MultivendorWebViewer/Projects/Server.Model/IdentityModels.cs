using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using MultivendorWebViewer.Server.Models;

namespace MultivendorWebViewer.Server.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser, IUser<string>
    {
        public User MultivendorUser { get; set; }

        public override string UserName
        {
            // TODO Should we ever need to guard for undefined multivendor-user?
            get { return MultivendorUser != null ? MultivendorUser.UserName : String.Empty; }
            set { if (MultivendorUser != null) MultivendorUser.UserName = value; }
        }

        public override ICollection<IdentityUserRole> Roles { get { return MultivendorUser == null ? null : new List<IdentityUserRole>() { new IdentityUserRole { RoleId = MultivendorUser.UserRole, UserId = this.Id, } }; } }

        public ApplicationUser(User multivendorUser)
        {
            if (multivendorUser != null)
            {
                // TODO Mess, we need new user definition in Assert
                MultivendorUser = multivendorUser;

                // TODO Hack, what do this mean?
                if (string.IsNullOrEmpty( multivendorUser.ExternalId) == true)
                {
                    multivendorUser.ExternalId = this.Id;
                }

                Id = multivendorUser.UserName;          // Id is the login name, internal Assert name or external backend system's user name
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            try
            {
                // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
                var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
                // Add custom user claims here
                return userIdentity;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }

}