(function ($, T, S) {
    S.mix(T, {
        getJson: function (url, data, success, error) {
            S.mix(data, {t: Math.random()});
            $.ajax({
                url: url,
                type: "post",
                dataType: "json",
                data: data,
                success: function (json) {
                    success && S.isFunction(success) && success.call(this, json);
                },
                error: function (json) {
                    error && S.isFunction(error) && error.call(this, json);
                }
            });
        },
        /**
         * 图片加载异常处理
         * @param selector
         * @param callback
         */
        imageError: function (selector, callback) {
            $(selector)
                .unbind("error.imageError")
                .bind("error.imageError", function () {
                    try {
                        this.setAttribute("raw-src", this.src);
                        this.src = "http://ued.tjzx.com/blank.gif";
                        this.style.background = "url(http://ued.tjzx.com/default.gif) no-repeat scroll 50% 50% transparent";
                        this.style.backgroundSize = "contain";
                    } catch (e) {
                    }
                    callback && "function" === typeof callback && callback();
                })
            ;
        },
        /**
         * 返回顶部
         */
        goTop: function () {
            $('body,html').animate({ scrollTop: 0}, $(window).scrollTop() / 5);
        },
        /**
         * 侧边栏
         */
        setFeedback: function () {
            if (!T.feedback) return false;
            var feedback = T.feedback;
            var fixBox = $('<div class="t-feedback" />');
            $("body").append(fixBox);
            var fixRight = ($(document).width() - $(".t-box").width()) / 2 - 45;
            fixBox.css("right", fixRight + "px");
            if (feedback.goTop) {
                fixBox.append('<div class="t-goTop fb-item" title="返回顶部"><s class="icon icon-bigger">&#xf077;</s></div>');
                var goTop = fixBox.find(".t-goTop");
                goTop
                    .bind("click.goTop", function () {
                        T.goTop();
                    })
                    .bind("hover", function () {
                        $(this).toggleClass("t-goTop-hover");
                    });
            }
            $(window).bind("scroll.feedback", function () {
                var top = $(this).scrollTop();
                if (top > 150) {
                    fixBox.fadeIn();
                }
                else fixBox.fadeOut();
            });
        }
    });
    /**
     * 顶部菜单栏
     */
    $(".j-menu").hover(function () {
        $(this).toggleClass("menu-hover");
    });
    T.setFeedback();
})(jQuery, TJZX, SINGER);
