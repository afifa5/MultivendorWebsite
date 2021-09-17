using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class NodeImage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NodeId { get; set; }
        [Required]
        public int ImageId { get; set; }
        public virtual Node Node { get; set; }
        public virtual Image Image { get; set; }
       
    }
}
