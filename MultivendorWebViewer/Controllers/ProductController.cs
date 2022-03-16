using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Common;
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
                return PartialView("_OrderInformation", priceViewModel.ToList());
            }
            return new EmptyResult();
        }

    }
}