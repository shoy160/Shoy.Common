/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function(a){function b(b){var d=a.data(b,"combotree").options,e=a.data(b,"combotree").tree;a(b).addClass("combotree-f"),a(b).combo(d);var f=a(b).combo("panel");e||(e=a("<ul></ul>").appendTo(f),a.data(b,"combotree").tree=e),e.tree(a.extend({},d,{checkbox:d.multiple,onLoadSuccess:function(c,f){var g=a(b).combotree("getValues");if(d.multiple)for(var h=e.tree("getChecked"),i=0;i<h.length;i++){var j=h[i].id;!function(){for(var a=0;a<g.length;a++)if(j==g[a])return;g.push(j)}()}a(b).combotree("setValues",g),d.onLoadSuccess.call(this,c,f)},onClick:function(e){c(b),a(b).combo("hidePanel"),d.onClick.call(this,e)},onCheck:function(a,e){c(b),d.onCheck.call(this,a,e)}}))}function c(b){var c=a.data(b,"combotree").options,d=a.data(b,"combotree").tree,e=[],f=[];if(c.multiple)for(var g=d.tree("getChecked"),h=0;h<g.length;h++)e.push(g[h].id),f.push(g[h].text);else{var i=d.tree("getSelected");i&&(e.push(i.id),f.push(i.text))}a(b).combo("setValues",e).combo("setText",f.join(c.separator))}function d(b,c){var d=a.data(b,"combotree").options,e=a.data(b,"combotree").tree;e.find("span.tree-checkbox").addClass("tree-checkbox0").removeClass("tree-checkbox1 tree-checkbox2");for(var f=[],g=[],h=0;h<c.length;h++){var i=c[h],j=i,k=e.tree("find",i);k&&(j=k.text,e.tree("check",k.target),e.tree("select",k.target)),f.push(i),g.push(j)}a(b).combo("setValues",f).combo("setText",g.join(d.separator))}a.fn.combotree=function(c,d){if("string"==typeof c){var e=a.fn.combotree.methods[c];return e?e(this,d):this.combo(c,d)}return c=c||{},this.each(function(){var d=a.data(this,"combotree");d?a.extend(d.options,c):a.data(this,"combotree",{options:a.extend({},a.fn.combotree.defaults,a.fn.combotree.parseOptions(this),c)}),b(this)})},a.fn.combotree.methods={options:function(b){var c=b.combo("options");return a.extend(a.data(b[0],"combotree").options,{originalValue:c.originalValue,disabled:c.disabled,readonly:c.readonly})},tree:function(b){return a.data(b[0],"combotree").tree},loadData:function(b,c){return b.each(function(){var b=a.data(this,"combotree").options;b.data=c;var d=a.data(this,"combotree").tree;d.tree("loadData",c)})},reload:function(b,c){return b.each(function(){var b=a.data(this,"combotree").options,d=a.data(this,"combotree").tree;c&&(b.url=c),d.tree({url:b.url})})},setValues:function(a,b){return a.each(function(){d(this,b)})},setValue:function(a,b){return a.each(function(){d(this,[b])})},clear:function(b){return b.each(function(){var b=a.data(this,"combotree").tree;b.find("div.tree-node-selected").removeClass("tree-node-selected");for(var c=b.tree("getChecked"),d=0;d<c.length;d++)b.tree("uncheck",c[d].target);a(this).combo("clear")})},reset:function(b){return b.each(function(){var b=a(this).combotree("options");b.multiple?a(this).combotree("setValues",b.originalValue):a(this).combotree("setValue",b.originalValue)})}},a.fn.combotree.parseOptions=function(b){return a.extend({},a.fn.combo.parseOptions(b),a.fn.tree.parseOptions(b))},a.fn.combotree.defaults=a.extend({},a.fn.combo.defaults,a.fn.tree.defaults,{editable:!1})}(jQuery);