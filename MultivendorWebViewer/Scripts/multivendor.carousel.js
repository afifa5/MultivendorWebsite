(function (multivendorWeb, $, undefined) {


    multivendorWeb.carousel = {

        className: "multivendor-web-carousel",

        getControls: function (ctx) {
            return $(multivendorWeb.helpers.findByClass(this.className, ctx, true));
        },

        init: function (controls) {
            if (!controls.length) return;

            controls.each(function () {
                var $control = $(this);

                if ($control.parents('.multivendor-web-carousel').length==0) {
                    $control.bind('touchstart', function (event) { startTouch(event, $control); });
                    $control.bind('touchmove', function (event) { moveTouch(event, $control); });
                    $control.bind('touchend', function (event) { touchEnd(event, $control); });
                }

                if (getSelectedSlideIndex($control) == -1) {
                    setSelectedSlideIndex(0, $control);
                }
                //var $slidesContainer = $control.children(".slides-container");
                //var $slides = $slidesContainer.children(".slide");

                // click on the navigation button
                $control.on("click", "> .nav", function (e) {
                    var wrap = $control.hasClass("wrap-navigation");
                    var $nav = $(this);
                    if ($nav.hasClass("next") && (!$nav.hasClass("last") || wrap)) offsetSelection(1, $control);
                    else if ($nav.hasClass("prev") && !$nav.hasClass("first") || wrap) offsetSelection(-1, $control);
                    return false;
                });

                // click on a thumbnail
                $control.on("click", "> .thumbnails-container > .thumb", function (e) {
                    setSelectedSlideIndex($(this).data("index"), $control);
                    return false;
                });
                
                // click on an indicator
                $control.on("click", "> .indicators-container > *", function (e) {
                    setSelectedSlideIndex($(this).index(), $control);
                    return false;
                });

                $control.on("click", "> .indicators-container", function (e) {
                    return false;
                });

                // if the carosuell if in slide mode, observe change of size of the carousell
                if ($control.data("change-mode") == "slide" && $control.children(".slides-container").children(".slide").length > 1) {
                    getResizeObserver().observe(this);
                }

                // run an interval timer if the carsousell should auto slide
                var autoSlide = $control.data("auto-slide");
                if (autoSlide > 0) {
                    var intervalArgs = {};
                    var intervalId = setInterval(function(args) {
                        if ($.contains(document.body, $control.get(0)) == true) {
                            offsetSelection(1, $control);
                        } else {
                            clearInterval(args.Id);
                        }
                    }, autoSlide, intervalArgs);
                    intervalArgs.Id = intervalId;
                }

                var mouseoverZoom = $control.data("mouseover-zoom");
                if (mouseoverZoom > 1) {
                    $control.on("mousemove", "> .slides-container > .slide.active", function (e) {
                        //var $slide = $control.find("> .slides-container > .slide.active");
                        var $slide = $(this);

                        var controlBound = $control.bounds();
                        var x = e.offsetX;// - controlBound.x;
                        var y = e.offsetY;// - controlBound.y;

                        $slide.css("transform-origin", x + "px " + y + "px");
                        $slide.css("transform", "scale(" + mouseoverZoom + ")");
                    });

                    $control.on("mouseleave", "> .slides-container > .slide.active", function (e) {
                        var $slide = $(this);
                        $slide.css("transform-origin", "");
                        $slide.css("transform", "");
                    });
                }

                $control.removeClass("init");

            });
        },

        // Selected specified slide
        select: function ($slide) {
            selectSlide($slide, $slide.closest("." + multivendorWeb.carousel.className));
        },

        // Select next slide. Wrap if neccesary and enabled.
        selectNext: function ($control) {
            offsetSelection(1, $control);
        },

        // Select previous slide. Wrap if neccesary and enabled.
        selectPrevious: function ($control) {
            offsetSelection(-1, $control);
        }
    }

    // Set the selected slide by a offset (from current selected index)
    function offsetSelection(offset, $control) {       
        $control.children(".slides-container").removeClass("ignore-transition"); 
        var index = getSelectedSlideIndex($control);
        setSelectedSlideIndex(index + offset, $control);
    }

    // Get the index of selected slide
    function getSelectedSlideIndex($control) {
        var index = $control.find("> .slides-container > .slide.active").index();
        return index >= 0 ? index : -1;
    }

    //Get total number of slides
    function getTotalSlides($control) {
        var count = $control.find("> .slides-container > .slide").length;
        return count;
    }

    // Get the selected slide by index
    function setSelectedSlideIndex(index, $control) {
        var $slides = $control.children(".slides-container").children(".slide");
        if (index < 0) {
            if ($control.hasClass("wrap-navigation")) {
                index = $slides.length - 1;
            } else index = 0;
        } else if (index >= $slides.length) {
            if ($control.hasClass("wrap-navigation")) {
                index = 0;
            } else index = $slides.length - 1;
        } 

        var $slide = $slides.eq(index);
        selectSlide($slide, $control);

        /*var $slide = $slides.eq(index);
        if ($slide.length == 0) { // if no valid slide exists on specified index
            if ($control.hasClass("wrap-navigation") == false) {
                return;
            } else {
                // in wrap mode, select last slide if specified index < 0, otherwise select first
                $slide = index < 0 ? $slides.last() : $slides.first();
            }
        }
        if ($slide.length > 0) {
            selectSlide($slide, $control);
        }*/
    }

    function positionSlideControl($control, $slidesContainer, $slides, index) {
        if ($slides.length > 1) {
            var slideBounds = $.map($slides, function (s) {
                var $s = $(s);
                return { $slide: $s, bounds: $s.bounds() };
            });

            var firstSlideInfo = slideBounds[0];
            var firstLeft = firstSlideInfo.bounds.left;
            var offset = firstLeft - slideBounds[index].bounds.left;

            if ($control.hasClass("all-visible") == true || $control.hasClass("partial-visible") == true) {
                var controlWidth = $control.get(0).clientWidth;
                var lastIndex = slideBounds.length - 1;
                var lastSlideInfo = slideBounds[lastIndex];
                var totalSlidesWidth = lastSlideInfo.bounds.right  - firstLeft;
                if (totalSlidesWidth > controlWidth && lastSlideInfo.bounds.right - firstLeft + offset < controlWidth) { // The last slide is to far to left
                    offset = controlWidth - (lastSlideInfo.bounds.right - firstLeft);
                }
                var $indicators = $control.children(".indicators-container").children();
                var allFullyVisible = true;
                for (var i = 0; i < slideBounds.length; i++) {
                    var slideInfo = slideBounds[i];
                    var bounds = slideInfo.bounds;
                    var fullyVisible = false;
                    if (bounds.left - firstLeft + offset >= 0 && bounds.left - firstLeft + offset <= controlWidth) {
                        // fully or partially visibe
                        fullyVisible = bounds.right - firstLeft + offset <= controlWidth + 1;
                        slideInfo.$slide.toggleClass("fully-visible", fullyVisible).toggleClass("partially-visible", !fullyVisible);
                        $indicators.eq(i).toggleClass("fully-visible", fullyVisible).toggleClass("partially-visible", !fullyVisible);
                        if (fullyVisible == false) {
                            allFullyVisible = false;
                        }
                    } else {
                        var partiallyVisible = bounds.right - firstLeft + offset > 0 && bounds.right - firstLeft + offset <= controlWidth;
                        slideInfo.$slide.toggleClass("fully-visible", false).toggleClass("partially-visible", partiallyVisible);
                        $indicators.eq(i).toggleClass("fully-visible", false).toggleClass("partially-visible", partiallyVisible);
                        allFullyVisible = false;
                    }
                    if (i == 0) {
                        $control.toggleClass("first-fully-visible", fullyVisible);
                    } else if (i == lastIndex) {
                        $control.toggleClass("last-fully-visible", fullyVisible);
                    }
                    $control.toggleClass("all-fully-visible", allFullyVisible);
                }
            }

            return offset;
        } 
        return null;
    }

    // Selected specified slide
    function selectSlide($slide, $control) {
        var $slidesContainer = $control.children(".slides-container");
        var $slides = $slidesContainer.children(".slide");
        var index = $slide.index();

        var offset = null;

        // position slide control
        if ($control.data("change-mode") == "slide") {
            offset = positionSlideControl($control, $slidesContainer, $slides, index);
        }

        if ($slide.hasClass("active") == false) { // Set active state on slide, indicator and thumbnails
            $slides.removeClass("active"); // remove .active on all slides
            $slide.addClass("active");  // add .active to active slide
            // remove .active on all indicators and then add .active to the active indicator (by index)
            $control.children(".indicators-container").children().removeClass("active").eq(index).addClass("active");
            // remove .active on all thumbnails and then add .active to the active thumbnail (by sepcified index)
            $control.children(".thumbnails-container").children(".thumb").removeClass("active").filter("[data-index=" + index + "]").addClass("active");
            // toggle .first and .last to indicate if first or last slide is active
            $control.toggleClass("first", index == 0).toggleClass("last", index == $slides.length - 1);
        }

        $control.attr('data-translate-x', offset);

        if (offset != null) {
            $slidesContainer.css("transform", "translateX(" + offset + "px)");
        }
    }

    // If we have a resize of the carousell control (in slide mode), we need to recalculate the pixel offset of the slides
    function resize(items) {
        for (var j = 0; j < items.length; j++) {
            var $control = $(items[j].target);

            if ($.contains(document.body, $control.get(0)) == false) {
                sharedResizeObserver.unobserve($control.get(0));
            }

            var $slidesContainer = $control.children(".slides-container");
            var $slides = $slidesContainer.children(".slide");
            var $slide = $slides.filter(".active");

            //var offset = $slides.first().offset().left - $slides.filter(".active").offset().left;
            var offset = positionSlideControl($control, $slidesContainer, $slides, $slide.index());

            if (offset != null) {
                $slidesContainer.addClass("ignore-transition"); // add a class to temporary suspend any transition animation
                $slidesContainer.css("transform", "translateX(" + offset + "px)");
                setTimeout(function () { // remove the suspension of transition animation. we cannot remove the class imidiate, thus a 0 timeout is used to let the offset take first.
                    $slidesContainer.removeClass("ignore-transition");
                }, 0);
            }
        }
    }

    var sharedResizeObserver = null;
    // helper method to get the carousell resize observer
    function getResizeObserver() {
        if (sharedResizeObserver == null) {
            sharedResizeObserver = multivendorWeb.resize.createResizeObserver(resize);
        }
        return sharedResizeObserver;
    }

    // Init controls

    $(function () {
        multivendorWeb.carousel.init(multivendorWeb.carousel.getControls());
    });

    $(document).htmlUpdated(function (e) {
        multivendorWeb.carousel.init(multivendorWeb.carousel.getControls(e.element));
    });


    function GetPerPixelSwipeSpeed($control,swipeTime) {
        var currentTranX = parseInt($control.attr('data-current-translate-x'));
        var previousTranslateX = parseInt($control.attr('data-translate-x'));
        var pixelPerMs = Math.abs(swipeTime / (Math.abs(currentTranX) - Math.abs(previousTranslateX)));

        //console.log("pixel per ms:" + pixelPerMs);
        return pixelPerMs;
    }

    function touchEnd(e, $control) {

        var startTime = $control.attr('data-start-touch-time');
        var endTime = GetCurrentTimeUniqueNumber();
        var swipeTime = endTime - startTime;

        var pixelPerMs = GetPerPixelSwipeSpeed($control,swipeTime);

        var previousTranslateX = parseInt($control.attr('data-current-translate-x'));
        $control.attr('data-translate-x', previousTranslateX);
        var $slidesContainer = $control.children(".slides-container").first();

        //aligning to the border if transition is out of bound


        if (previousTranslateX > 0 || previousTranslateX < -1 * (getSlideContainerWidth($control) - $control.width())) {
            var direction = "left";

            if (previousTranslateX > 0) {
                direction = "right";
            }


            applyTransition($control, $slidesContainer, previousTranslateX, 0, direction, 1000, true)
        }
        //----------------------------------------


        var initialX = $control.attr('data-start-touch-x');
        var initialY = $control.attr('data-start-touch-y');

        currentX = $control.attr('data-current-touch-x');
        currentY = $control.attr('data-current-touch-y');



        var diffX = initialX - currentX;
        var diffY = initialY - currentY;
        var index = getSelectedSlideIndex($control);
        var totalSlides = getTotalSlides($control);
        var controlWidth = $control.width();

        var startTime = $control.attr('data-start-touch-time');
        var endTime = GetCurrentTimeUniqueNumber();
        var swipeTime = endTime - startTime;

        var timeFactor = swipeTime < 250 ? true : false;

        var initialX = $control.attr('data-start-touch-x');
        var initialY = $control.attr('data-start-touch-y');

        currentX = $control.attr('data-current-touch-x');
        currentY = $control.attr('data-current-touch-y');



        var diffX = initialX - currentX;
        var diffY = initialY - currentY;
        var index = getSelectedSlideIndex($control);
        var totalSlides = getTotalSlides($control);
        var controlWidth = $control.width();

        var $slidesContainer = $control.children(".slides-container").first();

        if (Math.abs(diffX) > Math.abs(diffY)) {

            e.preventDefault();

            var areafactor = Math.ceil(((Math.abs(diffX) / controlWidth) * 100) / 10);
            var timeBonus = 1;

            if (swipeTime < 200) { timeBonus = 0; }

            //console.log("swipeTime:" + swipeTime);
            //console.log("time bonus:" + timeBonus)
            //console.log("areafactor:" + areafactor)
            //console.log("timeFactor:" + timeFactor)

            if (timeFactor == false)
            {
                $slidesContainer.css({ "transition": $control.attr('data-default-transition'), });
                for (var i = 1; i < 3; i++) {
                    sleep(function () { updateIndicators($control); }, 510, i);
                }
                return;
            }
            else {
                diffX = (areafactor + timeBonus + Math.ceil(1000 / swipeTime) * diffX);
            }

            // sliding horizontally
            var swipeDirection = "";

            if (diffX > 0) {
                // swiped left

                swipeDirection = "left";

            } else {
                // swiped right
                swipeDirection = "right";
            }

            var transitionTime = Math.abs(pixelPerMs * diffX);

            //console.log("new diffx:" + diffX+" , new transition time:" + transitionTime)

            newTranslateX = applyTransition($control, $slidesContainer, previousTranslateX, diffX, swipeDirection, transitionTime, true);
            
            for (var i = 1; i < 21; i++) {
                sleep(function () { updateIndicators($control); }, (transitionTime/20), i);
            }

            updateIndicators($control)

            //console.log("previous:" + previousTranslateX + " , diff:" + diffX + "new translateX:" + newTranslateX);

            $control.attr('data-translate-x', newTranslateX);

            $slidesContainer.css({ "transition": $control.attr('data-default-transition'), });
        } else {
            // sliding vertically
            if (diffY > 0) {
                // swiped up

                // console.log("swiped up");
            } else {
                // swiped down

                // console.log("swiped down");
            }
        }

        //console.log("difference" + diffY);

        $control.removeAttr('data-start-touch-x');
        $control.removeAttr('data-start-touch-y');
        $control.removeAttr('data-current-touch-x');
        $control.removeAttr('data-current-touch-y');



        //sleep(function () {


    }

    function startTouch(e, $control) {

        $control.attr('data-start-touch-x', e.originalEvent.touches[0].pageX);
        $control.attr('data-start-touch-y', e.originalEvent.touches[0].pageY);
        $control.attr('data-start-touch-time', GetCurrentTimeUniqueNumber());

        var $slidesContainer = $control.children(".slides-container").first();
        $control.attr('data-default-transition', $slidesContainer.css("transition"));

        //console.log("stop");
        var slideActive = $slidesContainer.find('.slide.active');
        if (slideActive.length > 0) {
            var anchorElement = slideActive.find('a');
            if ( !anchorElement.length || anchorElement.attr("href") == undefined || anchorElement.attr("href") == "#")
            {
                $slidesContainer.css({ "transform": "translateX(" + $slidesContainer.offset().left + "px)", "transition": "0s" });
                $control.attr('data-translate-x', $slidesContainer.offset().left);
            }
        }

    }


    function moveTouch(e, $control) {

        var initialX = $control.attr('data-start-touch-x')
        var initialY = $control.attr('data-start-touch-y')

        var currentX = e.originalEvent.touches[0].pageX;
        var currentY = e.originalEvent.touches[0].pageY;

        $control.attr('data-current-touch-x', currentX);
        $control.attr('data-current-touch-y', currentY);

        var diffX = initialX - currentX;
        var diffY = initialY - currentY;

        if (Math.abs(diffX) > Math.abs(diffY)) {

             // swiped left or right

            if (e.cancelable) {
                e.preventDefault();
            }
            //console.log(diffX)

            var moveX = (diffX * -1);

            var $slidesContainer = $control.children(".slides-container").first();
            var previousTranslateX = parseInt($control.attr('data-translate-x'));
            var newTranslateX = 0;

            if (previousTranslateX == null)
                previousTranslateX = 0;

            var swipeDirection = "";

            if (diffX > 0) {
                // swiped left

                swipeDirection = "left";

            } else {
                // swiped right
                swipeDirection = "right";
            } 

            newTranslateX=applyTransition($control, $slidesContainer, previousTranslateX, diffX, swipeDirection, 0,false)

            //console.log("previous:" + previousTranslateX + " , diff:" + diffX + "new translateX:" + newTranslateX);

            $control.attr('data-current-translate-x', newTranslateX);

            updateIndicators($control);
        }
        else {
                // swiped up or down
        }


    }


    //function sleep(ms) {
    //    return new Promise(resolve => setTimeout(resolve, ms));
    //}

    function updateIndicators($control) {

        var $indicatorsContainer = $control.children(".indicators-container").first();

        var $indicators = $indicatorsContainer.children('div');
        $indicators.removeClass("fully-visible active partially-visible");

        var $slidesContainer = $control.children(".slides-container").first();;
        var $slides = $slidesContainer.children(".slide");
        $slides.removeClass("fully-visible active partially-visible"); 

        offset = $control.offset();

        var containerLeft = offset.left;
        var containerTop = offset.top;
        var containerRight = $control.width();
        var containerBottom = $control.height();

        var active = false;
        var fullyVisible =[];
        var partiallyVisible = [];
        var visiblityRaioDiff = [];


        $slides.each(function () {

            var $indexSlide = $(this);

            var slideLeft = $(this).offset().left;
            var slideRight = ($(this).offset().left + $(this).width());

            //Fully visible
            if ((slideLeft >= containerLeft && slideRight <= containerRight))
            {
                $(this).addClass("fully-visible")
                var vIndex = $(this).index();
                fullyVisible.push(vIndex);

            }
            //partially visible 
            else if ((slideLeft >= containerLeft && slideLeft < containerRight)
                || slideRight <= containerRight && slideRight > containerLeft) {

                $(this).addClass("partially-visible")
                var vIndex = $(this).index();
                partiallyVisible.push(vIndex);

                //alert($(this).index());
            }


            visiblityRaioDiff.push(Math.abs(containerLeft - slideLeft) + Math.abs(containerRight - slideRight));
        });

        $indicators.each(
            function () {

                if (fullyVisible.includes($(this).index())) {
                    $(this).addClass("fully-visible")
                }
                else if (partiallyVisible.includes($(this).index()))
                {
                    $(this).addClass("partially-visible")
                }
            }
        );

        var activeIndex = visiblityRaioDiff.indexOf(Math.min.apply(this, visiblityRaioDiff));

        $slides.each(function () {
            if ($(this).index() == activeIndex) {

                $(this).addClass("active");
            }
        });


        $indicators.each(function () {
            if ($(this).index() == activeIndex) {

                $(this).addClass("active");
            }
        }

        );

        //if ($item != null) {
        //    $item.addClass("active");
        //}


        //if (active == false) {
        //    active = true;
        //    $(this).addClass("active")
        //    $indexSlide.addClass("active")
        //}

    }

    function applyTransition($control, $slidesContainer, previousTranslateX, diffX, swipeDirection,transitionSpeed,alignToBorder) {

        var newTranslateX = 0;

        if (swipeDirection == "left") {

            newTranslateX = previousTranslateX + (-1 * diffX);

            if (alignToBorder) {
                var minTransX = -1 * (getSlideContainerWidth($control) - $control.width());
                newTranslateX = newTranslateX < minTransX ? minTransX : newTranslateX;
            }
        }
        else {

            newTranslateX = previousTranslateX - diffX;

            if (alignToBorder) {
                newTranslateX = newTranslateX > 0 ? 0 : newTranslateX;
            }

        }

        $slidesContainer.css({ "transform": "translateX(" + (newTranslateX) + "px)", "transition": transitionSpeed+"ms" });

        return newTranslateX;
    }

    function getSlideContainerWidth($control) {

        var width = 0;
        var $slidesContainer = $control.children(".slides-container").first();

        if ($slidesContainer.width() == $control.width()) {
            var $slides = $slidesContainer.children(".slide");

            $slides.each(function () {
                width = $(this).width() + width;
            });

        } else {
            width = $slidesContainer.width();
        
        }

        return width;
    }


    function sleep(callback,ms,i) {
        setTimeout(function () { callback(); }, (ms*i)); 
    }

    function GetCurrentTimeUniqueNumber() {
        var date = new Date();
        var h = date.getHours();
        var m = date.getMinutes();
        var s = date.getSeconds();
        var ms = date.getMilliseconds();

        h == 0 ? 1 : h;
        m == 0 ? 1 : m;
        s == 0 ? 1 : s;

        var totalSeconds = (h * 3600) + (m * 60) + s;

        return ((totalSeconds * 1000) + ms)
    }

}(window.multivendorWeb = window.multivendorWeb || {}, jQuery));