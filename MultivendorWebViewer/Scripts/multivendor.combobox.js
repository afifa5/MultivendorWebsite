(function (digitalHalalMarket, multivendorWeb, $, document) {

    multivendorWeb.combobox = {

        getControls: function (ctx) {
            return $(multivendorWeb.helpers.findByClass("multivendor-combobox", ctx, true));
        },

        getText: function ($control) {
            return getText($control);
        },

        setText: function ($control, value) {
            return setText($control, value);
        },

        getValue: function ($control) {
            return getValue($control);
        },

        setValue: function ($control, value, updateSelection) {
            return setValue($control, value, updateSelection || true);
        },

        clear: function ($control) {
            clear($control);
        },

        getItems: function ($control) {
            return getItems($control);
        },

        getSelectedItems: function ($control) {
            return getSelectedItems($control);
        },

        focus: function ($control) {
            $control.children().eq(0).focus();
        },

        init: function ($controls) {
            if (!$controls.length) return;

            $controls.each(function () {
                var $control = $(this);
                var $childs = $control.children();
                var $input = $childs.eq(0);
                var $hiddenInput = $childs.eq(1);
                var $button = $childs.eq(2);
                var $itemsDropDown = $childs.eq(3);
                var $itemsContainer = $itemsDropDown.children().children(".items-container");

                var text = getText($control);
                if (!text.length) {
                    var val = getValue($control);
                    if (val.length) { // We have a value but no text, try take from item
                        var $item = $itemsContainer.find("li.selected");
                        if (!$item.length) { // If having a value but no item is selected, try select the item with the value. Not sure if this should be done by this component, the selection should have been setup correctly.
                            $item = $itemsContainer.find("li[data-value='" + val + "']").addClass("selected");
                        }
                        if ($item.length) {
                            text = getItemText($item);
                            setText($control, text);
                        }
                    }
                }
                if (text.length) {
                    $control.data("selected-text", text);
                    $control.addClass("contents");
                }

                var removeFilter = function () {
                    $itemsContainer.children("li").removeClass("nomatch");
                    $itemsDropDown.removeAttr("data-match-count");
                };

                var undo = function () {
                    var currentText = getText($control);
                    var lastSelectedText = $control.data("selected-text") || "";
                    if (lastSelectedText != currentText) {
                        setText($control, lastSelectedText);
                        if (!$control.hasClass("mobile")) {
                            closeDropDown($control, $itemsDropDown);
                        }
                        if ($control.hasClass("show-all")) {
                            removeFilter();
                        }
                    }
                };

                var selectItem = function ($item) {
                    var text = getItemText($item);
                    setText($control, text);
                    $control.data("selected-text", text);
                    if (!$control.hasClass("multi-select")) {
                        $item.parent().children("li").removeClass("selected");
                        if (!$item.hasClass("clear")) {
                            $item.addClass("selected");
                        }
                    }
                    setValue($control, $item.data("value"));
                    closeDropDown($control, $itemsDropDown);
                    if ($control.hasClass("show-all")) {
                        removeFilter();
                    }
                    $control.trigger("selectionChanged", { selection: getSelectedItems($control), selected: $item, unselected: $() });
                };

                $input.on("mousedown", function () {
                    if (multivendorWeb.helpers.isMobile() && $control.hasClass("show-all")) {
                        $input.prop("readonly", true);
                    }
                });

                $input.on("click", function () {
                    if ($control.data("filtering") == null) {
                        showDropDown($control, $itemsDropDown);
                    } else if ($control.hasClass("show-all")) {
                        removeFilter();
                        setText($control, "");
                        showDropDown($control, $itemsDropDown);
                    } else if ($control.hasClass("contents") || multivendorWeb.helpers.isMobile()) {
                        showDropDown($control, $itemsDropDown);
                    }
                });
             
                $input.on("change", function (e) {
                    var $mouseDownItem = $control.data("mousedown");
                    if ($mouseDownItem == null) {
                        var val = getText($control);
                        if (!val.length) {
                            setValue($control, "");
                            if (!$control.hasClass("required")) {
                                return;
                            }
                        }

                        if ($control.hasClass("allow-new")) {
                            setValue($control, val);
                        } else {
                            if ($itemsDropDown.data("match-count") > 0) {
                                var $matchingItem = getMatchingItems($itemsContainer.children("li:not(.nomatch)"), val);
                                if ($matchingItem.length) {
                                    selectItem($matchingItem);
                                } else {
                                    undo();
                                }
                            } else {
                                undo();
                            }
                        }
                    }

                });

                $input.on("input", function (e) {
                    var val = getText($control);
                    $control.toggleClass("contents", val.length > 0);

                    if (multivendorWeb.helpers.isIE() == true && $input.is(":focus") == false) return;
                    
                    if (val.length) {
                        var filteringMode = $control.data("filtering");
                        if (filteringMode) {
                            if (filteringMode == "client") {
                                var matches = 0, visible = 0;
                                var $items = $itemsContainer.children("li");

                                $items.each(function () {
                                    var $item = $(this);
                                    if (!$item.hasClass("visible")) {
                                        var value = $item.data("filter-value") || getItemText($item);
                                        var match = !value || !val || value.trim().toLowerCase().indexOf(val.trim().toLowerCase()) != -1;
                                        $item.toggleClass("nomatch", !match);
                                        if (match) matches++;
                                    } else {
                                        visible++;
                                    }
                                });
                                $itemsDropDown.attr("data-match-count", matches);
                                $itemsDropDown.toggleClass("visibles", visible > 0);

                                showDropDown($control, $itemsDropDown);
                                if (matches == 0) {
                                    // No matches
                                } else {

                                }
                            } else {
                                var url = $control.data("url");
                                $control.data("last-query", val);
                                $control.addClass("loading");

                                if (!$control.hasClass("show-all") && $itemsDropDown.attr("data-match-count") == 0) {
                                    if (!$control.hasClass("open")) {
                                        $itemsDropDown.removeAttr("data-match-count");
                                    }
                                    $itemsContainer.empty();
                                }
                                if (!$control.hasClass("init-loaded")) {
                                    showDropDown($control, $itemsDropDown);
                                }

                                multivendorWeb.helpers.getJson(url, { searchText: val }, function (data, status, xhr) {
                                    if (data.Query != $control.data("last-query")) return;
                                    var $html = $(data.Html);
                                    $itemsDropDown.attr("data-match-count", $html.filter("li:not(.clear)").length);
                                    $itemsContainer.htmlUpdate($html);
                                    $control.removeClass("loading");

                                    if (!$control.hasClass("init-loaded")) {
                                        updateDropDownBounds($control, $itemsDropDown);
                                        $control.addClass("init-loaded");
                                    } else {
                                        showDropDown($control, $itemsDropDown);
                                    }

                                });

                                //$itemsContainer.htmlFromUrl(url, { searchText: val }, function (data) {

                                //});
                            }
                        }
                    } else {
						$itemsDropDown.removeAttr("data-match-count");											  
                        //$itemsDropDown.attr("data-match-count", 0);
                        if (!$control.hasClass("mobile")) {
                            closeDropDown($control, $itemsDropDown);
                        }
                    }
                });

                var trySelectSingleItem = function () {
                    if ($control.hasClass("allow-new")) {
                        var $items = getMatchedItems($control);
                        if ($items.length === 1) {
                            var itemValue = getItemValue($items);
                            setValue($control, itemValue); // FIND IN LIST IF POSSIBLE?
                            text = getItemText($items);
                            setText($control, text);
                        }
                    }
                };

                $input.on("keydown", function (e) {
                    if (e.keyCode === 9) { // tab
                        var text = getText($control);
                        if (!text.length) {
                            $control.data("selected-text", "");
                        }
                        trySelectSingleItem();
                        closeDropDown($control, $itemsDropDown);
                    }

                    if (e.keyCode === 13) {

                        var isEnterKeyDisabled = $control.data("enter-key-disabled");
                        if (isEnterKeyDisabled === true) {
                            return false;
                        }
                    }
                });

                $input.on("keyup", function (e) {

                    if (e.keyCode === 13) { // enter
                        //if ($control.hasClass("allow-new") == true) {

                        var isEnterKeyDisabled = $control.data("enter-key-disabled");
                        if (isEnterKeyDisabled === true) {
                            return false;
                        }

                        closeDropDown($control, $itemsDropDown);

                        //var text = getText($control);
                        //var value = getValue($control);

                        trySelectSingleItem();

                        //var $matchingItems = getItemsByValue($items, text).first().addClass("selected");
                        //if ($matchingItems.length) {
                        //    text = getItemText($matchingItems);
                        //    setText($control, text);
                        //} else {
                        //    $matchingItems = getMatchingItems()
                        //}
                        //}
                        //return false;
                    } else if (e.keyCode === 27) { // esc
                        undo();
                    } /*else if (e.keyCode === 9) {

                    }*/
                    //if (matches == 1) {
                    //    $hiddenInput.val($items.find(":not(.nomatch)").val();
                    //}
                });

                //$input.on("blur", function () {
                //});

                $input.on("focusin", function (e) {
                    $control.addClass("input");

                    //if (multivendorWeb.helpers.isMobile() && !$control.hasClass("show-all")) {
                    //    $control.get(0).scrollIntoView({ behavior: "smooth", block: "start" });
                    //}
                });

                //$(document).on("focusout", function (e) {
                //    setTimeout(function () {
                //        //var $focused = $(document.activeElement); 
                //        if (!$control.find(':focus') && !$control.is(":focus")) {
                //            closeDropDown($control, $itemsDropDown);
                //        }
                //    }, 1);
                //});

                $input.on("focusout", function (e) {
                    $control.removeClass("input");
                    var $mouseDownItem = $control.data("mousedown");
                    if ($mouseDownItem == null && !$control.hasClass("allow-new")) {
                        undo();
                        //closeDropDown($control, $itemsDropDown);
                    }
                });
            
                $button.on("mousedown", function () {
                    var action;
                    if ($control.hasClass("input") && getText($control).length && !$control.hasClass("required") == true) {
                        action = "clear";
                    } else if ($control.hasClass("open")) {
                        action = "close";
                    } else if ($control.hasClass("show-all")) {
                        action = "open";
                    } else if ($control.data("filtering") == null) {
                        action = "open";
                    }
                    $button.data("action", action);
                });

                $button.on("click", function (e) {
                    var action = $button.data("action");
                    if (action === "clear") {
                        setText($control, "");
                    } else if (action === "close") {
                        closeDropDown($control, $itemsDropDown);
                    } else {
                        showDropDown($control, $itemsDropDown);
                    }
                });

                $itemsContainer.on("mousedown", "> li", function (e) {
                    if (e.button == 0) {
                        //selectItem($(e.target));
                        $control.data("mousedown", $(this));
                    }
                });

                $itemsContainer.on("mouseup", "> li", function (e) {
                    if (e.button == 0) {
                        var $mouseDownItem = $control.data("mousedown");
                        if ($mouseDownItem) {
                            selectItem($mouseDownItem);
                            $control.removeData("mousedown");
                            $input.focus();
                        }
                    }
                });

                $itemsDropDown.on("click", function (e) {
                    if ($control.hasClass("mobile") == true && e.target == $itemsDropDown.get(0)) {
                        closeDropDown($control, $itemsDropDown);
                        e.preventDefault();
                        return;
                    } else {
                        $searchTextBox = $itemsDropDown.find("> div > .mobile-search > input").focus();
                    }
                });
            });
        }
    };

    function getItemText($item) {
        return $item.hasClass("clear") ? "" : $item.data("text") || $item.text().trim();
    }

    function getItemValue($item) {
        return $item.hasClass("clear") ? "" : $item.data("value") || getItemText("text");
    }

    function getText($control) {
        return $control != null ? $control.children().eq(0).val().trim() : "";
    }

    function setText($control, value) {
        $control.children().eq(0).val(value);
        $control.toggleClass("contents", value.length > 0);
    }

    function getValue($control) {
        return $control != null ? $control.children().eq(1).val().trim() : "";
    }

    function setValue($control, value, updateSelection) {
        console.log(value);
        var $valueInput = $control.children().eq(1);
        var currentValue = $valueInput.val();
        if (currentValue != value) {
            $valueInput.val(value);

            // Update selection
            if (updateSelection == true) {
                var $items = getItems($control);
                $items.removeClass("selected");
                var $matchingItems = getItemsByValue($items, value).first().addClass("selected");
                if ($matchingItems.length) {
                    setText($control, getItemText($matchingItems));
                }
            }

            $control.prop("value", value);
            $control.trigger("change");
        }
    }

    function clear($control) {
        setText($control, "");
        setValue($control, "");
    }

    function getItems($control) {
        var $itemsContainer = $control.children().eq(3).children().children(".items-container");
        return $itemsContainer.children("li");
    }

    function getSelectedItems($control) {
        return getItems($control).filter(".selected");
    }

    function getMatchedItems($control) {
        return getItems($control).filter(":not(.nomatch)");
    }

    function getMatchingItems($items, val) {
        var $matchingItem = $items.filter(function () {
            var $item = $(this);
            var value = $item.data("filter-value") || getItemText($item);
            var match = value && val && value.trim().toLowerCase() == val.trim().toLowerCase();
            return match;
        });
        return $matchingItem;
    }

    function getItemsByValue($items, val) {
        var $matchingItem = $items.filter(function () {
            var $item = $(this);
            var value = getItemValue($item);
            var match = value == val;
            return match;
        });
        return $matchingItem;
    }

    function showDropDown($control, $itemsDropDown) {

        var isMobile = multivendorWeb.helpers.isMobile();

        $control.addClass("opening");
        updateDropDownBounds($control, $itemsDropDown);
        var $searchTextBox = $itemsDropDown.find("> div > .mobile-search > input");
        if (isMobile) {
            $searchTextBox.val(getText($control));
        }
        $control.addClass("open").removeClass("opening");
        //$itemsDropDown.css("display", "block");

        if (!isMobile) {
            var closeEvent = function () {
                closeDropDown($control, $itemsDropDown);
            };
            $itemsDropDown.data("close-event", closeEvent);
            $(window).one("resize scroll", closeEvent);
            var clickEvent = function (e) {
                var $clicked = $(e.target);
                var $comboBox = $clicked.closest(".multivendor-combobox");
                if ($comboBox.get(0) != $control.get(0)) {
                    closeDropDown($control, $itemsDropDown);
                    $(document).off("click", clickEvent);
                }
            };
            $(document).on("click", clickEvent);
        } else {
            //setTimeout(function () { $control.prop("readonly", true); }, 10);
            setTimeout(function () { $searchTextBox.focus(); }, 50);
        }
    }

    function closeDropDown($control, $itemsDropDown) {
        $control.removeClass("open").removeClass("mobile");
        var closeEvent = $itemsDropDown.data("close-event");
        $(window).off("resize scroll", closeEvent);
        if ($control.data("filtering") != null) {
            $control.children().eq(0).prop("readonly", false);
        }
    }

    function updateDropDownBounds($control, $itemsDropDown) {
        var isMobile = /*$control.hasClass("show-all") &&*/ multivendorWeb.helpers.isMobile();
        var $dropDownInner = $itemsDropDown.children();
        var $itemsContainer = $dropDownInner.children(".items-container");

        if (isMobile == false) {
            var viewportHeight = window.innerHeight;
            var viewportWidth = window.innerWidth;
            //var viewportScroll = $(document).scrollTop();

            $itemsContainer.css("max-height", "");
            $itemsDropDown.css({ "width": "", "top": "", "left": "", "max-height": "" });

            var itemsDropDownBounds = $itemsDropDown.bounds();
            var controlBounds = $control.bounds();

            var left = controlBounds.left;
            var right = left + itemsDropDownBounds.width;
            var width = Math.min(controlBounds.width, viewportWidth - 20);
            var top = controlBounds.bottom + 1;

            if (right >= viewportWidth - 10) {
                left = (viewportWidth - 10) - itemsDropDownBounds.width;
                $itemsDropDown.css({ "min-width": width + "px", "top": top + "px", "left": left + "px" });
                itemsDropDownBounds = $itemsDropDown.bounds();
            }

            //var itemsContainerBounds = $itemsContainer.bounds();
            
            top = controlBounds.bottom + 1;
            width = controlBounds.width;// Math.max(controlBounds.width, itemsContainerBounds.width);
            var height;


            var bottom = top + itemsDropDownBounds.height;
            if (bottom >= viewportHeight) { // Out of bounds

                var availableHeightAbove = controlBounds.top - 11;
                var availableHeightBelow = (viewportHeight - controlBounds.bottom) - 11;
                
                // put on top or bellow (or side?)
                if (availableHeightBelow >= availableHeightAbove) {
                    height = Math.min(availableHeightBelow, itemsDropDownBounds.height);
                } else {
                    height = Math.min(availableHeightAbove, itemsDropDownBounds.height);
                    top = controlBounds.top - height - 3;
                }
                //top = viewportHeight - itemsDropDownBounds.height;
            }

            $itemsDropDown.css({ "min-width": width + "px", "top": top + "px", "left": left + "px" });
            $itemsContainer.css("max-height", height != null ? height + "px" : "");
        } else {
            //var $placeholder = $dropDownInner.children(".placeholder");
            //if (!$placeholder.length) {
            //    var placeholder = $control.children().eq(0).attr("placeholder") || "";
            //    $placeholder = $("<h3 class='placeholder'>" + placeholder + "</h3>");
            //    $dropDownInner.prepend($placeholder);
            //}
            intiDropdownForMobile($control, $itemsDropDown);
            $itemsDropDown.css({ "width": "", "top": "", "left": "", "max-height": "" });
            $itemsContainer.css("max-height", "");
        }

        $control.toggleClass("mobile", isMobile);
    }

    function intiDropdownForMobile($control, $itemsDropDown) {
        if (!$itemsDropDown.hasClass("mobile-init")) {
            $itemsDropDown.addClass("mobile-init");

            var $dropDownInner = $itemsDropDown.children();

            var $mainSearchTextBox = $control.children().eq(0);
            //var $itemsContainer = $dropDownInner.children(".items-container");

            var $ok = $dropDownInner.children(".ok");
            $ok.on("click", function () {
                closeDropDown($control, $itemsDropDown);
                setValue($control, getText($control)); // FIND IN LIST IF POSSIBLE?
                //$control.trigger("selectionChanged");


                //$control.trigger("change");
                //$control.get(0).dispatchEvent(new Event('change'));
            });

            // Search text box (input and clear button)
            var $searchTextBox = $("<div class=\"mobile-search\"><input type=\"text\"/><i class=\"clear\"></i></div>");
            var $searchInput = $searchTextBox.children().eq(0);
            var $searchClear = $searchTextBox.children().eq(1);
            $dropDownInner.prepend($searchTextBox);

            $searchInput.on("input", function () {
                var text = $searchInput.val();
                setText($control, text);
                $mainSearchTextBox.get(0).dispatchEvent(new Event('input'));
            });

            $searchClear.on("click", function () {
                clear($control);
                $searchInput.val(getText($control));
                $mainSearchTextBox.get(0).dispatchEvent(new Event('input'));
            });
          
            // Add label
            var placeholder = $mainSearchTextBox.attr("placeholder") || " ";
            var $placeholder = $("<h3 class='placeholder'>" + placeholder + "</h3>");
            $dropDownInner.prepend($placeholder);

            var $close = $("<h3 class=\"close\"></h3>");
            $close.on("click", function () {
                closeDropDown($control, $itemsDropDown);
            });
            $dropDownInner.prepend($close);
        }
    }

    // Init controls

    $(function () {
        multivendorWeb.combobox.init(multivendorWeb.combobox.getControls());
    });

    $(document).htmlUpdated(function (e) {
        multivendorWeb.combobox.init(multivendorWeb.combobox.getControls(e.element));
    });

    //$(document).on("click", multivendorWeb.dropbox.selector + " ", function (event) {

    //});

}(window.digitalHalalMarket = window.digitalHalalMarket || {}, window.multivendorWeb = window.multivendorWeb || {}, window.jQuery, document));