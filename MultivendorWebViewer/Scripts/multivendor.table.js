(function (multivendorWeb, $, undefined) {

    multivendorWeb.table = {
        selector: ".multivendor-table",

        getTable: function (row) {
            return row ? row.closest(this.selector) : null;
        },

        getRows: function (table) {
            return table.children("tbody").children("tr");
        },

        getSelection: function (table) {
            return table ? table.find("tbody > tr.selected") : null;
        },

        clearSelection: function (table) {
            var rows = this.getSelection(table);

            if (rows && rows.length > 0) {
                rows.removeClass("selected");

                table.trigger("selectionChanged", { selection: rows, selected: $(), unselected: rows });
            }
        },

        findRows: function (table, rowFilter) {
            if ($.isFunction(rowFilter) || $.type(rowFilter) === "string") {
                if (table == null) return;
                return table.find("tbody > tr").filter(rowFilter);
            }
            return $();
        },

        select: function (rows, table, clearSelection, scrollTo) {

            if ($.isFunction(rows) || $.type(rows) === "string") {
                if (table == null) return;
                rows = table.find("tbody > tr").filter(rows);
            }
            table = table || this.getTable(rows);

            var prevSelection = this.getSelection(table);

            if (clearSelection != false) {
                prevSelection.removeClass("selected");
            }

            rows.addClass("selected");


            var newSelection = this.getSelection(table);
            table.raiseSelectionChanged(prevSelection, newSelection);

            if (scrollTo) {
                this.scrollTo(rows, table);
            }

            return newSelection;
        },

        unselect: function (rows, table) {
            if ($.isFunction(rows)) {
                if (table == null) return;
                rows = table.find("tbody > tr").filter(rows);
            }

            if (rows && rows.length > 0) {

                rows.removeClass("selected");

                table = table || this.getTable(rows);


                table.trigger("selectionChanged", { selection: this.getSelection(table), selected: $(), unselected: rows });
            }
        },

        scrollTo: function (row, table, force) {
            if (row && row.length) {

                table = table || this.getTable(row);

                //setTimeout(function () {
                //    var scrollContainer = table.offsetParent();
                //    if (scrollContainer.length) {
                //        var scrollTop = scrollContainer.offset().top;
                //        scrollContainer.scrollTop(row.position().top - 50);
                //    }
                //}, 50);

                setTimeout(function () {
                    row.get(0).scrollIntoView({ behavior: "smooth", block: "nearest" });

                    /*var scrollContainer = table.offsetParent();
                    if (scrollContainer.length) {
                        var rowTop = row.offset().top;
                        var scrollTop = scrollContainer.offset().top;

                        if (force || rowTop < scrollTop || rowTop + row.height() > scrollTop + scrollContainer.height()) {
                            scrollContainer.scrollTop(rowTop - table.offset().top);
                        }
                    }*/
                }, 10);
            }
        }
    };

    $(document).on("click", multivendorWeb.table.selector + " > tbody > tr:not(.none-selectable)", function (event) {
        var row = $(this);
        var table = multivendorWeb.table.getTable(row);
        if (table.length > 0) {
            var selectMode = table.data("selection-mode");
            if (selectMode == "multiple" && event.ctrlKey == true) {
                if (row.hasClass("selected") == false) {
                    multivendorWeb.table.select(row, table, false);
                } else {
                    multivendorWeb.table.unselect(row, table);
                }
            } else if (selectMode != "none") {
                multivendorWeb.table.select(row, table);
            }
        }
    });

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));