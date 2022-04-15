using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class CustomerViewModel
    {
        public CustomerViewModel(Customer customer, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = customer;

        }
        private Customer Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string FirstName { get { return Model?.FirstName; } }
        public string CustomerIdentity { get { return Model?.CustomerIdentity; } }
        public string LastName { get { return Model?.LastName; } }
        public string Email { get { return Model?.Email; } }
        public string PhoneNumber { get { return Model?.PhoneNumber; } }
        public string CareOf { get { return Model?.CareOf; } }
        public string Country { get { return Model?.Country; } }
        public string Address { get { return Model?.Address; } }
        public string PostCode { get { return Model?.PostCode; } }
        public string City { get { return Model?.City; } }


    }
}