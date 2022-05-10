using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Models
{

    public partial class Customer
    {
        public Customer()
        {
            
        }
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string CustomerIdentity { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Address { get; set; }
        
        [StringLength(50)]
        public string PostCode { get; set; }
        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string CareOf { get; set; }
       
        [StringLength(50)]
        public string Country { get; set; }


    }
}
