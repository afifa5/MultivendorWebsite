using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MultivendorWebViewer.Common
{
    public class EmailValidation : SingletonBase<EmailValidation>
    {
        /// <summary>
        /// This method calls private methods to validate different approach.
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public bool IsEmailVaild(string emailaddress)
        {
            bool result = false;

            if(string.IsNullOrEmpty(emailaddress))
                return false;

            if (IsValidEmailByIdnMapping(emailaddress))
            {
                if (IsValidEmailByDataAnnotations(emailaddress))
                {
                    result = true;
                }
                else
                {
                }
            }
            else { 
            }

            return result;
        }

        bool invalid = false;
        // This is from Mircorsoft but fails at some places.
        private bool IsValidEmailByIdnMapping(string emailaddress)
        {
            invalid = false;
            if (string.IsNullOrEmpty(emailaddress))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            emailaddress = Regex.Replace(emailaddress, @"(@)(.+)$", this.DomainMapper,RegexOptions.None);
            if (invalid)
                return false;

            // Return true if emailaddress is in valid e-mail format.
            bool idnMappingRegaxValid= Regex.IsMatch(emailaddress,
                                                     @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                                     @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                                                     RegexOptions.IgnoreCase);

            if (idnMappingRegaxValid == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private string DomainMapper(Match match)
        {
            IdnMapping idn = new IdnMapping();
            string domianname = match.Groups[2].Value;

            try
            {
                domianname = idn.GetAscii(domianname);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }

            return match.Groups[1].Value + domianname;
        }

        private bool IsValidEmailByDataAnnotations(string emailaddress)
        {
            return new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(emailaddress);
        }
    }
}