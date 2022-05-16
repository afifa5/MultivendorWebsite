using MultivendorWebViewer.Common;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.Components
{
    public class IconsDictionary : ObjectDictionary<IconDescriptor>
    {
        public IconsDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override string GetKeyForItem(IconDescriptor item)
        {
            return item.Id;
        }

        protected override IconDescriptor ResolveConflictingItem(IconDescriptor exisitingItem, IconDescriptor newItem)
        {
            if (newItem == null) return exisitingItem;
            var customIconType = newItem.GetType();
            if (customIconType == typeof(IconDescriptor) /*|| customIconType == exisitingItem.GetType()*/)
            {
                exisitingItem.Merge(newItem);
                return exisitingItem;
            }
            newItem.Merge(exisitingItem);
            return newItem;
        }
    }

    public class IconsManager:SingletonBase<IconsManager> 
    {
        
       
        public IconsManager() 
           
        {
            icons = new IconsDictionary();
            string selectedSiteIcon = "FontAwesome";
            if (StringComparer.OrdinalIgnoreCase.Equals(selectedSiteIcon, "Material"))
            {
                icons.AddRange(new IconDescriptor[]
                {
                    // REMEMBER TO ADD IN ALPHABETIC ORDER
                    //new MaterialIconDescriptor(Icons.AddOrganization, ""), 
					new MaterialIconDescriptor(Icons.AngleRight,"chevron_right"),
                    new MaterialIconDescriptor(Icons.Add, "add_circle_outline"),
                    new MaterialIconDescriptor(Icons.AddUser, "person_add"),
                    new MaterialIconDescriptor(Icons.ArrowCircleRight, "next_plan"),
                    new MaterialIconDescriptor(Icons.Archive, "insert_drive_file"),
                    new MaterialIconDescriptor(Icons.AngleDown, "expand_more"),
                    new MaterialIconDescriptor(Icons.Backward, "arrow_back"),
                    new MaterialIconDescriptor(Icons.Bars, "reorder"),
                    
                    new MaterialIconDescriptor(Icons.BarCode,"qr_code_2"),
                    new MaterialIconDescriptor(Icons.BarCodeRead,"qr_code_scanner"),
                    new MaterialIconDescriptor(Icons.Book, "menu_book"),
                    new MaterialIconDescriptor(Icons.Bullhorn, "announcement"),
                    new MaterialIconDescriptor(Icons.CaretDown, "arrow_drop_down"),
                    new MaterialIconDescriptor(Icons.CaretUp, "arrow_drop_up"),
                    new MaterialIconDescriptor(Icons.Check, "check"),
                    new MaterialIconDescriptor(Icons.CheckCircle,"check-circle"),
                    new MaterialIconDescriptor(Icons.ChevronDoubleLeft, "chevron_left"),
                    new MaterialIconDescriptor(Icons.ChevronDoubleRight, "chevron_right"),
                    new MaterialIconDescriptor(Icons.Chart,"poll"),
                    new MaterialIconDescriptor(Icons.ChevronDown, "expand_more"),
                    new MaterialIconDescriptor(Icons.ChevronLeft, "chevron_left"),
                    new MaterialIconDescriptor(Icons.ChevronRight, "chevron_right"),
                    new MaterialIconDescriptor(Icons.ChevronUp, "expand_less"),
                    new MaterialIconDescriptor(Icons.Close, "close"),
                    new MaterialIconDescriptor(Icons.Cog, "settings"),
                    new MaterialIconDescriptor(Icons.Company, "business"),
                    new MaterialIconDescriptor(Icons.Cogs, "engineering"),
                    new MaterialIconDescriptor(Icons.Comments, "comment_bank"),
                    new MaterialIconDescriptor(Icons.Copy, "file_copy"),
                    new MaterialIconDescriptor(Icons.Database, "table_chart"),
                    new MaterialIconDescriptor(Icons.Delete, "delete"),
                    new MaterialIconDescriptor(Icons.Earphone, "call"),
                    new MaterialIconDescriptor(Icons.Ellipsis, "more_horiz"),
                    new MaterialIconDescriptor(Icons.Envelope, "mail"),
                    new MaterialIconDescriptor(Icons.Eraser, "backspace"),
                    
                    //new FontAwesomeIconDescriptor(Icons.ExcelFile, "file-excel"),
                    new MaterialIconDescriptor(Icons.ExcelFile, "description"),

                    new MaterialIconDescriptor(Icons.Exchange, "sync_alt"),
                    new MaterialIconDescriptor(Icons.Exclamation, "error"),
                    new MaterialIconDescriptor(Icons.ExclamationLong, "priority_high"),
                    new MaterialIconDescriptor(Icons.ExclamationTriangel, "warning"),
                    
                    new MaterialIconDescriptor(Icons.FastBackward, "fast_rewind"),
                    
                    new MaterialIconDescriptor(Icons.Filter, "filter_alt"),
                    new MaterialIconDescriptor(Icons.FList, "list"),
                    new MaterialIconDescriptor(Icons.File, "description"),

                    //new FontAwesomeIconDescriptor(Icons.CsvFile, "file-csv"),
                    new MaterialIconDescriptor(Icons.CsvFile, "description"),
                    //new FontAwesomeIconDescriptor(Icons.FileSignature, "file-signature"),
                    new MaterialIconDescriptor(Icons.FileSignature, "description"),

                    new MaterialIconDescriptor(Icons.Forward, "fast_forward"),
                    new MaterialIconDescriptor(Icons.Fullscreen, "open_with"),
                    new MaterialIconDescriptor(Icons.Globe, "language"),
                    new MaterialIconDescriptor(Icons.Heart,"favorite_border"),
                    new MaterialIconDescriptor(Icons.Home,"home"),
                    new MaterialIconDescriptor(Icons.HeartFilled,"favorite"),
                    new MaterialIconDescriptor(Icons.Info, "info"),

                    new MaterialIconDescriptor(Icons.Image, "image"),
                    new MaterialIconDescriptor(Icons.Illustration, "photo_library"),
                    new MaterialIconDescriptor(Icons.LeftArrow, "arrow_back"),
                    
                    new MaterialIconDescriptor(Icons.Link, "link"),
                    new MaterialIconDescriptor(Icons.LinkExternal, "insert_link"),
                    
                    new MaterialIconDescriptor(Icons.List, "list"),
                    new MaterialIconDescriptor(Icons.ListAlt, "list_alt"),
                    new MaterialIconDescriptor(Icons.ShoppingList, "list_alt"),
                    new MaterialIconDescriptor(Icons.LogIn, "login"),
                    new MaterialIconDescriptor(Icons.LogOut, "logout"),
                    new MaterialIconDescriptor(Icons.LongArrowDown, "south"),
                    new MaterialIconDescriptor(Icons.Mail, "mail"),
                    new MaterialIconDescriptor(Icons.Minus, "remove"),
                    new MaterialIconDescriptor(Icons.Money, "credit_card"),
                    new MaterialIconDescriptor(Icons.News, "announcement"),
                    new MaterialIconDescriptor(Icons.Note, "note"),
                    new MaterialIconDescriptor(Icons.NotificationBell, "notifications"),
                    new MaterialIconDescriptor(Icons.Organizations, "business"),
                    new MaterialIconDescriptor(Icons.Paint, "brush"),
                    new MaterialIconDescriptor(Icons.Password, "vpn_key"),
                    //new FontAwesomeIconDescriptor(Icons.PdfFile, "file-pdf"),
                    new MaterialIconDescriptor(Icons.PdfFile, "description"),
                    new MaterialIconDescriptor(Icons.Pen, "edit"),
                    new MaterialIconDescriptor(Icons.Picture, "photo"),
                    new MaterialIconDescriptor(Icons.Pin, "push_pin"),
                    new MaterialIconDescriptor(Icons.Plus, "add"),
                    new MaterialIconDescriptor(Icons.Print, "print"),
                    //new FontAwesomeIconDescriptor(Icons.PowerPointFile, "file-powerpoint-o"),
                    new MaterialIconDescriptor(Icons.PowerPointFile, "description"),
                    new MaterialIconDescriptor(Icons.QuestionMark, "help_center"),
                    new MaterialIconDescriptor(Icons.Refresh, "sync"),
                    new MaterialIconDescriptor(Icons.Remove, "close"),
                    new MaterialIconDescriptor(Icons.Save, "save_alt"),
                    new MaterialIconDescriptor(Icons.SaveButton, "save"),
                    new MaterialIconDescriptor(Icons.Search, "search"),
                    new MaterialIconDescriptor(Icons.Send, "send"),
                    new MaterialIconDescriptor(Icons.SearchSlider, "tune"),
                    new MaterialIconDescriptor(Icons.ShoppingCart, "shopping_cart"),
                   
                    new MaterialIconDescriptor(Icons.ShoppingCartDown, "add_shopping_cart"),
                    
                    new MaterialIconDescriptor(Icons.Shrink, "close_fullscreen"),
                    
                    new FontAwesomeIconDescriptor(Icons.Slash,"slash"),

                    new MaterialIconDescriptor(Icons.Sorting, "sort"),

                    new MaterialIconDescriptor(Icons.SortUp, "arrow_drop_up"),
                    new MaterialIconDescriptor(Icons.SortDown, "arrow_drop_down"),

                    new FontAwesomeIconDescriptor(Icons.Spinner, "spinner"),

                    new MaterialIconDescriptor(Icons.Star, "star"),
                    new MaterialIconDescriptor(Icons.SupervisedUserCircle,"supervised_user_circle"),
                    new MaterialIconDescriptor(Icons.Sync,"sync"),
                    new MaterialIconDescriptor(Icons.Table, "table_rows"),
                    new MaterialIconDescriptor(Icons.Tag, "label_important"),
                    new MaterialIconDescriptor(Icons.TextDocument, "sticky_note_2"),
                    new MaterialIconDescriptor(Icons.TextFile, "description"),
                    new MaterialIconDescriptor(Icons.ThumbnailsLarge, "view_module"),
                    new MaterialIconDescriptor(Icons.ThumbnailsList, "view_list"),
                    new MaterialIconDescriptor(Icons.Trash, "delete"),
                    new MaterialIconDescriptor(Icons.Truck, "local_shipping"),
                    new MaterialIconDescriptor(Icons.Upload, "publish"),
                    new MaterialIconDescriptor(Icons.UnLink, "link_off"),
                    new MaterialIconDescriptor(Icons.User, "account_circle"),
                    new MaterialIconDescriptor(Icons.Users, "people_alt"),
                    new MaterialIconDescriptor(Icons.Video, "ondemand_video"),
                    new MaterialIconDescriptor(Icons.Warning, "warning"),
                    new FontAwesomeIconDescriptor(Icons.Weee, "Weee"),
                    new MaterialIconDescriptor(Icons.WindowClose,"cancel_presentation"),
                    new MaterialIconDescriptor(Icons.Wrench, "build"),
                    //new FontAwesomeIconDescriptor(Icons.WordFile, "file-word-o"),
                    new MaterialIconDescriptor(Icons.WordFile, "description"),
                    new MaterialIconDescriptor(Icons.Write, "text_format"),
                    //new FontAwesomeIconDescriptor(Icons.XmlFile, "file-code-o"),
                    new MaterialIconDescriptor(Icons.XmlFile, "description"),
                    new MaterialIconDescriptor(Icons.ZoomFull, "zoom_out_map"),
                    new MaterialIconDescriptor(Icons.ZoomFullWidth, "open_in_full"),
                    new MaterialIconDescriptor(Icons.ZoomIn, "zoom_in"),
                    new MaterialIconDescriptor(Icons.ZoomOut, "zoom_out"),
                    new MaterialIconDescriptor(Icons.Edit, "create"),
                    new MaterialIconDescriptor(Icons.QrCode,"qr_code"),

                });
            }
            if (StringComparer.OrdinalIgnoreCase.Equals(selectedSiteIcon, "FontAwesome")) { 
                icons.AddRange(new IconDescriptor[]
                {
                            // REMEMBER TO ADD IN ALPHABETIC ORDER
                    //new FontAwesomeIconDescriptor(Icons.AddOrganization, ""), 
                    new FontAwesomeIconDescriptor(Icons.AngleRight,"angle-right"),
                    new FontAwesomeIconDescriptor(Icons.Add, "plus-square"),
                    new FontAwesomeIconDescriptor(Icons.AddUser, "user-plus"),
                    new FontAwesomeIconDescriptor(Icons.ArrowCircleRight, "arrow-circle-o-right"),
                    new FontAwesomeIconDescriptor(Icons.Archive, "file-archive-o"),
                    new FontAwesomeIconDescriptor(Icons.AngleDown, "angle-down"),
                    new FontAwesomeIconDescriptor(Icons.Backward, "backward"),
                    new FontAwesomeIconDescriptor(Icons.Bars, "bars"),
                    new FontAwesomeIconDescriptor(Icons.BarCode,"barcode"),
                    new FontAwesomeIconDescriptor(Icons.BarCodeRead,"barcode-read"),
                    new FontAwesomeIconDescriptor(Icons.Book, "book"),
                    new FontAwesomeIconDescriptor(Icons.Bullhorn, "bullhorn"),
                    new FontAwesomeIconDescriptor(Icons.CaretDown, "caret-down"),
                    new FontAwesomeIconDescriptor(Icons.CaretUp, "caret-up"),
                    new FontAwesomeIconDescriptor(Icons.Check, "check"),
                    new FontAwesomeIconDescriptor(Icons.CheckCircle,"check-circle"),
                    new FontAwesomeIconDescriptor(Icons.ChevronDoubleLeft, "angle-double-left"),
                    new FontAwesomeIconDescriptor(Icons.ChevronDoubleRight, "angle-double-right"),
                    new FontAwesomeIconDescriptor(Icons.Chart,"chart-line"),
                    new FontAwesomeIconDescriptor(Icons.ChevronDown, "chevron-down"),
                    new FontAwesomeIconDescriptor(Icons.ChevronLeft, "chevron-left"),
                    new FontAwesomeIconDescriptor(Icons.ChevronRight, "chevron-right"),
                    new FontAwesomeIconDescriptor(Icons.ChevronUp, "chevron-up"),
                    new FontAwesomeIconDescriptor(Icons.Close, "times"),
                    new FontAwesomeIconDescriptor(Icons.Cog, "cog"),
                    new FontAwesomeIconDescriptor(Icons.Company, "building"),
                    new FontAwesomeIconDescriptor(Icons.Cogs, "cogs"),
                    new FontAwesomeIconDescriptor(Icons.Comments, "comments"),
                    new FontAwesomeIconDescriptor(Icons.Copy, "copy"),
                    new FontAwesomeIconDescriptor(Icons.Database, "database"),
                    new FontAwesomeIconDescriptor(Icons.Delete, "remove"),
                    new FontAwesomeIconDescriptor(Icons.Earphone, "phone"),
                    new FontAwesomeIconDescriptor(Icons.Ellipsis, "ellipsis-h"),
                    new FontAwesomeIconDescriptor(Icons.Envelope, "fas fa-envelope"),
                    new FontAwesomeIconDescriptor(Icons.Eraser, "eraser"),
                    new FontAwesomeIconDescriptor(Icons.ExcelFile, "file-excel"),
                    new FontAwesomeIconDescriptor(Icons.Exchange, "exchange-alt"),
                    new FontAwesomeIconDescriptor(Icons.Exclamation, "exclamation-circle"),
                    new FontAwesomeIconDescriptor(Icons.ExclamationLong, "exclamation"),
                    new FontAwesomeIconDescriptor(Icons.ExclamationTriangel, "exclamation-triangle"),
                    new FontAwesomeIconDescriptor(Icons.Heart,"heart"),
                    new FontAwesomeIconDescriptor(Icons.Home,"home"),
                    new FontAwesomeIconDescriptor(Icons.HeartFilled,"gratipay"),
                    new FontAwesomeIconDescriptor(Icons.FastBackward, "fast-backward"),
                    new FontAwesomeIconDescriptor(Icons.Filter, "filter"),
                    new FontAwesomeIconDescriptor(Icons.FList, "list"),
                    new FontAwesomeIconDescriptor(Icons.File, "file"),
                    new FontAwesomeIconDescriptor(Icons.CsvFile, "file-csv"),
                    new FontAwesomeIconDescriptor(Icons.FileSignature, "file-signature"),
                    new FontAwesomeIconDescriptor(Icons.Forward, "forward"),
                    new FontAwesomeIconDescriptor(Icons.Fullscreen, "expand"),
                    new FontAwesomeIconDescriptor(Icons.Globe, "globe"),
                    new FontAwesomeIconDescriptor(Icons.Info, "info-circle"),
                    new FontAwesomeIconDescriptor(Icons.Image, "file-image-o"),
                    new FontAwesomeIconDescriptor(Icons.Illustration, "images"),
                    new FontAwesomeIconDescriptor(Icons.LeftArrow, "arrow-left"),
                    new FontAwesomeIconDescriptor(Icons.Link, "external-link-square-alt"),
                    new FontAwesomeIconDescriptor(Icons.LinkExternal, "external-link-alt"),
                    new FontAwesomeIconDescriptor(Icons.List, "list"),
                    new FontAwesomeIconDescriptor(Icons.ListAlt, "list-alt"),
                    new FontAwesomeIconDescriptor(Icons.ShoppingList, "list-alt",fontAwsome5 : true),
                    new FontAwesomeIconDescriptor(Icons.LogIn, "sign-in-alt"),
                    new FontAwesomeIconDescriptor(Icons.LogOut, "sign-out-alt"),
                    new FontAwesomeIconDescriptor(Icons.LongArrowDown, "long-arrow-down"),
                    new FontAwesomeIconDescriptor(Icons.Mail, "envelope"),
                    new FontAwesomeIconDescriptor(Icons.Minus, "minus"),
                    new FontAwesomeIconDescriptor(Icons.Money, "money-bill-alt"),
                    new FontAwesomeIconDescriptor(Icons.News, "newspaper"),
                    new FontAwesomeIconDescriptor(Icons.Note, "edit"),
                    new FontAwesomeIconDescriptor(Icons.NotificationBell, "bell-o"),
                    new FontAwesomeIconDescriptor(Icons.Organizations, "sitemap"),
                    new FontAwesomeIconDescriptor(Icons.Paint, "paint-brush"),
                    new FontAwesomeIconDescriptor(Icons.Password, "key"),
                    new FontAwesomeIconDescriptor(Icons.PdfFile, "file-pdf"),
                    new FontAwesomeIconDescriptor(Icons.Pen, "pencil-alt"),
                    new FontAwesomeIconDescriptor(Icons.Picture, "photo"),
                    new FontAwesomeIconDescriptor(Icons.Pin, "thumbtack"),
                    new FontAwesomeIconDescriptor(Icons.Plus, "plus"),
                    new FontAwesomeIconDescriptor(Icons.Print, "print"),
                    new FontAwesomeIconDescriptor(Icons.PowerPointFile, "file-powerpoint-o"),
                    new FontAwesomeIconDescriptor(Icons.QuestionMark, "question"),
                    new FontAwesomeIconDescriptor(Icons.Refresh, "sync"),
                    new FontAwesomeIconDescriptor(Icons.Remove, "times"),
                    new FontAwesomeIconDescriptor(Icons.Save, "download"),
                    new FontAwesomeIconDescriptor(Icons.SaveButton, "fas fa-save"),
                    new FontAwesomeIconDescriptor(Icons.Search, "search"),
					new FontAwesomeIconDescriptor(Icons.Send, "paper-plane"),
                    new FontAwesomeIconDescriptor(Icons.SearchSlider, "sliders"),
                    new FontAwesomeIconDescriptor(Icons.ShoppingCart, "shopping-cart"),
                    new FontAwesomeIconDescriptor(Icons.ShoppingCartDown, "cart-arrow-down"),
                    new FontAwesomeIconDescriptor(Icons.Shrink, "compress"),
                    new FontAwesomeIconDescriptor(Icons.Slash,"slash"),
                    new FontAwesomeIconDescriptor(Icons.Sorting, "sort"),
                    new FontAwesomeIconDescriptor(Icons.SortUp, "sort-up"),
                    new FontAwesomeIconDescriptor(Icons.SortDown, "sort-down"),
                    new FontAwesomeIconDescriptor(Icons.Spinner, "spinner"),
                    new FontAwesomeIconDescriptor(Icons.Star, "star"),
                    new FontAwesomeIconDescriptor(Icons.SupervisedUserCircle,"user-lock"),
                    new FontAwesomeIconDescriptor(Icons.Sync,"sync"),
                    new FontAwesomeIconDescriptor(Icons.Table, "table"),
                    new FontAwesomeIconDescriptor(Icons.Tag, "tag"),
                    new FontAwesomeIconDescriptor(Icons.TextDocument, "file-alt"),
                    new FontAwesomeIconDescriptor(Icons.TextFile, "file-text-o"),
                    new FontAwesomeIconDescriptor(Icons.ThumbnailsLarge, "th-large"),
                    new FontAwesomeIconDescriptor(Icons.ThumbnailsList, "th-list"),
                    new FontAwesomeIconDescriptor(Icons.Trash, "trash"),
                    new FontAwesomeIconDescriptor(Icons.Truck, "truck"),
                    new FontAwesomeIconDescriptor(Icons.Upload, "upload"),
                    new FontAwesomeIconDescriptor(Icons.UnLink, "chain-broken"),
                    new FontAwesomeIconDescriptor(Icons.User, "user"),
                    new FontAwesomeIconDescriptor(Icons.Users, "users"),
                    new FontAwesomeIconDescriptor(Icons.Video, "file-video-o"),
                    new FontAwesomeIconDescriptor(Icons.Warning, "exclamation-triangle"),
                    new FontAwesomeIconDescriptor(Icons.Weee, "Weee"),
                    new FontAwesomeIconDescriptor(Icons.WindowClose,"window-close"),
                    new FontAwesomeIconDescriptor(Icons.Wrench, "wrench"),
                    new FontAwesomeIconDescriptor(Icons.WordFile, "file-word-o"),
                    new FontAwesomeIconDescriptor(Icons.Write, "font"),
                    new FontAwesomeIconDescriptor(Icons.XmlFile, "file-code-o"),
                    new FontAwesomeIconDescriptor(Icons.ZoomFull, "arrows-alt"),
                    new FontAwesomeIconDescriptor(Icons.ZoomFullWidth, "arrows-alt-h"),
                    new FontAwesomeIconDescriptor(Icons.ZoomIn, "search-plus"),
                    new FontAwesomeIconDescriptor(Icons.ZoomOut, "search-minus"),
                    new FontAwesomeIconDescriptor(Icons.Edit, "pencil-alt"),
                    new FontAwesomeIconDescriptor(Icons.QrCode,"qrcode"),
                
                });
            }
            // Add custom icons
            //if (siteContext.Configuration.IconDescriptors != null)
            //{
            //    /*First remove the Icons*/
              
            //    icons.AddRange(siteContext.Configuration.IconDescriptors);
            //}
        }

        //public IconsManager(SiteContext siteContext)
        //{
            //ProfileSettingsProvider.Default.Changed += Default_Changed;

        
        //}

        private IconsDictionary icons;

        //private Dictionary<Tuple<string, string>, IconsDictionary> contextedIcons = new Dictionary<Tuple<string, string>, IconsDictionary>();

        public IconDescriptor GetIcon(string id, string context = null)
        {
            //if (context != null)
            //{
            //    IconDescriptor ctxIcon;
            //    if (contextedIcons.TryGetValue(new Tuple<string, string>(id, context), out ctxIcon) == true)
            //    {
            //        var icon = icons[id];
            //    }
            //    // TODO
            //}
            //else
            //{
                
            return id != null ? icons[id] : null;
            //}
        }

        //protected IconDescriptor GetIconDescriptor(IconDescriptor customIcon, IconDescriptor defaultIcon)
        //{
        //    if (customIcon == null) return defaultIcon;
        //    var customIconType = customIcon.GetType();
        //    if (customIconType == typeof(IconDescriptor) || customIconType == defaultIcon.GetType())
        //    {
        //        defaultIcon.Merge(customIcon);
        //        return defaultIcon;
        //    }
        //    customIcon.Merge(defaultIcon);
        //    return customIcon;
        //}
    }

    public static class Icons
    {
        // A
       // public const string AddOrganization = "";

        public const string ArrowCircleRight = "arrowCircleRight";

        public const string AddUser = "addUser";

        public const string Archive = "archive";

        public const string Add = "plus-square";

        public const string AngleDown = "angle-down";

        public const string AngleRight = "angle-right";

        // B

        public const string Backward = "backward";

        public const string Book = "book";

        public const string Bars = "bars";

        public const string BarCode = "barcode";

        public const string BarCodeRead = "barcode-read";

        public const string Bullhorn = "bullhorn";

        // C

        public const string CaretDown = "caretDown";
        
        public const string CaretUp = "caretUp";

        public const string Chart = "chart";

        public const string Check = "check";

        public const string ChevronDoubleLeft = "chevronDoubleLeft";

        public const string ChevronDoubleRight = "chevronDoubleRight";

        public const string ChevronRight = "chevronRight";

        public const string ChevronLeft = "chevronLeft";

        public const string ChevronUp = "chevronUp";

        public const string ChevronDown = "chevronDown";

        public const string Close = "close";

        public const string Cog = "cog";

        public const string Company = "building";

        public const string Comment = "comment";

        public const string Comments = "comments";

        public const string Copy = "copy";

        public const string Cogs = "cogs";
       
        public const string CsvFile = "file-csv";

        public const string CheckCircle = "checkCircle";

        // D

        public const string Database = "database";

        public const string Delete = "delete";

        // E

        public const string Earphone = "earphone";

        public const string Edit = "pencil-alt";

        public const string Exchange = "exchange";

        public const string Ellipsis = "ellipsis";

        public const string Envelope = "envelope";

        public const string Eraser = "eraser";

        public const string Exclamation = "exclamation";

        public const string ExclamationLong = "exclamationLong";

        public const string ExclamationTriangel = "exclamationTriangel";

        public const string ExcelFile = "excelFile";

        // F

        public const string Filter = "filter";

        public const string File = "file";

        public const string FileSignature = "file-signature";

        public const string FastBackward = "fastBackward";

        public const string Forward = "forward";

        public const string Fullscreen = "fullscreen";

        // G

        public const string Globe = "globe";

        //H
        public const string Heart = "heart";

        public const string HeartFilled = "heartFilled";

        public const string Home = "home";

        // I

        public const string Info = "info";

        public const string Image = "image";
        
        public const string Illustration = "illustration";

        // L

        public const string Link = "link";

        public const string LinkExternal = "linkExternal";

        public const string LeftArrow = "leftArrow";

        public const string List = "list";

        public const string FList = "flist";

        public const string ListAlt = "listAlt";

        public const string ShoppingList = "ShoppingList";

        public const string LogIn = "logIn";

        public const string LogOut = "logOut";

        public const string LongArrowDown = "longArrowDown";

        // M

        public const string Mail = "mail";

        public const string Minus = "minus";

        public const string Money = "money";

        // N

        public const string News = "news";

        public const string Note = "note";

        public const string NotificationBell = "notificationBell";

        // O

        public const string Organizations = "organizations";

        // P

        public const string Paint = "paint";

        public const string Password = "password";

        public const string PdfFile = "pdfFile";

        public const string Pen = "pen";

        public const string Picture = "picture";

        public const string Pin = "pin";

        public const string Plus = "plus";

        public const string Print = "print";

        public const string PowerPointFile = "powerpointFile";

        // Q

        public const string QuestionMark = "questionMark";

        public const string QrCode = "qrCode";
 
        // R

        public const string Refresh = "refresh";

        public const string Remove = "remove";

        // S

        public const string Save = "save";

        public const string SaveButton = "saveButton";

        public const string Search = "search";

        public const string Send = "send";

        public const string SearchSlider = "searchSlider";

        public const string ShoppingCart = "shoppingCart";

        public const string ShoppingCartDown = "shoppingCartDown";

        public const string Shrink = "shrink";

        public const string Slash = "slash";
        
        public const string Spinner = "spinner";

        public const string Sorting = "sorting";

        public const string SortUp = "sort-up";

        public const string SortDown = "sort-down";

        public const string Star = "star";

        public const string SupervisedUserCircle = "supervisedUserCircle";

        public const string Sync = "sync";


        // T

        public const string Table = "table";

        public const string Tag = "tag";

        public const string TextDocument = "textDocument";

        public const string TextFile = "textFile";

        public const string ThumbnailsLarge = "thumbnailsLarge";

        public const string ThumbnailsList = "thumbnailsList";

        public const string Trash = "trash";

        public const string Truck = "truck";

        // U

        public const string UnLink = "unLink";

        public const string Upload = "upload";

        public const string User = "user";

        public const string Users = "users";

        // V

        public const string Video = "video";

        // W

        public const string Warning = "warning";

        public const string Weee = "weee";

        public const string Wrench = "wrench";

        public const string Write = "write";

        public const string WordFile = "wordFile";

        public const string WindowClose = "windowClose";

        // X

        public const string XmlFile = "xmlFile";

        // Z

        public const string ZoomIn = "zoomIn";

        public const string ZoomOut = "zoomOut";

        public const string ZoomFull = "zoomFull";

        public const string ZoomFullWidth = "zoomFullWidth";



    }
}