using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Server.Models
{

    public partial class Payment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }

        
        public int? UserId { get; set; }


        public DateTime? PaymentDate { get; set; }
        
        public string PaymentMethod { get; set; }
        
        public string PaymentReference { get; set; }
        [StringLength(50)]
        public string PaymentStatus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal PaidAmount { get; set; }

        
        [ForeignKey("UserId")]
        public  User User { get; set; }
        [ForeignKey("OrderId")]
        public  Order Order { get; set; }
    }
}
