/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
!function(a){function b(b){var d=a.data(b,"timespinner").options;a(b).addClass("timespinner-f"),a(b).spinner(d),a(b).unbind(".timespinner"),a(b).bind("click.timespinner",function(){var a=0;if(null!=this.selectionStart)a=this.selectionStart;else if(this.createTextRange){var e=b.createTextRange(),f=document.selection.createRange();f.setEndPoint("StartToStart",e),a=f.text.length}a>=0&&2>=a?d.highlight=0:a>=3&&5>=a?d.highlight=1:a>=6&&8>=a&&(d.highlight=2),c(b)}).bind("blur.timespinner",function(){e(b)})}function c(b){var c=a.data(b,"timespinner").options,d=0,e=0;if(0==c.highlight?(d=0,e=2):1==c.highlight?(d=3,e=5):2==c.highlight&&(d=6,e=8),null!=b.selectionStart)b.setSelectionRange(d,e);else if(b.createTextRange){var f=b.createTextRange();f.collapse(),f.moveEnd("character",e),f.moveStart("character",d),f.select()}a(b).focus()}function d(b,c){var d=a.data(b,"timespinner").options;if(!c)return null;for(var e=c.split(d.separator),f=0;f<e.length;f++)if(isNaN(e[f]))return null;for(;e.length<3;)e.push(0);return new Date(1900,0,0,e[0],e[1],e[2])}function e(b){function c(a){return(10>a?"0":"")+a}var e=a.data(b,"timespinner").options,f=a(b).val(),g=d(b,f);if(!g)return e.value="",void a(b).val("");var h=d(b,e.min),i=d(b,e.max);h&&h>g&&(g=h),i&&g>i&&(g=i);var j=[c(g.getHours()),c(g.getMinutes())];e.showSeconds&&j.push(c(g.getSeconds()));var k=j.join(e.separator);e.value=k,a(b).val(k)}function f(b,d){var f=a.data(b,"timespinner").options,g=a(b).val();""==g&&(g=[0,0,0].join(f.separator));for(var h=g.split(f.separator),i=0;i<h.length;i++)h[i]=parseInt(h[i],10);1==d?h[f.highlight]-=f.increment:h[f.highlight]+=f.increment,a(b).val(h.join(f.separator)),e(b),c(b)}a.fn.timespinner=function(c,d){if("string"==typeof c){var e=a.fn.timespinner.methods[c];return e?e(this,d):this.spinner(c,d)}return c=c||{},this.each(function(){var d=a.data(this,"timespinner");d?a.extend(d.options,c):(a.data(this,"timespinner",{options:a.extend({},a.fn.timespinner.defaults,a.fn.timespinner.parseOptions(this),c)}),b(this))})},a.fn.timespinner.methods={options:function(b){var c=a.data(b[0],"timespinner").options;return a.extend(c,{value:b.val(),originalValue:b.spinner("options").originalValue})},setValue:function(b,c){return b.each(function(){a(this).val(c),e(this)})},getHours:function(b){var c=a.data(b[0],"timespinner").options,d=b.val().split(c.separator);return parseInt(d[0],10)},getMinutes:function(b){var c=a.data(b[0],"timespinner").options,d=b.val().split(c.separator);return parseInt(d[1],10)},getSeconds:function(b){var c=a.data(b[0],"timespinner").options,d=b.val().split(c.separator);return parseInt(d[2],10)||0}},a.fn.timespinner.parseOptions=function(b){return a.extend({},a.fn.spinner.parseOptions(b),a.parser.parseOptions(b,["separator",{showSeconds:"boolean",highlight:"number"}]))},a.fn.timespinner.defaults=a.extend({},a.fn.spinner.defaults,{separator:":",showSeconds:!1,highlight:0,spin:function(a){f(this,a)}})}(jQuery);