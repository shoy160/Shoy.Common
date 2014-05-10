(function ($, T) {
    var h = new hTemplate({
        tmp: $("#h-temp").html(),
        empty: $("#h-empty").html(),
        container: $(".t-news-list"),
        pager: $(".h-pager"),
        size: 10,
        pageClick: function (page) {
            getList(page);
            $("body,html").animate({scrollTop: 0}.$(window).scrollTop() / 5);
        }
    });
    var getList = function (page) {
        T.getJson("/news_list", {
            page:page,
            size:10
        },function(json){
            if(json && json.state){
                h.bind(json.data.list,json.data.count);
            }
        });
    };
    getList(0);
})(jQuery, TJZX);