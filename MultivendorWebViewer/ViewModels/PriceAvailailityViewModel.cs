using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class PriceAvailailityViewModel
    {

        public PriceAvailailityViewModel(PriceAvailability price, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = price;
        }
        private PriceAvailability Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public decimal? Quantity { get { return Model.Quantity; } }
        public decimal? PriceInclVat { get { return GetPriceIncludingTax(); } }

        public string FormattedPriceInclVat { get { return GetPriceText(PriceInclVat); } }
        public string FormattedDiscount { get { return GetPriceText(Discount); } }
        public string ExpectedShippingDate { get { return Model.ExpectedShippingDate; } }
        public decimal? Discount { get {return Model.Discount; } }
        public int ProductId { get { return Model.ProductId; } }
        public int? UserId => Model.UserId;
        public string GetPriceText(decimal? price) {
            if (price.HasValue && price.Value > 0) {
                var priceText = price.Value.ToString("n", CultureInfo.GetCultureInfo(ApplicationRequestContext.SelectedCulture));
                return string.Format("{0} {1}", priceText, TextManager.Current.GetText(ApplicationRequestContext.Configuration != null ? ApplicationRequestContext.Configuration.SiteProfile.PriceCurrency : "BDT"));
            }
            return string.Empty;
        }
        private decimal? GetPriceIncludingTax() {
            decimal? totalAmount = 0;
            if (Model.UnitPrice.HasValue) totalAmount += Model.UnitPrice.Value;
            if(Model.TaxAmount.HasValue) totalAmount += Model.TaxAmount.Value;
            return totalAmount > 0 ? totalAmount : null;
        }


    }
}