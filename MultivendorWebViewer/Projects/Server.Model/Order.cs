using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.Server.Models
{
    

    public partial class Order
    {
        public Order() {
            OrderLines = new List<OrderLine>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderReference { get; set; }
        //Whom bought product
        [Required]
        public int CustomerId { get; set; }
        public string DeliveryMethodName { get; set; }
        
        [Column(TypeName = "numeric")]
        public decimal DeliveryCost { get; set; }
        public string OrderStatus { get; set; }

        [ForeignKey("CustomerId")]
        public  Customer Customer { get; set; }
        [ForeignKey("OrderId")]
        public List<OrderLine> OrderLines { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
