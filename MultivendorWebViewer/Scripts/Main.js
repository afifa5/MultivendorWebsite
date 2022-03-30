﻿(function ( digitalHalalMarket,multivendorWeb, $, document) {
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
    /*Input increment number. Order quantity increment*/
    $(document).on("click", ".increase", function (e) {
        //Get quantity value
        let quantityElement = $(this).closest(".multivendor-quantity").find(".add-qty");
        let quantity = quantityElement.val() ? parseInt(quantityElement.val()) + 1 : 1;
        if (quantity > parseInt($(quantityElement).attr("max"))) {
            quantity = $(quantityElement).attr("max");
        }
        quantityElement.val(quantity);
        quantityElement.trigger("change");
    });
    //input decrement number. Order quantity decrement
    $(document).on("click", ".decrease", function (e) {
        //Get quantity value
        let quantityElement = $(this).closest(".multivendor-quantity").find(".add-qty");
        let quantity = quantityElement.val() ? parseInt(quantityElement.val()) - 1 : 1;
        if (quantity < parseInt($(quantityElement).attr("min"))) {
            quantity = $(quantityElement).attr("min");
        }
        quantityElement.val(quantity)
        quantityElement.trigger("change");
    });
}(window.digitalHalalMarket = window.digitalHalalMarket || {}, window.multivendorWeb = window.multivendorWeb || {}, window.jQuery, document));