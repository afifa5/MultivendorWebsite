using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Server.Models
{

    public partial class OrderLine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        //Whose product bought
        public int? UserId { get; set; }

        //Comma seperated specifications
        public string SelectedSpecifications { get; set; }

        public DateTime? ExpectedShippingDate { get; set; }
        
        public string ShippingStatus { get; set; }


        [Column(TypeName = "numeric")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "numeric")]
        public decimal PriceInclTax { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Discount { get; set; }


        [Column(TypeName = "numeric")]
        public decimal SubTotal { get; set; }

        [ForeignKey("UserId")]
        public  User User { get; set; }
        [ForeignKey("OrderId")]
        public  Order Order { get; set; }
    }
}
