var data = [
    {name: '系统管理', role: 1, pid: 0, isParent: true},
    {name: '登记', role: 2, pid: 0, isParent: true},
    {name: '系统管理', role: 3, pid: 0, isParent: true},
    {name: '系统管理', role: 4, pid: 0, isParent: true},
    {name: '系统管理', role: 5, pid: 0, isParent: true},
    {name: '系统管理', role: 101, pid: 1, isParent: false},
    {name: '系统管理', role: 102, pid: 1, isParent: false}
];
(function ($) {
    /**
     * 数组查询
     * @param condition
     * @returns {Array}
     */
    Array.prototype.find = function (condition) {
        var arr = [];
        if ("object" === typeof condition) {
            for (var item in this) {
                var compare = 0, num = 0;
                for (var prop in condition) {
                    num++;
                    if (this[item][prop] === condition[prop])
                        compare++;
                }
                if (compare == num)
                    arr.push(this[item]);
            }
        }
        return arr;
    };
    $.fn.extend({
        hTree: function (options) {

        }
    });
    var hTree = function (data) {
        var tree = {
            fill: function (pid) {
                var $t = $(this),
                    list = data.find({pid: pid});
                if (list.length > 0) {
                    for (var i = 0; i < list.length; i++) {
                        $t.append(list[i].role);
                    }
                }
            }
        }
    };
})(jQuery);


