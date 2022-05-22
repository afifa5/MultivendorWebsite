using MultivendorWebViewer.Common;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string CustomerIdentity { get; set; }
        public string LastName { get; set; }
        //[Required(ErrorMessageResourceName = "UserNameRequired", ErrorMessageResourceType = typeof(CustomStrings))]
        //[DataType(DataType.EmailAddress)]
        //[Display(ResourceType = typeof(CustomStrings), Name = "UserName")]
        public string UserName { get; set; }

        public string Email { get; set; }

        //[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(CustomStrings))]
        //[DataType(DataType.Password)]
        //[Display(ResourceType = typeof(CustomStrings), Name = "Password")]
        public string Password { get; set; }
        //[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(CustomStrings))]
        //[DataType(DataType.Password)]
        //[Display(ResourceType = typeof(CustomStrings), Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }

        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }
        public string CareOf { get; set; }

        public string Country { get; set; }

        public string UserRole { get; set; }
        public string ModifiedBy { get; set; }

        public string CreatedBy { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModificationDate { get; set; }

    }
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "UserNameRequired", ErrorMessageResourceType = typeof(CustomStrings))]
        [Display(ResourceType = typeof(CustomStrings), Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(CustomStrings))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(CustomStrings), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(CustomStrings), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }

}