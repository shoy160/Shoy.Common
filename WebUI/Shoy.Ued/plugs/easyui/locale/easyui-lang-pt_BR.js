/*! tjzx.ued version:0.1.0 2014-05-10 09:49:49 */
$.fn.pagination&&($.fn.pagination.defaults.beforePageText="Página",$.fn.pagination.defaults.afterPageText="de {pages}",$.fn.pagination.defaults.displayMsg="Mostrando {from} a {to} de {total} itens"),$.fn.datagrid&&($.fn.datagrid.defaults.loadMsg="Processando, aguarde ..."),$.fn.treegrid&&$.fn.datagrid&&($.fn.treegrid.defaults.loadMsg=$.fn.datagrid.defaults.loadMsg),$.messager&&($.messager.defaults.ok="Ok",$.messager.defaults.cancel="Cancelar"),$.fn.validatebox&&($.fn.validatebox.defaults.missingMessage="Esse campo é requerido.",$.fn.validatebox.defaults.rules.email.message="Insira um endereço de email válido.",$.fn.validatebox.defaults.rules.url.message="Insira uma URL válida.",$.fn.validatebox.defaults.rules.length.message="Insira uma valor entre {0} e {1}.",$.fn.validatebox.defaults.rules.remote.message="Corrija esse campo."),$.fn.numberbox&&($.fn.numberbox.defaults.missingMessage="Esse campo é requerido."),$.fn.combobox&&($.fn.combobox.defaults.missingMessage="Esse campo é requerido."),$.fn.combotree&&($.fn.combotree.defaults.missingMessage="Esse campo é requerido."),$.fn.combogrid&&($.fn.combogrid.defaults.missingMessage="Esse campo é requerido."),$.fn.calendar&&($.fn.calendar.defaults.weeks=["D","S","T","Q","Q","S","S"],$.fn.calendar.defaults.months=["Jan","Fev","Mar","Abr","Mai","Jun","Jul","Ago","Set","Out","Nov","Dez"]),$.fn.datebox&&($.fn.datebox.defaults.currentText="Hoje",$.fn.datebox.defaults.closeText="Fechar",$.fn.datebox.defaults.okText="Ok",$.fn.datebox.defaults.missingMessage="Esse campo é requerido."),$.fn.datetimebox&&$.fn.datebox&&$.extend($.fn.datetimebox.defaults,{currentText:$.fn.datebox.defaults.currentText,closeText:$.fn.datebox.defaults.closeText,okText:$.fn.datebox.defaults.okText,missingMessage:$.fn.datebox.defaults.missingMessage});