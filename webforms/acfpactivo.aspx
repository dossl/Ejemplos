<%@ Page Title="Mantenimiento de Activos" Language="C#" MasterPageFile="~/Paginas/MasterPages/AGMT_NESTED01.master" AutoEventWireup="true" CodeBehind="acfpactivo.aspx.cs" Inherits="CodeasWeb.Paginas.Modulos.Activos.Mantenimientos.acfpactivo" %>

<%@ Register Src="~/WebUserControls/Mantenimientos/wucMantenimiento.ascx" TagName="wucMantenimiento"
    TagPrefix="wucMantenimiento" %>
<%@ Register Src="~/WebUserControls/wucMensajePopUp/wucMensajePopUp.ascx" TagName="wucMensajePopUp" TagPrefix="wucMensaje" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<%@ Register Src="~/WebUserControls/Busquedas/wucGenericBusqueda/wucGenericBusqueda.ascx" TagPrefix="wucGenericBusqueda" TagName="wucGenericBusqueda" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>


<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .cw_contenedorPagina {
            width: 600px;
        }

        .row {
            *zoom: 1;
        }

        .datos {
            margin: 10px 10px 10px 10px !important;
        }

        .disable-text {
            background-color: #c0c0c0;
        }

        .txtIdSolic {
            width: 120px !important;
        }

        .txtNombreusuario {
            width: 220px !important;
        }


        .montoStyle {
            text-align: right;
        }

        .txtIdProducto {
            width: 120px !important;
        }

        .txtDescProducto {
            width: 200px !important;
        }

        .cantidad {
            width: 90px !important;
        }

        .fechaStyle {
            width: 100px !important;
        }

        .detalle {
            width: 230px !important;
        }

        .txtmargin {
            margin-left: 75px;
        }

        .cssCodProvedor {
            width: 80px !important;
        }

        .cssNombreCopania {
            width: 200px !important;
        }

        .montoStyle {
            text-align: right;
        }

        .idProducto {
            width: 140px;
        }

        .producto {
            width: 100px;
        }

        .panelbuscar {
            margin-left: 0!important;
            width: 55px!important;
            display: inline;
            float: left;
            position: relative;
        }

        .btnUploadImage {
            border: none;
            height: 50px;
            width: 50px;
            margin-top: -5px !important;
            background: url(../../../../Images/subirImagen.png) no-repeat;
            cursor: pointer;
            background-size: contain;
        }
    </style>
    <script type="text/javascript">

        function pageLoad(sender, args) {

            try {
                $(".txtF5CA34FB-5743-41E0-8EA0-C9F1A097F7A3").each(function () {

                    if ($(this).length > 0 && $(this).val() != '') {
                        MostrarMensajePopup($(this).val(), function () {
                            $(".txtF5CA34FB-5743-41E0-8EA0-C9F1A097F7A3").val('');
                        });
                    }
                });



                var startDateTextBox = $('#<%=txtdfechaacti.ClientID %>');

                startDateTextBox.datepicker({
                    changeMonth: true,
                    changeYear: true
                });


            } catch (err) {
                alert(err.message);
            }
            validaPerioVidaPrimeraCarga();

            var ex = document.getElementById('tabla_repeaterEncargados');

            if (!$.fn.DataTable.fnIsDataTable(ex)) {
                try {
                    var aLengthMenu = [];
                    aLengthMenu[0] = [10, 15, 50, 100, -1];
                    aLengthMenu[1] = ["10", "15", "50", "100", "Todos"];
                    $("#tabla_repeaterEncargados").DataTable({
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bFilter": false,
                        "bProcessing": true,
                        "oLanguage": { "sUrl": vlc_RutaScripts + "oLanguage1.txt" }
                        //"fnDrawCallback": function (oSettings) {
                        //    check_items();
                        //},
                        // "aLengthMenu": aLengthMenu,
                        //"iDisplayLength": 10,
                        //  "sPaginationType": "full_numbers"
                    });
                } catch (err) {
                    alert(err.message);
                }
            }

            var ex = document.getElementById('tabla_repeaterRevMejoras');

            if (!$.fn.DataTable.fnIsDataTable(ex)) {
                try {
                    var aLengthMenu = [];
                    aLengthMenu[0] = [10, 15, 50, 100, -1];
                    aLengthMenu[1] = ["10", "15", "50", "100", "Todos"];
                    $("#tabla_repeaterRevMejoras").DataTable({
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bFilter": false,
                        "bProcessing": true,
                        "oLanguage": { "sUrl": vlc_RutaScripts + "oLanguage1.txt" }
                        //"fnDrawCallback": function (oSettings) {
                        //    check_items();
                        //},
                        // "aLengthMenu": aLengthMenu,
                        //"iDisplayLength": 10,
                        //  "sPaginationType": "full_numbers"
                    });
                } catch (err) {
                    alert(err.message);
                }
            }

            var ex = document.getElementById('tabla_repeaterDesvalorizacion');

            if (!$.fn.DataTable.fnIsDataTable(ex)) {
                try {
                    var aLengthMenu = [];
                    aLengthMenu[0] = [10, 15, 50, 100, -1];
                    aLengthMenu[1] = ["10", "15", "50", "100", "Todos"];
                    $("#tabla_repeaterDesvalorizacion").DataTable({
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bFilter": false,
                        "bProcessing": true,
                        "oLanguage": { "sUrl": vlc_RutaScripts + "oLanguage1.txt" }
                        //"fnDrawCallback": function (oSettings) {
                        //    check_items();
                        //},
                        // "aLengthMenu": aLengthMenu,
                        //"iDisplayLength": 10,
                        //  "sPaginationType": "full_numbers"
                    });
                } catch (err) {
                    alert(err.message);
                }
            }

            var ex = document.getElementById('tabla_repeaterPolizas');

            if (!$.fn.DataTable.fnIsDataTable(ex)) {
                try {
                    var aLengthMenu = [];
                    aLengthMenu[0] = [10, 15, 50, 100, -1];
                    aLengthMenu[1] = ["10", "15", "50", "100", "Todos"];
                    $("#tabla_repeaterPolizas").DataTable({
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bFilter": false,
                        "bProcessing": true,
                        "oLanguage": { "sUrl": vlc_RutaScripts + "oLanguage1.txt" }
                        //"fnDrawCallback": function (oSettings) {
                        //    check_items();
                        //},
                        // "aLengthMenu": aLengthMenu,
                        //"iDisplayLength": 10,
                        //  "sPaginationType": "full_numbers"
                    });
                } catch (err) {
                    alert(err.message);
                }
            }

            check_radio_buttons();

            show_loading_dialog(false);
        }

        String.Format = function () {
            var s = arguments[0];
            for (var i = 0; i < arguments.length - 1; i++) {
                var reg = new RegExp("\\{" + i + "\\}", "gm");
                s = s.replace(reg, arguments[i + 1]);
            }
            return s;
        }


        function rellenar(quien, que) {
            cadcero = '';
            if (que.length > 0) {
                for (i = 0; i < (6 - que.length) ; i++) {
                    cadcero += '0';
                }
                quien.value = cadcero + que;
            }
        }

        function BlurDecimal(_campo, _decimales) {
            var s_decimal = '.';
            var point_index = _campo.value.indexOf(s_decimal);
            if (point_index == -1) {
                _campo.value = _campo.value + s_decimal;
                point_index = _campo.value.indexOf(s_decimal);

            }
            if (point_index == 0) {
                _campo.value = '0' + _campo.value;
                point_index++;
            }

            while (_campo.value.length - 1 - point_index < _decimales) {
                _campo.value = _campo.value + '0';

            }


        }

        function MuestraRelojSelectItem() {
            show_loading_dialog(true);
        }

        function disable_Boton_EditarBorrarMantenimiento(habilita) {
            $('#hlEditar').attr('disabled', habilita);
            $('#hlBorrar').attr('disabled', habilita);
            $('.waitWindown').remove();
        }

        //Diálogo de confirmación
        function MostrarConfirmación(obj, t, m) {
            //t="error",t="confirmation",t="warning",t="question"
            var tipo = (t != "" || t != null) ? t : "confirmation";
            //mensaje="Este es mi mensaje"
            var mensaje = (m != "" || m != null) ? m : "Validación satisfactoria";

            var settings = JSON.stringify({ "Tipo": 2, "ElementosInvalidos": null, "Titulo": "Validando…", "MarcaElementos": false, "MensajeSimple": mensaje, "TipoMensajeCliente": tipo, "OnOkPopUp": null }, null, 2);
            if (t == "question") {
                var botonSi = {
                    caption: 'Si',
                    callback: function () {
                        show_loading_dialog(true);
                        $(".ZebraDialogOverlay").remove();
                        $('#' + obj.id + 'Server').click();
                    }
                };

                var botonNo = {
                    caption: 'No',
                    callback: function () {

                    }
                };
                $.Zebra_Dialog(mensaje, { 'type': tipo, 'title': "Confirmación requerida", 'buttons': [botonSi, botonNo] });
            } else {
                MostrarMensajePopup(settings, function () {
                    $(".txtF5CA34FB-5743-41E0-8EA0-C9F1A097F7A3").val('');
                });
            }

            return false;
        }



        function MostrarPolizas() {
            $("#modal_polizas").attr("title", "Buscar pólizas del activo");

            var dialogSetting = {
                width: 670,
                height: 460,
                modal: true,
                resizable: false,
                open: function (event, ui) {
                    return true;
                },
                close: function (event, ui) {
                    return true;
                },
                beforecreate: function (event, ui) {
                    return true;
                },
                buttons: {
                    "Cerrar": function () {
                        $(".ui-dialog-titlebar-close").click();
                        //$("#modal_msg").dialog("close");
                    }
                }
            }
            $("#modal_polizas").dialog(dialogSetting);
            $("#<%= hdn_show_modal.ClientID %>").val("0");

            return false;
        }

        function btnCmdBusFoto_Click() {
            $("[id$=btnCmdBusFoto]").click();
        }

        function showimagepreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {

                    $("#img").attr("src", e.target.result);

                    $("#ctl00_ctl00_Contenido_Contenido02_txtcrutaimgfactura").attr("value", e.target.result);
                    $("#hfRutaFoto").val(e.target.result);

                }
                reader.readAsDataURL(input.files[0]);
            }

            $("#img").css("height", "130px");
            $("#img").css("width", "130px");

            var url = "<%=GetBaseUrl()%>";


            $("#hfLocation").val(url + "Uploads/Activos/");

        }

        function redondeo(numero, decimales) {
            var flotante = parseFloat(numero);
            var resultado = Math.round(flotante * Math.pow(10, decimales)) / Math.pow(10, decimales);
            return resultado;
        }

        function ValidarTipoReferencia() {
            var selectted_item = <%=wucGenericBusquedaTipoReferencia.IdControl%>_obtener_selected_item();

            if (selectted_item.ctiporefer != "") {
                var nporcdepre = redondeo(selectted_item.nporcdepre, 2);
                var nperiovida = redondeo(selectted_item.nperiovida, 2);
                var txtnporcdepre = (nporcdepre == 0) ? nporcdepre : Math.round(100 / nporcdepre);
                $("#<%= txtpervida.ClientID %>").val(txtnporcdepre.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                $("#<%= txtnporcdepre.ClientID %>").val(nporcdepre.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                var val = "<%=insertRegistro.ToString()%>";
                if (val == "True") {
                    if ($('#<%= radtipodepre.ClientID %> [value="L"]').is(":checked")) {
                        $("#<%= txtnporcdepre.ClientID %>").val(nporcdepre.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                        $("#<%= txtnperiovida.ClientID %>").val("0.00");
                        $("#<%= txtpervida.ClientID %>").val(txtnporcdepre.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

                    } else {
                        $("#<%= txtnporcdepre.ClientID %>").val("0.00");
                        $("#<%= txtnperiovida.ClientID %>").val(nperiovida.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                    }
                }
            } else {
                $("#<%= txtnperiovida.ClientID %>").val("0.00");
                $("#<%= txtnporcdepre.ClientID %>").val("0.00");
                $("#<%= txtpervida.ClientID %>").val("0.00");
            }
            validaDepreciable();


        }

        function validaDepreciable() {
            if ($('#<%= radcactdeprec.ClientID %> [value="N"]').is(":checked")) {
                $("#<%= txtnporcdepre.ClientID %>").prop("readonly", "readonly");
                $("#<%= txtnporcdepre.ClientID %>").addClass("generic-decimal-textbox disabled-textbox");

                $("#<%= txtndepreacum.ClientID %>").prop("readonly", "readonly");
                $("#<%= txtndepreacum.ClientID %>").addClass("generic-decimal-textbox disabled-textbox");
                $("#<%= txtnporcdepre.ClientID %>").val("0.00");
                $("#<%= txtndepreacum.ClientID %>").val("0.00");
            } else {
                $("#<%= txtnporcdepre.ClientID %>").prop("readonly", "");
                $("#<%= txtnporcdepre.ClientID %>").removeClass("disabled-textbox");
                $("#<%= txtnporcdepre.ClientID %>").addClass("generic-decimal-textbox");

                $("#<%= txtndepreacum.ClientID %>").prop("readonly", "");
                $("#<%= txtndepreacum.ClientID %>").removeClass("disabled-textbox");
                $("#<%= txtndepreacum.ClientID %>").addClass("generic-decimal-textbox");


            }


        }

        function validaTipoDepre() {

            if ($('#<%= radtipodepre.ClientID %> [value="L"]').is(":checked")) {
                $("#<%= txtnporcdepre.ClientID %>").removeClass("hidden");
                $("#<%= txtnperiovida.ClientID %>").addClass("hidden");
                $("#<%= lblnperiovida.ClientID %>").addClass("hidden");
                $("#<%= lblnporcdepre.ClientID %>").removeClass("hidden");
                $("#<%= txtpervida.ClientID %>").removeClass("hidden");
                $("#<%= lblpervida.ClientID %>").removeClass("hidden");

            }
            else {

                $("#<%= txtnporcdepre.ClientID %>").addClass("hidden");
                $("#<%= txtnperiovida.ClientID %>").removeClass("hidden");
                $("#<%= lblnperiovida.ClientID %>").removeClass("hidden");
                $("#<%= lblnporcdepre.ClientID %>").addClass("hidden");
                $("#<%= txtpervida.ClientID %>").addClass("hidden");
                $("#<%= lblpervida.ClientID %>").addClass("hidden");

            }
            ValidarTipoReferencia();


        }

        function validaPerioVidaPrimeraCarga() {

            if ($('#<%= radtipodepre.ClientID %> [value="L"]').is(":checked")) {
                $("#<%= txtnporcdepre.ClientID %>").removeClass("hidden");
                $("#<%= txtnperiovida.ClientID %>").addClass("hidden");
                $("#<%= lblnperiovida.ClientID %>").addClass("hidden");
                $("#<%= lblnporcdepre.ClientID %>").removeClass("hidden");
                $("#<%= txtpervida.ClientID %>").removeClass("hidden");
                $("#<%= lblpervida.ClientID %>").removeClass("hidden");

            }
            else {

                $("#<%= txtnporcdepre.ClientID %>").addClass("hidden");
                $("#<%= txtnperiovida.ClientID %>").removeClass("hidden");
                $("#<%= lblnperiovida.ClientID %>").removeClass("hidden");
                $("#<%= lblnporcdepre.ClientID %>").addClass("hidden");
                $("#<%= txtpervida.ClientID %>").addClass("hidden");
                $("#<%= lblpervida.ClientID %>").addClass("hidden");

            }



        }

        function MostrarMsjValidacion(t, m) {
            //t="error",t="confirmation",t="warning"
            var tipo = (t != "" || t != null) ? t : "confirmation";
            //mensaje="Este es mi mensaje"
            var mensaje = (m != "" || m != null) ? m : "Validación satisfactoria";

            var settings = JSON.stringify({ "Tipo": 2, "ElementosInvalidos": null, "Titulo": "Validando…", "MarcaElementos": false, "MensajeSimple": mensaje, "TipoMensajeCliente": tipo, "OnOkPopUp": null }, null, 2);

            MostrarMensajePopup(settings, function () {
                $(".txtF5CA34FB-5743-41E0-8EA0-C9F1A097F7A3").val('');
            });

        }

        function validaAnnosVidaUtil() {

            var auxtxtpervida = $("#<%= txtpervida.ClientID %>").val().split(",");
            var txtpervida = auxtxtpervida.join("");
            if (txtpervida < 0) {
                MostrarMsjValidacion("warning", "El monto debe ser mayor o igual a cero.");
                $("#<%= txtpervida.ClientID %>").val("0.00");
            }

            var auxtxtporcdep = $("#<%= txtnporcdepre.ClientID %>").val().split(",");
            var txtporcdep = auxtxtporcdep.join("");
            var auxtxtmontoac = $("#<%= txtnvalorcom.ClientID %>").val().split(",");
            var txtmontoac = auxtxtmontoac.join("");
            var auxtxtvalresi = $("#<%= txtnvalorresi.ClientID %>").val().split(",");
            var txtvalresi = auxtxtvalresi.join("");
            var auxtxtdepacum = $("#<%= txtndepreacum.ClientID %>").val().split(",");
            var txtdepacum = auxtxtdepacum.join("");

            $("#<%= txtdepmens.ClientID %>").val(ConventirCadenaDecimales((txtporcdep / 100 * (txtmontoac - txtvalresi)) / 12, 2));
            $("#<%= txtmontact.ClientID %>").val(ConventirCadenaDecimales(txtmontoac - txtdepacum, 2));



        }

        function ConventirCadenaDecimales(cadena, decimales) {
            var result = parseFloat(cadena).toFixed(decimales).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            return result;
        }

        function validaPorcientoDep(campo, decimales) {
            var auxtxtporcdep = $("#<%= txtnporcdepre.ClientID %>").val().split(",");
            var txtporcdep = auxtxtporcdep.join("");
            if (txtporcdep < 0) {
                MostrarMsjValidacion("warning", "El monto debe ser mayor o igual a cero.");
                $("#<%= txtpervida.ClientID %>").val("0.00");
            }


            var auxtxtmontoac = $("#<%= txtnvalorcom.ClientID %>").val().split(",");
            var txtmontoac = auxtxtmontoac.join("");
            var auxtxtdepacum = $("#<%= txtndepreacum.ClientID %>").val().split(",");
            var txtdepacum = auxtxtdepacum.join("");

            $("#<%= txtdepmens.ClientID %>").val(ConventirCadenaDecimales((((txtporcdep / 100) * txtmontoac) / 12), 2));
            $("#<%= txtmontact.ClientID %>").val(ConventirCadenaDecimales(txtmontoac - txtdepacum, 2));

            BlurDecimal(campo, decimales);
        }



        function validaValorResidual(campo, decimales) {

            var auxtxtvalresi = $("#<%= txtnvalorresi.ClientID %>").val().split(",");
            var txtvalresi = auxtxtvalresi.join("");
            if (txtvalresi < 0) {
                MostrarMsjValidacion("warning", "El monto debe ser mayor o igual a cero.");
                $("#<%= txtpervida.ClientID %>").val("0.00");
            }
            var auxtxtporcdep = $("#<%= txtnporcdepre.ClientID %>").val().split(",");
            var txtporcdep = auxtxtporcdep.join("");
            var auxtxtmontoac = $("#<%= txtnvalorcom.ClientID %>").val().split(",");
            var txtmontoac = auxtxtmontoac.join("");
            var auxtxtdepacum = $("#<%= txtndepreacum.ClientID %>").val().split(",");
            var txtdepacum = auxtxtdepacum.join("");


            $("#<%= txtdepmens.ClientID %>").val(ConventirCadenaDecimales(((txtporcdep / 100 * (txtmontoac - txtvalresi)) / 12), 2));
            $("#<%= txtmontact.ClientID %>").val(ConventirCadenaDecimales(txtmontoac - txtdepacum, 2));

            BlurDecimal(campo, decimales);
        }

        function validaMontoDeCompra(campo, decimales) {
            var auxtxtvalresi = $("#<%= txtnvalorresi.ClientID %>").val().split(",");
            var txtvalresi = auxtxtvalresi.join("");
            var auxtxtporcdep = $("#<%= txtnporcdepre.ClientID %>").val().split(",");
            var txtporcdep = auxtxtporcdep.join("");
            var auxtxtmontoac = $("#<%= txtnvalorcom.ClientID %>").val().split(",");
            var txtmontoac = auxtxtmontoac.join("");
            var auxtxtdepacum = $("#<%= txtndepreacum.ClientID %>").val().split(",");
            var txtdepacum = auxtxtdepacum.join("");

            $("#<%= txtdepmens.ClientID %>").val(ConventirCadenaDecimales(((txtporcdep / 100 * (txtmontoac - txtvalresi)) / 12), 2));
            $("#<%= txtmontact.ClientID %>").val(ConventirCadenaDecimales(txtmontoac - txtdepacum, 2));


            BlurDecimal(campo, decimales);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido02" runat="server">
    <form id="formacfpactivo" runat="server">

        <div class="cw_divCentrado cw_contenedorPagina " style="width: 930px">
            <div class="container_12">

                <div class="container_12">

                    <div class="titulosCodeas">
                        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                        <%--<div con el mensaje aria-describedby confirmacion rekerida>--%>
                        <asp:Label ID="lblTituloP" runat="server" Text="Mantenimiento de Activos"></asp:Label>
                    </div>

                    <div class="">
                        <div class="container_12">
                            <div class="pro_wrapper">
                                <div class="grid_3" style="margin-right: 0!important;">
                                    <%--Botones Mantenimiento--%>
                                    <asp:Button CssClass="btn-md btn-new" Style="cursor: pointer; border: none; outline: none; background-color: transparent" ToolTip="Nuevo registro" ID="hlNuevo" ClientIDMode="Static" OnClientClick="show_loading_dialog(true)" OnClick="hlNuevo_OnClick" Text="" runat="server" />
                                    <asp:Button CssClass="btn-md btn-edit" Style="cursor: pointer; border: none; outline: none; background-color: transparent" ToolTip="Editar registro" ID="hlEditar" ClientIDMode="Static" OnClientClick="show_loading_dialog(true)" OnClick="hlEditar_OnClick" Text="" runat="server" />
                                    <asp:Button CssClass="btn-md btn-trash" Style="cursor: pointer; border: none; outline: none; background-color: transparent" ToolTip="Eliminar registro" ID="hlBorrar" ClientIDMode="Static" OnClientClick="return MostrarConfirmación(this,'question','¿Está seguro de eliminar el registro?');" OnClick="hlBorrar_OnClick" Text="" runat="server" />
                                    <asp:Button CssClass="hidden" ID="hlBorrarServer" ClientIDMode="Static" OnClick="hlBorrar_OnClick" Text="" runat="server" />

                                    <asp:Button CssClass="btn-md btn-accept" Visible="False" Style="cursor: pointer; border: none; outline: none; background-color: transparent" ToolTip="Confirmar transacción" ID="hlAceptar" ClientIDMode="Static" OnClientClick="show_loading_dialog(true)" OnClick="hlAceptar_OnClick" Text="" runat="server" />
                                    <asp:Button CssClass="btn-md btn-cancel" Visible="False" Style="cursor: pointer; border: none; outline: none; background-color: transparent" ToolTip="Cancelar transacción" ID="hlCancelar" ClientIDMode="Static" OnClientClick="return MostrarConfirmación(this,'question','¿Está seguro de cancelar la operación?');" Text="" runat="server" />
                                    <asp:Button CssClass="hidden" ID="hlCancelarServer" ClientIDMode="Static" OnClick="hlCancelar_OnClick" Text="" runat="server" />

                                    <%--Fin Botones Mantenimiento--%>
                                </div>
                                <div class="grid_1" Style="margin-left:-60px">
                                    <asp:Panel ID="panelBusqueda" CssClass="panelbuscar" runat="server">
                                        <wucGenericBusqueda:wucGenericBusqueda runat="server"
                                            ID="wucGenericBusqueda_BusquedaActivosEmp"
                                            IdControl="wucGenericBusqueda_BusquedaActivosEmp"
                                            NRegistrosPorDefecto="4"
                                            MenuNRegistrosPorPagina="2,4,8"
                                            PostBackOnIdSeleccionado="true"
                                            TablaConPaginacion="true" />

                                    </asp:Panel>

                                </div>

                                <asp:Panel ID="pnlRightBtn" CssClass="cont_columna pull-right" runat="server">
                                    <asp:Button ID="btnSalir" CssClass="btn-md btn-exit align-rigth" runat="server" OnClientClick="show_loading_dialog(true)" OnClick="btnSalir_Click" ToolTip="Salir" />
                                </asp:Panel>


                                <%-- MODALS BEGIN --%>
                                <div id="modal_msg" class="invisible" style="width: 100%; padding: 0;">
                                    <asp:HiddenField runat="server" ID="hdn_show_modal" />
                                    <div class="row" style="padding-top: 20px;">
                                        <div class="col-md-2" style="padding-right: 0;">
                                            <asp:Label runat="server" ID="modal_msg_ico"></asp:Label>
                                        </div>
                                        <div class="col-md-9" style="padding: 10px 0 0;">
                                            <asp:Label runat="server" ID="modal_msg_text"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <%-- MODALS END --%>
                                <div id="dialogVal" title="Confirmación requerida">
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
                <div class="separator"></div>
                <div class="separator"></div>
                <div class="cont_datos">

                    <asp:TabContainer ID="TabContainer1" style ="margin:5px" runat="server" ActiveTabIndex="0" TabIndex="1">

                        <ajaxToolkit:TabPanel runat="server" TabIndex="1" HeaderText="Características generales" ID="TabPanel1">
                            <ContentTemplate>
                                <asp:Panel ID="PanelDatos" runat="server">
                                    <div class="datos">
                                        <div class="separator"></div>
                                        <div class="container-fluid wuc_generic_panel" style="border: 1px solid #e0e0e0;">
                                            <div class="separator"></div>
                                            <div class="row">
                                                <div class="col-md-3-5">
                                                    <asp:Label ID="label1" runat="server" Text="Número de placa:" Width="60%"></asp:Label>
                                                    <asp:TextBox ID="txtcnumactivo" runat="server" onblur="rellenar(this,this.value)" Width="70px" Text=""></asp:TextBox>

                                                </div>


                                                <div class="col-md-4">
                                                    <asp:RadioButtonList RepeatDirection="Horizontal" Width="100%" ID="radcactdeprec" onchange="validaDepreciable()" runat="server">

                                                        <asp:ListItem Value="S" Selected="True" Text="Depreciable" />


                                                        <asp:ListItem Value="N" Text="No depreciable" />



                                                    </asp:RadioButtonList>


                                                </div>

                                                <div class="col-md-4">
                                                    <asp:Label Text="Estado del activo:" runat="server" />
                                                    <asp:Label ID="lblcestactivo" Text="" Style="color: black; font-size: 20px; margin-left: 10px" runat="server" />
                                                </div>


                                            </div>


                                            <div class="separator"></div>

                                            <div class="row">
                                                <div class="col-md-10">
                                                    <asp:Label ID="label2" runat="server" Text="Nombre del activo:" Width="20%"></asp:Label>
                                                    <asp:TextBox ID="txtcnomactiv" runat="server" Width="75%" Text=""></asp:TextBox>

                                                </div>
                                                <div class="col-md-1" style="width: 40px; margin-top: 8px">
                                                    <asp:Label ID="label3" runat="server" Text="Pólizas:"></asp:Label>


                                                </div>
                                                <div class="col-md-1" style="width: 65px; margin-left: 10px">
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                        <ContentTemplate>


                                                            <%-- Pólizas BEGIN --%>

                                                            <wucGenericBusqueda:wucGenericBusqueda runat="server"
                                                                ID="wucGenericBusquedaPolizas"
                                                                IdControl="wucGenericBusqueda_BusquedaPolizas"
                                                                NRegistrosPorDefecto="4"
                                                                MenuNRegistrosPorPagina="2,4,8"
                                                                PostBackOnIdSeleccionado="True"
                                                                TablaConPaginacion="true" />

                                                            <%-- Pólizas END --%>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>

                                            </div>
                                            <div class="separator"></div>
                                            <div class="row">
                                                <div class="col-md-2">
                                                    <asp:Label ID="Label4" Text="Tipo de activo:" runat="server" />

                                                </div>
                                                <div class="col-md-5" style="padding-left: 0px!important">
                                                    <asp:Panel ID="panelTipoActivos" Style="float: left" runat="server">
                                                        <wucGenericBusqueda:wucGenericBusqueda runat="server"
                                                            ID="wucGenericBusquedaTipoActivo"
                                                            IdControl="wucGenericBusqueda_BusquedaTipoActivo"
                                                            NRegistrosPorDefecto="4"
                                                            MenuNRegistrosPorPagina="2,4,8"
                                                            PostBackOnIdSeleccionado="False"
                                                            TablaConPaginacion="true" />
                                                    </asp:Panel>
                                                </div>

                                                <div class="col-md-5">
                                                    <asp:Label Text="Fecha de compra:" runat="server" />


                                                    <asp:TextBox runat="server" Style="width: 34%; margin-left: 18%" ID="txtdfechaacti" />


                                                </div>



                                            </div>
                                            <div class="separator"></div>
                                            <div class="row">
                                                <div class="col-md-7">
                                                    <asp:Label ID="Label5" Text="Centro de costo:" Width="29%" runat="server" />

                                                    <asp:DropDownList ID="ddlccodcentro" runat="server" Width="55%"></asp:DropDownList>

                                                </div>
                                            </div>
                                            <div class="separator"></div>

                                            <div class="row">
                                                <div class="col-md-2" style="padding-right: 0px!important">
                                                    <asp:Label ID="Label6" Text="Tipo de referencia:" runat="server" />

                                                </div>

                                                <div class="col-md-5" style="padding-left: 0px!important">
                                                    <asp:Panel ID="panelTipoReferencia" Style="float: left" runat="server">
                                                        <wucGenericBusqueda:wucGenericBusqueda runat="server"
                                                            ID="wucGenericBusquedaTipoReferencia"
                                                            IdControl="wucGenericBusqueda_BusquedaTipoReferencia"
                                                            NRegistrosPorDefecto="4"
                                                            MenuNRegistrosPorPagina="2,4,8"
                                                            PostBackOnIdSeleccionado="False"
                                                            clientFunctionOnSelect="ValidarTipoReferencia"
                                                            TablaConPaginacion="true" />
                                                    </asp:Panel>
                                                </div>


                                                <div class="col-md-5">
                                                    <asp:Label ID="Label7" Text="No. documento de compra:" runat="server" />


                                                    <asp:TextBox runat="server" Style="width: 34%" ID="txtcnumckcomp" />


                                                </div>



                                            </div>


                                            <div class="separator"></div>

                                            <div class="row">
                                                <div class="col-md-2-4" style="padding-right: 0px!important">
                                                    <asp:Label ID="Label8" Text="Tipo de depreciación:" runat="server" />

                                                </div>


                                                <div class="col-md-4-5" style="padding-left: 0px!important">

                                                    <asp:RadioButtonList RepeatDirection="Horizontal" Width="80%" ID="radtipodepre" onchange="validaTipoDepre()" runat="server">

                                                        <asp:ListItem Value="L" Text="Línea recta" />


                                                        <asp:ListItem Value="S" Selected="True" Text="Suma de dígitos" />



                                                    </asp:RadioButtonList>

                                                </div>

                                                <div class="col-md-4-5" style="padding-right: 0px !important; margin-left: 10px">

                                                    <asp:Label ID="lblpervida" Text="Años de vida útil:" Style="width: 50%" runat="server" />

                                                    <asp:TextBox runat="server" Style="width: 36%; text-align: right; margin-left: 20%" ID="txtpervida" />

                                                </div>



                                            </div>

                                            <div class="separator"></div>

                                            <div class="row">
                                                <div class="col-md-5" style="padding-right: 0px !important; margin-top: 20px">
                                                    <asp:Label ID="Label9" Text="Número de garantía:" Width="40%" runat="server" />

                                                    <asp:TextBox runat="server" Style="width: 55%" ID="txtcnumgarant" />

                                                </div>
                                                <div class="col-md-7">
                                                    <div class="col-md-12">
                                                        <asp:Label ID="Label10" Text="Descripción de la garantía:" runat="server" />
                                                    </div>
                                                    <div class="col-md-12" style="margin-top: 2px">

                                                        <asp:TextBox runat="server" TextMode="MultiLine" Height="50px" Width="95%" ID="txtcdescrigar" />
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="separator"></div>

                                            <div class="row" style="margin-left: 5px; margin-right: 5px">
                                                <asp:Label ID="Label15" Text="Características:" Style="margin-left: 15px;" runat="server" />

                                                <div class="col-md-12" style="border: 1px solid #e0e0e0; margin-top: 5px;">

                                                    <div class="col-md-4" style="margin-top: 10px; margin-bottom: 10px">
                                                        <asp:Label ID="Label13" Text="Marca:" runat="server" />
                                                        <asp:TextBox runat="server" Style="width: 55%" ID="txtcmarcaacti" />
                                                    </div>
                                                    <div class="col-md-4" style="margin-top: 10px; margin-bottom: 10px">
                                                        <asp:Label ID="Label12" Text="Modelo:" runat="server" />
                                                        <asp:TextBox runat="server" Style="width: 55%" ID="txtcmodeloact" />
                                                    </div>
                                                    <div class="col-md-4" style="margin-top: 10px; margin-bottom: 10px">
                                                        <asp:Label ID="Label14" Text="Serie:" runat="server" />
                                                        <asp:TextBox runat="server" Style="width: 55%" ID="txtcserieacti" />
                                                    </div>

                                                </div>

                                            </div>

                                            <div class="separator"></div>
                                            <div class="separator"></div>

                                            <div class="row">

                                                <div class="col-md-12" style="padding-left: 0px!important">

                                                    <div class="col-md-8" style="padding-left: 0px!important">
                                                        <div class="col-md-2" style="padding-right: 0px!important">
                                                            <asp:Label ID="Label17" Text="Proveedor:" runat="server" />

                                                        </div>
                                                        <div class="col-md-10" style="padding-left: 0px!important">
                                                            <asp:Panel ID="panelProveedor" Style="float: left" runat="server">
                                                                <wucGenericBusqueda:wucGenericBusqueda runat="server"
                                                                    ID="wucGenericBusquedaProveedor"
                                                                    IdControl="wucGenericBusqueda_BusquedaProveedor"
                                                                    NRegistrosPorDefecto="4"
                                                                    MenuNRegistrosPorPagina="2,4,8"
                                                                    PostBackOnIdSeleccionado="false"
                                                                    TablaConPaginacion="true" />
                                                            </asp:Panel>
                                                        </div>

                                                        <div class="col-md-2" style="padding-right: 0px !important; margin-top: 8px;">
                                                            <asp:Label ID="Label16" Text="Ubicación:" runat="server" />

                                                        </div>
                                                        <div class="col-md-10" style="padding-left: 0px!important; margin-top: 8px;">
                                                            <asp:Panel ID="panel1" Style="float: left" runat="server">
                                                                <wucGenericBusqueda:wucGenericBusqueda runat="server"
                                                                    ID="wucGenericBusquedaUbicacion"
                                                                    IdControl="wucGenericBusqueda_BusquedaUbicacion"
                                                                    NRegistrosPorDefecto="4"
                                                                    MenuNRegistrosPorPagina="2,4,8"
                                                                    PostBackOnIdSeleccionado="False"
                                                                    TablaConPaginacion="true" />
                                                            </asp:Panel>
                                                        </div>

                                                        <div class="col-md-2" style="margin-top: 8px;">
                                                            <asp:Label ID="Label18" Text="Descripción:" runat="server" />
                                                        </div>
                                                        <div class="col-md-10" style="margin-top: 5px">

                                                            <asp:TextBox runat="server" TextMode="MultiLine" Height="50px" Width="95%" ID="txtcdescactiv" />
                                                        </div>


                                                    </div>


                                                    <div class="col-md-4">

                                                        <div class="col-md-12" style="border: 1px solid #e0e0e0;">
                                                            <asp:HiddenField ID="hfRutaFoto" runat="server" Value="" ClientIDMode="Static" />
                                                            <asp:HiddenField ID="hfLocation" runat="server" Value="" ClientIDMode="Static" />
                                                            <div class="separator"></div>

                                                            <div class="col-md-12">
                                                                <div class="col-md-3">
                                                                    <asp:Label ID="label22" runat="server" Text="Foto:"></asp:Label>
                                                                </div>
                                                                <div class="col-md-9">
                                                                    <asp:FileUpload runat="server" ID="btnCmdBusFoto" CssClass="hidden" accept="image/bmp,image/gif,image/jpeg,image/png,image/tiff" onchange="showimagepreview(this)" EnableViewState="true" ClientIDMode="Static" />
                                                                    <asp:Image ID="img" runat="server" Height="130px" EnableViewState="true" ClientIDMode="Static" BorderWidth="1px" BorderStyle="Double" BackColor="#f0f0f0" />
                                                                </div>
                                                                <div class="separator"></div>

                                                            </div>

                                                            <div class="col-md-12">
                                                                <div class="col-md-12" style="margin-left: 40%; margin-top: 8px; margin-bottom: 5px">
                                                                    <asp:Button ID="btn_upload" runat="server" ToolTip="Subir foto" Style="cursor: pointer" CssClass="btnUploadImage" UseSubmitBehavior="false" CausesValidation="False" OnClientClick="btnCmdBusFoto_Click(); return false;" />
                                                                </div>

                                                            </div>


                                                        </div>



                                                    </div>



                                                </div>



                                            </div>

                                            <div class="separator"></div>

                                        </div>

                                    </div>
                                </asp:Panel>


                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" TabIndex="2" HeaderText="Valores" ID="TabPanel2">
                            <ContentTemplate>
                                <div class="separator"></div>
                                <asp:Panel ID="PanelDatos2" runat="server">
                                    <div class="container-fluid wuc_generic_panel" style="border: 1px solid #e0e0e0;">
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <asp:Label ID="lblnperiovida" Text="Años de vida útil:" runat="server" />
                                                <asp:TextBox runat="server" onblur="validaAnnosVidaUtil()" Style="width: 25%; text-align: right" ID="txtnperiovida" />
                                                <asp:Label ID="lblnporcdepre" Text="% Depreciación:" CssClass="hidden" runat="server" />
                                                <asp:TextBox runat="server" CssClass="hidden" onblur="validaPorcientoDep(this,2)" class="generic-decimal-textbox" generic-int-qty="23" Style="width: 25%; margin-left: 5px;" ID="txtnporcdepre" />

                                            </div>
                                            <div class="col-md-6">
                                                <asp:Label ID="Label20" Text="Depreciación mensual:" runat="server" />
                                                <asp:TextBox runat="server" Style="width: 35%; text-align: right" ID="txtdepmens" />

                                            </div>

                                        </div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <asp:Label ID="Label23" Text="Valor residual:" Width="27%" runat="server" />
                                                <asp:TextBox runat="server" class="generic-decimal-textbox" generic-int-qty="23" onblur="validaValorResidual(this,2)" Style="width: 35%" ID="txtnvalorresi" />

                                            </div>
                                            <div class="col-md-6">
                                                <asp:Label ID="Label24" Text="Valor de reposición:" Width="35.5%" runat="server" />
                                                <asp:TextBox runat="server" Style="width: 35%" CssClass="generic-decimal-textbox" generic-int-qty="23" onblur="BlurDecimal(this,2)" ID="txtnvalorrepo" />

                                            </div>

                                        </div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5">
                                                <asp:Label ID="Label25" Text="Montos generales:" Style="color: black; font-weight: bold" Width="50%" runat="server" />

                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:Label ID="Label26" Text="Depreciación acumulada:" Style="margin-left: 43%" Width="70%" runat="server" />

                                            </div>
                                            <div class="col-md-3">
                                                <asp:Label ID="Label27" Text="Valor en libros:" Style="margin-left: 41%" Width="50%" runat="server" />

                                            </div>

                                        </div>


                                        <div runat="server" style="border: 1px solid #e0e0e0; float: right; width: 75%; margin-right: 10px"></div>
                                        <div class="separator"></div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5" style="margin-left: 8px">
                                                <asp:Label ID="Label28" Text="Monto de compra nuevo:" Width="55%" runat="server" />

                                                <asp:TextBox runat="server" CssClass="generic-decimal-textbox" generic-int-qty="23" onblur="validaMontoDeCompra(this,2)" Style="width: 35%" ID="txtnvalorcom" />


                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:TextBox runat="server" Style="width: 50%; margin-left: 43%" CssClass="generic-decimal-textbox" generic-int-qty="23" onblur="validaMontoDeCompra(this,2)" ID="txtndepreacum" />


                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox runat="server" onclick="event.preventDefault()" onmouseover="return false" Style="width: 55%; margin-left: 41%; text-align: right;" ID="txtmontact" onkeypress="return false" />

                                            </div>

                                        </div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5" style="margin-left: 8px">
                                                <asp:Label ID="Label29" Text="Monto de compra inicial:" Width="55%" runat="server" />

                                                <asp:TextBox runat="server" Style="width: 35%" CssClass="generic-decimal-textbox" generic-int-qty="23" onblur="validaMontoDeCompra(this,2)" ID="txtnvalorcomi" />


                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:TextBox runat="server" Visible="False" Style="width: 50%; margin-left: 43%; text-align: right" ID="txtmontoact1" />


                                            </div>


                                        </div>

                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5">
                                                <asp:Label ID="Label30" Text="Mejoras del activo:" Style="color: black; font-weight: bold" Width="50%" runat="server" />

                                            </div>


                                        </div>


                                        <div id="Div2" runat="server" style="border: 1px solid #e0e0e0; float: right; width: 75%; margin-right: 10px"></div>
                                        <div class="separator"></div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5" style="margin-left: 8px">
                                                <asp:Label ID="Label31" Text="Total mejoras:" Width="55%" runat="server" />

                                                <asp:TextBox runat="server" Style="width: 35%; text-align: right" ID="txtnmontomejo" />


                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:TextBox runat="server" Style="width: 50%; margin-left: 43%; text-align: right" ID="txtnmontdpmej" />


                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox runat="server" Style="width: 55%; margin-left: 41%; text-align: right" ID="txtmonmejo" />

                                            </div>

                                        </div>

                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5">
                                                <asp:Label ID="Label32" Text="Revaluaciones del activo:" Style="color: black; font-weight: bold" Width="55%" runat="server" />

                                            </div>


                                        </div>


                                        <div id="Div3" runat="server" style="border: 1px solid #e0e0e0; float: right; width: 75%; margin-right: 10px"></div>
                                        <div class="separator"></div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="col-md-5" style="margin-left: 8px">
                                                <asp:Label ID="Label33" Text="Total revaluaciones:" Width="55%" runat="server" />

                                                <asp:TextBox runat="server" Style="width: 35%; text-align: right" ID="txtnmontoreva" />


                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:TextBox runat="server" Style="width: 50%; margin-left: 43%; text-align: right" ID="txtnmontdprev" />


                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox runat="server" Style="width: 55%; margin-left: 40%; text-align: right" ID="txtmonreva" />

                                            </div>

                                        </div>
                                        <div class="separator"></div>
                                        <div class="separator"></div>
                                        <div id="Div4" runat="server" style="border: 1px solid #e0e0e0; float: right; width: 98%; margin-right: 10px"></div>
                                        <div class="separator"></div>
                                        <div class="separator"></div>

                                        <div class="row">
                                            <div class="col-md-5">
                                                <asp:Label ID="Label34" Text="Total liquidaciones:" Width="57%" runat="server" />

                                                <asp:TextBox runat="server" Style="width: 35%" CssClass="generic-decimal-textbox" generic-int-qty="23" onblur="BlurDecimal(this,2)" ID="txtnmontliqui" />


                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:Label ID="Label35" Text="Desvalorización:" Width="45%" runat="server" />
                                                <asp:TextBox runat="server" Style="width: 50%; text-align: right" ID="txtnmontdesva" />


                                            </div>
                                            <div class="col-md-3-5">
                                                <asp:Label ID="Label36" Text="Valor actual:" Width="35%" runat="server" />
                                                <asp:TextBox runat="server" Style="width: 46%; text-align: right; color: black; font-weight: bold" ID="txtmonactu" />

                                            </div>

                                        </div>
                                        <div class="separator"></div>
                                        <div class="separator"></div>

                                    </div>
                                </asp:Panel>


                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" TabIndex="3" HeaderText="Depreciaciones,Revaluaciones,Mejoras y Encargados" ID="TabPanel3">
                            <ContentTemplate>

                                <asp:Panel ID="PanelDatos3" runat="server">
                                    <div class="container-fluid wuc_generic_panel" style="border: 1px solid #e0e0e0;">
                                        <div class="row">
                                            <div class="separator"></div>
                                            <asp:Label ID="Label37" Text="Encargados del activo:" Width="35%" Style="margin-left: 8px" runat="server" />
                                            <div class="separator"></div>
                                            <asp:Repeater ID="RepeaterEncargados" runat="server">


                                                <HeaderTemplate>

                                                    <table id="tabla_repeaterEncargados" style="width: 100%">
                                                        <thead>
                                                            <tr>

                                                                <th>Id</th>
                                                                <th>Cédula</th>
                                                                <th>Nombre</th>
                                                                <th>Departamento</th>
                                                                <th>Puesto</th>

                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>

                                                <ItemTemplate>

                                                    <tr id="Tr2" runat="server" visible="True" onclick="Seleccionar_Fila(this)">


                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="ccodperson" Style="width: 100%;" Text='<%# Eval("ccodperson")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("ncantidad")%>'--%>

                                                        </td>

                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="ccedulaper" Style="width: 100%;" Text='<%# Eval("ccedulaper")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cnumproduc")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="cnombre" Style="width: 100%;" Text='<%# Eval("cnombre")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>

                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="departamento" Style="width: 100%;" Text='<%# Eval("departamento")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>

                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="puesto" Style="width: 100%;" Text='<%# Eval("puesto")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>

                                                <FooterTemplate>
                                                    </tbody>
                                            </table>   
                                                   
                                                </FooterTemplate>


                                            </asp:Repeater>

                                        </div>
                                        <div class="separator"></div>
                                        <div class="row">
                                            <div class="separator"></div>
                                            <asp:Label ID="Label38" Text="Revaluaciones y mejoras:" Width="35%" Style="margin-left: 8px" runat="server" />
                                            <div class="separator"></div>
                                            <asp:Repeater ID="RepeaterRevMejoras" runat="server">


                                                <HeaderTemplate>

                                                    <table id="tabla_repeaterRevMejoras" style="width: 100%;">
                                                        <thead>
                                                            <tr>

                                                                <th>No. trans.</th>
                                                                <th style="width: 100px">Tipo</th>
                                                                <th style="width: 200px">Descripción</th>
                                                                <th style="width: 100px">Fecha</th>
                                                                <th>Monto revaluación</th>
                                                                <th>% revaluación</th>
                                                                <th>Depreciación acumulada</th>
                                                                <th>Valor residual</th>

                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>

                                                <ItemTemplate>

                                                    <tr id="Tr2" runat="server" visible="True" onclick="Seleccionar_Fila(this)">


                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="cnumtransa" Style="width: 100%;" Text='<%# Eval("cnumtransa")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("ncantidad")%>'--%>

                                                        </td>

                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="ctipotrans" Style="width: 100%;" Text='<%# Eval("ctipotrans")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cnumproduc")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="cdetallere" Style="width: 100%;" Text='<%# Eval("cdetallere")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="dfechamovi" Style="width: 100%;" Text='<%# Convert.ToDateTime(Eval("dfechamovi")).ToString("dd/MM/yyyy")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0; text-align: right">
                                                            <asp:Label ID="nvalorreva" Style="width: 100%;" Text='<%# FormatearDecimalString(Convert.ToDecimal(Eval("nvalorreva")))%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0; text-align: right">
                                                            <asp:Label ID="nporcereva" Style="width: 100%;" Text='<%# FormatearDecimalString(Convert.ToDecimal(Eval("nporcereva")))%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0; text-align: right">
                                                            <asp:Label ID="ndepreacum" Style="width: 100%;" Text='<%# FormatearDecimalString(Convert.ToDecimal(Eval("ndepreacum")))%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0; text-align: right">
                                                            <asp:Label ID="nvalorresi" Style="width: 100%;" Text='<%# FormatearDecimalString(Convert.ToDecimal(Eval("nvalorresi")))%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>

                                                <FooterTemplate>
                                                    </tbody>
                                            </table>   
                                                   
                                                </FooterTemplate>


                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </asp:Panel>

                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" TabIndex="4" HeaderText="Desvalorización" ID="TabPanel4">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server">
                                    <div class="container-fluid wuc_generic_panel" style="border: 1px solid #e0e0e0;">
                                        <div class="row">
                                            <div class="separator"></div>
                                            <asp:Label ID="Label19" Text="Desvalorizaciones:" Width="35%" Style="margin-left: 8px" runat="server" />
                                            <div class="separator"></div>
                                            <asp:Repeater ID="RepeaterDesvalorizacion" runat="server">


                                                <HeaderTemplate>

                                                    <table id="tabla_repeaterDesvalorizacion" style="width: 100%;">
                                                        <thead>
                                                            <tr>
                                                                <th>No. trans.</th>
                                                                <th>Descripción</th>
                                                                <th>Fecha</th>
                                                                <th>Desvalorización</th>

                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>

                                                <ItemTemplate>

                                                    <tr id="Tr2" runat="server" visible="True" onclick="Seleccionar_Fila(this)">


                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="cnumtransa" Style="width: 100%;" Text='<%# Eval("cnummovimi")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("ncantidad")%>'--%>

                                                        </td>

                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="ctipotrans" Style="width: 100%;" Text='<%# Eval("cdescmovim")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cnumproduc")%>'--%>
                                                        </td>

                                                        <td style="border: thin solid #C0C0C0;" class="txtCentrado">
                                                            <asp:Label ID="dfechamovi" Style="width: 100%;" Text='<%# Convert.ToDateTime(Eval("dfechamovi")).ToString("dd/MM/yyyy")%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                        <td style="border: thin solid #C0C0C0; text-align: right">
                                                            <asp:Label ID="nvalorreva" Style="width: 100%;" Text='<%# FormatearDecimalString(Convert.ToDecimal(Eval("monto")))%>' runat="server"></asp:Label>
                                                            <%-- Text='<%# Eval("cdescprodu")%>'--%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>

                                                <FooterTemplate>
                                                    </tbody>
                                            </table>   
                                                   
                                                </FooterTemplate>


                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </asp:Panel>

                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>

                    </asp:TabContainer>

                </div>

                <wucMensaje:wucMensajePopUp ID="wucMensajePopUp" ClientIDMode="Static" runat="server" CssClassContenedor="invisible" />
                <div class="separator"></div>
                <div class="separator"></div>

            </div>
        </div>
        <div class="separator"></div>

    </form>
</asp:Content>
