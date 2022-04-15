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
            var $context = $(".order-cart-body-container");
            var currentUrl = window.location.href;
            var urlTab = location.hash;
            if (urlTab != undefined && urlTab.length > 0) {
                tab = urlTab
                //$context.attr("data-tab", urlTab);
            }
            else {
                tab = $context.attr("data-tab");
            }
           
            return tab;
        },
        LoadOrderCostView: function () {
            var $context = $(document).find(".order-cart-body-container");
            var orderCostView = $context.find(".order-cost-container")
            let actionUrl = $context.data("order-cost-url");
            $.ajax({
                url: actionUrl,
                datatype: "html",
                type: "GET",
                cache: false,
                success: function (data) {
                    var html = $(data)
                    orderCostView.empty();
                    orderCostView.append(html)
                    if (orderCostView != undefined && orderCostView.length > 0) {
                        var tab = multivendorWeb.Order.getCurrentTab();
                        var previousButton = orderCostView.find(".previous-order-page")
                        var nextButton = orderCostView.find(".next-order-page")
                        switch (tab) {
                            case "#tab=cart":
                                if (!previousButton.hasClass("hidden")) previousButton.addClass("hidden")
                                if (nextButton.hasClass("hidden")) nextButton.removeClass("hidden")
                                break;
                            case "#tab=address":
                                if (previousButton.hasClass("hidden")) previousButton.removeClass("hidden")
                                if (nextButton.hasClass("hidden")) nextButton.removeClass("hidden")
                                break;
                            case "#tab=payment":
                                if (previousButton.hasClass("hidden")) previousButton.removeClass("hidden")
                                if (!nextButton.hasClass("hidden")) nextButton.addClass("hidden")
                                break;
                        }
                    }

                }

            })

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
            var customerData = null
            var selectedDeliveryMethod = null
            /*call to the controller function */
            let actionUrl = $context.data("order-address-url");
            $.ajax({
                url: actionUrl,
                datatype: "html",
                type: "GET",
                cache: false,
                success: function (data) {
                    var html = $(data)
                    orderAddressView.empty();
                    orderAddressView.append(html)
                }

            })
        },
        SaveShippingAddress: function () {
            var $context = $(document).find(".order-cart-body-container");
            var orderAddressView = $context.find(".order-shipping-billing-container")
            /*call to the controller function */
            let actionUrl = $context.data("order-address-url");
           
            var selectedDeliveryMethod = $("input[name='deliverymethod']:checked").val();
            var information = {
                "FirstName": $(".first-name-input").val(),
                "LastName": $(".last-name-input").val(),
                "CareOf": $(".care-of-input").val(),
                "Email": $(".customer-email-input").val(),
                "PhoneNumber": $(".customer-phone-input").val(),
                "Address": $(".address-input").val(),
                "PostCode": $(".post-code-input").val(),
                "City": $(".city-input").val(),
                "Country": $(".country-input").val(),
            }
            $.ajax({
                url: actionUrl,
                datatype: "json",
                type: "POST",
                cache: false,
                data
                success: function (data) {
                    var html = $(data)
                    orderAddressView.empty();
                    orderAddressView.append(html)
                }

            })
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
            let actionUrl = $context.data("order-payment-url");
            $.ajax({
                url: actionUrl,
                datatype: "html",
                type: "GET",
                cache: false,
                success: function (data) {
                    var html = $(data)
                    orderPaymentView.empty();
                    orderPaymentView.append(html)
                }

            })
        },
        LoadCartView: function () {
            var $context = $(document).find(".order-cart-body-container");
          
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

            let actionUrl = $context.data("order-cart-url");
            $.ajax({
                url: actionUrl,
                datatype: "html",
                type: "GET",
                cache: false,
                success: function (data) {
                    var html = $(data)
                    orderCartView.empty();
                    orderCartView.append(html)
                }

            })
        },
        UpdateCurrentTab: function (tab) {
            var $context = $(".order-cart-body-container");

            $context.attr("data-tab", tab);
            multivendorWeb.Order.LoadOrderCostView();

            switch (tab) {
                case "#tab=cart":
                    location.hash = "#tab=cart"
                    //chacke
                    multivendorWeb.Order.LoadCartView();
                    break;
                case "#tab=address":
                    location.hash = "#tab=address"
                    multivendorWeb.Order.LoadOrderAddressView();
                    break;
                case "#tab=payment":
                    location.hash = "#tab=payment"
                    multivendorWeb.Order.LoadpaymentView();
                    break;
            }

        }
    };
    $(document).on("click", ".next-order-page", function (e) {
        var orderCart = $(document).find(".order-cart-body-container");
        var orderprocess = orderCart.data("order-process").split(',')
        var tab = multivendorWeb.Order.getCurrentTab();
        //find next tab
        var index = orderprocess.findIndex(x => x == tab);
        if (index + 1 <= orderprocess.length - 1) {
            multivendorWeb.Order.UpdateCurrentTab(orderprocess[index + 1]);
        }

    });
    $(document).on("click", ".previous-order-page", function (e) {
        var orderCart = $(document).find(".order-cart-body-container");
        var orderprocess = orderCart.data("order-process").split(',')
        var tab = multivendorWeb.Order.getCurrentTab();
        //find next tab
        var index = orderprocess.findIndex(x => x == tab);
        if (index - 1 >= 0) {
            multivendorWeb.Order.UpdateCurrentTab(orderprocess[index - 1]);
        }

    });
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