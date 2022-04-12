(function (digitalHalalMarket, multivendorWeb, $, document) {
    multivendorWeb.Order = {
        GetTotalCounter: function () {
            var quantityUrl = $(document.body).data("order-total-counter");
            $.ajax({
                url: quantityUrl,
                dataType: "json",
                //contentType: 'application/json',
                type: "POST",
                cache: false,
                success: function (data) {
                    var $data = data
                    var totalQuantityHtml = $(document).find(".order-cart-container");
                    if ($data.status == true) {
                        totalQuantityHtml.find(".total-qty").text($data.totalCount)
                    }
                    else {
                        totalQuantityHtml.find(".total-qty").text("")
                    }

                }, error: function (ex) {

                }
            });
        /*end call */
        },
        getCurrentTab: function () {
            var tab = "";
            var currentUrl = window.location.href;
            var urlTab = location.hash;
            if (urlTab != undefined && urlTab.length > 0) {
                tab = urlTab
            }
            else {
                var $context = $(".order-cart-body-container");
                tab = $context.attr("data-tab");
            }
           
            return tab;
        },
        LoadOrderCostView: function () {


        },
        LoadOrderAddressView: function () {
            var $context = $(document).find(".order-cart-body-container");
            
            var orderCostView = $context.find(".order-cost-container")
            var orderCartView = $context.find(".order-items-container")
            if (!orderCartView.hasClass("hidden")) orderCartView.addClass("hidden")
            var orderAddressView = $context.find(".order-shipping-billing-container")
            if (orderAddressView.hasClass("hidden")) orderAddressView.removeClass("hidden")
            var orderPaymentView = $context.find(".order-payment-container")
            if (!orderPaymentView.hasClass("hidden")) orderPaymentView.addClass("hidden")
            //Get the tab selected
            var tabMenu = $(".order-cart-ul");
            tabMenu.children().removeClass("selected")
            tabMenu.find(".additional").addClass("selected")

            /*call to the controller function */
            //let actionUrl = $context.data("order-address-url");
            //$.ajax({
            //    url: actionUrl,
            //    datatype: "html",
            //    type: "GET",
            //    cache: false,
            //    success: function (data) {
            //        var html = $(data)
            //        orderAddressView.empty();
            //        orderAddressView.append(html)
            //        //multivendorWeb.PopUp.Show(html, userMenuhtml, null)
            //    }

            //})
        },
        LoadpaymentView: function () {
            var $context = $(document).find(".order-cart-body-container");

            var orderCostView = $context.find(".order-cost-container")
            var orderCartView = $context.find(".order-items-container")
            if (!orderCartView.hasClass("hidden")) orderCartView.addClass("hidden")
            var orderAddressView = $context.find(".order-shipping-billing-container")
            if (!orderAddressView.hasClass("hidden")) orderAddressView.addClass("hidden")
            var orderPaymentView = $context.find(".order-payment-container")
            if (orderPaymentView.hasClass("hidden")) orderPaymentView.removeClass("hidden")
            //Get the tab selected
            var tabMenu = $(".order-cart-ul");
            tabMenu.children().removeClass("selected")
            tabMenu.find(".order-cart-payment").addClass("selected")

            /*call to the controller function */
            //let actionUrl = $context.data("order-address-url");
            //$.ajax({
            //    url: actionUrl,
            //    datatype: "html",
            //    type: "GET",
            //    cache: false,
            //    success: function (data) {
            //        var html = $(data)
            //        orderAddressView.empty();
            //        orderAddressView.append(html)
            //        //multivendorWeb.PopUp.Show(html, userMenuhtml, null)
            //    }

            //})
        },
        LoadCartView: function () {
            var $context = $(document).find(".order-cart-body-container");

            var orderCostView = $context.find(".order-cost-container")
            var orderCartView = $context.find(".order-items-container")
            if (orderCartView.hasClass("hidden")) orderCartView.removeClass("hidden")
            var orderAddressView = $context.find(".order-shipping-billing-container")
            if (!orderAddressView.hasClass("hidden")) orderAddressView.addClass("hidden")
            var orderPaymentView = $context.find(".order-payment-container")
            if (!orderPaymentView.hasClass("hidden")) orderPaymentView.addClass("hidden")
            //Get the tab selected
            var tabMenu = $(".order-cart-ul");
            tabMenu.children().removeClass("selected")
            tabMenu.find(".shoppingcart").addClass("selected")
            
        },
        UpdateCurrentTab: function (tab) {
            var $context = $(".order-cart-body-container");
            $context.attr("data-tab", tab);
            switch (tab) {
                case "#tab=cart":
                    multivendorWeb.Order.LoadCartView();
                    break;
                case "#tab=address":
                    multivendorWeb.Order.LoadOrderAddressView();
                    break;
                case "#tab=payment":
                    multivendorWeb.Order.LoadpaymentView();
                    break;
            }
        }
    };
    $(document).on("click", ".add-to-order-button", function (e) {
        var addToOrder = $(this).closest(".add-to-order");
        var orderId = addToOrder.data("product-id");
        var quantity = addToOrder.find(".add-qty").val()
        /*call to the controller function */
        let actionUrl = addToOrder.data("add-to-order-url");
        $.ajax({
            url: actionUrl,
            data: { productId: orderId, quantity: quantity /*, page: 0, pageSize: pagesize */ },
            dataType: "json",
           // contentType: 'application/json',
            type: "POST",
            cache: false,
            success: function (data) {
                var $data = data
                if ($data.status == true) {
                    multivendorWeb.Order.GetTotalCounter();
                }

            }, error: function (ex) {

            }
        });
        /*end call */
    });
    $(document).on("change", ".order-cart-quantity", function (e) {

        var orderCart = $(document).find(".order-cart-body-container");
        var orderId = $(this).closest(".order-table-item").data("product-id");
        var quantity = $(this).val()
        /*call to the controller function */
        let actionUrl = orderCart.data("add-to-order-url");
        $.ajax({
            url: actionUrl,
            data: { productId: orderId, quantity: quantity /*, page: 0, pageSize: pagesize */ },
            dataType: "json",
            // contentType: 'application/json',
            type: "POST",
            cache: false,
            success: function (data) {
                var $data = data
                if ($data.status == true) {
                    //multivendorWeb.Order.GetTotalCounter();
                    window.location.reload();
                }

            }, error: function (ex) {

            }
        });
        /*end call */
    });
    $(document).on("click", ".delete-order", function (e) {

        var orderCart = $(document).find(".order-cart-body-container");
        var orderId = $(this).closest(".order-table-item").data("product-id");
        var quantity = 0
        /*call to the controller function */
        let actionUrl = orderCart.data("add-to-order-url");
        $.ajax({
            url: actionUrl,
            data: { productId: orderId, quantity: quantity /*, page: 0, pageSize: pagesize */ },
            dataType: "json",
            // contentType: 'application/json',
            type: "POST",
            cache: false,
            success: function (data) {
                var $data = data
                if ($data.status == true) {
                    //multivendorWeb.Order.GetTotalCounter();
                    window.location.reload();
                }

            }, error: function (ex) {

            }
        });
        /*end call */
    });
    $(document).on("click", ".clear-order", function (e) {
        var orderCart = $(document).find(".order-cart-body-container");
        /*call to the controller function */
        let actionUrl = orderCart.data("delete-all-order-url");
        $.ajax({
            url: actionUrl,
            dataType: "json",
            type: "POST",
            cache: false,
            success: function (data) {
                var $data = data
                if ($data.status == true) {
                    window.location.reload();
                }

            }, error: function (ex) {

            }
        });
        /*end call */
    });


    $(document).ready(function () {
        multivendorWeb.Order.GetTotalCounter();
        var tab = multivendorWeb.Order.getCurrentTab();
        multivendorWeb.Order.UpdateCurrentTab(tab);
        window.addEventListener('hashchange', function () {
            //update hash
            var currentHash = multivendorWeb.Order.getCurrentTab();
            multivendorWeb.Order.UpdateCurrentTab(currentHash);
        }, false);
    });
}(window.digitalHalalMarket = window.digitalHalalMarket || {}, window.multivendorWeb = window.multivendorWeb || {}, window.jQuery, document));