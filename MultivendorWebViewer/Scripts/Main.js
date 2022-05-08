(function (digitalHalalMarket, multivendorWeb, $, document) {
    multivendorWeb.Search = {
        SearchProduct: function (searchTerm) {
            /*call to the controller function */

            let actionUrl = $(document.body).data("quick-search-url");
            $.ajax({
                url: actionUrl,
                data: { term: searchTerm, displayCount : 20 },
                dataType: "html",
                type: "GET",
                cache: false,
                success: function (data) {
                    setTimeout(function () {
                        $("div").remove(".search-spinner");
                    }, 500);
                    $html = $(data);
                    multivendorWeb.Search.SearchOpen($html)

                }, error: function (ex) {

                }
            });
        },
        SearchOpen: function ($html) {
            var mainToolbarHeight = $(document.body).find(".main-toolbar");
            var mainSearchPane = $(document.body).find(".main-search-pane");
            mainSearchPane.empty()
            mainSearchPane.append($html)
            mainSearchPane.addClass("open")
            mainSearchPane.css("top", mainToolbarHeight.outerHeight())
            var $body = $(document.body);
            $body.off("click", quickSearchPanelGlobalClick);
            $body.on("click", quickSearchPanelGlobalClick);

            $body.off("keydown", quickSearchPanelGlobalKeydown);
            $body.on("keydown", quickSearchPanelGlobalKeydown);
        },
        close: function (panel, keepInputOpen) {
            keepInputOpen = keepInputOpen || false;
            if (!panel) panel = $(document).findByClass("main-search-pane");
            if (panel) {
                panel.removeClass("open");
                //sign.core.airspaceNeeded(panel, false);
                //$('.search-backdrop').remove();
                var $body = $(document.body);
                $body.off("click", quickSearchPanelGlobalClick);
                $body.off("keydown", quickSearchPanelGlobalKeydown);
                //layout.removeClass("search-dropdown-maximized");
            }
        },
    };
    $(document).on("input propertychange keyup", ".search-input", function (e) {
        var inputTerm = $(this).val();
        var mainSearchPane = $(document.body).find(".main-search-pane");
        mainSearchPane.removeClass("open")
        if (inputTerm != "" && inputTerm.length > 3) {
            $("<div class=\"search-spinner\"></div>").insertAfter($(".quick-search-input input[type=search]"));
            multivendorWeb.Search.SearchProduct(inputTerm);
        }
        else {
            mainSearchPane.empty()
        }
    });
    $(document).on("focus", ".search-input", function (e) {
        var searchResult = $(document.body).find(".search-result");
        if (searchResult.length > 0) {
            multivendorWeb.Search.SearchOpen(searchResult)
        }
        else {
            var inputTerm = $(this).val();
            var mainSearchPane = $(document.body).find(".main-search-pane");
            mainSearchPane.removeClass("open")
            if (inputTerm != "" && inputTerm.length > 3) {
                $("<div class=\"search-spinner\"></div>").insertAfter($(".quick-search-input input[type=search]"));
                multivendorWeb.Search.SearchProduct(inputTerm);
            }
            else {
                mainSearchPane.empty()
            }
        }
    });
    $(document).on("mouseleave", ".advance-search-container", function (e) {
        if (!$(this).hasClass("pinned"))
        $(this).removeClass("open")
    });
    $(document).on("click", ".pin-icon-container", function (e) {
        var searchContainer = $(this).closest(".advance-search-container")
        if (!searchContainer.hasClass("pinned"))
            searchContainer.addClass("pinned")
        else
            searchContainer.removeClass("pinned")
    });
    $(document).on("mouseenter", ".advance-search-container", function (e) {
        $(this).addClass("open")
    });
    
    $(document).on("focus", ".search-input", function (e) {
        var searchResult = $(document.body).find(".search-result");
        if (searchResult.length > 0) {
            multivendorWeb.Search.SearchOpen(searchResult)
        }
        else {
            var inputTerm = $(this).val();
            var mainSearchPane = $(document.body).find(".main-search-pane");
            mainSearchPane.removeClass("open")
            if (inputTerm != "" && inputTerm.length > 3) {
                $("<div class=\"search-spinner\"></div>").insertAfter($(".quick-search-input input[type=search]"));
                multivendorWeb.Search.SearchProduct(inputTerm);
            }
            else {
                mainSearchPane.empty()
            }
        }
    });

    function quickSearchPanelGlobalClick(e) {
        var $document = $(document);
        var panel = $document.find(".main-search-pane");
        if ($.contains(panel[0], e.target) == false && $.contains($document.find(".quick-search-input")[0], e.target) == false) {
            multivendorWeb.Search.close(panel);
            $(document.body).off(e);
        }
    };
    function quickSearchPanelGlobalKeydown(e) {
        if (e.keyCode == 27) {
            e.preventDefault();
            var $document = $(document);
            var panel = $document.find(".main-search-pane");
            multivendorWeb.Search.close(panel); // close on esc
        }
    };
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