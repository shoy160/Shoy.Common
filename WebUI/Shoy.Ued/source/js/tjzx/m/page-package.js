/**
 * 后台管理-体检套餐管理
 */
(function ($, T) {
    //初始化 分类
    T.getJson("/m/category/list", {
        state: 1
    }, function (json) {
        var list = json.data.list;
        if (json.state && list && list.length > 0) {
            var $sels = $("select[name='categoryId']");
            var html = "";
            for (var i = 0; i < list.length; i++) {
                html += singer.format("<option value='{id}'>{name}</option> ", list[i]);
            }
            $sels.append(html);
        }
    });
    //uploadify
    $("#fileUp").uploadify({
        'swf': '/uploadify/uploadify.swf',
        'uploader': '/m/package/uploadImage',
        'formData': {packageId: $("input[name=packageId]").val()},
        'buttonText': '选择文件',
        'fileTypeExts': '*.jpg;*.jpeg;*.gif;*.png',
        'fileTypeDesc': '图片文件',
        'fileSizeLimit': '500k',
        'multi': false,
        'fileObjName': 'fileData',
        'onUploadError': function () {

        },
        'onUploadSuccess': function (file, json) {
            json = eval('(' + json + ')');
            if (json && json.state) {
                $('input[name="picture"]').val(json.data.url);
                $(".t-package-img").html(singer.format('<img src="{0}" />', json.data.url));
                T.setFrameHeight();
            }
        },
        'onUploadComplete': function () {

        }
    });

    //百度编辑器
    window.UEDITOR_CONFIG.autoHeightEnabled = false;
    var editor = UE.getEditor("content");

    //form
    var vForm = $(".j-form").valid(),
        sForm = $(".s-form").valid({
            submit: function () {
                $(".j-search").click();
            }
        });
    vForm.init();

    T.stateArray["0"] = "隐藏";
    T.stateArray["1"] = "显示";
    var h = new hTemplate({
        tmp: $("#h-temp").html(),
        empty: $("#h-empty").html(),
        container: $(".j-htemplate"),
        size: 15,
        pager: $(".h-pager"),
        pageClick: function (page) {
            getList(page - 1);
        },
        filter: function (json) {
            json.links = '<a href="#" class="j-edit">编辑</a>';
            if (json.state == 0) {
                json.links += singer.format('<a href="#" class="j-state" data-state="1">{0}</a>', T.stateArray["1"]);
            } else if (json.state == 1) {
                json.links += singer.format('<a href="#" class="j-state" data-state="0">{0}</a>', T.stateArray["0"]);
            }
            return json;
        },
        complete: function () {
            T.setFrameHeight();
        }
    });
    var getList = function (page) {
        var data = sForm.json(),
            $t = $(this);
        singer.mix(data, { page: page });
        T.getJson("/m/package/list", data, function (json) {
            if (json.state) {
                h.bind(json.data.list, json.data.count);
                $(".m-table caption em").html(json.data.count);
            }
            $t.hasClass("btn") && T.setBtn($t, true);
        }, function () {
            h.bind();
        });
    };
    var loadItem = function (id) {
        if (!id || id <= 0) {
            vForm.reset();
            return false;
        }
        T.getJson("/m/package/item", {
            packageId: id
        }, function (json) {
            if (!json.state) {
                T.msg(json.msg || "获取套餐信息失败！");
                return false;
            }
            $(".m-panel-title li").removeClass("active");
            $(".m-panel-title ul").append("<li class=\"m-panel-add active\">编辑套餐</li>");
            $(".j-back").show();
            if (3 === json.data.sex) json.data.sex = [1, 2];
            vForm.bind(json.data);
            $(".t-package-img").html(singer.format('<img src="{0}" />', json.data.picture));
            editor.setContent(json.data.details);
            $(".m-panel-item").hide().eq(1).fadeIn();
            T.setFrameHeight();
            return false;
        });
    };
    getList(0);
    if ("#send" === location.hash) {
        $(".m-panel-title li:eq(1)").click();
    }

    $(".j-htemplate")
        .delegate(".j-send", "click", function () {
            $(".m-panel-title li:eq(1)").click();
            return false;
        });
    var stateColor = ["Gray", "Green"];
    $(document)
        .delegate(".m-panel-title li:eq(1)", "click.form", function () {
            loadItem();
        })
        .delegate(".j-search", "click", function () {
            if ($(this).hasClass("disabled")) return false;
            T.setBtn(this, false);
            getList.call(this, 0);
            return false;
        })
        .delegate(".j-submit", "click", function () {
            if ($(this).hasClass("disabled") || !vForm.check()) return false;
            T.setBtn(this, false);
            var formData = vForm.json();
            formData.details = encodeURIComponent(editor.getContent());
            T.getJson("/m/package/add", formData, function (json) {
                if (json.state) {
                    T.msg(formData.packageId > 0 ? "编辑成功！" : "添加成功！", "reload");
                } else {
                    T.msg(json.msg || "操作异常！");
                    T.setBtn(".j-submit", true);
                }
            }, function () {
                T.setBtn(".j-submit", true);
            });
            return false;
        })
        .delegate(".j-edit", "click", function () {
            var id = $(this).parents("tr").data("id");
            loadItem(id);
            return false;
        })
        .delegate(".j-state", "click", function () {
            var $t = $(this),
                id = $t.parents("tr").data("id"),
                state = $t.data("state");
            if (!id || "" === state) return false;
            T.setStateBtn.call($t, false);
            T.getJson("/m/package/updateState", {
                packageId: id,
                state: state
            }, function (json) {
                if (json.state) {
                    $t.parent().prev().html(
                        singer.format('<span style="color:{color}">{text}</font>',
                            {
                                color: stateColor[state],
                                text: T.stateArray[state + ""]
                            })
                    );
                    var nState = Math.abs(state - 1);
                    T.setStateBtn.call($t, json, nState);
                } else {
                    T.setStateBtn.call($t, json, state);
                }
            });
            return false;
        })
        .delegate(".j-delete", "click", function () {
            var id = $(this).parents("td").data("id");
            if (!confirm("确认删除该套餐？")) return false;
            T.getJson("/m/package/delete", {
                packageId: id
            }, function (json) {
                if (json.state) {
                    T.msg(json.msg || "删除成功！");
                    location.reload(true);
                } else {
                    T.msg(json.msg || "删除失败，请稍候重试！");
                }
            });
            return false;
        })
        .delegate(".j-restore", "click", function () {
            var id = $(this).parents("td").data("id");
            if (!confirm("确认还原该套餐？")) return false;
            T.getJson("/m/package/restore", {
                packageId: id
            }, function (json) {
                if (json.state) {
                    T.msg(json.msg || "还原成功！");
                    location.reload(true);
                } else {
                    T.msg(json.msg || "还原失败，请稍候重试！");
                }
            });
            return false;
        });
})(jQuery, TJZX);