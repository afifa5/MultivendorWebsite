using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.ViewModels
{
    [NotMapped]
    public class CategoryViewModel 
    {
        public CategoryViewModel(Category category, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = category;
        }
        private Category Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public int Id { get { return Model.Id; } }
        public string Identity { get { return Model.Identity; } }
        public string FormattedName { get { return Model != null && Model.Name!=null ? Model.Name.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
        public string FormattedNameDescription { get { return Model!=null && Model.Description!=null ? Model.Description.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
        public List<ImageViewModel> Images { get { return GetImages(); } }
        public List<NodeViewModel> Nodes { get { return GetNodes(); } }
        public List<NodeViewModel> GetNodes()
        {
            var alllist = new List<NodeViewModel>();
            if (Model!=null&& Model.CategoryNodes!=null && Model.CategoryNodes.Count() > 0)
            {
                var ids = Model.CategoryNodes.Select(p => p.Id).ToArray();
               var categoryNodes= ApplicationRequestContext.CategoryManager.GetNodesByIds(ids);
                foreach (var item in categoryNodes)
                {
                    alllist.Add(new NodeViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }
        public List<ImageViewModel> GetImages()
        {
            var alllist = new List<ImageViewModel>();
            if (Model != null && Model.CategoryImages != null && Model.CategoryImages.Count() > 0)
            {
                var ids = Model.CategoryImages.Select(p => p.Id).ToArray();
                var categoryImages = ApplicationRequestContext.ImageManager.GetImagesByIds(ids);
                
                foreach (var item in categoryImages)
                {
                    alllist.Add(new ImageViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }


    }
}