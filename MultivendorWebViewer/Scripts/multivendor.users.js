(function (multivendorWeb, $, undefined) {
    multivendorWeb.user = {
    }

    $(function () {
        var $view = $(document).findByClass("user-list-data-view");
        if ($view.length > 0) {
            multivendorWeb.dataview.initViews($view);
        }
    });
}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));