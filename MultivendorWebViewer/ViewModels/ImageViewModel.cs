using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.ViewModels
{
    public class ImageViewModel
    {
        public ImageViewModel(Image image, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = image;
        }
        private Image Model { get; set; }
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string Identity { get { return Model.Identity; } }

        public string ImageName { get { return Model.ImageName; } }
    }
}