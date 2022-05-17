using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Common
{
    public static class MultivendorUserCreator
    {
        public static User Create(System.Security.Principal.IIdentity identity)
        {
            if (identity is System.Security.Principal.WindowsIdentity) return Create(identity as System.Security.Principal.WindowsIdentity);
            if (identity is System.Security.Claims.ClaimsIdentity) return Create(identity as System.Security.Claims.ClaimsIdentity);
            return null;
        }

        public static User Create(System.Security.Principal.IPrincipal user)
        {
            if (user is WindowsPrincipal) return Create(user as WindowsPrincipal);
            if (user is System.Security.Claims.ClaimsPrincipal) return Create(user as System.Security.Claims.ClaimsPrincipal);
            return null;
        }

        public static User Create(WindowsPrincipal user)
        {
            WindowsIdentity identity = user.Identity as WindowsIdentity;
            return Create(identity);
        }

        public static User Create(System.Security.Principal.WindowsIdentity identity)
        {
            //string givenName = null;
            //if (identity.HasClaim(i => i.Type == ClaimTypes.GivenName)) givenName = identity.FindFirst(ClaimTypes.GivenName).Value;

            //string surname = null;
            //if (identity.HasClaim(i => i.Type == ClaimTypes.Surname)) surname = identity.FindFirst(ClaimTypes.Surname).Value;

            //string email = null;
            //if (identity.HasClaim(i => i.Type == ClaimTypes.Email)) email = identity.FindFirst(ClaimTypes.Email).Value;

            //string phoneNumber = null;
            //if (identity.HasClaim(i => i.Type == ClaimTypes.MobilePhone)) phoneNumber = identity.FindFirst(ClaimTypes.MobilePhone).Value;
            //if (phoneNumber == null && identity.HasClaim(i => i.Type == ClaimTypes.HomePhone)) phoneNumber = identity.FindFirst(ClaimTypes.HomePhone).Value;
            //if (phoneNumber == null && identity.HasClaim(i => i.Type == ClaimTypes.OtherPhone)) phoneNumber = identity.FindFirst(ClaimTypes.OtherPhone).Value;

            User newUser = new User
            {
                UserName = identity.Name,
                //FirstName = givenName,
                //LastName = surname,
                //Email = email,
                //PhoneNumber = phoneNumber,
                //UserGroups =
                //identity.Groups.Select(i => new UserGroup { GroupName = i.Value }).ToList()
            };

            //newUser.UserGroups.ForEach(i => i.User = newUser);
            //newUser.UserPermissions = PermissionConfigurationProvider.Default.Configuration.GetPermissionsContainingAnyGroup(newUser.UserGroups.Select(i => i.GroupName)).Select(j => j.Name).ToList();

            return newUser;
        }

        public static User Create(System.Security.Claims.ClaimsPrincipal user)
        {
            ClaimsIdentity identity = user.Identity as ClaimsIdentity;
            return Create(identity);
        }

        public static User Create(System.Security.Claims.ClaimsIdentity identity)
        {
            string givenName = null;
            if (identity.HasClaim(i => i.Type == ClaimTypes.GivenName)) givenName = identity.FindFirst(ClaimTypes.GivenName).Value;

            string surname = null;
            if (identity.HasClaim(i => i.Type == ClaimTypes.Surname)) surname = identity.FindFirst(ClaimTypes.Surname).Value;

            string email = null;
            if (identity.HasClaim(i => i.Type == ClaimTypes.Email)) email = identity.FindFirst(ClaimTypes.Email).Value;

            string phoneNumber = null;
            if (identity.HasClaim(i => i.Type == ClaimTypes.MobilePhone)) phoneNumber = identity.FindFirst(ClaimTypes.MobilePhone).Value;
            if (phoneNumber == null && identity.HasClaim(i => i.Type == ClaimTypes.HomePhone)) phoneNumber = identity.FindFirst(ClaimTypes.HomePhone).Value;
            if (phoneNumber == null && identity.HasClaim(i => i.Type == ClaimTypes.OtherPhone)) phoneNumber = identity.FindFirst(ClaimTypes.OtherPhone).Value;

            //string ClaimTypesGroups = "http://schemas.microsoft.com/ws/2008/06/identity/claims/groups";

            User newUser = new User
            {
                UserName = identity.Name,
                //FirstName = givenName,
                //LastName = surname,
                //Email = email,
                //PhoneNumber = phoneNumber,
                //UserGroups = identity
                //    .FindAll(ClaimTypesGroups)
                //    .Concat(identity.FindAll(ClaimTypes.Role))
                //    .Select(i => new UserGroup { GroupName = i.Value })
                //    .ToList()
            };

            //newUser.UserGroups.ForEach(i => i.User = newUser);
            //newUser.UserPermissions = PermissionConfigurationProvider.Default.Configuration.GetPermissionsContainingAnyGroup(newUser.UserGroups.Select(i => i.GroupName)).Select(j => j.Name).ToList();

            return newUser;
        }

    }
}
