using MultivendorWebViewer.Common;
using MultivendorWebViewer.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel(User user, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = user;
        }
        private User Model { get; set; }

        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public int Id => Model.Id;

        public int? CustomerId => Model.CustomerId;
        private CustomerViewModel customer;
        public CustomerViewModel Customer { get { return customer = customer ?? GetCustomer(); }} 
        public ImageViewModel Image => GetImage();
        public int? ImageId => Model.ImageId;

        public string UserName => Model.UserName;

        public string Password => Model.PassWord;
        public string CompanyName => Model.CompanyName;
        public bool? IsActive => Model.IsActive;

        public string UserRole => Model.UserRole;
        protected CustomerViewModel GetCustomer() => CustomerId.HasValue? new CustomerViewModel(ApplicationRequestContext.UserDBManager.GetCustomerById(CustomerId.Value), ApplicationRequestContext) : null;
        protected ImageViewModel GetImage() => ImageId.HasValue? new ImageViewModel(ApplicationRequestContext.ImageManager.GetImagesById(ImageId.Value), ApplicationRequestContext): null;

    }
    //public enum UserRole
    //{

    //    Admin = 14275,
    //    Vendor = 65842,
    //    Customer = 98529

    //}
}