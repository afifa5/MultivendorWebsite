using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.Common
{
    public class CustomStrings
    {
        public static string FirstName { get { return TextManager.Current.GetText("FirstName"); } }
        public static string LastName { get { return TextManager.Current.GetText("LastName"); } }
        public static string UserNameRequired { get { return TextManager.Current.GetText("UserNameRequired"); } }

        public static string UserRole{ get { return TextManager.Current.GetText("UserRole"); } }
        public static string UserCompany { get { return TextManager.Current.GetText("UserCompany"); } }
        public static string UserActive { get { return TextManager.Current.GetText("UserActive"); } }
        public static string OrdersList { get { return TextManager.Current.GetText("OrdersList"); } }
        public static string PasswordRequired { get { return TextManager.Current.GetText("PasswordRequired"); } }
        public static string RememberMe { get { return TextManager.Current.GetText("RememberMe"); } }
        public static string CareOf { get { return TextManager.Current.GetText("CareOf"); } }
        public static string Address { get { return TextManager.Current.GetText("Address"); } }
        public static string Add { get { return TextManager.Current.GetText("Add"); } }
        public static string MyProfile { get { return TextManager.Current.GetText("MyProfile"); } }
        public static string FilterOnSelector { get { return TextManager.Current.GetText("FilterOnSelector"); } }
        public static string All { get { return TextManager.Current.GetText("All"); } }
        public static string Nothing { get { return TextManager.Current.GetText("Nothing"); } }
        public static string SortBy { get { return TextManager.Current.GetText("SortBy"); } }
        public static string Administration { get { return TextManager.Current.GetText("Administration"); } }

        public static string AddToOrder { get { return TextManager.Current.GetText("AddToOrder"); } }
        public static string ShippingAddress { get { return TextManager.Current.GetText("ShippingAddress"); } }
        public static string PostCode { get { return TextManager.Current.GetText("PostCode"); } }
        public static string City { get { return TextManager.Current.GetText("City"); } }
        public static string Country { get { return TextManager.Current.GetText("Country"); } }
        public static string Email { get { return TextManager.Current.GetText("Email"); } }
        public static string ConfirmPassword { get { return TextManager.Current.GetText("ConfirmPassword"); } }

        public static string Phone { get { return TextManager.Current.GetText("Phone"); } }
        public static string PostOffice { get { return TextManager.Current.GetText("PostOffice"); } }
        public static string DHL { get { return TextManager.Current.GetText("DHL"); } }

        public static string SundarbanExpress { get { return TextManager.Current.GetText("SundarbanExpress"); } }

        public static string HomeDelivery { get { return TextManager.Current.GetText("HomeDelivery"); } }

        public static string PaymentMethod { get { return TextManager.Current.GetText("PaymentMethod"); } }
        public static string BankTransfer { get { return TextManager.Current.GetText("BankTransfer"); } }
        public static string CreditCard { get { return TextManager.Current.GetText("CreditCard"); } }

        public static string CreatedDate { get { return TextManager.Current.GetText("CreatedDate"); } }
        public static string ModifiedDate { get { return TextManager.Current.GetText("ModifiedDate"); } }
        public static string ModifiedBy { get { return TextManager.Current.GetText("ModifiedBy"); } }
        public static string Bkash { get { return TextManager.Current.GetText("Bkash"); } }
        public static string PaymentReferenceMessage { get { return TextManager.Current.GetText("PaymentReferenceMessage"); } }
        public static string Agent { get { return TextManager.Current.GetText("Agent"); } }
        public static string Rocket { get { return TextManager.Current.GetText("Rocket"); } }
        public static string CashOnDelivery { get { return TextManager.Current.GetText("CashOnDelivery"); } }
        public static string ContactInformation { get { return TextManager.Current.GetText("ContactInformation"); } }
        public static string ViewPhone { get { return TextManager.Current.GetText("ViewPhone"); } }
        public static string ViewEmail { get { return TextManager.Current.GetText("ViewEmail"); } }
        public static string PlaceOrder { get { return TextManager.Current.GetText("PlaceOrder"); } }
        public static string DeliveryMethod { get { return TextManager.Current.GetText("DeliveryMethod"); } }
        public static string MyOrder { get { return TextManager.Current.GetText("MyOrder"); } }
        public static string DownloadInvoice { get { return TextManager.Current.GetText("DownloadInvoice"); } }
        public static string Order { get { return TextManager.Current.GetText("Order"); } }
        public static string Reference { get { return TextManager.Current.GetText("Reference"); } }
        public static string OrderEmptyMessage { get { return TextManager.Current.GetText("OrderEmptyMessage"); } }
        public static string ProductDetail { get { return TextManager.Current.GetText("ProductDetail"); } }
        public static string Quantity { get { return TextManager.Current.GetText("Quantity"); } }
        public static string Search { get { return TextManager.Current.GetText("Search"); } }
        public static string Menu { get { return TextManager.Current.GetText("Menu"); } }
        public static string StartPage { get { return TextManager.Current.GetText("StartPage"); } }
        public static string OrderStatus { get { return TextManager.Current.GetText("OrderStatus"); } }
        public static string UserName { get { return TextManager.Current.GetText("UserName"); } }
        public static string UserList { get { return TextManager.Current.GetText("UserList"); } }
        public static string Inventory { get { return TextManager.Current.GetText("Inventory"); } }
        public static string ClearOrder { get { return TextManager.Current.GetText("ClearOrder"); } }
        public static string Next { get { return TextManager.Current.GetText("Next"); } }
        public static string Previous { get { return TextManager.Current.GetText("Previous"); } }
        public static string Password { get { return TextManager.Current.GetText("Password"); } }
        public static string Register { get { return TextManager.Current.GetText("Register"); } }
        public static string SignIn { get { return TextManager.Current.GetText("SignIn"); } }
        public static string CreateAccount { get { return TextManager.Current.GetText("CreateAccount"); } }
        public static string SignOut { get { return TextManager.Current.GetText("SignOut"); } }
        public static string InformationNotFound { get { return TextManager.Current.GetText("InformationNotFound"); } }
        public static string SubCatagories { get { return TextManager.Current.GetText("SubCatagories"); } }
        public static string Success { get { return TextManager.Current.GetText("Success"); } }
        public static string PlaceOrderSuccess { get { return TextManager.Current.GetText("PlaceOrderSuccess"); } }
        public static string Summary { get { return TextManager.Current.GetText("Summary"); } }
        public static string NumberOfItems { get { return TextManager.Current.GetText("NumberOfItems"); } }
        public static string TotalPriceInclVat { get { return TextManager.Current.GetText("TotalPriceInclVat"); } }
        public static string OrdersPrice { get { return TextManager.Current.GetText("OrdersPrice"); } }
        public static string TotalDiscount { get { return TextManager.Current.GetText("TotalDiscount"); } }
        public static string Products { get { return TextManager.Current.GetText("Products"); } }
        public static string Catalogue { get { return TextManager.Current.GetText("Catalogue"); } }
        public static string Name { get { return TextManager.Current.GetText("Name"); } }
        public static string Description { get { return TextManager.Current.GetText("Description"); } }
        public static string ShippingAndBilling { get { return TextManager.Current.GetText("ShippingAndBilling"); } }
        public static string Payment { get { return TextManager.Current.GetText("Payment"); } }
        public static string SignInMessage { get { return TextManager.Current.GetText("SignInMessage"); } }

        public static string StockInformation { get { return TextManager.Current.GetText("StockInformation"); } }
        public static string OutOfStock { get { return TextManager.Current.GetText("OutOfStock"); } }
        public static string Discount { get { return TextManager.Current.GetText("Discount"); } }
        public static string Specification { get { return TextManager.Current.GetText("Specification"); } }
        public static string ContactRepresentative { get { return TextManager.Current.GetText("ContactRepresentative"); } }
        public static string ViewAll { get { return TextManager.Current.GetText("ViewAll"); } }
        public static string Videos { get { return TextManager.Current.GetText("Videos"); } }
        public static string UnitPrice { get { return TextManager.Current.GetText("UnitPrice"); } }
        public static string Ordered { get { return TextManager.Current.GetText("Ordered"); } }
        public static string Shipped { get { return TextManager.Current.GetText("Shipped"); } }
        public static string Delivered { get { return TextManager.Current.GetText("Delivered"); } }
        public static string Returned { get { return TextManager.Current.GetText("Returned"); } }
        public static string Settled { get { return TextManager.Current.GetText("Settled"); } }
        public static string PriceIclVat { get { return TextManager.Current.GetText("PriceIclVat"); }}
    }
}