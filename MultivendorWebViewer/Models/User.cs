using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    
    public partial class User
    {
        public User()
        {
            
        }
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        
        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }
        
        [StringLength(50)]
        public string PostCode { get; set; }


        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50)]
        public string UserRole { get; set; }


    }
}
