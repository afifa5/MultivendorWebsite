(function ( digitalHalalMarket,multivendorWeb, $, document) {
    //Showing the availabe laguage in the toolbar in a dropdown list.
    $(document).on("click", ".language-container", function (e) {

    /*call to the controller function */
        let languageContainer = $(this);
        var actionUrl = $(this).data("language-item-url");
        $.ajax({
            url: actionUrl,
           /* data: { searchtext: searchtext, page: 0, pageSize: pagesize },*/
            dataType: "html",
            type: "GET",
            cache: false,
            success: function (html) {
                $html = $(html);
                multivendorWeb.PopUp.Show($html, languageContainer, null)

            }, error: function (ex) {
               
            }
        });
        /*end call */
    });
    $(document).on("click", ".user-menu", function (e) {
        var userMenuhtml = $(this);
        var url = userMenuhtml.data("user-menu-url")
        $.ajax({
            url: url,
            datatype: "html",
            type: "GET",
            cache: false,
            success: function (data) {
                var html = $(data)
                multivendorWeb.PopUp.Show(html, userMenuhtml, null)
            }

        })
    })
    //Setting language in dropdown list.
    $(document).on("click", ".language-item", function (e) {

        /*call to the controller function */
        let actionUrl = $(this).closest(".available-language-menu").data("set-language-url");
        let languageCode = $(this).data("culture");
        $.ajax({
            url: actionUrl,
            data: { languageCode: languageCode /*, page: 0, pageSize: pagesize */ },
            dataType: "json",
            type: "POST",
            cache: false,
            success: function (data) {
                var $data =data
                if ($data.status == true) {
                    window.location.reload();
                }

            }, error: function (ex) {

            }
        });
        /*end call */
    });
}(window.digitalHalalMarket = window.digitalHalalMarket || {}, window.multivendorWeb = window.multivendorWeb || {}, window.jQuery, document));