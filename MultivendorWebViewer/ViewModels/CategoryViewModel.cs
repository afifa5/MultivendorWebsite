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
        public string FormattedName { get { return Model.Name.GetTranslation(ApplicationRequestContext.SelectedCulture); } }
        public string FormattedNameDescription { get { return Model.Description.GetTranslation(ApplicationRequestContext.SelectedCulture); } }
        public List<NodeViewModel> Nodes { get { return GetNodes(); } }


        //All Functions
        public List<NodeViewModel> GetNodes()
        {
            var alllist = new List<NodeViewModel>();
            if (Model.CategoryNodes.Count() > 0)
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
        
        
    }
}