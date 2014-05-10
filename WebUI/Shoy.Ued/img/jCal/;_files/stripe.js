$(".stripe tr").hover(function(){
	$(this).toggleClass("over");
});

$(".qttable tr").hover(function(){
	$(this).toggleClass("over1");
});
(function($){
	$(".stripe tr:even").addClass("alt");
	$(".qttable tr:even").addClass("alt1");
})(jQuery);
/*
$(document).ready(function(){ //这个就是传说的ready   
	$(".stripe tr").mouseover(function(){   
	   //如果鼠标移到class为stripe的表格的tr上时，执行函数   
	  $(this).addClass("over");}).mouseout(function(){   
			//给这行添加class值为over，并且当鼠标一出该行时执行函数   
			$(this).removeClass("over");}) //移除该行的class   
  $(".stripe tr:even").addClass("alt");   
	//给class为stripe的表格的偶数行添加class值为alt
  });

$(document).ready(function(){ //这个就是传说的ready
	$(".qttable tr").mouseover(function(){   
	   //如果鼠标移到class为stripe的表格的tr上时，执行函数   
	  $(this).addClass("over1");}).mouseout(function(){   
			//给这行添加class值为over，并且当鼠标一出该行时执行函数   
			$(this).removeClass("over1");}) //移除该行的class   
  $(".qttable tr:even").addClass("alt1");   
	//给class为stripe的表格的偶数行添加class值为alt
  });
  */