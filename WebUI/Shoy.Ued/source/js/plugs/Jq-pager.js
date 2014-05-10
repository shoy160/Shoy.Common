(function ($, f) {
    var emptyFn = function () {
        },
        tmps = {
            default: {
                tmp: '<div class="pager-left"></div> ' +
                    '<div class="pager-center">' +
                    '<ul>' +
                    '<li><a href="#" class="pager-bg pager-first"></a></li>' +
                    '<li><a href="#" class="pager-bg pager-prev"></a></li>' +
                    '<li><span class="pager-split"></span></li>' +
                    '<li><span>' +
                    '<input type="text" value="{{currentPage}}" class="pager-input pager-current" /> ' +
                    '</span></li>' +
                    '<li><span class="pager-pages"> / {{totalPage}}</span></li>' +
                    '<li><span class="pager-split"></span></li>' +
                    '<li><a href="#" class="pager-bg pager-next"></a></li>' +
                    '<li><a href="#" class="pager-bg pager-latest"></a></li>' +
                    '<li><span class="pager-info"> 共{{count}}条</span></li>' +
                    '</ul>' +
                    '</div> ' +
                    '<div class="pager-right"></div> ',
                item: ''
            }
        };
    String.prototype.format = function () {
        if (arguments.length <= 0) return this;
        var result = this,
            type = function (obj) {
                return Object.prototype.toString.call(obj).slice(8, -1).toLowerCase();
            };
        if (1 === arguments.length && "object" === type(arguments[0])) {
            for (var key in arguments[0]) {
                var reg = new RegExp("\\{\\{" + key + "\\}\\}", "gi");
                result = result.replace(reg, arguments[0][key]);
            }
        } else {
            for (var i = 0; i < arguments.length; i++) {
                var reg = new RegExp("\\{" + i + "\\}", "gi");
                result = result.replace(reg, arguments[i]);
            }
        }
        //未绑定的默认以空字符填充
        reg = new RegExp("(\\{[0-9]+\\})|(\\{\\{[0-9a-z]+\\}\\})", "gi");
        result = result.replace(reg, "");
        return result;
    };
    $.fn.pager = function (options) {
        var opts = $.extend({
            count: 0,
            size: 15,
            current: 1,
            skin: 'default',
            callback: emptyFn()
        }, options || {});
        var bindPage = function (container, tmp, info, pageItem) {
            var html = tmp.format(info),
                prev = !f,
                next = !f,
                $html = $(html);
            if (1 == info.currentPage) prev = f;
            if (info.currentPage == info.totalPage) next = f;
            if (prev) {
                $html.find(".pager-first").data("page", 1);
                $html.find(".pager-prev").data("page", info.currentPage - 1);
            } else {
                $html.find(".pager-first,.pager-prev").addClass("disabled");
            }
            if (next) {
                $html.find(".pager-latest").data("page", info.totalPage);
                $html.find(".pager-next").data("page", info.currentPage + 1);
            } else {
                $html.find(".pager-latest,.pager-next").addClass("disabled");
            }
            $html
                .delegate("a", "click", function () {
                    if($(this).hasClass("disabled")) return false;
                    var page = ~~$(this).data("page");
                    if (page) {
                        info.currentPage = page;
                        info.callback && "function" === typeof info.callback && info.callback(page);
                        bindPage(container, tmp, info, pageItem);
                    }
                    return false;
                })
                .delegate("input","change",function(){
                    var page=$(this).val();
                    if(page <=0) page=1;
                    if(page>info.totalPage) page=info.totalPage;
                    $(this).val(page);
                    info.currentPage = page;
                    info.callback && "function" === typeof info.callback && info.callback(page);
                    bindPage(container, tmp, info, pageItem);
                });
            container.html($html);
        };
        return $.each($(this), function () {
            var $t = $(this),
                ps = $.extend({}, opts),
                psData = $t.data("pager") || $t.attr("data-pager"),
                totalPage,
                currentPage,
                temp,
                pageItem;
            if (psData && "string" === typeof psData)
                psData = eval('(' + psData + ')');
            if ("object" === typeof psData)
                ps = $.extend(ps, psData);
            totalPage = (Math.ceil(ps.count / ps.size) || 1);
            currentPage = ps.current;
            if (currentPage <= 0) currentPage = 1;
            if (currentPage > totalPage) currentPage = totalPage;
            if (totalPage <= 0) totalPage = 1;
            temp = tmps[ps.skin].tmp;
            pageItem = function (n) {
                var pagerTmp = tmps[ps.skin].item;
                if (!pagerTmp) return "";
                return pagerTmp.format({"class": (n === current ? "active" : ""), "page": n});
            }
            bindPage($t, temp, {
                totalPage: totalPage,
                currentPage: currentPage,
                count: ps.count,
                callback: ps.callback
            }, pageItem);
        });
    };
    $(".j-pager").pager();
})(jQuery, false);