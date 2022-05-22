
(function (multivendorWeb, $, undefined) {
    $(document).ready(function () {
        var timeout=parseInt( document.getElementById('SessionTimeOutVal').value);
        var activeUser = document.getElementById('CurrentActiveUser').value;
        var flag = false;
        var sessionUrl = multivendorWeb.helpers.mergeParams($(document.body).data("session-timeout-url"), { iconImage: "info", header: "SessionExpired", message: "InactiveSessionExpire" });
        if(activeUser !=null && activeUser !='')
        {
            $.sessionTimeout({
                warnAfter: (timeout * 60000),
                redirAfter: (timeout * 60000),
                onRedir: function () {
                    multivendorWeb.popup.show(null, { url: sessionUrl /*$(document.body).data("session-timeout-url") + "?" + "iconImage=glyphicon-info-sign&header=SessionExpired&message=InactiveSessionExpire"*/, autoClose: false, modal: true, verticalAlign: "center", horizontalAlign: "center" /*left: 0, right: 0*/ }, function (html) {
                        var $popup = $(html);
                    });
                }
            });
        }
    });
   
}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));
