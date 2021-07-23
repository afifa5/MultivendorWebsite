using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    [Table("PriceAvailability")]
    public partial class PriceAvailability
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int? UserId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Price { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
