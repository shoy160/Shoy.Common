/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function(a){function b(b){function c(a){var b=a.panel("options");return Math.min(Math.max(b.height,b.minHeight),b.maxHeight)}function d(a){var b=a.panel("options");return Math.min(Math.max(b.width,b.minWidth),b.maxWidth)}function e(a,b){if(a.length){var d=a.panel("options"),e=c(a);a.panel("resize",{width:k.width(),height:e,left:0,top:"n"==b?0:k.height()-e}),l.height-=e,"n"==b&&(l.top+=e,!d.split&&d.border&&l.top--),!d.split&&d.border&&l.height++}}function f(a,b){if(a.length){var c=a.panel("options"),e=d(a);a.panel("resize",{width:e,height:l.height,left:"e"==b?k.width()-e:0,top:l.top}),l.width-=e,"w"==b&&(l.left+=e,!c.split&&c.border&&l.left--),!c.split&&c.border&&l.width++}}var g=a.data(b,"layout"),i=g.options,j=g.panels,k=a(b);"BODY"==b.tagName?k._fit():i.fit?k.css(k._fit()):k._fit(!1);var l={top:0,left:0,width:k.width(),height:k.height()};e(h(j.expandNorth)?j.expandNorth:j.north,"n"),e(h(j.expandSouth)?j.expandSouth:j.south,"s"),f(h(j.expandEast)?j.expandEast:j.east,"e"),f(h(j.expandWest)?j.expandWest:j.west,"w"),j.center.panel("resize",l)}function c(c){function e(b){b.children("div").each(function(){var b=a.fn.layout.parsePanelOptions(this);"north,south,east,west,center".indexOf(b.region)>=0&&d(c,b,this)})}var f=a(c);f.addClass("layout"),e(f.children("form").length?f.children("form"):f),f.append('<div class="layout-split-proxy-h"></div><div class="layout-split-proxy-v"></div>'),f.bind("_resize",function(d,e){var f=a.data(c,"layout").options;return(1==f.fit||e)&&b(c),!1})}function d(c,d,e){d.region=d.region||"center";var g=a.data(c,"layout").panels,h=a(c),i=d.region;if(!g[i].length){var k=a(e);k.length||(k=a("<div></div>").appendTo(h));var l=a.extend({},a.fn.layout.paneldefaults,{width:k.length?parseInt(k[0].style.width)||k.outerWidth():"auto",height:k.length?parseInt(k[0].style.height)||k.outerHeight():"auto",doSize:!1,collapsible:!0,cls:"layout-panel layout-panel-"+i,bodyCls:"layout-body",onOpen:function(){var b=a(this).panel("header").children("div.panel-tool");b.children("a.panel-tool-collapse").hide();var d={north:"up",south:"down",east:"right",west:"left"};if(d[i]){var e="layout-button-"+d[i],g=b.children("a."+e);g.length||(g=a('<a href="javascript:void(0)"></a>').addClass(e).appendTo(b),g.bind("click",{dir:i},function(a){return f(c,a.data.dir),!1})),a(this).panel("options").collapsible?g.show():g.hide()}}},d);if(k.panel(l),g[i]=k,k.panel("options").split){var m=k.panel("panel");m.addClass("layout-split-"+i);var n="";"north"==i&&(n="s"),"south"==i&&(n="n"),"east"==i&&(n="w"),"west"==i&&(n="e"),m.resizable(a.extend({},{handles:n,onStartResize:function(){if(j=!0,"north"==i||"south"==i)var b=a(">div.layout-split-proxy-v",c);else var b=a(">div.layout-split-proxy-h",c);var d={display:"block"};"north"==i?(d.top=parseInt(m.css("top"))+m.outerHeight()-b.height(),d.left=parseInt(m.css("left")),d.width=m.outerWidth(),d.height=b.height()):"south"==i?(d.top=parseInt(m.css("top")),d.left=parseInt(m.css("left")),d.width=m.outerWidth(),d.height=b.height()):"east"==i?(d.top=parseInt(m.css("top"))||0,d.left=parseInt(m.css("left"))||0,d.width=b.width(),d.height=m.outerHeight()):"west"==i&&(d.top=parseInt(m.css("top"))||0,d.left=m.outerWidth()-b.width(),d.width=b.width(),d.height=m.outerHeight()),b.css(d),a('<div class="layout-mask"></div>').css({left:0,top:0,width:h.width(),height:h.height()}).appendTo(h)},onResize:function(b){if("north"==i||"south"==i){var d=a(">div.layout-split-proxy-v",c);d.css("top",b.pageY-a(c).offset().top-d.height()/2)}else{var d=a(">div.layout-split-proxy-h",c);d.css("left",b.pageX-a(c).offset().left-d.width()/2)}return!1},onStopResize:function(a){h.children("div.layout-split-proxy-v,div.layout-split-proxy-h").hide(),k.panel("resize",a.data),b(c),j=!1,h.find(">div.layout-mask").remove()}},d))}}}function e(b,c){var d=a.data(b,"layout").panels;if(d[c].length){d[c].panel("destroy"),d[c]=a();var e="expand"+c.substring(0,1).toUpperCase()+c.substring(1);d[e]&&(d[e].panel("destroy"),d[e]=void 0)}}function f(b,c,d){function e(d){var e;"east"==d?e="layout-button-left":"west"==d?e="layout-button-right":"north"==d?e="layout-button-down":"south"==d&&(e="layout-button-up");var f=a("<div></div>").appendTo(b);return f.panel(a.extend({},a.fn.layout.paneldefaults,{cls:"layout-expand layout-expand-"+d,title:"&nbsp;",closed:!0,doSize:!1,tools:[{iconCls:e,handler:function(){return g(b,c),!1}}]})),f.panel("panel").hover(function(){a(this).addClass("layout-expand-over")},function(){a(this).removeClass("layout-expand-over")}),f}function i(){var d=a(b),e=k.center.panel("options");if("east"==c){var f=e.width+m.width-28;return(m.split||!m.border)&&f++,{resizeC:{width:f},expand:{left:d.width()-m.width},expandP:{top:e.top,left:d.width()-28,width:28,height:e.height},collapse:{left:d.width(),top:e.top,height:e.height}}}if("west"==c){var f=e.width+m.width-28;return(m.split||!m.border)&&f++,{resizeC:{width:f,left:27},expand:{left:0},expandP:{left:0,top:e.top,width:28,height:e.height},collapse:{left:-m.width,top:e.top,height:e.height}}}if("north"==c){var g=e.height;return h(k.expandNorth)||(g+=m.height-28+(m.split||!m.border?1:0)),k.east.add(k.west).add(k.expandEast).add(k.expandWest).panel("resize",{top:27,height:g}),{resizeC:{top:27,height:g},expand:{top:0},expandP:{top:0,left:0,width:d.width(),height:28},collapse:{top:-m.height,width:d.width()}}}if("south"==c){var g=e.height;return h(k.expandSouth)||(g+=m.height-28+(m.split||!m.border?1:0)),k.east.add(k.west).add(k.expandEast).add(k.expandWest).panel("resize",{height:g}),{resizeC:{height:g},expand:{top:d.height()-m.height},expandP:{top:d.height()-28,left:0,width:d.width(),height:28},collapse:{top:d.height(),width:d.width()}}}}void 0==d&&(d="normal");var k=a.data(b,"layout").panels,l=k[c],m=l.panel("options");if(0!=m.onBeforeCollapse.call(l)){var n="expand"+c.substring(0,1).toUpperCase()+c.substring(1);k[n]||(k[n]=e(c),k[n].panel("panel").bind("click",function(){var d=i();return l.panel("expand",!1).panel("open").panel("resize",d.collapse),l.panel("panel").animate(d.expand,function(){a(this).unbind(".layout").bind("mouseleave.layout",{region:c},function(a){1!=j&&f(b,a.data.region)})}),!1}));var o=i();h(k[n])||k.center.panel("resize",o.resizeC),l.panel("panel").animate(o.collapse,d,function(){l.panel("collapse",!1).panel("close"),k[n].panel("open").panel("resize",o.expandP),a(this).unbind(".layout")})}}function g(c,d){function e(){var b=a(c),e=f.center.panel("options");return"east"==d&&f.expandEast?{collapse:{left:b.width(),top:e.top,height:e.height},expand:{left:b.width()-f.east.panel("options").width}}:"west"==d&&f.expandWest?{collapse:{left:-f.west.panel("options").width,top:e.top,height:e.height},expand:{left:0}}:"north"==d&&f.expandNorth?{collapse:{top:-f.north.panel("options").height,width:b.width()},expand:{top:0}}:"south"==d&&f.expandSouth?{collapse:{top:b.height(),width:b.width()},expand:{top:b.height()-f.south.panel("options").height}}:void 0}var f=a.data(c,"layout").panels,g=f[d],h=g.panel("options");if(0!=h.onBeforeExpand.call(g)){var i=e(),j="expand"+d.substring(0,1).toUpperCase()+d.substring(1);f[j]&&(f[j].panel("close"),g.panel("panel").stop(!0,!0),g.panel("expand",!1).panel("open").panel("resize",i.collapse),g.panel("panel").animate(i.expand,function(){b(c)}))}}function h(a){return a&&a.length?a.panel("panel").is(":visible"):!1}function i(b){var c=a.data(b,"layout").panels;c.east.length&&c.east.panel("options").collapsed&&f(b,"east",0),c.west.length&&c.west.panel("options").collapsed&&f(b,"west",0),c.north.length&&c.north.panel("options").collapsed&&f(b,"north",0),c.south.length&&c.south.panel("options").collapsed&&f(b,"south",0)}var j=!1;a.fn.layout=function(d,e){return"string"==typeof d?a.fn.layout.methods[d](this,e):(d=d||{},this.each(function(){var e=a.data(this,"layout");if(e)a.extend(e.options,d);else{var f=a.extend({},a.fn.layout.defaults,a.fn.layout.parseOptions(this),d);a.data(this,"layout",{options:f,panels:{center:a(),north:a(),south:a(),east:a(),west:a()}}),c(this)}b(this),i(this)}))},a.fn.layout.methods={resize:function(a){return a.each(function(){b(this)})},panel:function(b,c){return a.data(b[0],"layout").panels[c]},collapse:function(a,b){return a.each(function(){f(this,b)})},expand:function(a,b){return a.each(function(){g(this,b)})},add:function(c,e){return c.each(function(){d(this,e),b(this),a(this).layout("panel",e.region).panel("options").collapsed&&f(this,e.region,0)})},remove:function(a,c){return a.each(function(){e(this,c),b(this)})}},a.fn.layout.parseOptions=function(b){return a.extend({},a.parser.parseOptions(b,[{fit:"boolean"}]))},a.fn.layout.defaults={fit:!1},a.fn.layout.parsePanelOptions=function(b){a(b);return a.extend({},a.fn.panel.parseOptions(b),a.parser.parseOptions(b,["region",{split:"boolean",minWidth:"number",minHeight:"number",maxWidth:"number",maxHeight:"number"}]))},a.fn.layout.paneldefaults=a.extend({},a.fn.panel.defaults,{region:null,split:!1,minWidth:10,minHeight:10,maxWidth:1e4,maxHeight:1e4})}(jQuery);