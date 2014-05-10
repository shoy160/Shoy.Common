var tjzx = window.TJZX = window.TJZX || {};
(function (S, T) {
    S.mix(T, {
        ie: getIeVersion(),
        bodyClass: getBodyClass(),
        wideMode: function () {
            if (!T.compatible) return false;
            var w = Math.max(document.documentElement.clientWidth, document.body.clientWidth),
                c = 'wide-0',
                m = 2,
                cls;
            if (w >= 1220) m = 1;
            if (w <= 1000) m = 3;
            cls = c + m;
            if (S.bodyClass)
                cls = S.bodyClass + ' ' + cls;
            document.getElementsByTagName("body")[0].className = cls;
        },
        /**
         * 加入收藏
         * @param title
         */
        favorite: function (title) {
            var d = location.host;
            var c = title || "四川省人民医院 - 健康体检中心，您的健康管理专家！";
            if (document.all) {
                window.external.AddFavorite(d, c);
            } else {
                if (window.sidebar && S.isFunction(window.sidebar.addPanel)) {
                    window.sidebar.addPanel(c, d, "");
                } else {
                    alert("对不起，您的浏览器不支持此操作！\n请使用菜单栏或按Ctrl+D收藏本站。");
                }
            }
            S.cookie.set("_singer_fv", "1", 30, location.host);
        }
    });
    function getIeVersion() {
        if (window.ActiveXObject) {
            var ua = navigator.userAgent.toLowerCase();
            var version = ua.match(/msie ([\d.]+)/)[1];
            if (version)
                return ~~version;
        }
    }

    function getBodyClass() {
        return document.getElementsByTagName("body")[0].className;
    }
})(SINGER, TJZX);