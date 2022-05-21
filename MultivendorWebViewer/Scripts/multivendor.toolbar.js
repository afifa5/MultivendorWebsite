(function (multivendorWeb, $, undefined) {

    multivendorWeb.toolbar =
    {
        selector: ".multivendor-toolbar",

        getToolbar: function (item) {
            return item.closest(multivendorWeb.toolbar.selector);
        },

        getItems: function (toolbar, name) {
            return toolbar.children(name ? "li[data-name='" + name + "']" : "li");
        },

        setAvailibility: function (toolbar, name, available) {
            multivendorWeb.toolbar.getItems(toolbar, name).toggleClass("available", available);
        },

        showDropDown: function (item, options) {

            var dropDown = item.children(".multivendor-drop-down");

            options = $.extend(options || {}, dropDown.data("options"));

            if (options.direction == null) {
                options.direction = dropDown.data("dropdown-direction") || multivendorWeb.toolbar.getToolbar(item).data("dropdown-direction") || "up";
            }

            if (dropDown.length > 0 && options.html == null) {
                options.html = dropDown;
            }

            var opts = $.extend({ relativeElement: item, horizontalAlign: "center" }, options);
           

            if (opts.direction == "down") {
                opts.verticalAlign = opts.verticalAlign || "bottom";
                opts.top = opts.top || 5;
            } else {
                opts.verticalAlign = opts.verticalAlign || "top";
                opts.bottom = opts.top || 5;
            }

            if (opts.horizontalAlign == "right") {
                opts.right = 5;
            } else if (opts.verticalAlign == "left") {
                opts.left = 5;
            }
                
            multivendorWeb.popup.show(opts);
        }

    }

    $(document).on("click", multivendorWeb.toolbar.selector + " li:not(.not-clickable)", function (event) {
        var item = $(this);
        var name = item.data("name");
        var toolbar = item.closest(multivendorWeb.toolbar.selector);
        // If the item is checkable, toogle check and trigger check events
        if (item.hasClass("checkable") == true) {

            var checked = item.hasClass("checked");
            var type = item.data("check-type");
            if (type == "toggle") {
                item.toggleClass("checked")
            } else if (checked == false) {
                if (type == "radio") {
                    var group = item.data("check-group");
                    toolbar.children(group ? "li[data-check-group='" + group + "'].checked" : "li[data-check-type='radio'].checked").each(function () {
                        var other = $(this);
                        if (other != item) {
                            other.removeClass("checked");
                            toolbar.trigger("itemCheck", { toolbar: toolbar, item: other, name: other.data("name"), checked: false });
                            other.trigger("check", { checked: false });
                        }
                    });
                }

                item.addClass("checked");
            } else return;

            checked = item.hasClass("checked");
            toolbar.trigger("itemCheck", { toolbar: toolbar, item: item, name: name, checked: checked });
            item.trigger("check", { checked: checked });
        }

        // Trigger click event
        toolbar.trigger("itemClicked", { toolbar: toolbar, item: item, name: name, checked: item.hasClass("checked"), getItems: function () { return multivendorWeb.toolbar.getItems(toolbar); } });

        // If the item has a drop down, show it
        /*var dropdown = item.children(".multivendor-drop-down");
        if (dropdown.length > 0) {
            multivendorWeb.toolbar.showDropDown(item);
        }*/
    });

   
    $(document).on('click', multivendorWeb.toolbar.selector + ' .menu-icon', function (event) {
       
    });

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));