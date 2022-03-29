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
            return Number(percentStr.slice(0, -1));
        },

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
    $(document).ready(function () {
        multivendorWeb.Order.GetTotalCounter();
    });
}(window.digitalHalalMarket = window.digitalHalalMarket || {}, window.multivendorWeb = window.multivendorWeb || {}, window.jQuery, document));