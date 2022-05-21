(function (multivendorWeb, $, undefined) {

    multivendorWeb.popup = {

        defaults: {
            //container: null,
            //relativeElement: null,
            modal: false,
            autoClose: false,
            closeOnClick: false,
            //verticalAlign: null,
            //horizontalAlign: null,
            //width: null,
            //height: null,
            fixed: true,
            //top: null,
            //right: null,
            //bottom: null,
            //left: null,
            //timeout: null,
            //fadeout: null,
            //classNames: null,
            //url: null,
            //urlData: null,
            urlMethod: "get",
            //popup: null,
            overlay: true
        },

        containerClass: "multivendor-popup-container",

        popupClass: "multivendor-popup",

        getPopups: function (context) {
            return $(multivendorWeb.helpers.findByClass(this.popupClass, context));
        },

        createContainer: function () {
            return $("<div></div>").addClass(this.containerClass);
        },

        getDefaultContainerOwner: function () {
            return $(document.body);
        },

        getDefaultContainer: function () {
            var owner = this.getDefaultContainerOwner();
            var container = owner.children("." + this.containerClass);
            if (container.length == 0) {
                container = this.createContainer();
                owner.append(container);
            }
            return container;
        },

        isContainer: function (element) {
            return element.hasClass(this.containerClass);
        },

        createPopup: function () {
            return $("<div><div></div></div>").addClass(this.popupClass);
        },

        init: function (popups) {
            init(popups);
        },

        show: function (html, options, callback) {

            var $html = $(html);

            if ($html.length == 0 && $.isPlainObject(html)) {
                callback = options;
                options = html;
            }

            var opts = $.extend({}, $html.data("popup-options"), options);
        
            var popup;
            var isHtmlPopup = $html.hasClass(multivendorWeb.popup.popupClass);
            if (isHtmlPopup) {
                popup = $html;
            }

            if (opts.url && (!isHtmlPopup || !popup.hasClass("loading"))) {
                if (isHtmlPopup) {
                    popup.addClass("loading");
                    popup.trigger("popuploading");
                };

                multivendorWeb.helpers.ajax(opts.url, opts.urlData, function (data) {
                    if (data && data.length > 0) {
                        var p = show(popup, data, opts).notifyUpdate();

                        if (opts.loadOnce == true) {
                            // do something
                        }

                        if (isHtmlPopup) {
                            popup.removeClass("loading");
                        }

                        if (callback) callback(p, data);

                        if (popup) {
                            popup.trigger("popuploaded");
                            var owner = popup.data("owner");
                            if (owner) {
                                owner.trigger("ownedpopuploaded", { owner: owner, popup: popup });
                            }
                        }
                    }
                }, { type: opts.urlMethod });
            } else if ($html.length > 0) {
                var p = show(popup, isHtmlPopup ? null : $html, opts);
                if (callback) callback(p, data);
                return p;
            }

            return $();
        },

        close: function (popup) {
            popup.trigger("popupclosing");

            //var closeHandler = popup.data("closeHandler");
            //if (closeHandler) {
            //    $(window).off("focusin resize blur keydown submit", closeHandler);
            //    $(document).off("mousedown click", closeHandler);
            //}

            var backdrop = popup.data("backdrop");
            if (backdrop) {
                backdrop.remove();
            }
            if (popup.data("remove")) {
                popup.remove();
            } else {
                popup.addClass("closed");
                var owner = popup.data("owner"); // Move back to owner
                if (owner) {
                    owner.append(popup);
                }
                //select drop down component
                if (popup.hasClass("select")) {
                    var dropdownInput = $(document).find(popup.data("popup-activation-class"));//.findByClass("multivendor-select-input");
                    //set owner
                    var owner = dropdownInput.closest(".multivendor-select");
                    if (owner) {
                        owner.append(popup);

                    }
                    if (dropdownInput) {
                            if (dropdownInput.hasClass("popout"))
                            dropdownInput.removeClass("popout");
                        dropdownInput.findByClass("multivendor-select-input").attr("placeholder", popup.data("default-select-Text"));
                        //unknownOption
                        var unknownOptionAllow = popup.findByClass("select-Item-list").data("allowunknownoption")
                        var selectedText = popup.attr("data-selected-text");
                        if (selectedText) {
                            dropdownInput.findByClass("multivendor-select-input").val(selectedText);
                        }
                        else if (unknownOptionAllow.toLowerCase() == "false") {
                            dropdownInput.findByClass("multivendor-select-input").val("");
                        }


                        var options = popup.findByClass("select-Item-list").children();
                        options.each(function () {
                            if ($(this).hasClass("hidden"))
                                $(this).removeClass("hidden");
                          });
                          }
                    //remove the backdrop
                    
                    
                }
            //end select dropdown component
            }

            popup.trigger("popupclosed");
        },

        showDialog: function (dialog, modal) {
            if (dialog != null) {
                var $dialog = $(dialog);
                var $popup = multivendorWeb.popup.show($dialog, { autoClose: false, modal: modal != false, verticalAlign: "center", horizontalAlign: "center" /*left: 0, right: 0*/ });
                $dialog.find(".ok-button").on("click", function () {
                    multivendorWeb.popup.close($popup);
                });
                return $popup;
            }
            return null;
        },

        getPopupsByOwner: function (owner, context) {
            if (owner.length == 0) return $();
            var domO = owner[0];
            return multivendorWeb.popup.getPopups(context).filter(function () {
                var popup = $(this);
                var o = popup.data("owner");
                if (o == null) {
                    o = popup.parent();
                }
                return o != null && o[0] == domO;
            });
        },
        updatePopUp: function (popup, options) {
            var opts = $.extend({}, multivendorWeb.popup.defaults, options);
            var relativeElement = $(opts.relativeElement);
            var container = $(opts.container);
            if (container.length == 0) {
                container = multivendorWeb.popup.getDefaultContainer();
            }
            var alignElement = relativeElement.length && !opts.modal ? relativeElement : container.parent();
            updateBounds(popup, opts, alignElement);

           
        }
    }

    function show(popup, html, options) {

        var $html = $(html);

        var opts = $.extend({}, multivendorWeb.popup.defaults, options);

        // get the container to use

        /*var container = $(opts.container);
        // no container specified, get the default container
        if (container.length == 0) {
            if (html instanceof jQuery && html.hasClass(multivendorWeb.popup.containerClass) == true) {
                // specified html is a container
                container = html;
                html = container.html();
            } else {
                container = multivendorWeb.popup.getDefaultContainerOwner();
            }
        }

        // we have a container specified, but it is not a container. reuse or add a container to the element
        if (multivendorWeb.popup.isContainer(container) == false) {
            var parent = container;
            container = parent.children("." + multivendorWeb.popup.containerClass);
            if (container.length == 0) {
                container = multivendorWeb.popup.createContainer();
                parent.append(container);
            }
        }*/

        var relativeElement = $(opts.relativeElement);
        var owner = null;

        //var popup = opts.popup;
        if (popup == null || popup.length == 0) {

            //if (html instanceof jQuery && html.hasClass(multivendorWeb.popup.popupClass)) {
            //    // html is a popup
            //    popup = html;

            //    var popupOpts = popup.data("popup-options");
            //    if (popupOpts != null) {
            //        opts = $.extend(opts, popupOpts);
            //    }

            //    if (popup.parent().hasClass(multivendorWeb.popup.containerClass) == false) {
            //        var owner = popup.parent();
            //        if (owner.length > 0) {

            //            // if no relative element is set, set it to the owner
            //            if (opts.relativeElement == null) {
            //                opts.relativeElement = owner;
            //            }

            //            popup.data("owner", owner);
            //        }
            //        container.append(popup); // add the popup to the container
            //    }
            //} else if (html == null) {
            //    // no html is specified, try find a popup in the container
            //    popup = container.children("." + multivendorWeb.popup.popupClass);
            //}

            //if (popup == null || popup.length == 0) {
                // create popup
                popup = multivendorWeb.popup.createPopup();
                popup.data("remove", true);
                popup.children().first().html($html);

                //container.append(popup); // add the popup to the container
            //}
        }
        else {
            // We have a popup, is the html already in the popup? If not add the html;
            if ($html.length && $html.closest(multivendorWeb.popup.popupClass) != popup) {
                var innerPopup = popup.children().first();
                if (innerPopup.length == 0) {
                    innerPopup = $("<div></div>");
                    popup.html(innerPopup);
                }
                innerPopup.html($html);
            }

            owner = popup.parent();
            if (owner.length) {
                if (relativeElement.length == 0) {
                    relativeElement = owner;
                }
                popup.data("owner", owner);
                owner.data("pop-up", popup);
            }
        }

        var container = $(opts.container);
        if (container.length == 0) {
            container = multivendorWeb.popup.getDefaultContainer();
        }

        popup.addClass(opts.classNames);
        popup.addClass("initializing");

        if (opts.modal) {
            var backdrop = $("<div class='popup-backdrop'/>");
            container.append(backdrop);
            popup.data("backdrop", backdrop);
            popup.addClass("modal");
        }
        if (opts.customClass)
        {
            popup.addClass(opts.customClass);
        }

        container.append(popup);

        popup.trigger("popupshowing");

        popup.removeClass("closed");

        var alignElement = relativeElement.length && !opts.modal? relativeElement : container.parent();

        // set the position and size, if specified, of the popup

        updateBounds(popup, opts, alignElement);

        if (opts.autoClose == false) {
            $(window).on("resize", function () {
                updateBounds(popup, opts, alignElement);
            });
            popup.on("resized", function () {
                updateBounds(popup, opts, alignElement);
            });
        }

        //$(window).scroll(function () {
        //    updateBounds(popup, opts, alignElement);
        //});

        /*popup.on("resized", function () {
            updateBounds(popup, opts, alignElement);
        });*/
        if (opts.modal != true || (opts.modal == true && opts.autoClose == true)) {
            
            var closeHandler = function (event) {
                var outsidePopup = $(event.target).closest(container).length == 0;
                if (outsidePopup == false) {
                    var bounds = $(event.target).bounds();
                    //var css = window.getComputedStyle(event.target, ":before");

                    if (event.offsetX < 0 || event.offsetY < 0 || event.offsetX > bounds.width || event.offsetY > bounds.height) {
                        outsidePopup = true;
                    }
                }
                var type = event.type;
                if (((type == "mousedown" || type == "focusin" || type == "DOMMouseScroll" || type == "mousewheel") && outsidePopup == true)
                    || (type == "keydown" && event.keyCode == 27)
                    || (type == "blur" && event.target == window)
                    || (type == "resize")) {
                    $(window).off(windowHandlers, closeHandler);
                    $(document).off(documentHandlers, closeHandler);
                    multivendorWeb.popup.close(popup);

                } else if (outsidePopup == false) {
                    var srcElement = $(event.srcElement);
                    var closerAction = srcElement.data("popup-closer");
                    if ((type == "click" && (opts.closeOnClick == true || closerAction == "click"))
                        || (type == "submit" && closerAction == "submit")) {
                        if (!$(event.target).hasClass("multivendor-select-input"))
                        multivendorWeb.popup.close(popup);
                        //$(window).off(closeHandler);
                        //$(document).off(closeHandler);
                    }
                }
            }

            // TODO! THIS LOGIC SEEMS THAT IT BELONGS THE THE SELECT, NOT THIS GENERIC POPUP
            //select drop down component
            //if (popup.hasClass("select")) {
            //    var dropdownInput = $(document).find(popup.data("popup-activation-class"));//.findByClass("multivendor-select-input");
            //    if (dropdownInput) {
            //        popup.css("min-width", dropdownInput.outerWidth());
            //        if (!dropdownInput.hasClass("popout"))
            //            dropdownInput.addClass("popout");
            //        dropdownInput.findByClass("multivendor-select-input").attr("placeholder", "");
            //        dropdownInput.findByClass("multivendor-select-input").val("");
            //        //calculate overflow
            //        var options = popup.findByClass("select-Item-list").children();
            //        if (popup.data("max-item-count") > 0 && popup.data("max-item-count") < options.length) {
            //            var height = 0;
            //            var i = 0;
            //            options.each(function () {

            //                if (i >= popup.data("max-item-count"))
            //                    return false;
            //                else
            //                    height += $(this).outerHeight();
            //                i += 1;
            //            });
            //            //for (i = 0; i < popup.data("max-tem-count"); i++) {
            //            //    var item = options[i].offset();
            //            //    height += $(item).outerHeight();
            //            //}
            //            var windowHeight = $(window).height();
            //            var optionTop = popup.offset().top;
            //            var dropdownHeight = windowHeight - optionTop;
            //            if (dropdownHeight > 130)
            //            popup.findByClass("select-Item-list").css("height", (height < dropdownHeight ? height : dropdownHeight) +"px");
            //        }
            //        //endCalculateOverflow
            //    }
            //    $('body').on('mousewheel DOMMouseScroll', closeHandler);

            //}
            //end select dropdown component

            //popup.data("closeHandler", closeHandler);

            var windowHandlers = "keydown submit";
            var documentHandlers = "mousedown click";
            if (opts.autoClose == true) {
                windowHandlers = windowHandlers + " resize focusin blur";
            }

            $(window).on(windowHandlers, closeHandler);
            $(document).on(documentHandlers, closeHandler);

            if (opts.timeout > 0) {
                setTimeout(function () {
                    var fadeout = opts.fadeout;
                    if (fadeout == true) {
                        fadeout = 500;
                    }

                    if (fadeout > 0) {
                        popup.fadeOut(fadeout, function () { multivendorWeb.popup.close($(this)); });
                    } else {
                        multivendorWeb.popup.close(popup);
                    }
                }, opts.timeout);
            }

        }

        popup.removeClass("initializing");

        popup.trigger("popupshown");
       
        if (owner) {
            owner.trigger("ownedpopupshown", { owner: owner, popup: popup });
        }

        return popup;
    }
    function updateBounds(popup, opts, alignElement) {

        if (opts.width) {
            popup.width(opts.width);
        }

        if (opts.height) {
            popup.height(opts.height);
        }

        var fixed = opts.fixed;
        var left = opts.left;
        var right = opts.right;
        var top = opts.top;
        var bottom = opts.bottom;

        var hPositionSet = right != null || left != null;
        var vPositionSet = top != null || bottom != null;
        var hAlignSet = opts.horizontalAlign != null;
        var vAlignSet = opts.verticalAlign != null;

        if (hPositionSet || vPositionSet || hAlignSet || vAlignSet) {
            popup.css("position", fixed ? "fixed" : "absolute");

            var alignOffset = alignElement.offset();

            /* Remake! No business structure dependencies here! */
            var mainlayout = $(".main-layout").children(".detachables.float");
            if (mainlayout != undefined && mainlayout.length > 0) {
                alignOffset.top = alignOffset.top - $(window).scrollTop();
            }

            var alignWidth = alignElement.outerWidth();
            var alignHeight = alignElement.outerHeight();  
            var alignLeft = fixed ? alignOffset.left : 0;
            var alignTop = fixed ? alignOffset.top : 0;
            var alignRight = fixed ? alignOffset.left + alignWidth : alignWidth;
            //new layout strategy
         
            //end new layout strategy
            var alignBottom = fixed ? alignOffset.top + alignHeight : alignHeight;

            //Check if we have place to show full popup size in the bottom
            var docHeight = $(window).outerHeight();
            var totalHeight = popup.outerHeight() + alignBottom;
            if (totalHeight > docHeight) {
                top = null;
                bottom = 5;
            }

            var alignX = 0;
            var alignY = 0;

            if (opts.horizontalAlign == "left") {
                alignX = alignLeft;
            } else if (opts.horizontalAlign == "right") {
                alignX = alignRight;
            } else if (opts.horizontalAlign == "center") {
                alignX = alignLeft + alignWidth / 2;
            }

            if (opts.verticalAlign == "top") {
                alignY = alignTop;
            } else if (opts.verticalAlign == "bottom") {
                alignY = alignBottom;
            } else if (opts.verticalAlign == "center") {
                alignY = alignTop + alignHeight / 2;
            }

            if (hPositionSet) {
                if (left != null) {
                    var leftPos = hAlignSet ? alignX + left : left;
                    var windowWidth = $(window).outerWidth();
                    var popUpWidth = $(popup).outerWidth();
                    var isOutWindow = (windowWidth - (popUpWidth + leftPos)) < 0;
                    if (!isOutWindow && popUpWidth > 0) {
                        popup.css("left", leftPos);
                    }
                    else {
                        //no space left. Check if have on right
                        if (popUpWidth < leftPos && popUpWidth > 0) {
                            popup.css("right", windowWidth - leftPos);
                        }
                        else {
                            popup.css("max-width", windowWidth);
                        }

                    }
                      var o = popup.offset();
                    if (o.right < 0) {
                        popup.css("left", popup.css("left") +o.right);
                }
                }
                if (right != null) {
                    popup.css("right", hAlignSet ? ($(window).outerWidth() - alignX) + right : right);
                    var o = popup.offset();
                    if (o.left < 0) {
                        popup.css("right", popup.css("right") + o.left);
                    }
                }
            } else if (hAlignSet) {
                popup.css("left", alignX - popup.outerWidth() / 2);
            }

            if (vPositionSet) {
                if (docHeight < popup.outerHeight())
                {
                    popup.css("max-height", docHeight);
                    popup.css("overflow-y", "auto");
                    popup.width += 10;
                }
                if (top != null) {
                    var topHeight = vAlignSet ? alignY + top : top
                    popup.css("top", topHeight > 0 ? topHeight : 0);
                }
                if (bottom != null) {
                    var topHeight = alignY - popup.outerHeight() - alignHeight - bottom;
                    popup.css("top", topHeight > 0 ? topHeight : 0);
                    //popup.css("bottom", vAlignSet ? ($(document).outerHeight() - alignY) + bottom : bottom);
                }
            } else if (vAlignSet) {

                var topHeight = alignY - popup.outerHeight() / 2;
                if (opts.modal == true) {
                    var topalignHeight = $(window).outerHeight() - popup.outerHeight();
                    if (topalignHeight > 0) {
                        topHeight = topalignHeight / 2;
                    }
                    else {
                        topHeight = 0;
                        popup.css("overflow-y", "auto");

                    }
                }
                popup.css("top", topHeight > 0 ? topHeight : 0);
            }

        } else if (fixed) {
            popup.css("position", "fixed");
        }
    } 

    function init(popups) {
        popups.filter("[data-popup-activation-mode]").each(function () {       
            var item = $(this);
            if (item.data("inited") == true) return;
            item.data("inited", true);
            var mode = item.data("popup-activation-mode");
            if (mode != null) {
                var getRootedSelector = function (selector) {
                    if (selector == null || selector.length == 0) {
                        return { root: item, selector: null };
                    } else if (selector.lastIndexOf("this:", 0) === 0) {
                        return { root: item, selector: selector.substring(5) };
                    } else if (selector.lastIndexOf("parents:", 0) === 0) {
                        return { root: item.parents(selector.substring(8)), selector: null };
                    } else if (selector.lastIndexOf("closest:", 0) === 0) {
                        return { root: item.closest(selector.substring(8)), selector: null };
                    } else {
                        return { root: document, selector: selector };
                    }
                }

                var activatorSelector = item.data("popup-activator");
                var activatorRoot = getRootedSelector(activatorSelector);
                //turn off this selectoron mobile devices
                var deviceWidth = $(window).width();
                var dropdownTouch = deviceWidth <= 1024 && Modernizr.touch && $(activatorRoot.root).hasClass("multivendor-select");
                if (!dropdownTouch) { 
                        $(activatorRoot.root).on(mode, activatorRoot.selector, function () {
                            if (item.hasClass(multivendorWeb.popup.popupClass) == true) { // if the item is a pop-up, show it
                                multivendorWeb.popup.show(item);
                            } else {
                                var popupOptions = item.data("popup-options"); // if the item contains a pop-up definition (but is no pop-up itself), display the pop-up
                                if (popupOptions != null) {
                                    multivendorWeb.popup.show(popupOptions);
                                }

                                var popupSelector = item.data("popup");
                                if (popupSelector != null) { // show the targeted pop-up
                                    var popupRoot = getRootedSelector(popupSelector | multivendorWeb.popup.popupClass);
                                    var popUp = $(popupRoot.root).find(popupRoot.selector);
                                    if (popUp.length > 0) {
                                        show(popUp);
                                    }
                                }
                            }
                        });
                }
                //dropdown special case handle
            }
        });
    }

    $(function () {
        init(multivendorWeb.popup.getPopups());
    });

    $(document).htmlUpdated(function (e) {
        init(multivendorWeb.popup.getPopups(e.element));
    });

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));