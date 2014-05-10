(function ($, T) {
    //百度编辑器
    window.UEDITOR_CONFIG.autoHeightEnabled = false;
    var editor = UE.getEditor("content"),
        contentCache = {};
    var loadItem = function (type) {
        if (contentCache.hasOwnProperty(type)) {
            editor.setContent(contentCache[type]);
            return false;
        }
        T.getJson("/m/process/item", {
            type: type
        }, function (json) {
            if (json && json.state) {
                var content = json.data.content;
                contentCache[type] = content;
                editor.setContent(content);
            }
        });
    };
    $(".m-panel-title")
        .undelegate("li", "click")
        .delegate("li", "click", function () {
            if ($(this).hasClass("active")) return false;
            var $t = $(this),
                $label = $(".m-form-label:eq(0)");
            $t.addClass("active").siblings("li").removeClass("active");
            $label.html($t.text());
            $('input[name="type"]').val($t.data("type"));
            loadItem($t.data("type"));
        });
    $(".j-submit").bind("click", function () {
        if ($(this).hasClass("disabled")) return false;
        var $t = $(this);
        T.setBtn($t, false);
        var formData = {
            type: $('input[name="type"]').val(),
            content: encodeURIComponent(editor.getContent())
        };
        T.getJson("/m/process/update", formData, function (json) {
            if (json.state) {
                contentCache[formData.type] = decodeURIComponent(formData.content);
                T.msg("提交成功");
            } else {
                T.msg(json.msg || "操作异常！");
            }
            T.setBtn($t, true);
        }, function () {
            T.setBtn($t, true);
        });
        return false;
    });
})(jQuery, TJZX);