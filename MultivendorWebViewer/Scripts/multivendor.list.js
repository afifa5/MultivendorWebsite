(function (multivendorWeb, $, undefined) {

    multivendorWeb.list = multivendorWeb.helpers.createListBase("multivendor-list", "li");

    multivendorWeb.helpers.setupInit(multivendorWeb.list);

    multivendorWeb.menu = multivendorWeb.helpers.createListBase("multivendor-menu", "li");

    multivendorWeb.helpers.setupInit(multivendorWeb.menu);

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));