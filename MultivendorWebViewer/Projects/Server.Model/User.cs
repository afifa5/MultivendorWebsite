using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Server.Models
{
    
    public partial class User
    {
        public User()
        {
            
        }
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [NotMapped]
        public string ExternalId { get; set; }
        public int? ImageId { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string PassWord { get; set; }


        [StringLength(50)]
        public string CompanyName { get; set; }
        public bool? IsActive { get; set; }
        [StringLength(50)]
        public string UserRole { get; set; }


    }
}
