(function (multivendorWeb, $, undefined) {

    window.multivendorWeb = multivendorWeb; // Alias

    var isIE;
    var isOldEdge;
    multivendorWeb.resize = {

        minIntervall: 10,

        maxIntervall: 300,

        checkForResize: function () {
            if (timer != null) {
                clearTimeout(timer);
                checkForResizeInternal();
            }
        },

        createResizeObserver: function (resizeCallback) {
            if (window.ResizeObserver != null) {
                return new window.ResizeObserver(resizeCallback);
            }

            var callBack = function (items) {
                var wrapped = $.map(items, function (i) { return { target: i, contentRect: i.getBoundingClientRect() } });
                resizeCallback(wrapped);
            }

            return new ResizeObserverPolyfill(callBack);
        }
    }

    multivendorWeb.helpers = {

        isIE: function () {
            if (!isIE) {
                var ua = window.navigator.userAgent;
                isIE = ua.indexOf('MSIE ') > -1 || ua.indexOf('Trident/') > -1;
            }
            return isIE;
        },

        isOldEdge: function () {
            if (!isOldEdge) {
                isOldEdge = window.navigator.userAgent.indexOf('Edge/') > 0;
                //return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
            }
            return isOldEdge;
        },


        isAppleDevice: function () {
            return ((navigator.userAgent.toLowerCase().indexOf("ipad") > -1) || (navigator.userAgent.toLowerCase().indexOf("iphone") > -1) || (navigator.userAgent.toLowerCase().indexOf("ipod") > -1));
        },

        ajax: function (url, data, callback, settings) {
            return ajax(url, data, callback, settings)
        },

        getJson: function (url, data, callback) {
            return this.json("get", url, data, callback);
        },

        postJson: function (url, data, callback) {
            return this.json("post", url, data, callback);
        },


        putJson: function (url, data, callback) {
            return this.json("put", url, data, callback);
        },

        json: function (type, url, data, callback) {
            return this.ajax(url, data, callback, { type: type });
        },

        trimStart: function (string, char) {
            var length = string.length;
            var i = 0;
            while (i < length && string.charAt(i) == char) {
                i++;
            };
            return i > 0 ? string.slice(i) : string;
        },

        trimEnd: function (string, char) {
            var i = string.length;
            while (i > 0 && string.charAt(i - 1) == char) {
                i--;
            };
            return i < string.length ? string.slice(0, i) : string;
        },

        setEquals: function (x, y) {
            var xl = x != null ? x.length : 0;
            var yl = y != null ? y.length : 0;
            return (xl == yl) && (xl == 0 || x.every(function(e, i) {
                return y.indexOf(e) !== -1; 
            }));
        },

        sequenceEquals: function (x, y) {            
            var xl = x != null ? x.length : 0;
            var yl = y != null ? y.length : 0;
            return (xl == yl) && (xl == 0 || x.every(function (e, i) {
                return e === y[i];
            }));
        },

        sum: function (items, selector) {
            var $this = $(this);
            var sum = 0;
            $.each(function (index, item) {
                sum += selector(item);
            });
            return sum;
        },

        toDictionary: function(list, keySelector, valueSelector) {
            var dict = {};
            list.forEach(function (item) {
                dict[keySelector(item)] = valueSelector != null ? valueSelector(item) : item;
            });
            return dict;
        },

        formatString: function (string, arguments) {

            if (string != null) {
                var formatted = string;
                for (var arg in arguments) {
                    var regexp = new RegExp('\\{' + arg + '\\}', 'gi');
                    var value = arguments[arg];
                    if (value == null) {
                        value = "";
                    }
                    formatted = formatted.replace(regexp, value);
                }
                return formatted;
            }

            return string;
        },
        autoScroll: function ($target, fastScroll, delay) {
            if ($target.length) {
                var scrollAmount = $target.offset().top;
                //var $window = $(window);

                if (fastScroll == true) {
                    // FAST SCROLL
                    var scrollToQuickNavigation = function () { $target.get(0).scrollIntoView({ behavior: "auto", block: "start" }); };
                    scrollToQuickNavigation();
                } else {

                    // SMOOTH SCROLL
                    var scrollToQuickNavigation = function () { $target.get(0).scrollIntoView({ behavior: "smooth", block: "start" }); }; // behavior: "smooth"

                    if (delay) {
                        setTimeout(scrollToQuickNavigation, multivendorWeb.helpers.isOldEdge() == true || multivendorWeb.helpers.isIE() == true ? delay * 2 : delay);
                    } else {
                        scrollToQuickNavigation();
                    }
                }
            }
        },
        mergeParams: function (url, params, replace, includeNull) {
            if (replace !== false) {
                if (!$.isPlainObject(params)) {
                    params = this.toObject();
                }

                var urlParamsIndex = url.indexOf("?");
                if (urlParamsIndex > 0) {
                    var urlParams = this.toObject(url.slice(urlParamsIndex));
                    params = $.extend({}, urlParams, params);

                    if (includeNull !== true) {
                        for (key in params) {
                            var value = params[key];
                            if (value == null)
                                delete params[key];
                        }
                    }

                    url = url.slice(0, urlParamsIndex);
                }
            }

            params = $.isPlainObject(params) ? $.param(params).replace(/\+/g, "%20") : this.trimStart(params, '?'); // HACK

            return url + (url.indexOf('?') >= 0 ? '&' : '?') + params;
        },

        //removeParam: function (url, param) {
        //    if (param) {
        //        var urlParamsIndex = url.indexOf("?");
        //        if (urlParamsIndex > 0) {
        //            url.indexOf(param, urlParamsIndex) {

        //            }
        //        }
        //    }
        //},

        toObject: function (params) {
            if (params == null || $.isPlainObject(params)) {
                return params; // is already an object
            }

            params = this.trimStart(this.trimStart(params, '#'), '?');

            var obj = {};
            if (params.length > 0) {
                var attributes = params.split("&");
                for (i = 0; i < attributes.length; i++) {
                    var entry = attributes[i].split("=");             
                    if (entry.length > 0) {
                        if (entry.length > 1) {
                            var val =  decodeURIComponent(entry[1]);
                            obj[entry[0]] = isNaN(val) ? val : +val;
                        }
                        else {
                            obj[entry[0]] = null;
                        }
                    }
                }
            }
            return obj;
        },
        
        sequenceEquals: function (items1, items2) {

            if (items1 == null && items2 == null) {
                return true;
            }

            if (!items1 || !items1.length || !items2 || !items2.length || items1.length !== items2.length) {
                return false;
            }
            for (var i = 0, length = items1.length; i < length; i++) {
                if (items1[i] !== items2[i]) {
                    return false;
                }
            }
            return true;
        },

        distinct: function (anArray) {
            //return $.grep(anArray, function (v, k) {
            //    return $.inArray(v, anArray) === k;
            //});
            var result = [];
            $.each(anArray, function (i, v) {
                if ($.inArray(v, result) == -1) result.push(v);
            });
            return result;
        },

        substract: function (array1, array2) {
            return $.grep(array1, function (value, i) {
                return $.inArray(value, array2) === -1;
            });
            //var result = [];
            //$.each(array1, function (i, value) {
            //    if ($.inArray(value, array2) === -1) result.push(value);
            //});
            //return result;
        },

        log: function (message, optionalParams) {
            if (typeof console != "undefined") { 
                console.log(message, optionalParams);
            } 
        },

        findByClass: function (name, context, includeSelf) {

            if (!context) {
                return document.getElementsByClassName(name);
            }

            //var ctx = context instanceof jQuery ? (context.length ? context.get(0) : null) : context;
            //if (ctx) {
            //    var result = ctx.getElementsByClassName(name);
            //    if (includeSelf === true && $(context).hasClass(name)) {
            //        result = $(result).toArray();
            //        result.unshift(ctx);
            //    }
            //    return result;
            //}

            var $ctx = $(context);
            if ($ctx && $ctx.length) {
                var result = [];

                $ctx.each(function () {
                    if (this.getElementsByClassName) {
                        var elements = this.getElementsByClassName(name);
                        if (elements.length > 0) {
                            for (var i = 0; i < elements.length; i++) {
                                result.push(elements[i]);
                            }
                            //result = result.concat(elements);
                        }
                        if (includeSelf === true && $(this).hasClass(name) === true) {
                            result.push(this);
                        }
                    } /*else {
                        console.log("findByClass error");
                    }*/
                    
                });

                //var result = $ctx.get(0).getElementsByClassName(name);
                //if (includeSelf === true && $ctx.hasClass(name)) {
                //    result = $(result).toArray();
                //    result.unshift($ctx.get(0));
                //}
                return result;
            }

            return null;
        },

        setupInit: function (obj) {
            $(function () {
                obj.init($(multivendorWeb.helpers.findByClass(obj.className, null, true)));
            });

            $(document).htmlUpdated(function (e) {
                obj.init($(multivendorWeb.helpers.findByClass(obj.className, e.element, true)));
            });
        },

        createBase: function (namespace, name, className, obj) {

            var base = {
                className: className,

                getViews: function (context) {
                    return $(multivendorWeb.helpers.findByClass(base.className, context, true));
                },

                init: function (views) {
                },

                onPageLoad: function () {
                    var views = base.getViews();
                    if (views.length) {
                        base.init(views);
                    }
                },

                onLoad: function (e) {
                    var views = base.getViews(e.element);
                    if (views.length) {
                        base.init(views);
                    }
                }
            };

            obj = $.extend(base, obj);

            namespace[name] = obj;

            $(obj.onPageLoad());

            $(document).htmlUpdated(obj.onLoad);

            return obj;
        },

        createListBase: function (className, itemSelector) {

            return {
                className: className,

                itemSelector: itemSelector,

                init: function (lists) {

                    if (!lists.length) return;

                    lists.on("click", " > " + this.itemSelector, function (e) {
                        var item = $(this);

                        var list = item.closest("." + className);

                        list.trigger("itemClick", { item: item });


                        var selectMode = list.data("selection-mode");
                        if (selectMode == "multiple" && e.ctrlKey) {
                            if (!item.hasClass("selected")) {
                                multivendorWeb.list.select(item, list, false);
                            } else {
                                multivendorWeb.list.unselect(item, list);
                            }
                        } else if (selectMode != "none") {
                            multivendorWeb.list.select(item, list);
                        }
                        list.focus();

                        if (item.hasClass("checkable")) {
                            var checkType = item.data("check-type");
                            if (checkType != null) {
                                var selected = item.hasClass("selected");
                                if (selected == false) {
                                    if (checkType.Radio) {
                                        var checkGroup = item.data("check-group");
                                        //var checkGroupItems = getItems(list).filter(function() { $i = $(this).data(" });
                                        var checkGroupItems = multivendorWeb.list.getItems(list);
                                        multivendorWeb.list.unselect(checkGroupItems, list, true);
                                        multivendorWeb.list.select(item, list, false, false);
                                        //return;
                                    }
                                }

                                if (checkType.Toggle) {
                                    if (selected == true) {
                                        multivendorWeb.list.unselect(item, list);
                                    } else {
                                        multivendorWeb.list.select(item, list, false, false);
                                    }
                                }
                            }
                        }

                        list.trigger("itemClicked", { item: item });
                    });

                    lists.next(".show-link").on("click", function (e) {
                        var $list = $(this).prev();
                        $list.toggleClass("show-all");
                    });
                    //list.attr("tabindex", -1); // TODO WHAT IS THIS!!!??? IN THE BASE CLASS FOR A LIST???!!! @#&!

                    //list.on("keydown", function (e) {
                    //    alert(e.keyCode);
                    //});
                },

                getList: function (item) {
                    return item ? item.closest("." + this.className) : null;
                },

                getItems: function (list) {
                    return list.find(this.itemSelector);
                },

                getSelection: function (list) {
                    return list ? this.getItems(list).filter(".selected") : null;
                },

                getSelectedIndexes: function (list) {
                    var items = this.getItems(list);
                    return $.map(this.getSelection(list), function (i) { return items.index(i); });
                },

                //selectByIndex: function (indexes, list, clearSelection, scrollTo) {
                //    var items = this.getItems(list);
                //    this.select(function () { $.map(indexes, function (i) { return items[i]; }), list, clearSelection, scrollTo);
                //},

                clearSelection: function (list, stopEvent) {
                    this.select(null, list, null, null, stopEvent);
                },

                selectByName: function(name, list, clearSelection, scrollTo, stopEvent) {
                    return this.select(function (i, item) {
                        return $(item).data("name") == name;
                    }, list, clearSelection, scrollTo, stopEvent);
                },

                select: function (items, list, clearSelection, scrollTo, stopEvent) {
                    // items
                    if ($.isFunction(items)) {
                        if (list == null) return;
                        items = this.getItems(list).filter(items);
                    } else {
                        items = items || $();
                    }

                    // list
                    list = list || this.getList(items);

                    // selection
                    var prevSelection = this.getSelection(list);

                    if (clearSelection != false) {
                        prevSelection.removeClass("selected");
                    }

                    items.addClass("selected");

                    // scroll
                    if (scrollTo) {
                        this.scrollTo(items, list);
                    }

                    if (stopEvent != true) {
                        // trigger
                        list.raiseSelectionChanged(prevSelection, this.getSelection(list));
                    }
                },

                unselect: function (items, list, stopEvent) {
                    //this.select(this.getSelection(list).not(function (i, e) { return $.inArray(items, e) != -1; }), list);
                    this.getItems(list);

                    if ($.isFunction(items)) {
                        if (list == null) return;
                        list = list.find(this.itemSelector).filter(items);
                    }

                    if (items && items.length) {

                        items.removeClass("selected");

                        list = list || this.getList(items);

                        if (stopEvent != true) {
                            list.trigger("selectionChanged", { selection: this.getSelection(list), selected: $(), unselected: items });
                        }
                    }
                },

                scrollTo: function (item, list, force) {
                    if (item && item.length) {
                        list = list || this.getList(item);

                        var scrollContainer = list.offsetParent();
                        if (scrollContainer.length) {
                            var itemTop = item.offset().top;
                            var scrollTop = scrollContainer.offset().top;
                            if (force || itemTop < scrollTop || itemTop + item.height() > scrollTop + scrollContainer.height()) {
                                scrollContainer.scrollTop(itemTop - list.offset().top);
                            }
                        }
                    }
                }
            };
        },

        loadScripts: function (scripts) {
            if (scripts) {
                if (loadedScripts[scripts] != true) {
                    loadedScripts[scripts] = true;
                    $(document.body).append(scripts);
                }
            }
        },

        getCurrentMedia: function () {
            if (multivendor.helpers.getMediaQuery("mobile").matches) return "mobile";
            if (multivendor.helpers.getMediaQuery("tablet").matches) return "tablet";
            return "default";
        },

        getMediaQuery: function (id) {
            var mq = mediaQuery[id];
            if (mq == null) {
                if (document.body.currentStyle != null) {
                    mq = window.matchMedia(document.body.currentStyle.getAttribute(id + "-query"));
                    mediaQuery[id] = mq;
                } else {
                    mq = window.matchMedia(window.getComputedStyle(document.body).getPropertyValue("--" + id + "-query"));
                    mediaQuery[id] = mq;
                }
            }
            return mq;
        },

        isMobile: function () { return multivendor.helpers.getMediaQuery("mobile").matches; },

        isTablet: function () { return multivendor.helpers.getMediaQuery("tablet").matches; },

        mediaQueryChanged: function (id, callback) {
            var mq = multivendor.helpers.getMediaQuery(id);
            if (mq != null) {
                mq.addListener(callback);
            }
        }
    };

    mediaQuery = {};

    loadedScripts = {};

    function ajax(url, data, callback, settings) {

        if ($.isFunction(data)) {
            settings = callback;
            callback = data;
            data = undefined;
        }

        if ($.isPlainObject(callback)) {
            settings = callback;
            callback = undefined;
        }

        var hasSettings = $.isPlainObject(settings);
        var putPost = hasSettings && (settings.type == "put" || settings.type == "post");

        var ajaxSettings = {
            //type: type,
            url: url,
            data: putPost && $.isPlainObject(data) ? JSON.stringify(data) : data,
            //dataType: "json",
            contentType: "application/json",
            success: callback
            //traditional: putPost && $.isPlainObject(data)
        };

        if (hasSettings) {
            //if (putPost) {
            //    ajaxSettings.contentType = "application/json";
            //}
            ajaxSettings = $.extend(ajaxSettings, settings)
        };

        return $.ajax(ajaxSettings);
    }

    function ajaxHtml(url, data, callback, settings, method, context, notify) {
        //arguments.length;

        if ($.isFunction(data)) {
            callback = data;
            data = null;
        }

        if ($.isPlainObject(callback)) {
            context = method;
            method = settings;
            settings = callback;
            callback = undefined;
        }

        if (context != null && context.hasClass("loader")) {
            context.addClass("loading");
        }

        ajax(url, data, function (d) {

            var html = $(d);
            if (html.length == 0) {
                html = d;
                context.text(html);
            } else {
                switch (method) {
                    case "replace":
                        context.replaceWith(html).remove();
                        context = html;
                        break;
                    case "html": context.html(html); break;
                    case "append": context.append(html); break;
                    case "prepend": context.prepend(html); break;
                }
            }

            if (context.hasClass("loader")) {
                context.removeClass("loading");
            }
            
            if (notify != false) {
                context.notifyUpdate();
            }

            if (callback) {
                callback(html, context);
            }
        }, settings);
    }
   
    $.fn.extend({
        findBySelector: function(selector) {
            var context = $(this);
            if (selector.parent) {
                context = context.parent(selector.parent);
            } else if (selector.closest) {
                context = context.closest(selector.closest);
            }
            if (selector.find) {
                context = context.find(selector.find);
            }
            return context;
        },

        replaceFromUrl: function (url, data, callback, settings) {
            ajaxHtml(url, data, callback, settings, "replace", $(this));
        },
        htmlFromUrl: function (url, data, callback, settings) {
            ajaxHtml(url, data, callback, settings, "html", $(this));
        },
        appendFromUrl: function (url, data, callback, settings) {
            ajaxHtml(url, data, callback, settings, "append", $(this));
        },
        prependFromUrl: function (url, data, callback, settings) {
            ajaxHtml(url, data, callback, settings, "prepend", $(this));
        },
        htmlUpdate: function (html, method, notify) {
            var context = $(this);      
            var $html = $(html);

            if (notify != false) {
                context.trigger("htmlUpdating", { html: $html });
            }

            switch (method || "html") {
                case "before": $html.insertBefore(context); break;
                case "after": $html.insertAfter(context); break;
                case "replace":
                    context.replaceWith($html).remove();
                    context = $html;
                    break;
                case "append": context.append($html);
                    break;
                case "prepend": context.prepend($html); break;
                case "html": context.html($html); break;
            }

            if (notify != false) {
                context.notifyUpdate();
            }

            return context;
        },
        notifyUpdate: function () {
            var $this = $(this);
            $(document).trigger(jQuery.Event("htmlUpdated", { element: $this }));
            return $this;
        },
        htmlUpdated: function (fn) {
            return this.on("htmlUpdated", fn);
        },
        hashObjChanged: function (fn) {
            return this.on("hashobjchange", fn);
        },
        hasAnyClass: function () {
            var $this = $(this);
            for (var i = 0; i < arguments.length; i++) {
                if ($this.hasClass(arguments[i])) return true;
            }
            return false;
        },
        classList: function () { return this.attr('class').split(/\s+/); },
        findByClass: function (className, includeSelf) {
            return $(multivendorWeb.helpers.findByClass(className, this, includeSelf));
            //return $(this.get(0).getElementsByClassName(className));
        },
        findByTagName: function (tagName) {
            return $(this.get(0).getElementsByTagName(tagName));
        },
        bounds: function () {
            var bounds = this.get(0).getBoundingClientRect();
            if (bounds.width == undefined) {
                return { left: bounds.left, top: bounds.top, right: bounds.right, bottom: bounds.bottom, width: bounds.right - bounds.left, height: bounds.bottom - bounds.top };
            };
            if (this.length > 1) {
                let l = bounds.left, r = bounds.right, t = bounds.top, b = bounds.bottom;
                for (var i = 1; i < this.length; i++) {
                    bounds = this.get(i).getBoundingClientRect();
                    if (bounds.left < l) { l = bounds.left };
                    if (bounds.right > r) { r = bounds.right };
                    if (bounds.top < t) { t = bounds.top };
                    if (bounds.bottom > b) { b = bounds.bottom };
                }
                return { left: l, top: t, right: r, bottom: b, width: r - l, height: b - t };
            }
            return bounds;
        },
        maxWidth: function () {
            return Math.max.apply(Math, $.map($(this), function (item) { return $(item).bounds().width; }));
            //return Math.max.apply(Math, $.map($(this), function (item) { return $(item).width(); }));
        },
        maxHeight: function () {
            return Math.max.apply(Math, $.map($(this), function (item) { return $(item).bounds().height; }));
            //return Math.max.apply(Math, $.map($(this), function (item) { return $(item).height(); }));
        },
        selectMany: function (selector) {
            var array = [];
            $(this).each(function () {
                var items = selector($(this));
                if ($.isArray(items) == true) {
                    for (var i; i < items.length; i++) {
                        array.push(items[i]);
                    }
                } else {
                    array.push(items);
                }
            });
            return array;
        },
        reverse: [].reverse,
        selectionChanged: function (fn) {
            return this.on("selectionChanged", fn);
        },
        raiseSelectionChanged: function (prevSelection, currentSelection, always) {
            var selected = multivendorWeb.helpers.substract(currentSelection, prevSelection);
            var unselected = multivendorWeb.helpers.substract(prevSelection, currentSelection);

            if (always === true || selected.length || unselected.length) {
                $(this).trigger("selectionChanged", { selection: currentSelection, selected: selected, unselected: unselected });
            }
        }, 
        mergeAttributes: function (attributes, overwrite) {
            var $this = $(this);
            $.each(attributes instanceof jQuery ? attributes.prop("attributes") : attributes, function () {
                if (overwrite != false || (this.name == "class" || $this.attr(this.name) == null)) {
                    if (this.name != "class") {
                        $this.attr(this.name, this.value);
                    } else {
                        $this.addClass(this.value);
                    }
                }
            });
        },
        getPropertyValue: function (name) {
            var $this = $(this);
            var e = $this.get(0);
            var style = e.currentStyle;
            if (style != null) {
                return style.getAttribute(name);
            } else {
                style = window.getComputedStyle(e);
                return style.getPropertyValue("--" + name);
            }
        },
        getPropertyValues: function () {
            var result = {};
            var $this = $(this);
            var e = $this.get(0);
            var style = e.currentStyle;
            if (style != null) {
                for (var i = 0; i < arguments.length; i++) {
                    var name = arguments[i];
                    result[name] = style.getAttribute(name);
                }
            } else {
                style = window.getComputedStyle(e);
                for (var i = 0; i < arguments.length; i++) {
                    var name = arguments[i];
                    result[name] = style.getPropertyValue("--" + name);
                }
            }
            return result;
        },
        getPresentionContext: function(views, searchPopup) {
            var $this = $(this);
            let context = {};

            let find = function ($that, ctx) {
                if (ctx == null) ctx = {};
                let $closestPresentationView = $that.closest(".presentation-view[data-presentation-id]"); // could be itself

                if ($closestPresentationView.length) {
                    ctx.presentationId = $closestPresentationView.data("presentation-id");
                    if (views) {
                        ctx.$presentation = $closestPresentationView;
                    }

                    let $closestPartModuleView = $closestPresentationView.closest(".part-assembly-view[data-presentation-id]");
                    if ($closestPartModuleView.length) {
                        let partModuleId = $closestPartModuleView.data("presentation-id");
                        if (partModuleId != ctx.presentationId) {
                            ctx.partModuleId = partModuleId;
                            if (views) {
                                ctx.$partModule = $closestPartModuleView;
                            }
                        }
                    }

                    let $closestCatalogueView = $closestPresentationView.closest(".catalogue-view[data-presentation-id]");
                    if ($closestCatalogueView.length) {
                        let catalogueId = $closestCatalogueView.data("presentation-id");
                        if (catalogueId != ctx.presentationId) {
                            ctx.catalogueId = catalogueId;
                            if (views) {
                                ctx.$catalogue = $closestCatalogueView;
                            }
                        }
                    }
                }

                //var $rowPresentationView = $that.closest(".row-presentation-view[data-row-id]");
                //if ($rowPresentationView.length) {
                //    if (views) {
                //        ctx.$rowPresentation = $rowPresentationView;
                //    }
                //    ctx.rowId = $rowPresentationView.data("row-id");
                //}

                //if (views || !ctx.rowId) {
                let $tableRow = $that.closest(".part-assembly-row-table tr[data-row-id]");
                if ($tableRow.length) {
                    ctx.$partModuleTableRow = $tableRow;
                    if (!ctx.rowId) {
                        let rowId = $tableRow.data("row-id");
                        if (rowId) {
                            ctx.rowId = rowId;
                        }
                    }
                }
                //}

                let $closestNodeView = $that.closest(".catalogue-node-view");
                if ($closestNodeView.length) {
                    if (views) {
                        ctx.$catalogueNode = $closestNodeView;
                    }
                    ctx.catalogueNodeId = $closestNodeView.data("node-id");
                }
                return ctx;
            }

            if (searchPopup !== false) {
                let $closestPopUpOwner = $this.closest(".multivendow-web-popup").data("owner");
                if ($closestPopUpOwner != null && $closestPopUpOwner.length) {
                    $.extend(context, find($closestPopUpOwner), find($this));
                } else {
                    find($this, context);
                }
            } else {
                find($this, context);
            }

            let hash = multivendorWeb.hashObj;
            if (hash) {
                if (!context.catalogueNodeId && hash.node) {
                    context.catalogueNodeId = hash.node;
                }
                if (!context.partModuleRowId && hash.rowView) {
                    context.partModuleRowId = hash.rowView;
                }
            }
            return context;
        }
    });

    if (!document.getElementsByClassName) {

        if (document.querySelectorAll) {
            document.getElementsByClassName = function (className) {
                return this.querySelectorAll("." + className);
            };
        } else {
            document.getElementsByClassName = function (className) {
                return $(this).find("." + className);
            };
        }

        Element.prototype.getElementsByClassName = document.getElementsByClassName;
    }

    $(window).on("hashchange", function () {
        multivendorWeb.hashObj = multivendorWeb.getHashObj();
        $(document).trigger("hashobjchange");
    });

    multivendorWeb.hashObj = multivendorWeb.helpers.toObject(window.location.hash);

    multivendorWeb.getHashObj = function () {
        return multivendorWeb.helpers.toObject(window.location.hash);
    };

    multivendorWeb.setHashObj = function (hashObj) {
        window.location.hash = $.param(hashObj, false).replace(/\+/g, "%20");
        multivendorWeb.hashObj = multivendorWeb.getHashObj();
    };

    multivendorWeb.mergeHashObj = function (obj) { // Trigger happy? Check for change?
        if (obj) {
            window.location.hash = $.param($.extend(multivendorWeb.hashObj || {}, obj), false).replace(/\+/g, "%20"); // HACK!
            multivendorWeb.hashObj = multivendorWeb.getHashObj();
        }
    };

    multivendorWeb.removeHashKey = function (key) {
        if (key && multivendorWeb.hashObj) {
            delete multivendorWeb.hashObj[key];
            multivendorWeb.setHashObj(multivendorWeb.hashObj);
        }
    }

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));