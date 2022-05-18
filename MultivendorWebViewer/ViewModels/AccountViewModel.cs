using MultivendorWebViewer.Common;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
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