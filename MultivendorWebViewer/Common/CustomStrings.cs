using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.Common
{
    public class CustomStrings
    {
        public static string Order { get { return TextManager.Current.GetText("Order"); } }
        public static string Search { get { return TextManager.Current.GetText("Search"); } }
        public static string Menu { get { return TextManager.Current.GetText("Menu"); } }
        public static string StartPage { get { return TextManager.Current.GetText("StartPage"); } }
        public static string UserName { get { return TextManager.Current.GetText("UserName"); } }
        public static string Password { get { return TextManager.Current.GetText("Password"); } }
        public static string InformationNotFound { get { return TextManager.Current.GetText("InformationNotFound"); } }
        public static string SubCatagories { get { return TextManager.Current.GetText("SubCatagories"); } }
        public static string Products { get { return TextManager.Current.GetText("Products"); } }
        public static string StockInformation { get { return TextManager.Current.GetText("StockInformation"); } }
        public static string Discount { get { return TextManager.Current.GetText("Discount"); } }
        public static string Specification { get { return TextManager.Current.GetText("Specification"); } }
        public static string PriceIclVat { get { return TextManager.Current.GetText("PriceIclVat"); }
        }
    }
}