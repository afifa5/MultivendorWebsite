using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    public partial class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderReference { get; set; }
        //Whom bought product
        [Required]
        public int CustomerId { get; set; }
        public  Customer Customer { get; set; }
    }
}
