/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function($){function init(a){function b(c){var d=[];return c.addClass("menu"),d.push(c),c.hasClass("menu-content")||c.children("div").each(function(){var c=$(this).children("div");if(c.length){c.insertAfter(a),this.submenu=c;var e=b(c);d=d.concat(e)}}),d}function c(b){var c=$.parser.parseOptions(b[0],["width"]).width;b.hasClass("menu-content")?b[0].originalWidth=c||b._outerWidth():(b[0].originalWidth=c||0,b.children("div").each(function(){var b=$(this),c=$.extend({},$.parser.parseOptions(this,["name","iconCls","href",{separator:"boolean"}]),{disabled:b.attr("disabled")?!0:void 0});if(c.separator&&b.addClass("menu-sep"),!b.hasClass("menu-sep")){b[0].itemName=c.name||"",b[0].itemHref=c.href||"";var d=b.addClass("menu-item").html();b.empty().append($('<div class="menu-text"></div>').html(d)),c.iconCls&&$('<div class="menu-icon"></div>').addClass(c.iconCls).appendTo(b),c.disabled&&setDisabled(a,b[0],!0),b[0].submenu&&$('<div class="menu-rightarrow"></div>').appendTo(b),bindMenuItemEvent(a,b)}}),$('<div class="menu-line"></div>').prependTo(b)),setMenuWidth(a,b),b.hide(),bindMenuEvent(a,b)}$(a).appendTo("body"),$(a).addClass("menu-top"),$(document).unbind(".menu").bind("mousedown.menu",function(a){var b=$("body>div.menu:visible"),c=$(a.target).closest("div.menu",b);c.length||$("body>div.menu-top:visible").menu("hide")});for(var d=b($(a)),e=0;e<d.length;e++)c(d[e])}function setMenuWidth(a,b){var c=$.data(a,"menu").options,d=b.attr("style");b.css({display:"block",left:-1e4,height:"auto",overflow:"hidden"});var e=0;b.find("div.menu-text").each(function(){e<$(this)._outerWidth()&&(e=$(this)._outerWidth()),$(this).closest("div.menu-item")._outerHeight($(this)._outerHeight()+2)}),e+=65,b._outerWidth(Math.max(b[0].originalWidth||0,e,c.minWidth)),b.children("div.menu-line")._outerHeight(b.outerHeight()),b.attr("style",d)}function bindMenuEvent(a,b){var c=$.data(a,"menu");b.unbind(".menu").bind("mouseenter.menu",function(){c.timer&&(clearTimeout(c.timer),c.timer=null)}).bind("mouseleave.menu",function(){c.options.hideOnUnhover&&(c.timer=setTimeout(function(){hideAll(a)},100))})}function bindMenuItemEvent(a,b){b.hasClass("menu-item")&&(b.unbind(".menu"),b.bind("click.menu",function(){if(!$(this).hasClass("menu-item-disabled")){if(!this.submenu){hideAll(a);var b=$(this).attr("href");b&&(location.href=b)}var c=$(a).menu("getItem",this);$.data(a,"menu").options.onClick.call(a,c)}}).bind("mouseenter.menu",function(){if(b.siblings().each(function(){this.submenu&&hideMenu(this.submenu),$(this).removeClass("menu-active")}),b.addClass("menu-active"),$(this).hasClass("menu-item-disabled"))return void b.addClass("menu-active-disabled");var c=b[0].submenu;c&&$(a).menu("show",{menu:c,parent:b})}).bind("mouseleave.menu",function(a){b.removeClass("menu-active menu-active-disabled");var c=b[0].submenu;c?a.pageX>=parseInt(c.css("left"))?b.addClass("menu-active"):hideMenu(c):b.removeClass("menu-active")}))}function hideAll(a){var b=$.data(a,"menu");return b&&$(a).is(":visible")&&(hideMenu($(a)),b.options.onHide.call(a)),!1}function showMenu(a,b){var c,d;b=b||{};var e=$(b.menu||a);if(e.hasClass("menu-top")){var f=$.data(a,"menu").options;if($.extend(f,b),c=f.left,d=f.top,f.alignTo){var g=$(f.alignTo);c=g.offset().left,d=g.offset().top+g._outerHeight()}c+e.outerWidth()>$(window)._outerWidth()+$(document)._scrollLeft()&&(c=$(window)._outerWidth()+$(document).scrollLeft()-e.outerWidth()-5),d+e.outerHeight()>$(window)._outerHeight()+$(document).scrollTop()&&(d=$(window)._outerHeight()+$(document).scrollTop()-e.outerHeight()-5)}else{var h=b.parent;c=h.offset().left+h.outerWidth()-2,c+e.outerWidth()+5>$(window)._outerWidth()+$(document).scrollLeft()&&(c=h.offset().left-e.outerWidth()+2);var d=h.offset().top-3;d+e.outerHeight()>$(window)._outerHeight()+$(document).scrollTop()&&(d=$(window)._outerHeight()+$(document).scrollTop()-e.outerHeight()-5)}e.css({left:c,top:d}),e.show(0,function(){e[0].shadow||(e[0].shadow=$('<div class="menu-shadow"></div>').insertAfter(e)),e[0].shadow.css({display:"block",zIndex:$.fn.menu.defaults.zIndex++,left:e.css("left"),top:e.css("top"),width:e.outerWidth(),height:e.outerHeight()}),e.css("z-index",$.fn.menu.defaults.zIndex++),e.hasClass("menu-top")&&$.data(e[0],"menu").options.onShow.call(e[0])})}function hideMenu(a){function b(a){a.stop(!0,!0),a[0].shadow&&a[0].shadow.hide(),a.hide()}a&&(b(a),a.find("div.menu-item").each(function(){this.submenu&&hideMenu(this.submenu),$(this).removeClass("menu-active")}))}function findItem(a,b){function c(f){f.children("div.menu-item").each(function(){var f=$(a).menu("getItem",this),g=e.empty().html(f.text).text();b==$.trim(g)?d=f:this.submenu&&!d&&c(this.submenu)})}var d=null,e=$("<div></div>");return c($(a)),e.remove(),d}function setDisabled(a,b,c){var d=$(b);d.hasClass("menu-item")&&(c?(d.addClass("menu-item-disabled"),b.onclick&&(b.onclick1=b.onclick,b.onclick=null)):(d.removeClass("menu-item-disabled"),b.onclick1&&(b.onclick=b.onclick1,b.onclick1=null)))}function appendItem(target,param){var menu=$(target);if(param.parent){if(!param.parent.submenu){var submenu=$('<div class="menu"><div class="menu-line"></div></div>').appendTo("body");submenu.hide(),param.parent.submenu=submenu,$('<div class="menu-rightarrow"></div>').appendTo(param.parent)}menu=param.parent.submenu}if(param.separator)var item=$('<div class="menu-sep"></div>').appendTo(menu);else{var item=$('<div class="menu-item"></div>').appendTo(menu);$('<div class="menu-text"></div>').html(param.text).appendTo(item)}param.iconCls&&$('<div class="menu-icon"></div>').addClass(param.iconCls).appendTo(item),param.id&&item.attr("id",param.id),param.name&&(item[0].itemName=param.name),param.href&&(item[0].itemHref=param.href),param.onclick&&("string"==typeof param.onclick?item.attr("onclick",param.onclick):item[0].onclick=eval(param.onclick)),param.handler&&(item[0].onclick=eval(param.handler)),param.disabled&&setDisabled(target,item[0],!0),bindMenuItemEvent(target,item),bindMenuEvent(target,menu),setMenuWidth(target,menu)}function removeItem(a,b){function c(a){if(a.submenu){a.submenu.children("div.menu-item").each(function(){c(this)});var b=a.submenu[0].shadow;b&&b.remove(),a.submenu.remove()}$(a).remove()}c(b)}function destroyMenu(a){$(a).children("div.menu-item").each(function(){removeItem(a,this)}),a.shadow&&a.shadow.remove(),$(a).remove()}$.fn.menu=function(a,b){return"string"==typeof a?$.fn.menu.methods[a](this,b):(a=a||{},this.each(function(){var b=$.data(this,"menu");b?$.extend(b.options,a):(b=$.data(this,"menu",{options:$.extend({},$.fn.menu.defaults,$.fn.menu.parseOptions(this),a)}),init(this)),$(this).css({left:b.options.left,top:b.options.top})}))},$.fn.menu.methods={options:function(a){return $.data(a[0],"menu").options},show:function(a,b){return a.each(function(){showMenu(this,b)})},hide:function(a){return a.each(function(){hideAll(this)})},destroy:function(a){return a.each(function(){destroyMenu(this)})},setText:function(a,b){return a.each(function(){$(b.target).children("div.menu-text").html(b.text)})},setIcon:function(a,b){return a.each(function(){var a=$(this).menu("getItem",b.target);a.iconCls?$(a.target).children("div.menu-icon").removeClass(a.iconCls).addClass(b.iconCls):$('<div class="menu-icon"></div>').addClass(b.iconCls).appendTo(b.target)})},getItem:function(a,b){var c=$(b),d={target:b,id:c.attr("id"),text:$.trim(c.children("div.menu-text").html()),disabled:c.hasClass("menu-item-disabled"),name:b.itemName,href:b.itemHref,onclick:b.onclick},e=c.children("div.menu-icon");if(e.length){for(var f=[],g=e.attr("class").split(" "),h=0;h<g.length;h++)"menu-icon"!=g[h]&&f.push(g[h]);d.iconCls=f.join(" ")}return d},findItem:function(a,b){return findItem(a[0],b)},appendItem:function(a,b){return a.each(function(){appendItem(this,b)})},removeItem:function(a,b){return a.each(function(){removeItem(this,b)})},enableItem:function(a,b){return a.each(function(){setDisabled(this,b,!1)})},disableItem:function(a,b){return a.each(function(){setDisabled(this,b,!0)})}},$.fn.menu.parseOptions=function(a){return $.extend({},$.parser.parseOptions(a,["left","top",{minWidth:"number",hideOnUnhover:"boolean"}]))},$.fn.menu.defaults={zIndex:11e4,left:0,top:0,minWidth:120,hideOnUnhover:!0,onShow:function(){},onHide:function(){},onClick:function(){}}}(jQuery);