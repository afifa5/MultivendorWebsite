using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    public partial class Order
    {
        public int Id { get; set; }

        [Required]
        public string OrderReference { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "numeric")]
        public decimal SubTotal { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
