/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function($){function _1(a){var b=$.data(a,"tabs").options;if("left"!=b.tabPosition&&"right"!=b.tabPosition&&b.showHeader){var c=$(a).children("div.tabs-header"),d=c.children("div.tabs-tool"),e=c.children("div.tabs-scroller-left"),f=c.children("div.tabs-scroller-right"),g=c.children("div.tabs-wrap"),h=c.outerHeight();b.plain&&(h-=h-c.height()),d._outerHeight(h);var i=0;$("ul.tabs li",c).each(function(){i+=$(this).outerWidth(!0)});var j=c.width()-d._outerWidth();i>j?(e.add(f).show()._outerHeight(h),"left"==b.toolPosition?(d.css({left:e.outerWidth(),right:""}),g.css({marginLeft:e.outerWidth()+d._outerWidth(),marginRight:f._outerWidth(),width:j-e.outerWidth()-f.outerWidth()})):(d.css({left:"",right:f.outerWidth()}),g.css({marginLeft:e.outerWidth(),marginRight:f.outerWidth()+d._outerWidth(),width:j-e.outerWidth()-f.outerWidth()}))):(e.add(f).hide(),"left"==b.toolPosition?(d.css({left:0,right:""}),g.css({marginLeft:d._outerWidth(),marginRight:0,width:j})):(d.css({left:"",right:0}),g.css({marginLeft:0,marginRight:d._outerWidth(),width:j})))}}function _c(_d){var _e=$.data(_d,"tabs").options,_f=$(_d).children("div.tabs-header");if(_e.tools)if("string"==typeof _e.tools)$(_e.tools).addClass("tabs-tool").appendTo(_f),$(_e.tools).show();else{_f.children("div.tabs-tool").remove();for(var _10=$('<div class="tabs-tool"><table cellspacing="0" cellpadding="0" style="height:100%"><tr></tr></table></div>').appendTo(_f),tr=_10.find("tr"),i=0;i<_e.tools.length;i++){var td=$("<td></td>").appendTo(tr),_11=$('<a href="javascript:void(0);"></a>').appendTo(td);_11[0].onclick=eval(_e.tools[i].handler||function(){}),_11.linkbutton($.extend({},_e.tools[i],{plain:!0}))}}else _f.children("div.tabs-tool").remove()}function _12(a){var b=$.data(a,"tabs"),c=b.options,d=$(a);c.fit?$.extend(c,d._fit()):d._fit(!1),d.width(c.width).height(c.height);for(var e=$(a).children("div.tabs-header"),f=$(a).children("div.tabs-panels"),g=e.find("div.tabs-wrap"),h=g.find(".tabs"),i=0;i<b.tabs.length;i++){var j=b.tabs[i].panel("options"),k=j.tab.find("a.tabs-inner"),l=parseInt(j.tabWidth||c.tabWidth)||void 0;l?k._outerWidth(l):k.css("width",""),k._outerHeight(c.tabHeight),k.css("lineHeight",k.height()+"px")}if("left"==c.tabPosition||"right"==c.tabPosition)e._outerWidth(c.showHeader?c.headerWidth:0),f._outerWidth(d.width()-e.outerWidth()),e.add(f)._outerHeight(c.height),g._outerWidth(e.width()),h._outerWidth(g.width()).css("height","");else{var m=e.children("div.tabs-scroller-left,div.tabs-scroller-right,div.tabs-tool");e._outerWidth(c.width).css("height",""),c.showHeader?(e.css("background-color",""),g.css("height",""),m.show()):(e.css("background-color","transparent"),e._outerHeight(0),g._outerHeight(0),m.hide()),h._outerHeight(c.tabHeight).css("width",""),_1(a);var n=c.height;isNaN(n)?f.height("auto"):f._outerHeight(n-e.outerHeight());var l=c.width;isNaN(l)?f.width("auto"):f._outerWidth(l)}}function _1c(a){var b=$.data(a,"tabs").options,c=_1f(a);if(c){var d=$(a).children("div.tabs-panels"),e="auto"==b.width?"auto":d.width(),f="auto"==b.height?"auto":d.height();c.panel("resize",{width:e,height:f})}}function _23(a){var b=$.data(a,"tabs").tabs,c=$(a);c.addClass("tabs-container");var d=$('<div class="tabs-panels"></div>').insertBefore(c);c.children("div").each(function(){d[0].appendChild(this)}),c[0].appendChild(d[0]),$('<div class="tabs-header"><div class="tabs-scroller-left"></div><div class="tabs-scroller-right"></div><div class="tabs-wrap"><ul class="tabs"></ul></div></div>').prependTo(a),c.children("div.tabs-panels").children("div").each(function(){var c=$.extend({},$.parser.parseOptions(this),{selected:$(this).attr("selected")?!0:void 0}),d=$(this);b.push(d),_36(a,d,c)}),c.children("div.tabs-header").find(".tabs-scroller-left, .tabs-scroller-right").hover(function(){$(this).addClass("tabs-scroller-over")},function(){$(this).removeClass("tabs-scroller-over")}),c.bind("_resize",function(b,c){var d=$.data(a,"tabs").options;return(1==d.fit||c)&&(_12(a),_1c(a)),!1})}function _29(a){function b(a){var b=0;return a.parent().children("li").each(function(c){return a[0]==this?(b=c,!1):void 0}),b}var c=$.data(a,"tabs"),d=c.options;$(a).children("div.tabs-header").unbind().bind("click",function(e){if($(e.target).hasClass("tabs-scroller-left"))$(a).tabs("scrollBy",-d.scrollIncrement);else if($(e.target).hasClass("tabs-scroller-right"))$(a).tabs("scrollBy",d.scrollIncrement);else{var f=$(e.target).closest("li");if(f.hasClass("tabs-disabled"))return;var g=$(e.target).closest("a.tabs-close");if(g.length)_4c(a,b(f));else if(f.length){var h=b(f),i=c.tabs[h].panel("options");i.collapsible?i.closed?_41(a,h):_6b(a,h):_41(a,h)}}}).bind("contextmenu",function(c){var e=$(c.target).closest("li");e.hasClass("tabs-disabled")||e.length&&d.onContextMenu.call(a,c,e.find("span.tabs-title").html(),b(e))})}function _31(a){var b=$.data(a,"tabs").options,c=$(a).children("div.tabs-header"),d=$(a).children("div.tabs-panels");c.removeClass("tabs-header-top tabs-header-bottom tabs-header-left tabs-header-right"),d.removeClass("tabs-panels-top tabs-panels-bottom tabs-panels-left tabs-panels-right"),"top"==b.tabPosition?c.insertBefore(d):"bottom"==b.tabPosition?(c.insertAfter(d),c.addClass("tabs-header-bottom"),d.addClass("tabs-panels-top")):"left"==b.tabPosition?(c.addClass("tabs-header-left"),d.addClass("tabs-panels-right")):"right"==b.tabPosition&&(c.addClass("tabs-header-right"),d.addClass("tabs-panels-left")),1==b.plain?c.addClass("tabs-header-plain"):c.removeClass("tabs-header-plain"),1==b.border?(c.removeClass("tabs-header-noborder"),d.removeClass("tabs-panels-noborder")):(c.addClass("tabs-header-noborder"),d.addClass("tabs-panels-noborder"))}function _36(a,b,c){var d=$.data(a,"tabs");c=c||{},b.panel($.extend({},c,{border:!1,noheader:!0,closed:!0,doSize:!1,iconCls:c.icon?c.icon:void 0,onLoad:function(){c.onLoad&&c.onLoad.call(this,arguments),d.options.onLoad.call(a,$(this))}}));var e=b.panel("options"),f=$(a).children("div.tabs-header").find("ul.tabs");e.tab=$("<li></li>").appendTo(f),e.tab.append('<a href="javascript:void(0)" class="tabs-inner"><span class="tabs-title"></span><span class="tabs-icon"></span></a>'),$(a).tabs("update",{tab:b,options:e})}function _3c(a,b){var c=$.data(a,"tabs").options,d=$.data(a,"tabs").tabs;void 0==b.selected&&(b.selected=!0);var e=$("<div></div>").appendTo($(a).children("div.tabs-panels"));d.push(e),_36(a,e,b),c.onAdd.call(a,b.title,d.length-1),_12(a),b.selected&&_41(a,d.length-1)}function _42(a,b){var c=$.data(a,"tabs").selectHis,d=b.tab,e=d.panel("options").title;d.panel($.extend({},b.options,{iconCls:b.options.icon?b.options.icon:void 0}));var f=d.panel("options"),g=f.tab,h=g.find("span.tabs-title"),i=g.find("span.tabs-icon");if(h.html(f.title),i.attr("class","tabs-icon"),g.find("a.tabs-close").remove(),f.closable?(h.addClass("tabs-closable"),$('<a href="javascript:void(0)" class="tabs-close"></a>').appendTo(g)):h.removeClass("tabs-closable"),f.iconCls?(h.addClass("tabs-with-icon"),i.addClass(f.iconCls)):h.removeClass("tabs-with-icon"),e!=f.title)for(var j=0;j<c.length;j++)c[j]==e&&(c[j]=f.title);if(g.find("span.tabs-p-tool").remove(),f.tools){var k=$('<span class="tabs-p-tool"></span>').insertAfter(g.find("a.tabs-inner"));if($.isArray(f.tools))for(var j=0;j<f.tools.length;j++){var l=$('<a href="javascript:void(0)"></a>').appendTo(k);l.addClass(f.tools[j].iconCls),f.tools[j].handler&&l.bind("click",{handler:f.tools[j].handler},function(a){$(this).parents("li").hasClass("tabs-disabled")||a.data.handler.call(this)})}else $(f.tools).children().appendTo(k);var m=12*k.children().length;f.closable?m+=8:(m-=3,k.css("right","5px")),h.css("padding-right",m+"px")}_12(a),$.data(a,"tabs").options.onUpdate.call(a,f.title,_4b(a,d))}function _4c(a,b){var c=$.data(a,"tabs").options,d=$.data(a,"tabs").tabs,e=$.data(a,"tabs").selectHis;if(_52(a,b)){var f=_53(a,b),g=f.panel("options").title,h=_4b(a,f);if(0!=c.onBeforeClose.call(a,g,h)){var f=_53(a,b,!0);f.panel("options").tab.remove(),f.panel("destroy"),c.onClose.call(a,g,h),_12(a);for(var i=0;i<e.length;i++)e[i]==g&&(e.splice(i,1),i--);var j=e.pop();j?_41(a,j):d.length&&_41(a,0)}}}function _53(a,b,c){var d=$.data(a,"tabs").tabs;if("number"==typeof b){if(0>b||b>=d.length)return null;var e=d[b];return c&&d.splice(b,1),e}for(var f=0;f<d.length;f++){var e=d[f];if(e.panel("options").title==b)return c&&d.splice(f,1),e}return null}function _4b(a,b){for(var c=$.data(a,"tabs").tabs,d=0;d<c.length;d++)if(c[d][0]==$(b)[0])return d;return-1}function _1f(a){for(var b=$.data(a,"tabs").tabs,c=0;c<b.length;c++){var d=b[c];if(0==d.panel("options").closed)return d}return null}function _5f(a){for(var b=$.data(a,"tabs"),c=b.tabs,d=0;d<c.length;d++)if(c[d].panel("options").selected)return void _41(a,d);_41(a,b.options.selected)}function _41(a,b){var c=$.data(a,"tabs"),d=c.options,e=c.tabs,f=c.selectHis;if(0!=e.length){var g=_53(a,b);if(g){var h=_1f(a);if(h){if(g[0]==h[0])return;if(_6b(a,_4b(a,h)),!h.panel("options").closed)return}g.panel("open");var i=g.panel("options").title;f.push(i);var j=g.panel("options").tab;j.addClass("tabs-selected");var k=$(a).find(">div.tabs-header>div.tabs-wrap"),l=j.position().left,m=l+j.outerWidth();if(0>l||m>k.width()){var n=l-(k.width()-j.width())/2;$(a).tabs("scrollBy",n)}else $(a).tabs("scrollBy",0);_1c(a),d.onSelect.call(a,i,_4b(a,g))}}}function _6b(a,b){var c=$.data(a,"tabs"),d=_53(a,b);if(d){var e=d.panel("options");e.closed||(d.panel("close"),e.closed&&(e.tab.removeClass("tabs-selected"),c.options.onUnselect.call(a,e.title,_4b(a,d))))}}function _52(a,b){return null!=_53(a,b)}function _77(a,b){var c=$.data(a,"tabs").options;c.showHeader=b,$(a).tabs("resize")}$.fn.tabs=function(a,b){return"string"==typeof a?$.fn.tabs.methods[a](this,b):(a=a||{},this.each(function(){var b,c=$.data(this,"tabs");c?(b=$.extend(c.options,a),c.options=b):($.data(this,"tabs",{options:$.extend({},$.fn.tabs.defaults,$.fn.tabs.parseOptions(this),a),tabs:[],selectHis:[]}),_23(this)),_c(this),_31(this),_12(this),_29(this),_5f(this)}))},$.fn.tabs.methods={options:function(a){var b=a[0],c=$.data(b,"tabs").options,d=_1f(b);return c.selected=d?_4b(b,d):-1,c},tabs:function(a){return $.data(a[0],"tabs").tabs},resize:function(a){return a.each(function(){_12(this),_1c(this)})},add:function(a,b){return a.each(function(){_3c(this,b)})},close:function(a,b){return a.each(function(){_4c(this,b)})},getTab:function(a,b){return _53(a[0],b)},getTabIndex:function(a,b){return _4b(a[0],b)},getSelected:function(a){return _1f(a[0])},select:function(a,b){return a.each(function(){_41(this,b)})},unselect:function(a,b){return a.each(function(){_6b(this,b)})},exists:function(a,b){return _52(a[0],b)},update:function(a,b){return a.each(function(){_42(this,b)})},enableTab:function(a,b){return a.each(function(){$(this).tabs("getTab",b).panel("options").tab.removeClass("tabs-disabled")})},disableTab:function(a,b){return a.each(function(){$(this).tabs("getTab",b).panel("options").tab.addClass("tabs-disabled")})},showHeader:function(a){return a.each(function(){_77(this,!0)})},hideHeader:function(a){return a.each(function(){_77(this,!1)})},scrollBy:function(a,b){return a.each(function(){function a(){var a=0,b=d.children("ul");return b.children("li").each(function(){a+=$(this).outerWidth(!0)}),a-d.width()+(b.outerWidth()-b.width())}var c=$(this).tabs("options"),d=$(this).find(">div.tabs-header>div.tabs-wrap"),e=Math.min(d._scrollLeft()+b,a());d.animate({scrollLeft:e},c.scrollDuration)})}},$.fn.tabs.parseOptions=function(a){return $.extend({},$.parser.parseOptions(a,["width","height","tools","toolPosition","tabPosition",{fit:"boolean",border:"boolean",plain:"boolean",headerWidth:"number",tabWidth:"number",tabHeight:"number",selected:"number",showHeader:"boolean"}]))},$.fn.tabs.defaults={width:"auto",height:"auto",headerWidth:150,tabWidth:"auto",tabHeight:27,selected:0,showHeader:!0,plain:!1,fit:!1,border:!0,tools:null,toolPosition:"right",tabPosition:"top",scrollIncrement:100,scrollDuration:400,onLoad:function(){},onSelect:function(){},onUnselect:function(){},onBeforeClose:function(){},onClose:function(){},onAdd:function(){},onUpdate:function(){},onContextMenu:function(){}}}(jQuery);