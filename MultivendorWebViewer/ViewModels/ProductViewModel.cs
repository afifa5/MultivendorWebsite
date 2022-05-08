using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel(Product product, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = product;
        }
        private Product Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public int Id { get { return Model.Id; } }
        public string Identity { get { return Model.Identity; } }
        public bool? IsActive => Model.IsActive;
        public int SequenceNumber { get { return Model.SequenceNumber.HasValue ? Model.SequenceNumber.Value : 0; } }
        public Text Name => Model.Name;
        public Text Description => Model.Description;
        public string FormattedName { get { return Model != null && Model.Name != null ? Model.Name.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
        public string FormattedDescription { get { return Model != null && Model.Description != null ? Model.Description.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
        public List<ImageViewModel> Images { get { return GetImages(); } }

        public List<VideoViewModel> Videos { get { return GetVideos(); } }
        public List<SpecificationViewModel> Specifications { get { return GetSpecifications(); } }
        public List<VideoViewModel> GetVideos()
        {
            var alllist = new List<VideoViewModel>();
            if (Model != null && Model.ProductVideos != null && Model.ProductVideos.Count() > 0)
            {
                var ids = Model.ProductVideos.Select(p => p.VideoId).ToArray();
                var productVideos = ApplicationRequestContext.ImageManager.GetIVideosByIds(ids);

                foreach (var item in productVideos)
                {
                    alllist.Add(new VideoViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }
        public List<ImageViewModel> GetImages()
        {
            var alllist = new List<ImageViewModel>();
            if (Model != null && Model.ProductImages != null && Model.ProductImages.Count() > 0)
            {
                var ids = Model.ProductImages.Select(p => p.ImageId).ToArray();
                var categoryImages = ApplicationRequestContext.ImageManager.GetImagesByIds(ids);

                foreach (var item in categoryImages)
                {
                    alllist.Add(new ImageViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }
        public List<SpecificationViewModel> GetSpecifications()
        {
            var alllist = new List<SpecificationViewModel>();
            if (Model != null && Model.ProductSpecifications != null && Model.ProductSpecifications.Count() > 0)
            {
                var ids = Model.ProductSpecifications.Select(p => p.SpecificationId).ToArray();
                var nodeProducts = ApplicationRequestContext.ProductManager.GetSpecifications(ids);

                foreach (var item in nodeProducts)
                {
                    alllist.Add(new SpecificationViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }
        public string GetUrl()
        {
            var routeValues = new { id = Id } /*Dictionary<string, object>()*/;
            return UrlUtility.Action(ApplicationRequestContext, "Index", "Product", routeValues);
        }
    }
}