(function (multivendorWeb, $, undefined) {

    loadedIncludes = {};

    includesRegistered = false;

    multivendorWeb.core = {
        init: function (context) {
            if (!context.length) return;

            // includes
            var includes = this.findIncludes(context, context.data("include-recursive"));
            if (includes.length) {
                this.registerIncludes($.map((includesRegistered ? context : $(document)).findByTagName("script"), function (tag) { return tag.outerHTML.replace(new RegExp('"', 'g'), "'"); }));
                includesRegistered = true;
                this.loadIncludes(includes);
            }

            // async loaders
            var loaders = context.findByClass("multivendor-async", true);
            if (loaders.length) {
                $.each(loaders, function () {
                    var $loader = $(this); 
                    var loaderOptions = $loader.data("async");
                    var $loaderParent = $loader.parent();
                    $loader.removeAttr("data-async").removeData("async");
                    if (loaderOptions && loaderOptions.Url) {
                        $loader.addClass("async-loading");
                        var loadFunc = function () {
                            var batchId = loaderOptions.Id || "";
                            var loaderItems = $loader.findByClass("multivendor-async-item").map(function (i, e) { $e = $(this); return { e: $e, o: $e.data("async-item") } }).filter(function () { return (this.o.BatchId || "") === batchId });
                            $.each(loaderItems, function () {
                                this.e.removeAttr("data-async-item").removeData("async-item").addClass("async-loading");
                            });
                            var attr = { batchId: batchId };
                            if (loaderOptions.Data) {
                                keys = Object.keys(loaderOptions.Data);
                                if (keys.length > 0) {
                                    for (var key in loaderOptions.Data) {
                                        attr[key] = loaderOptions.Data[key];
                                    }
                                } else {
                                    attr["data"] = loaderOptions.Data;
                                }
                            }
                            var firstLoaderItem = null;
                            if (loaderItems.length) {
                                firstLoaderItem = loaderItems[0];
                                attr["items"] = $.map(loaderItems, function (i) { return { Id: i.o.Id, Data: i.o.Data } });
                            }

                            multivendorWeb.helpers.postJson(loaderOptions.Url, attr, function (data) {
                                if (data.AsyncResults) {
                                    var itemsDict = multivendorWeb.helpers.toDictionary($.grep(loaderItems, function (i) { return i.o.Id != null }), function (i) { return i.o.Id; });
                                    for (var i = 0; i < data.AsyncResults.length; i++) {
                                        var loadedItem = data.AsyncResults[i];
                                        if (loadedItem.ItemId) { // We have a matching loader item
                                            //var element = loadedItem.Selector ? $context.find(loadedItem.Selector) : loadedItem.AsyncItemId ? $context.find("[data-as") : $context;
                                            var loaderItem = itemsDict[loadedItem.ItemId];
                                            if (loaderItem) {
                                                var $ctx = loadedItem.Selector ? loaderItem.e.find(loadedItem.Selector) : loaderItem.e.Selector ? loaderItem.e.find(loaderItem.o.Selector) : loaderOptions.Selector ? loaderItem.e.find(loaderOptions.Selector) : loaderItem.e;
                                                var method = loadedItem.InsertMethod || loaderItem.InsertMethod || loaderOptions.InsertMethod || "html";
                                                $ctx.htmlUpdate(loadedItem.Html, method.toLowerCase(), loadedItem.Notify || true);
                                                //loaderItem.e.removeClass("async-loading");
                                            }
                                        } else { // no loader item, target by selector
                                            var $ctx = loadedItem.Selector ? $loader.find(loadedItem.Selector) : loaderOptions.Selector ? $loader.find(loaderOptions.Selector) : $loader;
                                            var method = loadedItem.InsertMethod || loaderOptions.InsertMethod || ($loader != $ctx ? "html" : "append");
                                            $ctx.htmlUpdate(loadedItem.Html, method.toLowerCase(), loadedItem.Notify || true);
                                            //loaderItem.e.removeClass("async-loading");
                                        }
                                    }
                                } else { // Single html view
                                    if (firstLoaderItem) {
                                        var $ctx = firstLoaderItem.o.Selector ? $loader.find(firstLoaderItem.o.Selector) : loaderOptions.Selector ? $loader.find(loaderOptions.Selector) : $loader;
                                        var method = firstLoaderItem.InsertMethod || loaderOptions.InsertMethod || "html";
                                        $ctx.htmlUpdate(data, method.toLowerCase(), true);
                                        //firstLoaderItem.e.removeClass("async-loading");
                                    } else {
                                        var $ctx = loaderOptions.Selector ? $loader.find(loaderOptions.Selector) : $loader;
                                        var method = loaderOptions.InsertMethod || "html";
                                        if ($loader != $ctx || data.length > 0) {
                                            $ctx.htmlUpdate(data, method.toLowerCase(), true);
                                        }
                                    }
                                }

                                $.each(loaderItems, function () { this.e.removeClass("async-loading"); });
                                $loader.removeClass("async-loading");
                                $loader.trigger("async-loaded");
                                if (!$loader.parent().length) { // The loader have been replaced, event cannot bubble, trigger on parent instead
                                    $loaderParent.trigger("async-loaded");
                                    //$loader.triggerHandler("async-loaded");
                                }
                            });
                        };

                        var loadDelay = loaderOptions.LoadDelay;
                        if (loadDelay) {
                            setTimeout(loadFunc, loadDelay);
                        } else {
                            loadFunc();
                        }
                    }

                });
            }
        },

        findIncludes: function (context, recursive) {
            var includeContainers = (recursive === false ? $(context) : $(context).findByClass("multivendor-include", true));
            var includes = [];
            includeContainers.each(function () {
                $.each($(this).data("includes").split(";"), function (i, include) {
                    if ($.inArray(include, includes) == -1) {
                        includes.push(include);
                    }
                });
            });
            return includes;
        },

        registerIncludes: function (includes) {
            $.each(includes, function (i, include) { loadedIncludes[include] = true; });
        },

        loadIncludes: function (includes) {
            if (!includes.length) return;
           
            var elements = $();
            $.each(includes, function (i, include) {
                if (!loadedIncludes[include]) {
                    loadedIncludes[include] = true;
                    elements = elements.add(include);
                }
            });
            $(document.body).append(elements);
        },

        airspaceTaken: function () {
            return multivendorWeb.helpers.isIE() && (multivendorWeb.helpers.findByClass("airspace").length > 0 || document.getElementsByTagName("object").length > 0 || document.getElementsByTagName("embedd").length > 0);
        },

        airspaceNeeded: function (element, needed, force) {
            if (force === true || this.airspaceTaken()) {
                element.trigger("airspaceNeeded", [needed]);
            }
        }
    }

    $(function () {
        multivendorWeb.core.init($(document));
    });

    $(document).htmlUpdated(function (e) {
        multivendorWeb.core.init($(e.element));
    });

    if (multivendorWeb.helpers.isIE() == true) {
        $(document).on("airspaceNeeded", function (e, needed) {
            var target = $(e.target);
            var taker = target.children(".airspace");
            if (needed === false) {
                target.removeClass("airspace-needed");
                taker.remove();
            } else {
                if (!taker.length) {
                    target.addClass("airspace-needed");
                    target.prepend("<iframe class='airspace' src='about:blank' frameBorder='0' scrolling='no'/>");
                }
            }
        });
    }

    multivendorWeb.viewMode = {
        className: "multivendor-view-mode",

        init: function (controls) {
            if (!controls.length) return;

            controls.on("itemCheck", function (e, data) {
                if (data.checked) {
                    $(this).trigger("changed", multivendorWeb.viewMode.getState($(this)));
                }
            });

            controls.find(".multivendor-menu").on("selectionChanged", function () {
                var control = $(this).closest(".multivendor-popup").data("owner").closest(".multivendor-view-mode");
                control.trigger("changed", $.extend(multivendorWeb.viewMode.getState(control), multivendorWeb.viewMode.getState($(this))));
            });
        },

        getState: function (control) {
            var selected = control.find(".checked,.selected");
            var state = {};
            var view = selected.filter("[data-view]").data("view");
            if (view) state.view = view;
            var attr = selected.filter("[data-attr]").data("attr");
            if (attr) state.attr = attr;
            var dir = selected.filter("[data-dir]").data("dir"); 
            if (dir) state.dir = dir;
            return state;
        }
    }

    multivendorWeb.helpers.setupInit(multivendorWeb.viewMode);

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));