using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MultivendorWebViewer.Membership
{
    public class PrincipalManager : IPrincipal
    {
        public IIdentity Identity
        {
            get;
            private set;
        }

        public PrincipalManager(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            if (roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullUserName { get; set; }
        
        public string[] roles { get; set; }

        //public string Password { get; set; }
        //public string Email { get; set; }
        //public string CustomerCode { get; set; }
        //public string ShippingAddress { get; set; }
        //public string BillingAddress { get; set; }

    }
}