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
    }
}