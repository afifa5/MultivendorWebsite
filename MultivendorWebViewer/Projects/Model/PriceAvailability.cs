using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Models
{
   

    [Table("PriceAvailability")]
    public partial class PriceAvailability
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
       
        public int? UserId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Quantity { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? UnitPrice { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? TaxAmount { get; set; }
        //Store as 21 May 2022
        [StringLength(50)]
        public string ExpectedShippingDate { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Discount { get; set; }
        [ForeignKey("ProductId")]
        public  Product Product { get; set; }
        

    }
}
