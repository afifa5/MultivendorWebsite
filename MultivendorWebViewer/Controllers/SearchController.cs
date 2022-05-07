using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultivendorWebViewer.Manager;
using MultivendorWebViewer.ViewModels;

namespace MultivendorWebViewer.Controllers
{
    public class SearchController : BaseController
    {
        public ActionResult Search(string term, int? displayCount = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(term)) {
                    var allProducts = ApplicationRequestContext.ProductManager.GetAllProduct().SelectMany(p=>p).Distinct();
                    var allSpecification = ApplicationRequestContext.ProductManager.GetAllSpecifications().SelectMany(p => p).Where(h => h.SpecificationMode == (int)SpecificationMode.Highlighted).Distinct();
                    //Searched Specification
                    var serachedSpecification = allSpecification.Where(item =>
                    {
                        if ((item != null &&  item.SpecificationText != null && item.SpecificationText.TextTranslations != null && item.SpecificationText.TextTranslations.Any(p => p.Translation.IndexOf(term, StringComparison.CurrentCultureIgnoreCase) != -1)))
                        {
                            return true;
                        }

                        return false;
                    }).ToLookup(p => p.Id);

                    allProducts = allProducts.Where(e => { 
                    if ((e.Name != null && e.Name.TextTranslations!=null && e.Name.TextTranslations.Any(p=> p.Translation.IndexOf(term, StringComparison.CurrentCultureIgnoreCase) != -1) ) ||
                          (e.Description != null && e.Description.TextTranslations != null && e.Description.TextTranslations.Any(p => p.Translation.IndexOf(term, StringComparison.CurrentCultureIgnoreCase) != -1)) ||
                            (e.ProductSpecifications != null && serachedSpecification!=null && serachedSpecification.Any() && e.ProductSpecifications.Any(p => serachedSpecification[p.SpecificationId].FirstOrDefault() != null))
                           )
                    {
                        return true;
                    }

                    return false;
                    });
                    if (allProducts != null && allProducts.Any()) {
                        var totalCount = allProducts.Count();
                        var allProductsViewModel = allProducts.Select(p => new ProductViewModel(p, ApplicationRequestContext)).OrderBy(p => p.FormattedName).Take(displayCount.HasValue  ? displayCount.Value : totalCount);
                        var searchHitViewModel = new SearchHitViewModel(term) {  DisplayCount = displayCount.HasValue && displayCount.Value <= totalCount? displayCount : totalCount, TotalCount = totalCount, Products = allProductsViewModel };
                        return PartialView("SearchResult", searchHitViewModel);
                    }
                     

                }

            }
            catch (Exception ex)
            {
            }

            return new EmptyResult();
        }

    }
}