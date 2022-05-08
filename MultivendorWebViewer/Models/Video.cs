using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class Video
    {
        public Video()
        {
           
        }
        [Key]
        public int Id { get; set; }

        public int? SequenceNumber { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public string VideoName { get; set; }

       

       
    }
}
