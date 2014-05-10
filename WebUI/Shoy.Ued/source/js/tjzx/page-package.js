(function ($, T) {
    var size = 10;
    var h = new hTemplate({
        tmp: $("#h-temp").html(),
        empty: $("#h-empty").html(),
        container: $(".t-package-list"),
        pager: $(".h-pager"),
        size: size,
        pageClick: function (page) {
            getList(page - 1);
            return false;
        }
    });
    var getList = function (page) {
        var reg = /\/packages\/(\d+)/;
        var id = 0;
        if (reg.test(location.href)) {
            id = RegExp.$1 || 0;
        }
        T.getJson("/package_list", {
            id: id,
            page: page,
            size: size
        }, function (json) {
            if (json && json.state) {
                var list = [];
                for (var i = 0; i < 32; i++)
                    list.push(json.data.list[0]);
                h.bind(list, 259);
                $("body,html").animate({scrollTop: 0}, $(window).scrollTop() / 5);
                $(".t-package-list li")
                    .bind("mouseenter", function () {
                        var $t = $(this);
                        $t.css("z-index", 15).addClass("active").find(".p-feature").fadeIn("fast",function(){
                            //避免移动过快的bug
                            $t.css("z-index", 15);
                        });
                    })
                    .bind("mouseleave", function () {
                        var $t = $(this);
                        $t.removeClass("active").find(".p-feature").fadeOut("fast", function () {
                            $t.css("z-index", 10);
                        });
                    });
            }
        });
    };
    getList(0);
})(jQuery, TJZX);