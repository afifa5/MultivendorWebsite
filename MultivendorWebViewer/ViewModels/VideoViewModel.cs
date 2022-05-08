using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.ViewModels
{
    public class VideoViewModel
    {
        public VideoViewModel(Video video, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = video;
        }
        private Video Model { get; set; }

        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string Identity { get { return Model.Identity; } }
        public int Id { get { return Model.Id; } }
        public int SequenceNumber { get { return Model.SequenceNumber.HasValue ? Model.SequenceNumber.Value : 0; } }
        public string VideoName { get { return Model.VideoName; } }
        public string GetUrl()
        {
            var routeValues = new { videoId = Id , fileName = VideoName } /*Dictionary<string, object>()*/;
            return UrlUtility.Action(ApplicationRequestContext, "Video", "Content", routeValues);
        }
        //public string GetThumbnailUrl(int? width= null, int? height =null)
        //{
        //    var routeValues = new { imageId = Id, fileName = ImageName, width = width.HasValue?width.Value:300, height = height.HasValue? height.Value: 225 } /*Dictionary<string, object>()*/;
        //    return UrlUtility.Action(ApplicationRequestContext, "ImageThumbnail", "Content", routeValues);
        //}
    }
}