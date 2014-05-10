(function ($, f, u) {
    /**
     * 验证规则
     * @type {{}}
     */
    var valid = {
        rules: {
            "require": /[\w\W]+/,
            "mobile": /^1[3458][0-9]{9}$/,
            "email": /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/,
            "url": /^(\w+:\/\/)?\w+(\.\w+)+.*$/,
            "ip":/^(((25[0-5])|(2[0-4]\d)|(1\d{2})|([1-9]?\d))\.){3}((25[0-5])|(2[0-4]\d)|(1\d{2})|([1-9]?\d))$/,
            "number": /^\d+$/
        },
        rulesMsg: {
            "require": "不能为空！",
            "mobile": "请填写手机号码！",
            "email": "请填写邮箱！",
            "url": "请填写网址！",
            "ip":"请输入正确的IP地址！",
            "number": "请填写数字！"
        },
        getValue: function (obj) {
            var val,
                form = this;
            if (obj.is(":radio")) {
                val = form.find(":radio[name='" + obj.attr("name") + "']:checked").val();
            } else if (obj.is(":checkbox")) {
                val = "";
                form.find(":checkbox[name='" + obj.attr("name") + "']:checked").each(function () {
                    val += $(this).val() + ',';
                });
            } else {
                val = obj.val();
            }
            val = (val === u ? "" : val);
            return $.trim(val);
        },
        setValue: function (name, value) {
            var form = this,
                $item, $list, obj;
            obj = form.find('[name="' + name + '"]').eq(0);
            if (obj.is(":radio")) {
                $list = form.find(":radio[name='" + name + "']");
                if ("" === value) {
                    $list.each(function () {
                        this.checked = false;
                    });
                    return false;
                }
                for (var i = 0; i < $list.length; i++) {
                    $item = $list.eq(i);
                    var v = $item.val();
                    if (v === value || (!isNaN(v) && ~~v === value)) {
                        $item.get(0).checked = true;
                        break;
                    }
                }
            } else if (obj.is(":checkbox")) {
                var valueArray = [];
                if (value && "string" === typeof value)
                    valueArray = value.split(',');
                else if (!isNaN(value))
                    valueArray.push(value);
                if ($.isArray(value)) {
                    valueArray = value;
                }
                $list = form.find(":checkbox[name='" + name + "']");
                if ("" === value) {
                    $list.each(function () {
                        this.checked = false;
                    });
                    return false;
                }
                $list.each(function () {
                    $item = $(this);
                    var v = $item.val();
                    var checked = ($.inArray(v, valueArray) >= 0 || (!isNaN(v) && $.inArray(~~v, valueArray) >= 0));
                    $item[0].checked = checked;
                });
            } else if (obj.is("select")) {
                $list = form.find("select[name='" + name + "'] option");
                value = value.toString();
                for (var i = 0; i < $list.length; i++) {
                    $item = $list.eq(i);
                    if (value === $item.val()) {
                        $item.get(0).selected = true;
                        break;
                    }
                }
            } else {
                $list = form.find('[name="' + name + '"]');

                $list.val(value) || $list.text(value);
            }
        },
        check: function (form) {
            var $t = $(this),
                ps = $(form).data("valid"),
                opt = $t.data("rule");
            if (!opt) return true;
            if ("string" === typeof opt) {
                opt = eval('(' + opt + ')');
            }
            var rule = ps.rules[opt.type] || new RegExp(opt.type, "gi"),
                value = valid.getValue.call(form, $t),
                $tip = opt.tip ? $(opt.tip) : $t.siblings(".m-form-tip");
            if (!$tip || $tip.length != 1) {
                $t.parent().append("<span class='m-form-tip'></span>");
                $tip = $t.siblings(".m-form-tip");
            }
            if (!rule.test(value)) {
                var msg = opt.msg || ps.rulesMsg[opt.type] || "输入错误！";
                $t.addClass("control-error");
                $tip.removeClass("valid-success").addClass("valid-error").html(msg);
                if (!$(form).data("focus")) {
                    $t.focus();
                    $(form).data("focus", !f);
                }
                return false;
            }
            $t.removeClass("control-error");
            $tip.removeClass("valid-error").addClass("valid-success").html("");
            return true;
        }
    };

    var validForm = function (forms, options) {
        var opts = $.extend({
            rules: valid.rules,
            rulesMsg: valid.rulesMsg,
            submit: f
        }, options || {});
        var $forms = this.forms = $(forms);
        $.each(this.forms, function (i) {
            var $form = $forms.eq(i),
                optionData = $form.data("form") || $form.attr("data-form"),
                ps = $.extend({}, opts);
            if (optionData && "string" === typeof optionData) {
                optionData = eval('(' + optionData + ')');
            }
            if (optionData && "object" === typeof optionData) {
                ps = $.extend(ps, optionData);
            }
            $form.data("valid", ps);
            var $inputs = $form.find("[data-valid]");
            $inputs.each(function () {
                $(this).data("rule", $(this).data("valid") || $(this).attr("data-valid"));
            });
            $form.find("[name]").bind("blur", function () {
                if ($(this).data("rule"))
                    valid.check.call(this, $form);
            });
            $form.find("[key-submit]").bind("keyup", function (e) {
                if (13 === e.keyCode) {
                    ps.submit && "function" === typeof ps.submit && ps.submit.call(this);
                }
            });
        });
        return this;
    };
    validForm.prototype = {
        check: function () {
            var r = !f,
                $forms = this.forms;
            $forms.each(function (i) {
                var $form = $forms.eq(i);
                $form.data("focus", f);
                $form.find("[name]").each(function () {
                    if (!valid.check.call(this, $form))
                        r = f;
                });
            });
            return r;
        },
        json: function (i) {
            var $form = this.forms.eq(i || 0),
                formData = {};
            $.each($form.find("[name]"), function () {
                if (!formData.hasOwnProperty(this.name))
                    formData[this.name] = valid.getValue.call($form, $(this));
            });
            return formData;
        },
        serialize: function (i) {
            var json = this.json(i),
                arr = [];
            for (var t in json) {
                arr.push(t + '=' + encodeURI(json[t]));
            }
            return arr.join('&');
        },
        bind: function (json, i) {
            var $form = this.forms.eq(i || 0);
            $form.find(".control-error").removeClass("control-error");
            $form.find(".m-form-tip").remove();
            if ("object" !== typeof json) {
                this.reset(i);
                return false;
            }
            for (var name in json) {
                if (json.hasOwnProperty(name))
                    valid.setValue.call($form, name, json[name]);
            }
        },
        init: function () {
            for (var i = 0; i < this.forms.length; i++) {
                var $form = this.forms.eq(i);
                if (!$form.data("init"))
                    $form.data("init", this.json(i));
            }
        },
        reset: function (i) {
            var $form = this.forms.eq(i || 0);
            $form.find(".m-form-tip").remove();
            $form.find(".control-error").removeClass("control-error");
            this.bind($form.data("init"), i);
        }
    };
    $.fn.extend({
        /**
         * form 表单验证
         */
        valid: function (options) {
            return new validForm(this, options);
        }
    });
})(jQuery, false, undefined);