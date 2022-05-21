(function (multivendorWeb, $, undefined) {

    multivendorWeb.dataview = {

        className: "multivendor-dataview",

        //getAndInit: function(context) {
        //    return this.init(this.getViews(context));
        //},

        init: function (context, loadCallback) {
            var views = multivendorWeb.dataview.getViews(context);
            multivendorWeb.dataview.initViews(views, loadCallback);
            return views;
        },

        initViews: function (views, loadCallback, loadedCallback) {

            views.each(function () {
                var $view = $(this);

                if (loadCallback) {
                    $view.data("loadCallback", loadCallback);
                    //$view.on("loadContent", function (e, data) {
                    //    var $this = $(this);
                    //    if (this != e.target) return;
                    //    //e.stopImmediatePropagation = true;

                    //    $this.data("loadCallback")($(this), data);
                    //});
                }

                if (loadedCallback) {
                    $view.data("loadedCallback", loadedCallback);
                    //$view.on("loadedContent", function (e) {
                    //    var $this = $(this);
                    //    if (this != e.target) return;
                    //    //e.stopImmediatePropagation = true;

                    //    $this.data("loadedCallback")($(this));
                    //});
                }



                $view.on("selectionChanged", "li[data-type='drop-down-list']", function (e, data) {
                    var $this = $(this);
                    var updateOptions = $this.data("view-update");
                    var options = $.extend({}, { content: true, pagination: true, resetPaging: true }, updateOptions);

                    multivendorWeb.dataview.updateView($view, options);
                });

                $view.on("change keyup input cut paste", ".tools-container .query-input", function (e, data) {
                    var val = $(this).val();
                    if (lastInputChangeValue != val) {
                        lastInputChangeValue = val;
                        multivendorWeb.dataview.updateView($view, { content: true, pagination: true, resetPaging: true });
                    }
                });


                var delayedLoad = $view.data("delayed-load");
                if (delayedLoad) {
                    setTimeout(function () {
                        multivendorWeb.dataview.updateView($view, { init: true, content: true }, function (view) {
                            view.trigger("initialized");
                        });
                    }, delayedLoad);
                }
                else {
                    multivendorWeb.dataview.updateView($view, { init: true, content: true }, function (view) {
                        view.trigger("initialized");
                    });
                }

                var facetContainerSelector = $view.data("facet-container");
                var $facetContainer = $(document).find(facetContainerSelector);
                if ($facetContainer.length) {

                    $facetContainer.data("view", $view);
                    var facetChangeFunction = function (e) {
                        var $facetContainer = $(this);
                        if ($(e.target).closest("[data-value-name]").length) {
                            let $facetView = $facetContainer.data("view");
                            multivendorWeb.dataview.updateView($facetView, { content: true, pagination: true, resetPaging: true });
                        }
                    };

                    var prevFunction = $facetContainer.data("callback");
                    if (prevFunction) {
                        $facetContainer.off("change", prevFunction);
                    }

                    $facetContainer.data("callback", facetChangeFunction);
                    $facetContainer.on("change", facetChangeFunction);
                }

            });

            views.on("click", function () {
                var $view = $(this);
            });
        },

        updateContent: function (view) {
            multivendorWeb.dataview.updateView(view, { updateContent: true });
        },

        requestId: 0,

        getRequestId: function () {
            var id = multivendorWeb.dataview.requestId;
            multivendorWeb.dataview.requestId = id + 1;

            return Math.floor(Math.random() * Math.pow(2, 24));// << 8) | id;
        },

        updateView: function (view, options, callback) {

            //$.extend();
            options = options || {};

            var state = multivendorWeb.dataview.getState(view);
            var init = options.init == true || view.hasClass("init") == true;
            var request = { state: state, report: options.report == true, resetPaging: options.resetPaging == true };
            if (init == true) {
                request.init = true;
                request.hasTools = view.children(".tools-container").length > 0;
                request.hasPagination = view.children(".pagination-tools").length > 0;
                if (options.tools == null && request.hasTools == false) options.tools = true;
                if (options.pagination == null && request.hasPagination == false) options.pagination = true;
                //request.elements = (view.children(".tools").length ? 0 : 1) | (view.children(".content").length ? 0 : 2) | (view.children(".pagination-tools").length ? 0 : 4);
            }
            if (options.content != null) request.updateContent = options.content;
            if (options.pagination != null) request.updatePagination = options.pagination;
            if (options.tools != null) request.updateTools = options.tools;
            if (options.paginationChanged != null) request.paginationChanged = options.paginationChanged;
            if (options.sortChanged != null) request.sortChanged = options.sortChanged;
            if (options.viewChanged != null) request.viewChanged = options.viewChanged;
            if (options.filterChanged != null) request.filterChanged = options.filterChanged;

            var loadArgs = { request: request };

            var loadCallback = view.data("loadCallback");
            if (loadCallback) loadCallback(view, loadArgs, options.args);
            view.trigger("loadContent", loadArgs);

            var requestId = multivendorWeb.dataview.getRequestId();
            loadArgs.request.requestId = requestId;
            loadArgs.request = JSON.stringify(loadArgs.request);

            var contentSelector = view.data("content-selector");
            var $content = contentSelector ? view.findBySelector(contentSelector) : view.children(".content-container");

            if (options.report == true) {
                window.location.href = multivendorWeb.helpers.mergeParams(view.data("url"), loadArgs, false);
                return;
            }

            $content.addClass("loading");

            view.data("request-id", requestId);
            multivendorWeb.helpers.getJson(view.data("url"), loadArgs, function (data, status, xhr) {

                view.trigger("handleContent", { data: data, status: status, xhr: xhr });

                var type = xhr.getResponseHeader("content-type") || "";
                if (type.indexOf('html') > -1) {
                    view.htmlUpdate(data);
                } else {

                    var callerRequestId = view.data("request-id");
                    if (callerRequestId != null && data.requestId != null && data.requestId != callerRequestId) {
                        return;
                    };

                    if (data.itemCount != null && data.itemCount != -1) {
                        view.attr("data-item-count", data.itemCount);
                        view.data("item-count", data.itemCount);
                    }

                    if (data.content != null) {

                        $content.htmlUpdate(data.content);

                        if (contentSelector) {
                            
                        }
                    }

                    if (data.tools != null) {
                        var $tools = $(data.tools);
                        var $currentTools = view.children(".tools-container");
                        if ($currentTools.length > 0) {
                            $currentTools.htmlUpdate($tools, "replace");
                            //$currentTools.htmlUpdate($tools, $tools.is("ul") ? "replace" : "html");
                        } else {
                            view.htmlUpdate($tools, "prepend", false);
                            $tools.notifyUpdate();
                        }
                    }

                    if (data.sort != null) {
                        var $sort = $(data.sort);
                        var $currentTools = view.children(".tools-container");
                        if ($currentTools.length > 0) {
                            var $currentSort = $currentTools.find(".sort-tool");
                            if ($currentSort.length > 0) {
                                $currentSort.htmlUpdate($sort, "replace");
                            } else {
                                $currentTools.htmlUpdate($sort, "append", false); // TODO. In correct position.
                            }
                        }
                    }

                    if (data.pagination != null) {
                        var $pagination = $(data.pagination);
                        var $currentPagination = view.children(".pagination-tools");
                        if ($currentPagination.length > 0) {
                            $currentPagination.htmlUpdate($pagination, $pagination.is("ul") ? "replace" : "html");
                        } else {
                            view.htmlUpdate($pagination, "append", false);
                            $pagination.notifyUpdate();
                        }
                    }

                    if (data.itemCountDescription != null) {
                        var $itemCountDesc = $(data.itemCountDescription);
                        var $currentitemCountDesc = view.find(".item-count-container");
                        if ($currentitemCountDesc.length > 0) {
                            $currentitemCountDesc.htmlUpdate($itemCountDesc.children());
                        } else {
                            view.htmlUpdate($itemCountDesc, "append", false);
                            $itemCountDesc.notifyUpdate();
                        }
                    }

                    if (data.facets != null) {
                        var facetContainerSelector = view.data("facet-container");
                        var $facetContainer = $(document).find(facetContainerSelector);
                        if ($facetContainer.length) {
                            $facetContent = $(data.facets);
                            $facetContainer.htmlUpdate($facetContent);
                        }
                        //var $facetContainer = $(document).find(data.facets.container);
                        //if ($facetContainer.length) {
                        //    $facetContent = $(data.facets.content);
                        //    $facetContainer.htmlUpdate($facetContent);
                        //}
                    }
                }

                $content.removeClass("loading");

                view.removeClass("init");

                var loadedCallback = view.data("loadedCallback");
                if (loadedCallback) loadedCallback(view, $content.children());

                view.trigger("loadedContent");

                if (callback) {
                    callback(view);
                }
            });


        },

        //updateControl: function(view, selector) {

        //},

        getState: function (view) {
            var state = {};

            var itemCount = view.data("item-count");
            if (itemCount) {
                state.itemCount = itemCount;
            }

            var paginationTools = view.children(".pagination-tools");
            if (paginationTools.length > 0) {
                var pageSelector = paginationTools.find("li.checked[data-page]");
                var pageSize = paginationTools.find("li.checked[data-page-size]");

                var pagination = { page: pageSelector.data("page"), pageSize: pageSize.data("page-size") };
                var pageId = pageSelector.data("page-id");
                if (pageId) { selection["pageId"] = pageId; };
                var pageSizeId = pageSize.data("page-size-id");
                if (pageSizeId) { pagination["pageSizeId"] = pageSizeId; };


                //paginationTools.find("li[data-page-size]").map(function () {
                //    var $tool = $(this);
                //    var result = { size: $tool.}
                //});

                var pageSizes = $.map(paginationTools.find("li[data-page-size]"), function (e) {
                    return $(e).data("page-size");
                });

                if (pageSizes.length) {
                    pagination.pageSizes = pageSizes.reverse();
                }

                state.pagination = pagination;
            }

            //var sortTool = view.find(".sort-tool");
            //var sortDropDowns = $(document).find(".sort-selectors");
            //var sortDropDown = sortDropDowns.filter(function () {
            //    return $(this).data("owner")[0] == sortTool[0];
            //});
            var sortDropDown = multivendorWeb.popup.getPopupsByOwner(view.find(".sort-tool"));
            if (sortDropDown.length > 0) {
                var selectedSortTool = sortDropDown.find("li.selected");
                if (selectedSortTool.length > 0) {
                    var sortId = selectedSortTool.data("name");
                    if (sortId) {
                        state.sortId = sortId;
                    }

                    var sortDir = selectedSortTool.attr("data-sort-dir");
                    if (sortDir) {
                        var sort = { direction: sortDir };
                        var sortType = selectedSortTool.data("sort-type");
                        if (sortType) sort.type = sortType;
                        state.sort = sort;
                    }
                }
            }

            var tools = view.find(".tools-container .tools");

            var selectedViewTool = tools.find(".view-tool.checked");
            if (selectedViewTool.length > 0) {
                var viewId = selectedViewTool.data("name");
                if (viewId) {
                    state.viewId = viewId;
                }
            }

            var customTools = tools.findByClass("custom-tool");
            state.tools = [];// {}; 
            customTools.each(function () {
                var $tool = $(this);
                var tool = { name: $tool.data("name"), checked: $tool.hasClass("checked") };
                if ($tool.data("type") == "drop-down-list") {
                    var selectedValue = $tool.attr("data-selected-item");
                    if (selectedValue) {
                        tool.selectedValue = selectedValue;
                    }

                    var $selectedItems = $tool.find(".multivendor-menu > li.selected");
                    if (!$selectedItems.length) {
                        var $popUp = $tool.data("popUp");

                        if ($popUp != null) {
                            if ($popUp.length) {
                                $selectedItems = $popUp.find(".multivendor-menu > li.selected");
                            }
                        }
                    }
                    if ($selectedItems.length) {
                        tool.selectedValues = $selectedItems.map(function () { return $(this).data("name"); }).get();
                    }
                }
                state.tools.push(tool);
            });

            var filterTool = view.find(".filter-container");
            if (filterTool.length > 0) {
                var filter = {};
                var query = filterTool.find(".query-input").val();
                if (query) {
                    filter.query = query;
                }

                var filterDropDown = multivendorWeb.popup.getPopupsByOwner(view.find(".filter-settings-button"));
                if (filterDropDown.length > 0) {
                    var properties = $.map(filterDropDown.find("li.selected"), function (e) {
                        var $f = $(e);
                        var r = { id: $f.data("name") };
                        var p = $f.data("property");
                        if (p) {
                            r.property = p;
                        }
                        return r;
                    });

                    if (properties.length > 0) {
                        filter.properties = properties;
                    }
                }

                if (filter.query || filter.properties) {
                    state.filter = filter;
                }
            }

            var facetContainerSelector = view.data("facet-container");
            var $facetContainer = $(document).find(facetContainerSelector);
            if ($facetContainer.length) {
                var $facets = $facetContainer.find(".facet");
                if ($facets.length) {
                    var facets = [];
                    $facets.each(function () {
                        var $facet = $(this);
                        var facet = { id: $facet.data("id"), values: [] };

                        var $inputs = $facet.find("input");
                        $inputs.each(function () {
                            var $input = $(this);
                            facet.values.push({ name: $input.closest("[data-value-name]").data("value-name"), value: $input.val() });
                        });

                        var $valControls = $facet.find("[data-value-name]:not(input)");
                        $valControls.each(function () {
                            var $valControl = $(this);
                            facet.values.push({ name: $valControl.data("value-name"), value: $valControl.data("value") });
                        });

                        facets.push(facet);
                    });

                    if (!state.filter) state.filter = {};
                    state.filter.facets = facets;
                }
            }

            return state;
        },

        getViews: function (context) {
            return $(multivendorWeb.helpers.findByClass(this.className, context));
        }
    };

    $document = $(document);

    var lastInputChangeValue = null;

//    $(function () {
///*        multivendorWeb.dataview.init()*/;
//    });

//    $(document).htmlUpdated(function (e) {
//        //multivendorWeb.dataview.init(e.element);
//    });

    $document.on("itemClicked", ".multivendor-dataview", function (e, data) {

        if (this != $(e.target).closest(".multivendor-dataview").get(0)) return;
        //e.stopImmediatePropagation = true;
        var $view = $(this);

        var toolbar = data.item.parent();
        if (data.item.hasClass("view-tool")) {
            multivendorWeb.dataview.updateView($view, { content: true, pagination: true, viewChanged: true });
        }
        //else if (data.item.hasClass("sort-tool")) {
        //}
        //else if (data.item.hasClass("filter-tool")) {
        //}
        else if (data.item.hasClass("download-tool")) {
            multivendorWeb.dataview.updateView($view, { report: true });
        }
        else {

            //alert(data.toolbar.get(0).outerHTML);
            if (toolbar.hasClass("pagination-tools") == true) {
                multivendorWeb.dataview.updateView($view, { content: true, pagination: true });
                //multivendorWeb.dataview.updateView($view, { pagination: true });
                //multivendorWeb.dataview.updateView($view, { content: true });
            } else {
                //if (data.item.hasClass("custom-tool") {
                //    $view.trigger("customToolClicked");
                //}
                if (data.item.data("type") == "check") {
                    multivendorWeb.dataview.updateView($view, { content: true, pagination: true });
                }

            }
        }
    });

    $document.on("selectionChanged", ".filter-properties", function () {
        var $view = $(this).data("owner").closest(".multivendor-dataview");
        multivendorWeb.dataview.updateView($view, { content: true, pagination: true, tools: true, resetPaging: true });
    });

    //$(document).on("selectionChanged", ".sort-selectors", function (e, data) {
    //    var $view = $(this).data("owner").closest(".multivendor-dataview");
    //    multivendorWeb.dataview.updateView($view, { content: true });
    //});

    $document.on("itemClick", ".sort-selectors", function (e, data) {
        var item = data.item;
        if (item.hasClass("selected") == true) {
            var sortDir = item.attr("data-sort-dir");
            $(item).attr("data-sort-dir", sortDir == 1 ? 0 : 1);
        }
    });

    $document.on("itemClicked", ".sort-selectors", function () {
        var $view = $(this).data("owner").closest(".multivendor-dataview");
        multivendorWeb.dataview.updateView($view, { content: true, tools: true,sortChanged: true });
    });

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));