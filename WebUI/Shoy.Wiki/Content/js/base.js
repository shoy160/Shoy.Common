(function ($, S) {
    var loadTheme = function () {
        var mode = S.cookie.get("theme-mode");
        if (!mode) return false;
        var $style = $("#theme-style");
        if (!$style.length) {
            $style = $('<link id="theme-style" rel="stylesheet" />');
            $("head").append($style);
        }
        $style.attr("href", "/Content/css/theme/" + mode + ".css");
        var $t = $(".theme-mode li[data-style='" + mode + "']");
        $t.addClass("current").siblings().removeClass("current");
        return true;
    };
    var loadSidebar = function (toggle) {
        var status = S.cookie.get("sidebar-mode") || 0;
        if (toggle) {
            status = !(status === 1);
            S.cookie.set("sidebar-mode", (status ? 1 : 0), 60 * 24 * 90);
        }
        var $container = $(".page-container");
        $container.toggleClass("sidebar-closed", (status == 1));
    };
    loadTheme();
    loadSidebar();
    /**
     *初始化状态
     */
    var locationHref = window.location.href;
    if (window.location.pathname == "/") {
        locationHref += "home";
    }
    $(".page-sidebar>ul>li>a").each(function () {

        if (locationHref.indexOf($(this).attr("href")) > 0) {
            $(this).parent().addClass("active open");
            $(this).append("<span class='m-caret fa fa-caret-left'></span>");

            $("#navigation .page-title span").html($(this).text());
            $("#navigation .page-title small").html($(this).attr("title") || "");
            $("#navigation .breadcrumb li:eq(1) span").html($(this).text());
            $("#navigation .breadcrumb li:eq(1) i").remove();
            $("#navigation .breadcrumb li:eq(2)").remove();

            //document.title = $(this).text() + " - " + document.title;

            return false;
        } else {
            var parent = $(this);
            $(this).next("ul").each(function () {
                $("a", $(this)).each(function () {
                    if (locationHref.indexOf($(this).attr("href")) > 0) {
                        $(this).parent().addClass("active");

                        parent.parent().addClass("active open");
                        $(".arrow", parent).addClass("open").before("<span class='m-caret fa fa-caret-left'></span>");

                        $("#navigation .page-title span").html($(this).text());
                        $("#navigation .page-title small").html($(this).attr("title") || "");
                        $("#navigation .breadcrumb li:eq(1) span").html(parent.text());
                        $("#navigation .breadcrumb li:eq(2) span").html($(this).text());

                        //document.title = $(this).text() + " - " + document.title;

                        return false;
                    }
                });
            });
        }
    });
    $('.page-sidebar .has-sub > a').click(function () {
        var handleContentHeight = function () {
            var content = $('.page-content');
            var sidebar = $('.page-sidebar');

            if (!content.attr("data-height")) {
                content.attr("data-height", content.height());
            }


            if (sidebar.height() > content.height()) {
                content.css("min-height", sidebar.height() + 20);
            } else {
                content.css("min-height", content.attr("data-height"));
            }
        }
        $(this).blur();
        var $t = $(this).parent(),
            slideTime = 200,
            $sub = $t.find(".sub"),
            $arrow = $t.find(".arrow"),
            openCss = "open";
        if ($t.hasClass(openCss)) {
            $arrow.removeClass(openCss);
            $sub.slideUp(slideTime, function () {
                handleContentHeight();
                $t.removeClass(openCss);
            });
        } else {
            $arrow.addClass(openCss);
            var $last = $t.siblings('.open');
            if ($last.length) {
                $last.each(function (i, item) {
                    var $item = $(item);
                    if (!$item.hasClass("active")) {
                        $item.find(".arrow").removeClass(openCss);
                        $item.find(".sub").slideUp(slideTime, function () {
                            $item.removeClass(openCss);
                        });
                    }
                });
            }
            $sub.slideDown(slideTime, function () {
                handleContentHeight();
                $t.addClass(openCss);
            });
        }
    });
    $(".sidebar-toggler").bind("click", function () {
        loadSidebar(true);
    });
    $(".btn-theme").bind("click", function () {
        var $t = $(this),
            $mode = $t.siblings('.theme-mode'),
            status = $t.hasClass("btn-theme-active");
        if (status) {
            $t.removeClass("btn-theme-active").html('<i class="fa fa-cog"></i>');
            $mode.addClass("hide");
        } else {
            $t.addClass("btn-theme-active").html('<i class="fa fa-times"></i>');
            $mode.removeClass("hide");
        }
    });
    $(".theme-mode li").bind("click", function () {
        var $t = $(this),
            mode = $t.data("style"),
            isCurrent = $t.hasClass("current");
        if (isCurrent) return false;
        S.cookie.set("theme-mode", mode, 60 * 24 * 90);
        loadTheme();
        $(".btn-theme").click();
        return true;
    });
    $(".theme-fixed input[type=checkbox]").bind("change", function () {
        var fixed = this.checked;
        $("body").toggleClass("fixed-top", fixed);
        $(".navbar-inverse").toggleClass("navbar-fixed-top", fixed);
    });
})(jQuery, SINGER)