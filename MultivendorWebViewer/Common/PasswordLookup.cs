using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer;

namespace MultivendorWebViewer.Common
{

    public class PasswordLookup : SingletonBase<PasswordLookup>, IPasswordHasher
    {
        public virtual string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public virtual PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (String.IsNullOrEmpty(hashedPassword)) throw new ArgumentOutOfRangeException("hashedPassword");
            if (String.IsNullOrEmpty(providedPassword)) throw new ArgumentOutOfRangeException("providedPassword");

            if (Crypto.VerifyHashedPassword(hashedPassword, providedPassword))
            {
                return PasswordVerificationResult.Success;
            }
            return PasswordVerificationResult.Failed;
        }
    }
}