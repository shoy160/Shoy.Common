/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function($){function _1(a){a._remove()}function _3(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel,e=d.children("div.panel-header"),f=d.children("div.panel-body");b&&$.extend(c,{width:b.width,height:b.height,left:b.left,top:b.top}),c.fit?$.extend(c,d._fit()):d._fit(!1),d.css({left:c.left,top:c.top}),isNaN(c.width)?d.width("auto"):d._outerWidth(c.width),e.add(f)._outerWidth(d.width()),isNaN(c.height)?f.height("auto"):(d._outerHeight(c.height),f._outerHeight(d.height()-e._outerHeight())),d.css("height",""),c.onResize.apply(a,[c.width,c.height]),$(a).find(">div,>form>div").triggerHandler("_resize")}function _a(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel;b&&(null!=b.left&&(c.left=b.left),null!=b.top&&(c.top=b.top)),d.css({left:c.left,top:c.top}),c.onMove.apply(a,[c.left,c.top])}function _f(a){$(a).addClass("panel-body");var b=$('<div class="panel"></div>').insertBefore(a);return b[0].appendChild(a),b.bind("_resize",function(){var b=$.data(a,"panel").options;return 1==b.fit&&_3(a),!1}),b}function _13(_14){var _15=$.data(_14,"panel").options,_16=$.data(_14,"panel").panel;if(_15.tools&&"string"==typeof _15.tools&&_16.find(">div.panel-header>div.panel-tool .panel-tool-a").appendTo(_15.tools),_1(_16.children("div.panel-header")),_15.title&&!_15.noheader){var _17=$('<div class="panel-header"><div class="panel-title">'+_15.title+"</div></div>").prependTo(_16);_15.iconCls&&(_17.find(".panel-title").addClass("panel-with-icon"),$('<div class="panel-icon"></div>').addClass(_15.iconCls).appendTo(_17));var _18=$('<div class="panel-tool"></div>').appendTo(_17);if(_18.bind("click",function(a){a.stopPropagation()}),_15.tools)if($.isArray(_15.tools))for(var i=0;i<_15.tools.length;i++){var t=$('<a href="javascript:void(0)"></a>').addClass(_15.tools[i].iconCls).appendTo(_18);_15.tools[i].handler&&t.bind("click",eval(_15.tools[i].handler))}else $(_15.tools).children().each(function(){$(this).addClass($(this).attr("iconCls")).addClass("panel-tool-a").appendTo(_18)});_15.collapsible&&$('<a class="panel-tool-collapse" href="javascript:void(0)"></a>').appendTo(_18).bind("click",function(){return 1==_15.collapsed?_3c(_14,!0):_2c(_14,!0),!1}),_15.minimizable&&$('<a class="panel-tool-min" href="javascript:void(0)"></a>').appendTo(_18).bind("click",function(){return _47(_14),!1}),_15.maximizable&&$('<a class="panel-tool-max" href="javascript:void(0)"></a>').appendTo(_18).bind("click",function(){return 1==_15.maximized?_4b(_14):_2b(_14),!1}),_15.closable&&$('<a class="panel-tool-close" href="javascript:void(0)"></a>').appendTo(_18).bind("click",function(){return _19(_14),!1}),_16.children("div.panel-body").removeClass("panel-body-noheader")}else _16.children("div.panel-body").addClass("panel-body-noheader")}function _1a(a){function b(b){$(a).html(b),$.parser&&$.parser.parse($(a))}var c=$.data(a,"panel"),d=c.options;if(d.href){if(!c.isLoaded||!d.cache){if(0==d.onBeforeLoad.call(a))return;c.isLoaded=!1,_1e(a),d.loadingMessage&&$(a).html($('<div class="panel-loading"></div>').html(d.loadingMessage)),$.ajax({url:d.href,cache:!1,dataType:"html",success:function(e){b(d.extractor.call(a,e)),d.onLoad.apply(a,arguments),c.isLoaded=!0}})}}else d.content&&(c.isLoaded||(_1e(a),b(d.content),c.isLoaded=!0))}function _1e(a){var b=$(a);b.find(".combo-f").each(function(){$(this).combo("destroy")}),b.find(".m-btn").each(function(){$(this).menubutton("destroy")}),b.find(".s-btn").each(function(){$(this).splitbutton("destroy")}),b.find(".tooltip-f").each(function(){$(this).tooltip("destroy")})}function _23(a){$(a).find("div.panel:visible,div.accordion:visible,div.tabs-container:visible,div.layout:visible").each(function(){$(this).triggerHandler("_resize",[!0])})}function _25(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel;if(1==b||0!=c.onBeforeOpen.call(a)){d.show(),c.closed=!1,c.minimized=!1;var e=d.children("div.panel-header").find("a.panel-tool-restore");e.length&&(c.maximized=!0),c.onOpen.call(a),1==c.maximized&&(c.maximized=!1,_2b(a)),1==c.collapsed&&(c.collapsed=!1,_2c(a)),c.collapsed||(_1a(a),_23(a))}}function _19(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel;(1==b||0!=c.onBeforeClose.call(a))&&(d._fit(!1),d.hide(),c.closed=!0,c.onClose.call(a))}function _31(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel;(1==b||0!=c.onBeforeDestroy.call(a))&&(_1e(a),_1(d),c.onDestroy.call(a))}function _2c(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel,e=d.children("div.panel-body"),f=d.children("div.panel-header").find("a.panel-tool-collapse");1!=c.collapsed&&(e.stop(!0,!0),0!=c.onBeforeCollapse.call(a)&&(f.addClass("panel-tool-expand"),1==b?e.slideUp("normal",function(){c.collapsed=!0,c.onCollapse.call(a)}):(e.hide(),c.collapsed=!0,c.onCollapse.call(a))))}function _3c(a,b){var c=$.data(a,"panel").options,d=$.data(a,"panel").panel,e=d.children("div.panel-body"),f=d.children("div.panel-header").find("a.panel-tool-collapse");0!=c.collapsed&&(e.stop(!0,!0),0!=c.onBeforeExpand.call(a)&&(f.removeClass("panel-tool-expand"),1==b?e.slideDown("normal",function(){c.collapsed=!1,c.onExpand.call(a),_1a(a),_23(a)}):(e.show(),c.collapsed=!1,c.onExpand.call(a),_1a(a),_23(a))))}function _2b(a){var b=$.data(a,"panel").options,c=$.data(a,"panel").panel,d=c.children("div.panel-header").find("a.panel-tool-max");1!=b.maximized&&(d.addClass("panel-tool-restore"),$.data(a,"panel").original||($.data(a,"panel").original={width:b.width,height:b.height,left:b.left,top:b.top,fit:b.fit}),b.left=0,b.top=0,b.fit=!0,_3(a),b.minimized=!1,b.maximized=!0,b.onMaximize.call(a))}function _47(a){var b=$.data(a,"panel").options,c=$.data(a,"panel").panel;c._fit(!1),c.hide(),b.minimized=!0,b.maximized=!1,b.onMinimize.call(a)}function _4b(a){var b=$.data(a,"panel").options,c=$.data(a,"panel").panel,d=c.children("div.panel-header").find("a.panel-tool-max");0!=b.maximized&&(c.show(),d.removeClass("panel-tool-restore"),$.extend(b,$.data(a,"panel").original),_3(a),b.minimized=!1,b.maximized=!1,$.data(a,"panel").original=null,b.onRestore.call(a))}function _50(a){var b=$.data(a,"panel").options,c=$.data(a,"panel").panel,d=$(a).panel("header"),e=$(a).panel("body");c.css(b.style),c.addClass(b.cls),b.border?(d.removeClass("panel-header-noborder"),e.removeClass("panel-body-noborder")):(d.addClass("panel-header-noborder"),e.addClass("panel-body-noborder")),d.addClass(b.headerCls),e.addClass(b.bodyCls),b.id?$(a).attr("id",b.id):$(a).attr("id","")}function _56(a,b){$.data(a,"panel").options.title=b,$(a).panel("header").find("div.panel-title").html(b)}$.fn._remove=function(){return this.each(function(){$(this).remove();try{this.outerHTML=""}catch(a){}})};var TO=!1,_59=!0;$(window).unbind(".panel").bind("resize.panel",function(){_59&&(TO!==!1&&clearTimeout(TO),TO=setTimeout(function(){_59=!1;var a=$("body.layout");a.length?a.layout("resize"):$("body").children("div.panel,div.accordion,div.tabs-container,div.layout").triggerHandler("_resize"),_59=!0,TO=!1},200))}),$.fn.panel=function(a,b){return"string"==typeof a?$.fn.panel.methods[a](this,b):(a=a||{},this.each(function(){var b,c=$.data(this,"panel");c?(b=$.extend(c.options,a),c.isLoaded=!1):(b=$.extend({},$.fn.panel.defaults,$.fn.panel.parseOptions(this),a),$(this).attr("title",""),c=$.data(this,"panel",{options:b,panel:_f(this),isLoaded:!1})),_13(this),_50(this),1==b.doSize&&(c.panel.css("display","block"),_3(this)),1==b.closed||1==b.minimized?c.panel.hide():_25(this)}))},$.fn.panel.methods={options:function(a){return $.data(a[0],"panel").options},panel:function(a){return $.data(a[0],"panel").panel},header:function(a){return $.data(a[0],"panel").panel.find(">div.panel-header")},body:function(a){return $.data(a[0],"panel").panel.find(">div.panel-body")},setTitle:function(a,b){return a.each(function(){_56(this,b)})},open:function(a,b){return a.each(function(){_25(this,b)})},close:function(a,b){return a.each(function(){_19(this,b)})},destroy:function(a,b){return a.each(function(){_31(this,b)})},refresh:function(a,b){return a.each(function(){$.data(this,"panel").isLoaded=!1,b&&($.data(this,"panel").options.href=b),_1a(this)})},resize:function(a,b){return a.each(function(){_3(this,b)})},move:function(a,b){return a.each(function(){_a(this,b)})},maximize:function(a){return a.each(function(){_2b(this)})},minimize:function(a){return a.each(function(){_47(this)})},restore:function(a){return a.each(function(){_4b(this)})},collapse:function(a,b){return a.each(function(){_2c(this,b)})},expand:function(a,b){return a.each(function(){_3c(this,b)})}},$.fn.panel.parseOptions=function(a){var b=$(a);return $.extend({},$.parser.parseOptions(a,["id","width","height","left","top","title","iconCls","cls","headerCls","bodyCls","tools","href",{cache:"boolean",fit:"boolean",border:"boolean",noheader:"boolean"},{collapsible:"boolean",minimizable:"boolean",maximizable:"boolean"},{closable:"boolean",collapsed:"boolean",minimized:"boolean",maximized:"boolean",closed:"boolean"}]),{loadingMessage:void 0!=b.attr("loadingMessage")?b.attr("loadingMessage"):void 0})},$.fn.panel.defaults={id:null,title:null,iconCls:null,width:"auto",height:"auto",left:null,top:null,cls:null,headerCls:null,bodyCls:null,style:{},href:null,cache:!0,fit:!1,border:!0,doSize:!0,noheader:!1,content:null,collapsible:!1,minimizable:!1,maximizable:!1,closable:!1,collapsed:!1,minimized:!1,maximized:!1,closed:!1,tools:null,href:null,loadingMessage:"Loading...",extractor:function(a){var b=/<body[^>]*>((.|[\n\r])*)<\/body>/im,c=b.exec(a);return c?c[1]:a},onBeforeLoad:function(){},onLoad:function(){},onBeforeOpen:function(){},onOpen:function(){},onBeforeClose:function(){},onClose:function(){},onBeforeDestroy:function(){},onDestroy:function(){},onResize:function(){},onMove:function(){},onMaximize:function(){},onRestore:function(){},onMinimize:function(){},onBeforeCollapse:function(){},onBeforeExpand:function(){},onCollapse:function(){},onExpand:function(){}}}(jQuery);