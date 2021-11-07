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
        public int Id { get { return Model.Id; } }
        public int SequenceNumber { get { return Model.SequenceNumber.HasValue ? Model.SequenceNumber.Value : 0; } }
        public string ImageName { get { return Model.ImageName; } }
        public string GetUrl()
        {
            var routeValues = new { imageId = Id , fileName = ImageName} /*Dictionary<string, object>()*/;
            return UrlUtility.Action(ApplicationRequestContext, "Image", "Content", routeValues);
        }
        public string GetThumbnailUrl()
        {
            var routeValues = new { imageId = Id, fileName = ImageName, width = 300, height = 225 } /*Dictionary<string, object>()*/;
            return UrlUtility.Action(ApplicationRequestContext, "ImageThumbnail", "Content", routeValues);
        }
    }
}