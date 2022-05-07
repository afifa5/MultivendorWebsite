using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class NavigationPathController : BaseController
    {
        public ActionResult NavigationBar( int? nodeId = null,int? productId = null)
        {
            var path = new List<NavigationPathViewModel>();
            int currentNodeId;
            List<int> nodeIds = new List<int>();
            int categoryId;
            try
            {
                if (nodeId.HasValue)
                {
                    currentNodeId = nodeId.Value;
                l:
                    var sobNode = ApplicationRequestContext.CategoryManager.GetSubnode(currentNodeId);
                    if (sobNode == null)
                    {
                        //Get catalogue Id 
                        categoryId = ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId.Value;
                    }
                    else {
                        currentNodeId = sobNode.OriginalNodeId;
                        nodeIds.Add(currentNodeId);
                        goto l;
                    }
                }
                else {
                    categoryId = ApplicationRequestContext.Configuration.SiteProfile.StartCatalogueId.Value;
                }
                 var category = ApplicationRequestContext.CategoryManager.GetCategory(categoryId, ApplicationRequestContext);
                var categoryViewModel = new CategoryViewModel(category, ApplicationRequestContext);
                 path.Add(new NavigationPathViewModel(ApplicationRequestContext) { Url = categoryViewModel.GetUrl(), DisplayText = CustomStrings.StartPage });
                if (nodeIds.Any()) {
                    nodeIds.Reverse();

                }
                foreach (int item in nodeIds) {
                    var node = ApplicationRequestContext.CategoryManager.GetNodeById(item);
                    var nodeViewModel = new NodeViewModel(node, ApplicationRequestContext);
                    path.Add(new NavigationPathViewModel(ApplicationRequestContext) { Url = nodeViewModel.GetUrl(), DisplayText = nodeViewModel.FormattedName });
                }

            }
            catch (Exception)
            {
               
            }
            return PartialView("NavigationBar", path);

        }


    }
}