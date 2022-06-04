using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class ProductController : BaseController
    {
        [HttpGet]
        public  ActionResult Index(int id)
        {
            var product = ApplicationRequestContext.ProductManager.GetProductById(id);
            var viewModel = new ProductViewModel(product, ApplicationRequestContext);
            return View(viewModel);

        }

        public ActionResult GetOrderInformation(int productId) {
            var prices = ApplicationRequestContext.ProductManager.GetpriceByproductId(productId);
            if (prices != null) {
                var priceViewModel = prices.Select(p => new PriceAvailailityViewModel(p, ApplicationRequestContext));
                //if(priceViewModel != null)
                return AsyncHtml(partialView: "_OrderInformation", model: priceViewModel.ToList(), insertMethod: HtmlInsertMethod.Html);
                //return PartialView("_OrderInformation", priceViewModel.ToList());
            }
            return new EmptyResult();
        }
        public ActionResult GetContactInformation(int productId)
        {
            var prices = ApplicationRequestContext.ProductManager.GetpriceByproductId(productId);
            if (prices != null)
            {
                var priceViewModel = prices.Select(p => new PriceAvailailityViewModel(p, ApplicationRequestContext));
                if (prices != null && prices.Any()) {
                    var userManager = ApplicationRequestContext.UserDBManager;
                    var userModels = prices.Where(p => p.UserId.HasValue).Select(s => userManager.GetUserByUserId(s.UserId.Value)).ToArray();
                    if (userModels != null && userModels.Any()) {
                        var userViewModel = userModels.Select(p => new UserViewModel(p, ApplicationRequestContext));
                        return AsyncHtml(partialView: "_ContactInformation", model: userViewModel, insertMethod: HtmlInsertMethod.Html);

                        //return PartialView("_ContactInformation", userViewModel);

                    }
                }
            }
            return new EmptyResult();
        }
        public  ActionResult NodeProductsDataView(DataViewRequest request, int nodeId)
        {
            var node = ApplicationRequestContext.CategoryManager.GetNodeById(nodeId);
            var allProducts = new List<ProductViewModel>();
            if (node.ProductNodes != null && node.ProductNodes.Any())
            {
                var productIds = node.ProductNodes.Select(p => p.ProductId).ToArray();
                var products = ApplicationRequestContext.ProductManager.GetProductByIds(productIds);
                foreach (var item in products)
                {
                    allProducts.Add(new ProductViewModel(item, ApplicationRequestContext));
                }
            }

            var tools = new List<ToolBarItem>();


           
                //tools.Insert(0, new ToolBarItem
                //{
                //    Label = "New address",
                //    Icon = ApplicationRequestContext.GetIcon(Icons.Plus),
                //    Id = "CreateCompanyAddress",
                //    ClassNames = "create-new-company-address",
                //    TextAlignment = ToolBarAlignment.Left,
                //    Location = DataViewOptions.ToolLocations.AfterSortSelectors,
                //});
           

            var options = request.State.CreateOptions();
            //options.PageSizes = new PaginationPageSize[] { 25, 50, 100 };
            //options.DefaultPageSize = 25;
            options.DefaultSortSelector = "FormattedName";
            options.Tools = tools;
            options.DisplayFilter = true;
            //options.DownloadToolAvailable = true;
            options.DisplayItemCountDescription = true;
            options.SortSelectors = new DataViewSelectorCollection<DataViewSortSelector>
                          {
                              new DataViewSortSelector { Id = "FormattedName", Label = CustomStrings.Name, Sort = new Sort("FormattedName") },
                              new DataViewSortSelector { Id = "FormattedDescription", Label = CustomStrings.Description, Sort = new Sort("FormattedDescription") },
                            };

            options.FilterSelectors = new DataViewSelectorCollection<DataViewFilterSelector>
                          {
                              new DataViewFilterSelector { Id = "FormattedName", Label = CustomStrings.Name, Property = "FormattedName" },
                              new DataViewFilterSelector { Id = "FormattedDescription", Label = CustomStrings.Description, Property = "FormattedDescription" },

                          };
            var result = new DataViewResult(request, ApplicationRequestContext)
            {

                DataViewOptions = options,
                Content = state =>
                {
                    var dataViewModel = new DataViewString<ProductViewModel>(ApplicationRequestContext, allProducts,state:state);
                    return new DataViewContentResult("_NodeProducts", dataViewModel);
                }
               // ,
               // ReportColumnsProvider = cr => new[]
               //{
               //     new TableReportColumn { Id = "FormattedName", Header = CustomStrings.Name, PropertyName = "FormattedName" },
               //     new TableReportColumn { Id = "FormattedDescription", Header = CustomStrings.Description, PropertyName = "FormattedDescription" },
               //},
               // ReportFileNameProvider = () =>
               // {
               //     return "allProducts.csv";
               // }
            };
            return result;
        }
    }
}