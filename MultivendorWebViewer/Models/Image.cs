using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class Image
    {
        public Image()
        {
           
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public string ImageName { get; set; }

       
    }
}
