using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class NodeImage
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public int ImageId { get; set; }
       
        [ForeignKey("ImageId")]
        public  Image Image { get; set; }
        [ForeignKey("NodeId")]
        public  Node Node { get; set; }
    }
}
