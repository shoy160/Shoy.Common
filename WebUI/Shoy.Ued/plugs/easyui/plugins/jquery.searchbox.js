/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function($){function _1(a){$(a).addClass("searchbox-f").hide();var b=$('<span class="searchbox"></span>').insertAfter(a),c=$('<input type="text" class="searchbox-text">').appendTo(b);$('<span><span class="searchbox-button"></span></span>').appendTo(b);var d=$(a).attr("name");return d&&(c.attr("name",d),$(a).removeAttr("name").attr("searchboxName",d)),b}function _6(a,b){var c=$.data(a,"searchbox").options,d=$.data(a,"searchbox").searchbox;b&&(c.width=b),d.appendTo("body"),isNaN(c.width)&&(c.width=d._outerWidth());var e=d.find("span.searchbox-button"),f=d.find("a.searchbox-menu"),g=d.find("input.searchbox-text");d._outerWidth(c.width)._outerHeight(c.height),g._outerWidth(d.width()-f._outerWidth()-e._outerWidth()),g.css({height:d.height()+"px",lineHeight:d.height()+"px"}),f._outerHeight(d.height()),e._outerHeight(d.height());var h=f.find("span.l-btn-left");h._outerHeight(d.height()),h.find("span.l-btn-text,span.m-btn-downarrow").css({height:h.height()+"px",lineHeight:h.height()+"px"}),d.insertAfter(a)}function _e(a){function b(b){c.searchbox.find("a.searchbox-menu").remove();var d=$('<a class="searchbox-menu" href="javascript:void(0)"></a>').html(b.text);d.prependTo(c.searchbox).menubutton({menu:c.menu,iconCls:b.iconCls}),c.searchbox.find("input.searchbox-text").attr("name",b.name||b.text),_6(a)}var c=$.data(a,"searchbox"),d=c.options;if(d.menu){c.menu=$(d.menu).menu({onClick:function(a){b(a)}});var e=c.menu.children("div.menu-item:first");c.menu.children("div.menu-item").each(function(){var a=$.extend({},$.parser.parseOptions(this),{selected:$(this).attr("selected")?!0:void 0});return a.selected?(e=$(this),!1):void 0}),e.triggerHandler("click")}else c.searchbox.find("a.searchbox-menu").remove(),c.menu=null}function _17(a){var b=$.data(a,"searchbox"),c=b.options,d=b.searchbox.find("input.searchbox-text"),e=b.searchbox.find(".searchbox-button");d.unbind(".searchbox").bind("blur.searchbox",function(){c.value=$(this).val(),""==c.value?($(this).val(c.prompt),$(this).addClass("searchbox-prompt")):$(this).removeClass("searchbox-prompt")}).bind("focus.searchbox",function(){$(this).val()!=c.value&&$(this).val(c.value),$(this).removeClass("searchbox-prompt")}).bind("keydown.searchbox",function(b){return 13==b.keyCode?(b.preventDefault(),c.value=$(this).val(),c.searcher.call(a,c.value,d._propAttr("name")),!1):void 0}),e.unbind(".searchbox").bind("click.searchbox",function(){c.searcher.call(a,c.value,d._propAttr("name"))}).bind("mouseenter.searchbox",function(){$(this).addClass("searchbox-button-hover")}).bind("mouseleave.searchbox",function(){$(this).removeClass("searchbox-button-hover")})}function _1d(a){var b=$.data(a,"searchbox"),c=b.options,d=b.searchbox.find("input.searchbox-text");""==c.value?(d.val(c.prompt),d.addClass("searchbox-prompt")):(d.val(c.value),d.removeClass("searchbox-prompt"))}$.fn.searchbox=function(a,b){return"string"==typeof a?$.fn.searchbox.methods[a](this,b):(a=a||{},this.each(function(){var b=$.data(this,"searchbox");b?$.extend(b.options,a):b=$.data(this,"searchbox",{options:$.extend({},$.fn.searchbox.defaults,$.fn.searchbox.parseOptions(this),a),searchbox:_1(this)}),_e(this),_1d(this),_17(this),_6(this)}))},$.fn.searchbox.methods={options:function(a){return $.data(a[0],"searchbox").options},menu:function(a){return $.data(a[0],"searchbox").menu},textbox:function(a){return $.data(a[0],"searchbox").searchbox.find("input.searchbox-text")},getValue:function(a){return $.data(a[0],"searchbox").options.value},setValue:function(a,b){return a.each(function(){$(this).searchbox("options").value=b,$(this).searchbox("textbox").val(b),$(this).searchbox("textbox").blur()})},getName:function(a){return $.data(a[0],"searchbox").searchbox.find("input.searchbox-text").attr("name")},selectName:function(a,b){return a.each(function(){var a=$.data(this,"searchbox").menu;a&&a.children('div.menu-item[name="'+b+'"]').triggerHandler("click")})},destroy:function(a){return a.each(function(){var a=$(this).searchbox("menu");a&&a.menu("destroy"),$.data(this,"searchbox").searchbox.remove(),$(this).remove()})},resize:function(a,b){return a.each(function(){_6(this,b)})}},$.fn.searchbox.parseOptions=function(_2a){var t=$(_2a);return $.extend({},$.parser.parseOptions(_2a,["width","height","prompt","menu"]),{value:t.val(),searcher:t.attr("searcher")?eval(t.attr("searcher")):void 0})},$.fn.searchbox.defaults={width:"auto",height:22,prompt:"",value:"",menu:null,searcher:function(){}}}(jQuery);