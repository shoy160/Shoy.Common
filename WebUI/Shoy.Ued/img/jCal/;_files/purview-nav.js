(function ($) {

    //绑定角色
    var roles = [
        {name: '初始用户', role: 1},
        {name: '基础用户', role: 2},
        {name: '个人登记预约', role: 3},
        {name: '初始用户', role: 4},
        {name: '基础用户', role: 5, state: 1},
        {name: '个人登记预约', role: 6},
        {name: '初始用户', role: 7},
        {name: '基础用户', role: 8, state: 1},
        {name: '个人登记预约', role: 9}
    ];

    var rightList = [
        {name: '系统管理1', right_id: 1, pid: 0, state: 0},
        {name: '登记2', right_id: 2, pid: 0, state: 0},
        {name: '系统管理3', right_id: 3, pid: 0, state: 0},
        {name: '系统管理4', right_id: 4, pid: 0, state: 1},
        {name: '系统管理5', right_id: 5, pid: 0, state: 1},
        {name: '子权限101', right_id: 101, pid: 1, state: 0},
        {name: '子权限102', right_id: 102, pid: 1, state: 1},
        {name: '子权限201', right_id: 201, pid: 102, state: 1},
        {name: '子权限204', right_id: "204k", pid: 102, state: 1, ks: 1},
        {name: '子权限205', right_id: 205, pid: 102, state: 1}
    ];

    var roleRight = [
        {role: 5, right_id: 205},
        {role: 1, right_id: 101},
        {role: 1, right_id: 3},
        {role: 2, right_id: 205}
    ];

    var MT = new moveTree({
        containers: {
            roleBox: $(".j-roles"),
            unBox: $(".purviewdiv-center-b-div1-k1:eq(0)"),
            ownBox: $(".purviewdiv-center-b-div1-k1:eq(1)")
        },
        core: {
            rights: rightList,
            roles: roles,
            relations: roleRight
        }
    });
    window.MT=MT;
    var util=MT.util;
    var trees = util.trees;
    $(".purviewdiv-center-b-div1-k2-right").click(function () {
        util.move(trees.unTree,trees.ownTree);
        return false;
    });
    $(".purviewdiv-center-b-div1-k2-left").click(function () {
        util.move(trees.ownTree,trees.unTree);
        return false;
    });

    $(".j-roles input[name='chkRole']").bind("change", function () {
        var $t = $(this), checked = this.checked, val = $t.val();
        var relations = util.relations.find({role: val});
        if (relations.length == 0 || relations[0].rights.length == 0) return false;
        var rights = relations[0].rights;
        if (checked) {
            trees.unTree.select_node(rights);
        } else {
            trees.ownTree.select_node(rights);
        }
    });

    $(".buttom-qd").bind("click", function () {
        if ($(this).hasClass("disabled")) return false;
        var rights = [], allRights = [], ks = [], roles = [];
        //获取角色
        var $roles = $('input[name="chkRole"]:checked');
        $roles.each(function () {
            roles.push(this.value);
        });
        var ownTree = trees.ownTree;
        ownTree.select_all();
        allRights = ownTree.get_selected();
        ownTree.deselect_all();
        for (var i in allRights) {
            if (!allRights.hasOwnProperty(i)) continue;
            var node = ownTree.get_node(allRights[i]);
            if (node.children.length == 0) {
                if (node.li_attr.ks) {
                    ks.push(node.id);
                } else {
                    rights.push(node.id);
                }
            }
        }
        console.log(rights);
        console.log(ks);
        console.log(roles);
    });
})(jQuery);