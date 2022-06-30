using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.Membership
{
    public class PrincipalManagerSerializeModel
    {
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