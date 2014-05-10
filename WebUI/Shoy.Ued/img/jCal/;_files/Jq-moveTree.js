var moveTree = function (options) {
    var F = false,
        EA = [];
    /**
     * 数组查询 [{a:1,b:2},{a:2,b:2}].find({a:2}) -> [{a:2,b:2}]
     *          [{a:1,b:2},{a:2,b:2}].find({a:2},true) -> [{a:1,b:2}]
     * @param condition
     * @returns {Array}
     */
    Array.prototype.find = function (condition, filter) {
        var arr = [];
        if ("object" === typeof condition) {
            for (var item in this) {
                if (!this.hasOwnProperty(item) || undefined === this[item]) continue;
                var compare = 0, num = 0;
                for (var prop in condition) {
                    num++;
                    var val = this[item][prop],
                        con = condition[prop];
                    if (val !== undefined && con !== undefined && val == con)
                        compare++;
                }
                if ((!filter && compare == num) || (filter && compare != num))
                    arr.push(this[item]);
            }
        }
        return arr;
    };

    /**
     * 删除数组  [{a:1,b:2},{a:2,b:2}].del({a:2}) -> [{a:1,b:2}]
     */
    Array.prototype.del = Array.prototype.del || function (condition) {
        return this.find(condition, true);
    };
    options = $.extend({}, {
        containers: {
            unBox: F,
            ownBox: F,
            roleBox: F
        },
        core: {
            rights: EA,
            roles: EA,
            relations: EA
        }
    }, options || {});
    var util = {
        relations: (function () {
            var ships = EA,
                relations = options.core.relations;
            if (!relations || !relations.length)
                return ships;
            for (var i = 0; i < relations.length; i++) {
                var item = relations[i];
                if (item && item.role) {
                    var ship = ships.find({role: item.role});
                    if (!ship.length) {
                        ships.push({role: item.role, rights: [item.right_id]});
                    } else {
                        ship[0].rights.push(item.right_id);
                    }
                }
            }
            return ships;
        })(),
        fillParents: function (rights) {
            var data = $.extend([], rights);
            for (var i = 0; i < rights.length; i++) {
                var item = rights[i],
                    pid = item.pid;
                var findParent = function (pid) {
                    if (pid > 0 && !rights.find({right_id: pid}).length) {
                        var parent = options.core.rights.find({right_id: pid});
                        if (parent.length) {
                            data.push(parent[0]);
                            findParent(parent[0].pid);
                        }
                    }
                };
                findParent(pid);
            }
            return data;
        },
        filterRights: function (state) {
            var data = [], array = [], rights = options.core.rights;
            if (!rights || !rights.length)
                return data;
            if (state) array = rights.find({state: 1});
            else array = rights.find({state: 0});
            array = util.fillParents(array);
            for (var i in array) {
                if (!array.hasOwnProperty(i)) continue;
                var item = array[i];
                data.push({
                    id: item.right_id,
                    parent: (0 === item.pid ? "#" : item.pid),
                    text: item.name,
                    icon: false,
                    state: {
                        opened: state,
                        selected: false
                    },
                    li_attr: {ks: item.ks || 0}
                });
            }
            return data.sort(function (i, j) {
                return i.id > j.id;
            });
        }
    };
    (function (U) {
        U.unRights = U.filterRights(0);
        U.ownRights = U.filterRights(1);
        var roles = options.core.roles;
        if (roles && roles.length && options.containers.roleBox) {
            var $roles = $(options.containers.roleBox);
            for (var i = 0; i < roles.length; i++) {
                var item = '<label class="checkbox">' +
                    '<input type="checkbox" name="chkRole" value="' + roles[i].role + '"' + (roles[i].state ? " checked" : "") + ' />' +
                    roles[i].name +
                    '</label>'
                $roles.append(item);
            }
        }
        U.trees = {unTree: F, ownTree: F};
        var def = {
                "core": {
                    "check_callback": true,
                    "data": []
                },
                "checkbox": {
                    "keep_selected_style": false
                },
                "plugins": [ "checkbox" , "sort"]
            },
            unBox = options.containers.unBox,
            ownBox = options.containers.ownBox;
        $.jstree.defaults.sort = function (a, b) {
            return a > b ? 1 : -1;
        };
        if (unBox) {
            $(unBox).jstree($.extend({}, def, {core: {data: U.unRights}}));
            U.trees.unTree = $(unBox).jstree(true);
        }
        if (ownBox) {
            $(ownBox).jstree($.extend({}, def, {core: {data: U.ownRights}}));
            U.trees.ownTree = $(ownBox).jstree(true);
        }
        U.move = function (source,target) {
            var arr = $.extend([], source.get_selected());
            if (arr && arr.length) {
                for (var i = 0; i < arr.length; i++) {
                    var node = source.get_node(arr[i]);
                    if (!target.get_node(node.id)) {
                        if (node.parents.length > 1) {
                            //检测父节点状态
                            for (var pi = node.parents.length - 2; pi >= 0; pi--) {
                                var pNode = target.get_node(node.parents[pi]);
                                if (!pNode) {
                                    pNode = source.get_node(node.parents[pi]);
                                    target.create_node(pNode.parent,
                                        {id: pNode.id, text: pNode.text, parent: pNode.parent, icon: false, state: {opened: true}, li_attr: pNode.li_attr});
                                } else {
                                    target.open_node(pNode.id);
                                }
                            }
                        }
                        target.create_node(node.parent, {id: node.id, text: node.text, parent: node.parent, icon: false, li_attr: node.li_attr});
                    }
                }
                source.deselect_node(arr);
                source.delete_node(arr);
            }
        };
    })(util);
    this.util = util;
};