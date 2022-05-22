'use strict';
(function (multivendorWeb, $, undefined) {
    $.sessionTimeout = function (options) {
        var timeout = parseInt(document.getElementById('SessionTimeOutVal').value);
        var defaults = {
            warnAfter: timeout * 60000, // 15 minutes
            redirAfter: timeout * 60000, // 20 minutes
            ignoreUserActivity: false,
            onStart: false,
            onRedir: false
        };
        var opt = defaults,
            timer,
            countdown = {};
        // Extend user-set options over defaults
        if (options) {
            opt = $.extend(defaults, options);
        }
        // Reset timer on any of these events
        if (!opt.ignoreUserActivity) {
            var mousePosition = [-1, -1];
            $(document).on('keyup mouseup mousemove touchend touchmove', function(e) {
                if (e.type === 'mousemove') {
                    if (e.clientX === mousePosition[0] && e.clientY === mousePosition[1]) {
                        return;
                    }
                    mousePosition[0] = e.clientX;
                    mousePosition[1] = e.clientY;
                }
                startSessionTimer();
            });
        }

        function startSessionTimer() {
            // Clear session timer
            clearTimeout(timer);
            if (typeof opt.onStart === 'function') {
                opt.onStart(opt);
            }
            // Set session timer
            timer = setTimeout(function() {
            
                startDialogTimer();
            }, opt.warnAfter);
        }
        function startDialogTimer() {
            // Clear session timer
            clearTimeout(timer);
            // Set dialog timer
            timer = setTimeout(function() {
                // Check for onRedir callback function and if there is none, launch redirect
                if (typeof opt.onRedir !== 'function') {
                    window.location = opt.redirUrl;
                } else {
                    opt.onRedir(opt);
                }
            }, (opt.redirAfter - opt.warnAfter));
        }
        // Start session timer
        startSessionTimer();

    };
}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));
