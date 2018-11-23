using CodeasWeb.HelperClasses;
using CodeasWeb.WebUserControls.wucMensajePopUp;
using CodeasWebEntities.Entities.Admin;
using CodeasWebEntities.Entities.CuentasPorPagar;
using CodeasWebEntities.Entities.Activos;
using CodeasWebEntities.Entities.PlanillasRH;
using CodeasWebEntities.Entities.General;
using CodeasWebExceptions.Exceptions;
using CodeasWebServices.Services.Contabilidad;
using CodeasWebServices.Services.CuentasPorPagar;
using CodeasWebServices.Services.Deducciones;
using CodeasWebTools.Helpers;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CodeasWeb.Paginas.Modulos.Proveeduria.Reportes;
using CodeasWebDataAccess.DataAccess.Activos;
using CodeasWebEntities.Entities.Contabilidad;
using CodeasWebServices.Services.Activos;
using CodeasWebServices.Services.PlanillasRH;
using CodeasWebServices.Services.Inventario;
using CodeasWebTools.Sql;
using CodeasWebTools.Web;
using Microsoft.Reporting.WebForms;
using CodeasWebTools.Reports;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Org.BouncyCastle.Asn1;
using Border = DocumentFormat.OpenXml.Spreadsheet.Border;
using CheckBox = System.Web.UI.WebControls.CheckBox;
using ListItem = System.Web.UI.WebControls.ListItem;


namespace CodeasWeb.Paginas.Modulos.Activos.Mantenimientos
{
    public partial class acfpactivo : CWBasePage
    {


        string cultureInfoParaDecimales = CWLectorWebConfig.Met_CultureInfoParaDecimales();
        CultureInfo cultureInfo = new CultureInfo(CWLectorWebConfig.Met_CultureInfoParaDecimales());
        NumberStyles numberStyle = NumberStyles.Integer | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint;


        protected BEactactivos ActivoSelect
        {
            get
            {
                if (ViewState["ActivoSelect"] == null)
                    ViewState["ActivoSelect"] = new BEactactivos();
                return (BEactactivos)ViewState["ActivoSelect"];
            }
            set
            {
                ViewState["ActivoSelect"] = value;
            }
        }
        
      
        protected bool insertRegistro
        {
            get
            {
                if (ViewState["insertRegistro"] == null)
                    ViewState["insertRegistro"] = false;
                return (bool)ViewState["insertRegistro"];
            }
            set
            {
                ViewState["insertRegistro"] = value;
            }
        }

        protected bool MostrarMensajeReporte
        {
            get
            {
                if (ViewState["MostrarMensajeReporte"] == null)
                    ViewState["MostrarMensajeReporte"] = false;
                return (bool)ViewState["MostrarMensajeReporte"];
            }
            set
            {
                ViewState["MostrarMensajeReporte"] = value;
            }
        }
        protected bool borrarRegistro
        {
            get
            {
                if (ViewState["borrarRegistro"] == null)
                    ViewState["borrarRegistro"] = false;
                return (bool)ViewState["borrarRegistro"];
            }
            set
            {
                ViewState["borrarRegistro"] = value;
            }
        }

        protected bool actualizarRegistro
        {
            get
            {
                if (ViewState["actualizarRegistro"] == null)
                    ViewState["actualizarRegistro"] = false;
                return (bool)ViewState["actualizarRegistro"];
            }
            set
            {
                ViewState["actualizarRegistro"] = value;
            }
        }

        private List<BErhempleado> listEmpleados
        {
            get
            {
                if (Session["listEmpleados"] == null)
                    return new List<BErhempleado>();

                return (List<BErhempleado>)Session["listEmpleados"];
            }
            set
            {
                Session["listEmpleados"] = value;
            }
        }

        private List<BEactrevmejo> listRevMejoras
        {
            get
            {
                if (Session["listRevMejoras"] == null)
                    return new List<BEactrevmejo>();

                return (List<BEactrevmejo>)Session["listRevMejoras"];
            }
            set
            {
                Session["listRevMejoras"] = value;
            }
        }

        private List<BEacttransac> listDesvalorizaciones
        {
            get
            {
                if (Session["listDesvalorizaciones"] == null)
                    return new List<BEacttransac>();

                return (List<BEacttransac>)Session["listDesvalorizaciones"];
            }
            set
            {
                Session["listDesvalorizaciones"] = value;
            }
        }

        private List<BEactpolizas> listPolizas
        {
            get
            {
                if (Session["listPolizas"] == null)
                    return new List<BEactpolizas>();

                return (List<BEactpolizas>)Session["listPolizas"];
            }
            set
            {
                Session["listPolizas"] = value;
            }
        }

        protected string RutaFoto
        {
            get
            {
                if (ViewState["RutaFoto"] == null)
                    ViewState["RutaFoto"] = string.Empty;
                return (string)ViewState["RutaFoto"];
            }
            set
            {
                ViewState["RutaFoto"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            this.Met_ValidarSesion();
            Uri MyUrl = Request.Url;
          

            if (!IsPostBack)
            {
                this.Met_ValidarSesion();
               
                this.Met_ActivarCampos(false);

                Met_ObtenerLinkYoutubeOpcion();

               
                int eventoSeleccionado = 0;
                this.LimpiaCampos(true, eventoSeleccionado);
                Met_CargarCentrosCosto();

               
            }
            AsignarMaxLength();

            Met_CrearYouTubeLink();

            this.Met_MostrarMantenimiento();
            

            #region configurando_wucGenericBusqueda_BusquedaActivosEmp

            //inicio paso 3 wuc generic configurando el user control 
            //------------------------------------------------------------------------------------
           
            //el nombre del sp que ejecuta el handler
            wucGenericBusqueda_BusquedaActivosEmp.action = "06011";
            wucGenericBusqueda_BusquedaActivosEmp.show_only_big_button = true;
            wucGenericBusqueda_BusquedaActivosEmp.Title = "Buscar activos";
            //textfield1data y textfield2data son los textfield principales(los de al lado de la lupa)
            wucGenericBusqueda_BusquedaActivosEmp.TextField1Data = "cnumactivo";//TextField1Data es el textfield donde se pone el valor del codigo
            wucGenericBusqueda_BusquedaActivosEmp.TextField2Data = "cnomactiv";//TextField2Data es el textfield donde se pone la descripccion

            //diccionario donde se pone los campos de la tabla
            //y del campo a filtrar
            //(si se quiere filtar por otro campos se debe crear otros diccionario similar)
            var area_fields = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"cnumactivo", "Número de activo"},
                {"cnomactiv", "Descripción"}
            };

            // filteroperator es el diccionario donde se pone el operador a filtrar
            var area_filteroperator = new Dictionary<string, string>
            {
                //llave, valor
                //donde la llave es el operador a filtrar
                //donde el valor es la accion que se va a realizar en el sp
                {"Contiene", "COMO"},
                {"Ninguno", "NONE"},
                {"Igual a", "IGUAL"},
                {"Mayor que", "MAYOR"},
                {"Menor que", "MENOR"},
                {"Mayor o igual que", "MAYIG"},
                {"Menor o igual que", "MENIG"},
                {"Diferente a", "DIFER"}
            };

           

            //se asignan los diccionarios 
            wucGenericBusqueda_BusquedaActivosEmp.FilterOperator = area_filteroperator;
            wucGenericBusqueda_BusquedaActivosEmp.FilterField = area_fields;
            wucGenericBusqueda_BusquedaActivosEmp.TableFields = area_fields;
            wucGenericBusqueda_BusquedaActivosEmp.clientFunctionOnSelect = "MuestraRelojSelectItem";


            //se asigna el control y la accion a realizar cuando se selecciona el item de la tabla
            wucGenericBusqueda_BusquedaActivosEmp.CargaControl += wucBusqueda_CargaControl;
            wucGenericBusqueda_BusquedaActivosEmp.ItemSelected += wucGenericBusqueda_BusquedaActivosEmp_Seleccionado;
            // -----------------------------------------------------------------------
            //fin paso 3 wuc generic configurando el user control 
            #endregion  #region configurando_wucGenericBusqueda_BusquedaActivosEmp

            #region configurando_wucGenericBusqueda_Busquedatipoactivos

            //inicio paso 3 wuc generic configurando el user control 
            //------------------------------------------------------------------------------------
           
            //el nombre del sp que ejecuta el handler
            wucGenericBusquedaTipoActivo.action = "06013";
            wucGenericBusquedaTipoActivo.Title = "Buscar tipos de activo";
            //textfield1data y textfield2data son los textfield principales(los de al lado de la lupa)
            wucGenericBusquedaTipoActivo.TextField1Data = "ctipoactiv";//TextField1Data es el textfield donde se pone el valor del codigo
            wucGenericBusquedaTipoActivo.TextField2Data = "cdestipact";//TextField2Data es el textfield donde se pone la descripccion

            //diccionario donde se pone los campos de la tabla
            //y del campo a filtrar
            //(si se quiere filtar por otro campos se debe crear otros diccionario similar)
            var area_fields_tipoactivos = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"ctipoactiv", "Tipo de activo"},
                {"cdestipact", "Descripción"}
            };

            // filteroperator es el diccionario donde se pone el operador a filtrar
            //var area_filteroperator_tipoactivos = new Dictionary<string, string>
            //{
            //    //llave, valor
            //    //donde la llave es el operador a filtrar
            //    //donde el valor es la accion que se va a realizar en el sp
            //    {"Contiene", "COMO"},
            //    {"Ninguno", "NONE"},
            //    {"Igual a", "IGUAL"},
            //    {"Mayor que", "MAYOR"},
            //    {"Menor que", "MENOR"},
            //    {"Mayor o igual que", "MAYIG"},
            //    {"Menor o igual que", "MENIG"},
            //    {"Diferente a", "DIFER"}
            //};

           

            //se asignan los diccionarios 
            //wucGenericBusquedaTipoActivo.FilterOperator = area_filteroperator_tipoactivos;
            wucGenericBusquedaTipoActivo.FilterField = area_fields_tipoactivos;
            wucGenericBusquedaTipoActivo.TableFields = area_fields_tipoactivos;


            //se asigna el control y la accion a realizar cuando se selecciona el item de la tabla
            wucGenericBusquedaTipoActivo.CargaControl += wucBusqueda_CargaControl;
            wucGenericBusquedaTipoActivo.ItemSelected += wucGenericBusquedaTipoActivo_Seleccionado;
            // -----------------------------------------------------------------------
            //fin paso 3 wuc generic configurando el user control 
            #endregion configurando_wucGenericBusqueda_Busquedatipoactivos


            #region configurando_wucGenericBusqueda_Busquedatiporeferencia

            //inicio paso 3 wuc generic configurando el user control 
            //------------------------------------------------------------------------------------

            //el nombre del sp que ejecuta el handler
            wucGenericBusquedaTipoReferencia.action = "09006";
            wucGenericBusquedaTipoReferencia.Title = "Buscar tipos de referencia";
            //textfield1data y textfield2data son los textfield principales(los de al lado de la lupa)
            wucGenericBusquedaTipoReferencia.TextField1Data = "ctiporefer";//TextField1Data es el textfield donde se pone el valor del codigo
            wucGenericBusquedaTipoReferencia.TextField2Data = "cdescrefer";//TextField2Data es el textfield donde se pone la descripccion

            //diccionario donde se pone los campos de la tabla
            //y del campo a filtrar
            //(si se quiere filtar por otro campos se debe crear otros diccionario similar)
            var area_fields_tiporeferencia = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"ctiporefer", "Tipo de referencia"},
                {"cdescrefer", "Descripción"},
                {"nporcdepre","hidden"},
                {"nperiovida","hidden"}
            };

            // filteroperator es el diccionario donde se pone el operador a filtrar
            var area_filteroperator_tiporeferencia = new Dictionary<string, string>
            {
                //llave, valor
                //donde la llave es el operador a filtrar
                //donde el valor es la accion que se va a realizar en el sp
                {"Contiene", "COMO"},
                {"Ninguno", "NONE"},
                {"Igual a", "IGUAL"},
                {"Mayor que", "MAYOR"},
                {"Menor que", "MENOR"},
                {"Mayor o igual que", "MAYIG"},
                {"Menor o igual que", "MENIG"},
                {"Diferente a", "DIFER"}
            };



            //se asignan los diccionarios 
            //wucGenericBusquedaTipoReferencia.FilterOperator = area_filteroperator_tiporeferencia;
            wucGenericBusquedaTipoReferencia.FilterField = area_fields_tiporeferencia;
            wucGenericBusquedaTipoReferencia.TableFields = area_fields_tiporeferencia;


            //se asigna el control y la accion a realizar cuando se selecciona el item de la tabla
            wucGenericBusquedaTipoReferencia.CargaControl += wucBusqueda_CargaControl;
            wucGenericBusquedaTipoReferencia.ItemSelected += wucGenericBusquedaTipoReferencia_Seleccionado;
            // -----------------------------------------------------------------------
            //fin paso 3 wuc generic configurando el user control 
            #endregion configurando_wucGenericBusqueda_Busquedatiporeferencia




            #region configurando_wucGenericBusquedaProveedor

            //inicio paso 3 wuc generic configurando el user control 
            //------------------------------------------------------------------------------------

            //el nombre del sp que ejecuta el handler
            wucGenericBusquedaProveedor.action = "06012";
            wucGenericBusquedaProveedor.Title = "Buscar proveedores";
            //textfield1data y textfield2data son los textfield principales(los de al lado de la lupa)
            wucGenericBusquedaProveedor.TextField1Data = "ccodprovee";//TextField1Data es el textfield donde se pone el valor del codigo
            wucGenericBusquedaProveedor.TextField2Data = "ccompania";//TextField2Data es el textfield donde se pone la descripccion

            //diccionario donde se pone los campos de la tabla
            //y del campo a filtrar
            //(si se quiere filtar por otro campos se debe crear otros diccionario similar)
            var area_fields_proveedor = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"ccodprovee", "Cédula de Proveedor"},
                {"ccompania", "Nombre"},
                {"ccuentacon", "hidden"},
                {"ctipasient", "hidden"},
                {"nsobregiro", "hidden"}
            };

            // filteroperator es el diccionario donde se pone el operador a filtrar
            var area_filteroperator_proveedor = new Dictionary<string, string>
            {
                //llave, valor
                //donde la llave es el operador a filtrar
                //donde el valor es la accion que se va a realizar en el sp
                {"Contiene", "COMO"},
                {"Ninguno", "NONE"},
                {"Igual a", "IGUAL"},
                {"Mayor que", "MAYOR"},
                {"Menor que", "MENOR"},
                {"Mayor o igual que", "MAYIG"},
                {"Menor o igual que", "MENIG"},
                {"Diferente a", "DIFER"}
            };



            //se asignan los diccionarios 
           // wucGenericBusquedaTipoReferencia.FilterOperator = area_filteroperator_proveedor;
            wucGenericBusquedaProveedor.FilterField = area_fields_proveedor;
            wucGenericBusquedaProveedor.TableFields = area_fields_proveedor;


            //se asigna el control y la accion a realizar cuando se selecciona el item de la tabla
            wucGenericBusquedaProveedor.CargaControl += wucBusqueda_CargaControl;
            wucGenericBusquedaProveedor.ItemSelected += wucGenericBusquedaProveedor_Seleccionado;
            // -----------------------------------------------------------------------
            //fin paso 3 wuc generic configurando el user control 
            #endregion configurando_wucGenericBusquedaProveedor

            #region configurando_wucGenericBusquedaUbicacion

            //inicio paso 3 wuc generic configurando el user control 
            //------------------------------------------------------------------------------------

            //el nombre del sp que ejecuta el handler
            wucGenericBusquedaUbicacion.action = "06014";
            wucGenericBusquedaUbicacion.Title = "Buscar ubicaciones";
            //textfield1data y textfield2data son los textfield principales(los de al lado de la lupa)
            wucGenericBusquedaUbicacion.TextField1Data = "cidubicaci";//TextField1Data es el textfield donde se pone el valor del codigo
            wucGenericBusquedaUbicacion.TextField2Data = "cdeubicaci";//TextField2Data es el textfield donde se pone la descripccion

            //diccionario donde se pone los campos de la tabla
            //y del campo a filtrar
            //(si se quiere filtar por otro campos se debe crear otros diccionario similar)
            var area_fields_ubicacion = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"cidubicaci", "Código"},
                {"cdeubicaci", "Descripción"}
            };

            // filteroperator es el diccionario donde se pone el operador a filtrar
            var area_filteroperator_ubicacion = new Dictionary<string, string>
            {
                //llave, valor
                //donde la llave es el operador a filtrar
                //donde el valor es la accion que se va a realizar en el sp
                {"Contiene", "COMO"},
                {"Ninguno", "NONE"},
                {"Igual a", "IGUAL"},
                {"Mayor que", "MAYOR"},
                {"Menor que", "MENOR"},
                {"Mayor o igual que", "MAYIG"},
                {"Menor o igual que", "MENIG"},
                {"Diferente a", "DIFER"}
            };



            //se asignan los diccionarios 
            // wucGenericBusquedaTipoReferencia.FilterOperator = area_filteroperator_ubicacion;
            wucGenericBusquedaUbicacion.FilterField = area_fields_ubicacion;
            wucGenericBusquedaUbicacion.TableFields = area_fields_ubicacion;


            //se asigna el control y la accion a realizar cuando se selecciona el item de la tabla
            wucGenericBusquedaUbicacion.CargaControl += wucBusqueda_CargaControl;
            wucGenericBusquedaUbicacion.ItemSelected += wucGenericBusquedaUbicacion_Seleccionado;
            // -----------------------------------------------------------------------
            //fin paso 3 wuc generic configurando el user control 
            #endregion configurando_wucGenericBusquedaUbicacion

            #region configurando_wucGenericBusquedaPolizas

            //inicio paso 3 wuc generic configurando el user control 
            //------------------------------------------------------------------------------------

            //el nombre del sp que ejecuta el handler
            wucGenericBusquedaPolizas.action = "06015";
            wucGenericBusquedaPolizas.Title = "Pólizas del activo";
            //textfield1data y textfield2data son los textfield principales(los de al lado de la lupa)
            wucGenericBusquedaPolizas.TextField1Data = "ctippoliza";//TextField1Data es el textfield donde se pone el valor del codigo
            wucGenericBusquedaPolizas.TextField2Data = "cnumpoliza";//TextField2Data es el textfield donde se pone la descripccion

            //diccionario donde se pone los campos de la tabla
            //y del campo a filtrar
            //(si se quiere filtar por otro campos se debe crear otros diccionario similar)
            var area_fields_poliza = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"ctippoliza", "Tipo"},
                {"cnumpoliza", "#Póliza"},
                {"cdespoliza", "Descripción"},
                {"dfechavige", "Inicio"},
                {"dfechavenc", "Venc."}
            };

            var extrafields = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"cnumactivo", txtcnumactivo.Text}
               
            };


            //se asignan los diccionarios 
            // wucGenericBusquedaTipoReferencia.FilterOperator = area_filteroperator_proveedor;
            wucGenericBusquedaPolizas.ExtraFilterField = extrafields;

            
            wucGenericBusquedaPolizas.FilterField = area_fields_poliza;
            wucGenericBusquedaPolizas.TableFields = area_fields_poliza;
            wucGenericBusquedaPolizas.muestra_solo_tabla = true;
            wucGenericBusquedaPolizas.show_only_big_button = true;


            //se asigna el control y la accion a realizar cuando se selecciona el item de la tabla
            wucGenericBusquedaPolizas.CargaControl += wucBusqueda_CargaControl;
           // wucGenericBusquedaPolizas.ItemSelected += wucGenericBusquedaProveedor_Seleccionado;
            // -----------------------------------------------------------------------
            //fin paso 3 wuc generic configurando el user control 
            #endregion configurando_wucGenericBusquedaPolizas

            

            llenarRepeaterEncargados();
            llenarRepeaterRevMejoras();
            llenarRepeaterDesvalorizaciones();

            var url = GetBaseUrl();
            hfLocation.Value = url + "Uploads/Activos/";

            //If first time page is submitted and we have file in FileUpload control but not in session
            // Store the values to SEssion Object
            if (Session["btnCmdBusFotoE"] == null && btnCmdBusFoto.HasFile)
            {


                Session["btnCmdBusFotoE"] = btnCmdBusFoto;
                img.Width = 130;
                img.Height = 130;
                img.Attributes.Add("src", hfRutaFoto.Value);

            }
            // Next time submit and Session has values but FileUpload is Blank
            // Return the values from session to FileUpload
            else if (Session["btnCmdBusFotoE"] != null && (!btnCmdBusFoto.HasFile))
            {
                btnCmdBusFoto = (FileUpload)Session["btnCmdBusFotoE"];
                img.Width = 130;
                img.Height = 130;
                img.Attributes.Add("src", hfRutaFoto.Value);

            }
            // Now there could be another sictution when Session has File but user want to change the file
            // In this case we have to change the file in session object
            else if (btnCmdBusFoto.HasFile)
            {
                Session["btnCmdBusFotoE"] = btnCmdBusFoto;
                img.Width = 130;
                img.Height = 130;
                img.Attributes.Add("src", hfRutaFoto.Value);

            }

        }



        #region Método que instancia el web user control de mantenimiento
        // Instancia del web user control de mostrar mantenimiento.
        private void Met_MostrarMantenimiento()
        {

            hlNuevo.Visible = true;
            hlEditar.Visible = true;
            hlBorrar.Visible = true;
            hlAceptar.Visible = false;
            hlCancelar.Visible = false;
           
            panelBusqueda.CssClass = "panelbuscar";

            if (actualizarRegistro || insertRegistro)
            {
                hlNuevo.Visible = false;
                hlEditar.Visible = false;
                hlBorrar.Visible = false;
                hlAceptar.Visible = true;
                hlCancelar.Visible = true;
                panelBusqueda.CssClass = "hidden";
               

            }
        }
        #endregion

        #region Método que toma la acción tomada del mantenimiento del formulario

        // metodo que toma la acción tomada del mantenimiento del formulario

        void EventoSeleccionado(int eventoSeleccionado)
        {
           
            switch (eventoSeleccionado)
            {
                //Registro nuevo
                case 1:
                    
                    //wucGenericBusqueda_BusquedaActivosEmp.Visible=false; 
                    
                    insertRegistro = true;
                    actualizarRegistro = false;
                    borrarRegistro = false;
                    Met_MostrarMantenimiento();
                    this.Met_ActivarCampos(true);
                    this.LimpiaCampos(true, eventoSeleccionado);
                    txtcnumactivo.Enabled = true;
                    radcactdeprec.Enabled = true;
                    txtdfechaacti.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    txtcnumactivo.Focus();
                    break;

                //Editar registro
                case 2:
                    if (string.IsNullOrEmpty(txtcnumactivo.Text))
                    {
                        MensajeCliente mc = null;
                        mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", "Debe seleccionar un activo.");
                        wucMensajePopUp.Met_SetearMensajeCliente(mc);
                        break;
                    }
                    else
                    {
                        insertRegistro = false;
                        actualizarRegistro = true;
                        borrarRegistro = false;                     
                        Met_MostrarMantenimiento();
                        this.Met_ActivarCampos(true);
                        txtcnumactivo.Enabled = false;
                        radcactdeprec.Enabled = false;
                        
                        
                        break;
                    }
                //Borrar registro
                case 3:
                    borrarRegistro = true;
                    if (!Met_EliminarActivos())
                    {
                        this.Met_ActivarCampos(false);
                    }
                    else
                    {

                        insertRegistro = false;
                        actualizarRegistro = false;
                        borrarRegistro = false;
                        LimpiaCampos(true,eventoSeleccionado);
                        ActivoSelect = new BEactactivos();
                        this.Met_ActivarCampos(false);
                    }

                    break;
                //Aceptar
                case 4:
                    //Si voy a insertar
                    if (insertRegistro == true)
                    {
                        if (!Met_InsertarActivos())
                        {
                            this.Met_ActivarCampos(true);
                        }
                        else
                        {
                            insertRegistro = false;
                            actualizarRegistro = false;
                            borrarRegistro = false;
                            Met_MostrarMantenimiento();
                            this.Met_ActivarCampos(false);                           
                            if (!string.IsNullOrEmpty(ActivoSelect.cnumactivo))
                            {
                                Met_CargarActivos(txtcnumactivo.Text);

                            }
                            //div_BusqEspecialidad.Visible = true; 
                           
                        }
                                            
                        break;
                    }

                    //accion de actualizar un registro
                    if (actualizarRegistro == true)
                    {
                        //Actualizo el registro
                        if (Met_ActualizaActivos())
                        {
                            insertRegistro = false;
                            actualizarRegistro = false;
                            borrarRegistro = false;
                            Met_MostrarMantenimiento();

                            if (!string.IsNullOrEmpty(txtcnumactivo.Text))
                                Met_CargarActivos(txtcnumactivo.Text);
                            this.Met_ActivarCampos(false);                            
                        }
                        break;
                    }

                    break;
                //cancelar
                case 5:
                    insertRegistro = false;
                    actualizarRegistro = false;
                    borrarRegistro = false;
                   if (!string.IsNullOrEmpty(ActivoSelect.cnumactivo))
                    {
                        this.Met_CargarActivos(ActivoSelect.cnumactivo);
                       
                    }
                    else
                    {
                        LimpiaCampos(true, eventoSeleccionado);
                    }
                   
                    Met_MostrarMantenimiento();
                    this.Met_ActivarCampos(false);
                    //div_BusqEspecialidad.Visible = true;
                   
                    break;

            }// fin switch

        }

        #endregion

       

        #region Método para cargar control de las busqueda de estados de los empleados

        protected void wucBusqueda_CargaControl(object sender, CodeasWebTools.Web.GenericEventArgs<CodeasWebEntities.Entities.Admin.BEGeAdminUsu> e)
        {
            BEGeAdminUsu usuarioLogueado = new BEGeAdminUsu();
            usuarioLogueado.CCodigoUsu = base.sesionActual.CCodUsuari;
            usuarioLogueado.CCriterBus = base.sesionActual.CriterBusqueda;
            usuarioLogueado.NNumRegCon = base.sesionActual.NumRegistrosBusqueda;
            e.Argument = usuarioLogueado;
        }
        
        /// <summary>
        /// Este metodo asigna el máximo de caracteres
        /// </summary>
        private void AsignarMaxLength()
        {
            txtcnumactivo.MaxLength = 6;
            txtcdescactiv.MaxLength = 256;
            txtcnomactiv.MaxLength = 100;
            txtcmarcaacti.MaxLength = 20;
            txtcserieacti.MaxLength = 13;
            txtcmodeloact.MaxLength = 20;
            txtcnumckcomp.MaxLength = 13;
            txtcnumgarant.MaxLength = 10;
            txtcdescrigar.MaxLength = 250;



        }

        private void llenarRepeaterEncargados()
        {
            if (!String.IsNullOrEmpty(ActivoSelect.cnumactivo))
            {
                if (!insertRegistro)
                {
                    listEmpleados =
                        new Serrhempleado(this.Met_CadenaConexion()).Met_ObtenerEncargados(ActivoSelect.cnumactivo);

                    RepeaterEncargados.DataSource = listEmpleados;
                    RepeaterEncargados.DataBind();
                }
                else
                {
                    listEmpleados = new List<BErhempleado>();
                    RepeaterEncargados.DataSource = listEmpleados;
                    RepeaterEncargados.DataBind();
                }

            }
            else
            {
                listEmpleados = new List<BErhempleado>();
                RepeaterEncargados.DataSource = listEmpleados;
                RepeaterEncargados.DataBind();
            }
           
        }


        private void llenarRepeaterRevMejoras()
        {
            if (!String.IsNullOrEmpty(ActivoSelect.cnumactivo))
            {
                if (!insertRegistro)
                {
                    listRevMejoras =
                        new Seractrevmejo(this.Met_CadenaConexion()).Met_ObtenerRevMejoras(ActivoSelect.cnumactivo);
                    RepeaterRevMejoras.DataSource = listRevMejoras;
                    RepeaterRevMejoras.DataBind();
                }
                else
                {
                    listRevMejoras = new List<BEactrevmejo>();
                    RepeaterRevMejoras.DataSource = listRevMejoras;
                    RepeaterRevMejoras.DataBind();
                }
            }
            else
            {
                listRevMejoras = new List<BEactrevmejo>();
                RepeaterRevMejoras.DataSource = listRevMejoras;
                RepeaterRevMejoras.DataBind();
            }
        }

        private void llenarRepeaterDesvalorizaciones()
        {
            if (!String.IsNullOrEmpty(ActivoSelect.cnumactivo))
            {
                if (!insertRegistro)
                {
                    listDesvalorizaciones =
                        new Seracttransac(this.Met_CadenaConexion()).Met_ObtenerDesvalorizaciones(
                            ActivoSelect.cnumactivo);
                    RepeaterDesvalorizacion.DataSource = listDesvalorizaciones;
                    RepeaterDesvalorizacion.DataBind();
                }
                else
                {
                    listDesvalorizaciones = new List<BEacttransac>();
                    RepeaterDesvalorizacion.DataSource = listDesvalorizaciones;
                    RepeaterDesvalorizacion.DataBind();
                }

            }
            else
            {
                listDesvalorizaciones = new List<BEacttransac>();
                RepeaterDesvalorizacion.DataSource = listDesvalorizaciones;
                RepeaterDesvalorizacion.DataBind();
            }
        }


      
        #endregion

        #region Método para obtener el activo seleccionado y además carga la info de la mismo
        
        protected void wucGenericBusqueda_BusquedaActivosEmp_Seleccionado(object sender, GenericEventArgs<Dictionary<string, string>> e)
        {
           
            if (!string.IsNullOrEmpty(e.Argument["cnumactivo"]))
            {
               
                ActivoSelect.cnumactivo=txtcnumactivo.Text = e.Argument["cnumactivo"];
                ActivoSelect.cnomactiv = txtcnomactiv.Text = e.Argument["cnomactiv"];
                var extrafields = new Dictionary<string, string>   {
                //llave, valor 
                //donde la llave es el identificador del campo 
                //donde el valor es el nombre que se ve visible en la columna de la tabla
                // si el valor es hidden la columna no se mostrara
                {"cnumactivo", ActivoSelect.cnumactivo}
               
            };


                //se asignan los diccionarios 
                // wucGenericBusquedaTipoReferencia.FilterOperator = area_filteroperator_proveedor;
                wucGenericBusquedaPolizas.ExtraFilterField = extrafields;
                Met_CargarActivos(e.Argument["cnumactivo"]);

            }
           
        }

        
        #endregion

        protected void wucGenericBusquedaTipoActivo_Seleccionado(object sender, GenericEventArgs<Dictionary<string, string>> e)
        {

            if (!string.IsNullOrEmpty(e.Argument["ctipoactiv"]))
            {
                wucGenericBusquedaTipoActivo.GetTextField2().Text = e.Argument["cdestipact"];
                
            }

        }


        protected void wucGenericBusquedaProveedor_Seleccionado(object sender, GenericEventArgs<Dictionary<string, string>> e)
        {

            if (!string.IsNullOrEmpty(e.Argument["ccodprovee"]))
            {
                wucGenericBusquedaProveedor.GetTextField2().Text = e.Argument["ccompania"];

            }

        }

        protected void wucGenericBusquedaUbicacion_Seleccionado(object sender, GenericEventArgs<Dictionary<string, string>> e)
        {
            if (!string.IsNullOrEmpty(e.Argument["cidubicaci"]))
            {
                wucGenericBusquedaUbicacion.GetTextField2().Text = e.Argument["cdeubicaci"];

            }

        }


        protected void wucGenericBusquedaTipoReferencia_Seleccionado(object sender, GenericEventArgs<Dictionary<string, string>> e)
        {
            
            if (!string.IsNullOrEmpty(e.Argument["ctiporefer"]) && !string.IsNullOrEmpty(wucGenericBusquedaTipoReferencia.GetTextField1().Text))
            {
                wucGenericBusquedaTipoReferencia.GetTextField2().Text = e.Argument["cdescrefer"];
               
            }

        }

        private void Met_CargarCentrosCosto()
        {
            Seractactivos seractactivos = new Seractactivos(this.Met_CadenaConexion());
            List<BECoCentcDep> centros = seractactivos.Met_ObtenerCentrosCosto();

            ddlccodcentro.Items.Clear();
            if (centros == null) return;
            ddlccodcentro.Items.Add(new ListItem("", ""));
            foreach (var a in centros)
            {
                ListItem l = new ListItem(a.CDesCentro, a.CCodCentro);
                ddlccodcentro.Items.Add(l);
            }


        }

    

        private string FormatearDateTimeString(DateTime? valor)
        {

            return HelperMethods.Met_Formatearfechacorta(valor);

        }
        public string FormatearDecimalString(decimal valor)
        {

            return HelperMethods.Met_FormatearDecimal(valor, cultureInfoParaDecimales);

        }

        public decimal ConvertirStringDecimal(string valor)
        {

            return HelperMethods.Met_ConvertirADecimal(valor, cultureInfoParaDecimales);

        }

        #region Método para cargar la información de un activo
        private void Met_CargarActivos(string codigo)
        {
            MensajeCliente mc = null;
           try
            {
                
               if(ActivoSelect!=null && !string.IsNullOrEmpty(ActivoSelect.cnumactivo))
               {

                   Seractactivos seractactivos = new Seractactivos(Met_CadenaConexion());
                   ActivoSelect = new BEactactivos();
                   ActivoSelect = seractactivos.Met_ObtenerActivoXID(codigo);
                   


                   txtcnumactivo.Text = ActivoSelect.cnumactivo;
                   wucGenericBusquedaTipoActivo.GetTextField1().Text = ActivoSelect.ctipoactiv;
                   if (!string.IsNullOrEmpty(ActivoSelect.ctipoactiv))
                     wucGenericBusquedaTipoActivo.Met_ReloadSelectedItem(Met_CadenaConexion());
                   //txtctipoactiv.Text = ActivoSelect.ctipoactiv;
                   txtcdescactiv.Text = ActivoSelect.cdescactiv;
                   txtcnomactiv.Text = ActivoSelect.cnomactiv;
                   txtcmarcaacti.Text = ActivoSelect.cmarcaacti;
                   txtcserieacti.Text = ActivoSelect.cserieacti;
                   txtcmodeloact.Text = ActivoSelect.cmodeloact;
                   txtdfechaacti.Text = FormatearDateTimeString(ActivoSelect.dfechaacti);
                   txtnvalorcom.Text = FormatearDecimalString(ActivoSelect.nvalorcom);
                   txtnporcdepre.Text = FormatearDecimalString(ActivoSelect.nporcdepre);
                   txtndepreacum.Text = FormatearDecimalString(ActivoSelect.ndepreacum);
                   wucGenericBusquedaProveedor.GetTextField1().Text = ActivoSelect.ccodproved;
                   if (!string.IsNullOrEmpty(ActivoSelect.ccodproved))
                   wucGenericBusquedaProveedor.Met_ReloadSelectedItem(Met_CadenaConexion());
                   //txtcdescactiv.Text = ActivoSelect.ccodproved;
                   txtcnumckcomp.Text = ActivoSelect.cnumckcomp;
                   //txtcdescactiv.Text = ActivoSelect.ccoddepart;
                   radcactdeprec.SelectedValue = ActivoSelect.cactdeprec;
                   txtnmontomejo.Text = FormatearDecimalString(ActivoSelect.nmontomejo);
                   txtnmontoreva.Text = FormatearDecimalString(ActivoSelect.nmontoreva);
                   txtnmontdpmej.Text = FormatearDecimalString(ActivoSelect.nmontdpmej);
                   txtnmontdprev.Text = FormatearDecimalString(ActivoSelect.nmontdprev);
                   txtnmontliqui.Text = FormatearDecimalString(ActivoSelect.nmontliqui);
                   txtnvalorresi.Text = FormatearDecimalString(ActivoSelect.nvalorresi);
                   txtnvalorrepo.Text = FormatearDecimalString(ActivoSelect.nvalorrepo);
                   
                   if (ActivoSelect.cestactivo=="V")
                   {
                       lblcestactivo.Text = "VIGENTE";
                   }
                   else if (ActivoSelect.cestactivo=="D")
                   {
                        lblcestactivo.Text = "DEPRECIADO";
                   }
                   else if (ActivoSelect.cestactivo == "L")
                   {
                       lblcestactivo.Text = "LIQUIDADO";
                   }
                   else
                   {
                       lblcestactivo.Text = "PARCIAL";
                   }

                   wucGenericBusquedaTipoReferencia.GetTextField1().Text = ActivoSelect.ccodrefere;
                   if (!string.IsNullOrEmpty(ActivoSelect.ccodrefere))
                   wucGenericBusquedaTipoReferencia.Met_ReloadSelectedItem(Met_CadenaConexion());
                   //txtccodrefere.Text = ActivoSelect.ccodrefere;
                   radtipodepre.SelectedValue = ActivoSelect.ctipodepre;
                   txtnperiovida.Text = FormatearDecimalString(ActivoSelect.nperiovida);
                   txtnmontdesva.Text = FormatearDecimalString(ActivoSelect.nmontdesva);
                   //txtcdescactiv.Text = ActivoSelect.cfotoactiv;
                   img.Width = 130;
                   img.Height = 130;
                   img.Attributes.Add("src", hfLocation.Value + ActivoSelect.cfotoactiv);
                   

                   if (!string.IsNullOrEmpty(ActivoSelect.cfotoactiv) && !ActivoSelect.cfotoactiv.Equals("c:\\visual\\"))
                   {
                       img.Width = 130;
                       img.Height = 130;
                       img.Attributes.Add("src", hfLocation.Value + ActivoSelect.cfotoactiv);
                       hfRutaFoto.Value = hfLocation.Value + ActivoSelect.cfotoactiv;
                   }
                   else
                   {
                           img.Width = 130;
                           img.Height = 130;
                           img.Attributes.Add("src", hfLocation.Value + "SinFoto.png");
                           hfRutaFoto.Value = hfLocation.Value + "SinFoto.png";
                           Session["btnCmdBusFotoE"] = null;
                           
                      
                   }
                   wucGenericBusquedaUbicacion.GetTextField1().Text = ActivoSelect.cidubicaci;
                   if (!string.IsNullOrEmpty(ActivoSelect.cidubicaci))
                   wucGenericBusquedaUbicacion.Met_ReloadSelectedItem(Met_CadenaConexion());
                   //txtcidubicaci.Text = ActivoSelect.cidubicaci;
                   txtcnumgarant.Text = ActivoSelect.cnumgarant;
                   txtcdescrigar.Text = ActivoSelect.cdescrigar;
                   //txtcdescactiv.Text = ActivoSelect.nvalorcom1;
                   txtnvalorcomi.Text = FormatearDecimalString(ActivoSelect.nvalorcomi);
                   ddlccodcentro.SelectedValue = ActivoSelect.CCODCENTRO;
                   //txtcdescactiv.Text = ActivoSelect.cnumeorden;


                   txtpervida.Text = (ActivoSelect.nporcdepre == 0) ? FormatearDecimalString(ActivoSelect.nporcdepre) : FormatearDecimalString(Math.Round(100 / ActivoSelect.nporcdepre, 0));


                   txtdepmens.Text =
                       FormatearDecimalString(((ActivoSelect.nvalorcom - ActivoSelect.nvalorresi)*
                                               ActivoSelect.nporcdepre/100)/12);
                   txtmontact.Text = FormatearDecimalString(ConvertirStringDecimal(txtnvalorcom.Text) - ConvertirStringDecimal(txtndepreacum.Text));

                   txtmonmejo.Text =
                       FormatearDecimalString(ConvertirStringDecimal(txtnmontomejo.Text) -
                                              ConvertirStringDecimal(txtnmontdpmej.Text));

                   txtmonreva.Text = FormatearDecimalString(ConvertirStringDecimal(txtnmontoreva.Text) -
                                              ConvertirStringDecimal(txtnmontdprev.Text));

                   txtmonactu.Text = FormatearDecimalString(ConvertirStringDecimal(txtmontact.Text) + ConvertirStringDecimal(txtmonmejo.Text) + ConvertirStringDecimal(txtmonreva.Text) - ConvertirStringDecimal(txtnmontliqui.Text) -
                                              ConvertirStringDecimal(txtnmontdesva.Text));

                   llenarRepeaterEncargados();
                   llenarRepeaterRevMejoras();
                   llenarRepeaterDesvalorizaciones();
                   

               }
               
            }
            catch
            {
                mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", "Debe seleccionar nuevamente el activo.");
 
            }
        }
        #endregion

        //protected string UploadFolderPath = "~/Uploads/Activos/";
        private string GuardarArchivo(BEactactivos empleado)
        {

            if (btnCmdBusFoto.HasFile && !string.IsNullOrEmpty(img.Attributes["src"]))
            {
                string UploadFolderPath = Server.MapPath("~/Uploads/Activos/");
                string extension = "." + btnCmdBusFoto.PostedFile.FileName.Split('.')[1];
                btnCmdBusFoto.SaveAs(UploadFolderPath + empleado.cnumactivo + extension);
                return empleado.cnumactivo + extension;
            }
            else
                return RutaFoto;

        }

        #region Método para activar los campos del formulario
        public void Met_ActivarCampos(bool activarCampos)
        {
            if (ActivoSelect != null && !string.IsNullOrEmpty(ActivoSelect.cnumactivo))
            {
                ClientScript.RegisterStartupScript(GetType(), "Validador","disable_Boton_EditarBorrarMantenimiento(false);", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "Validador", "disable_Boton_EditarBorrarMantenimiento(true);", true);
           
            }

            ActivarDesactivarCampos(this.PanelDatos, activarCampos);
            ActivarDesactivarCampos(this.PanelDatos2, activarCampos);
            ActivarDesactivarCampos(this.PanelDatos3, activarCampos);

          
            wucGenericBusquedaTipoReferencia.Met_Enable(activarCampos);
            wucGenericBusquedaTipoActivo.Met_Enable(activarCampos);
            wucGenericBusquedaProveedor.Met_Enable(activarCampos);
            wucGenericBusquedaUbicacion.Met_Enable(activarCampos);
            wucGenericBusquedaPolizas.Met_Enable(activarCampos);

            if (insertRegistro)
            {
                ActivarDesactivarCampos(PanelDatos2,false);


                txtnvalorresi.Attributes.Remove("readonly");
                txtnvalorresi.Attributes.Remove("class");
                txtnvalorresi.CssClass = "generic-decimal-textbox";

                txtnvalorrepo.Attributes.Remove("readonly");
                txtnvalorrepo.Attributes.Remove("class");
                txtnvalorrepo.CssClass = "generic-decimal-textbox";

                txtndepreacum.Attributes.Remove("readonly");
                txtndepreacum.Attributes.Remove("class");
                txtndepreacum.CssClass = "generic-decimal-textbox";

                txtnvalorcom.Attributes.Remove("readonly");
                txtnvalorcom.Attributes.Remove("class");
                txtnvalorcom.CssClass = "generic-decimal-textbox";

                txtnmontliqui.Attributes.Remove("readonly");
                txtnmontliqui.Attributes.Remove("class");
                txtnmontliqui.CssClass = "generic-decimal-textbox";

                txtnporcdepre.Attributes.Remove("readonly");
                txtnporcdepre.Attributes.Remove("class");
                txtnporcdepre.CssClass = "generic-decimal-textbox";

                ddlccodcentro.Enabled = true;
                txtpervida.Attributes.Add("readonly", "readonly");
                txtpervida.CssClass = "disabled-textbox";

                txtnporcdepre.Attributes.Add("readonly", "readonly");
                txtnporcdepre.CssClass = "disabled-textbox";
                


            }
            else if (actualizarRegistro)
            {
                ActivarDesactivarCampos(PanelDatos2,false);
                ddlccodcentro.Enabled = true;


                txtnvalorresi.Attributes.Remove("readonly");
                txtnvalorresi.Attributes.Remove("class");
                txtnvalorresi.CssClass = "generic-decimal-textbox";

                txtnvalorrepo.Attributes.Remove("readonly");
                txtnvalorrepo.Attributes.Remove("class");
                txtnvalorrepo.CssClass = "generic-decimal-textbox";

                
                txtnvalorcom.Attributes.Remove("readonly");
                txtnvalorcom.Attributes.Remove("class");
                txtnvalorcom.CssClass = "generic-decimal-textbox";

                txtnporcdepre.Attributes.Remove("readonly");
                txtnporcdepre.Attributes.Remove("class");
                txtnporcdepre.CssClass = "generic-decimal-textbox";

                txtndepreacum.Attributes.Remove("readonly");
                txtndepreacum.Attributes.Remove("class");
                txtndepreacum.CssClass = "generic-decimal-textbox";

                txtpervida.Attributes.Add("readonly", "readonly");
                txtpervida.CssClass = "disabled-textbox";

                txtnporcdepre.Attributes.Add("readonly", "readonly");
                txtnporcdepre.CssClass = "disabled-textbox";

              
               
            }
          
        }
        #endregion

       

        #region Método para Obtener los Parametros de Un formulario
        public BEactactivos Met_ObtnerParametrosDeFormulario(BEactactivos estados, string conexion)
        {
            estados = new BEactactivos();           
            try
            {
                estados.cnumactivo = txtcnumactivo.Text;
                estados.ctipoactiv = wucGenericBusquedaTipoActivo.GetTextField1().Text;
                estados.cdescactiv = txtcdescactiv.Text;
                estados.cnomactiv = txtcnomactiv.Text;
                estados.cmarcaacti = txtcmarcaacti.Text;
                estados.cserieacti = txtcserieacti.Text;
                estados.cmodeloact = txtcmodeloact.Text;
                estados.dfechaacti = Convert.ToDateTime(txtdfechaacti.Text);
                estados.nvalorcom = ConvertirStringDecimal(txtnvalorcom.Text);
                estados.nporcdepre = (radtipodepre.SelectedValue == "L")
                    ? ConvertirStringDecimal(txtnporcdepre.Text)
                    : ConvertirStringDecimal("0.00");

                estados.ndepreacum = ConvertirStringDecimal(txtndepreacum.Text);
                estados.cnumckcomp = txtcnumckcomp.Text;
                estados.cidubicaci = wucGenericBusquedaUbicacion.GetTextField1().Text;
                estados.ccodproved = wucGenericBusquedaProveedor.GetTextField1().Text;
                estados.cactdeprec = radcactdeprec.SelectedValue;
                estados.nvalorresi = ConvertirStringDecimal(txtnvalorresi.Text);
                estados.nvalorrepo = ConvertirStringDecimal(txtnvalorrepo.Text);

                if ((radtipodepre.SelectedValue == "S" && (ConvertirStringDecimal(txtnvalorcom.Text) - ConvertirStringDecimal(txtndepreacum.Text) - ConvertirStringDecimal(txtnmontdesva.Text)) > ConvertirStringDecimal(txtnvalorresi.Text) && ConvertirStringDecimal(txtnmontliqui.Text) == 0) || (radtipodepre.SelectedValue == "S" && ConvertirStringDecimal(txtnmontliqui.Text) == 0))
                {
                    estados.cestactivo = "V";
                }
                else if ((radtipodepre.SelectedValue == "S" && (ConvertirStringDecimal(txtnvalorcom.Text) - ConvertirStringDecimal(txtndepreacum.Text) - ConvertirStringDecimal(txtnmontdesva.Text)) == ConvertirStringDecimal(txtnvalorresi.Text) && ConvertirStringDecimal(txtnmontliqui.Text) == 0))
                {
                    estados.cestactivo = "D";
                }
                else if (ConvertirStringDecimal(txtnmontliqui.Text) > 0)
                {
                    estados.cestactivo = "L";
                }
                else
                {
                    estados.cestactivo = "";
                }
                estados.ccodrefere = wucGenericBusquedaTipoReferencia.GetTextField1().Text;
                estados.ctipodepre = radtipodepre.SelectedValue;
                estados.nmontliqui = ConvertirStringDecimal(txtnmontliqui.Text);
                estados.nperiovida = (radtipodepre.SelectedValue == "S")
                   ? ConvertirStringDecimal(txtnperiovida.Text)
                   : ConvertirStringDecimal("0.00");


                estados.cnumgarant = txtcnumgarant.Text;
                estados.cdescrigar = txtcdescrigar.Text;
                estados.CCODCENTRO = ddlccodcentro.SelectedValue;

                estados.nmontomejo = ConvertirStringDecimal(txtnmontomejo.Text);
                estados.nmontoreva = ConvertirStringDecimal(txtnmontoreva.Text);
                estados.nmontdpmej = ConvertirStringDecimal(txtnmontdpmej.Text);
                estados.nmontdprev = ConvertirStringDecimal(txtnmontdprev.Text);
                estados.nmontdesva = ConvertirStringDecimal(txtnmontdesva.Text);
                estados.nvalorcomi = ConvertirStringDecimal(txtnvalorcomi.Text);

                estados.ccoddepart=String.Empty;
                estados.cnumeorden=String.Empty;
                





            }
            catch (GenericException ex)
            {
                var mensaje = new MensajeCliente(MensajeCliente.TipoMensaje.Error, "Operación cancelada", ex.Mensaje);
                wucMensajePopUp.Met_SetearMensajeCliente(mensaje);
            }
            return estados;
        }
        #endregion

        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
          
            if (appUrl != "/")
            {
                if (appUrl.StartsWith("/") && !appUrl.EndsWith("/"))
                {
                    appUrl = appUrl + "/";
                }
                else if (!appUrl.StartsWith("/") && appUrl.EndsWith("/"))
                {
                    appUrl = "/"+ appUrl;
                }
                else if (!appUrl.StartsWith("/") && !appUrl.EndsWith("/"))
                {
                    appUrl ="/"+ appUrl + "/";
                }
                //appUrl = "/" + appUrl + "/";
            }

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        #region Método para limpiar los campos del formulario
        public void LimpiaCampos(bool limpiarCampos,int eventoSeleccionado)
        {
            if (limpiarCampos)
            {
                foreach (var control in PanelDatos.Controls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).Text = "";
                        
                    }
                    if (control is CheckBox)
                    {
                        ((CheckBox)control).Checked = false;
                    }


                    if (control is CheckBoxList)
                    {
                        CheckBoxList chkList = ((CheckBoxList)control);

                        foreach (ListItem items in chkList.Items)
                        {
                            items.Selected = true;
                        }
                    }
                    if (control is DropDownList)
                    {
                        DropDownList chkList = ((DropDownList)control);

                        chkList.ClearSelection();
                      
                    }
                    
                    

                }

                var url = GetBaseUrl();
                hfLocation.Value = url + "Uploads/Activos/" + "SinFoto.png";
                img.Height = 130;
                img.Width = 130;
                img.Attributes.Add("src", hfLocation.Value);
                RutaFoto = string.Empty;

                hfRutaFoto.Value = hfLocation.Value + "SinFoto.png";

                Session["btnCmdBusFotoE"] = null;

                

                txtnmontomejo.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnmontoreva.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnmontdpmej.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnmontdprev.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnmontliqui.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnvalorresi.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnvalorrepo.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnvalorcomi.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));

                txtpervida.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));


                txtdepmens.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtmontact.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));

                txtmonmejo.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));

                txtmonreva.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));

                txtmonactu.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));

                txtnvalorcom.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnporcdepre.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtndepreacum.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnperiovida.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                txtnmontdesva.Text = FormatearDecimalString(Convert.ToDecimal("0.00"));
                lblcestactivo.Text = "VIGENTE";
                radcactdeprec.SelectedValue = "S";
                radtipodepre.SelectedValue = "S";
                wucGenericBusquedaTipoReferencia.GetTextField1().Text = "";
                wucGenericBusquedaTipoReferencia.GetTextField2().Text = "";
                wucGenericBusquedaProveedor.GetTextField1().Text = "";
                wucGenericBusquedaProveedor.GetTextField2().Text = "";
                wucGenericBusquedaTipoActivo.GetTextField1().Text = "";
                wucGenericBusquedaTipoActivo.GetTextField2().Text = "";
                wucGenericBusquedaUbicacion.GetTextField1().Text = "";
                wucGenericBusquedaUbicacion.GetTextField2().Text = "";

                

                llenarRepeaterEncargados();
                llenarRepeaterRevMejoras();
                llenarRepeaterDesvalorizaciones();
                


            }
        }


        public void ActivarDesactivarCampos(Panel panel, bool activa)
        {
          
                foreach (var control in panel.Controls)
                {
                    if (control is TextBox)
                    {
                        if (activa)
                        {
                            ((TextBox) control).Attributes.Remove("readonly");
                            ((TextBox)control).CssClass = "";

                        }
                        else
                        {
                            ((TextBox)control).Attributes.Add("readonly", "readonly");
                            ((TextBox)control).CssClass = "disabled-textbox";



                        }
                      

                    }
                    if (control is CheckBox)
                    {
                        ((CheckBox)control).Enabled = activa;
                    }


                    if (control is RadioButtonList)
                    {
                        RadioButtonList chkList = ((RadioButtonList)control);

                        foreach (ListItem items in chkList.Items)
                        {
                            items.Enabled = activa;
                        }
                    }

                    if (control is CheckBoxList)
                    {
                        CheckBoxList chkList = ((CheckBoxList)control);

                        foreach (ListItem items in chkList.Items)
                        {
                            items.Enabled = activa;
                        }
                    }
                    if (control is DropDownList)
                    {
                        DropDownList chkList = ((DropDownList)control);

                        chkList.Enabled = activa;

                    }
                    if (control is Button)
                    {
                        Button chkList = ((Button)control);

                        chkList.Enabled = activa;

                    }


               }

                if (activa)
                {
                    txtnvalorresi.Attributes.Remove("readonly");
                    txtnvalorresi.Attributes.Remove("class");
                    txtnvalorresi.CssClass = "generic-decimal-textbox";

                    txtnvalorrepo.Attributes.Remove("readonly");
                    txtnvalorrepo.Attributes.Remove("class");
                    txtnvalorrepo.CssClass = "generic-decimal-textbox";

                    txtndepreacum.Attributes.Remove("readonly");
                    txtndepreacum.Attributes.Remove("class");
                    txtndepreacum.CssClass = "generic-decimal-textbox";

                    txtnvalorcom.Attributes.Remove("readonly");
                    txtnvalorcom.Attributes.Remove("class");
                    txtnvalorcom.CssClass = "generic-decimal-textbox";

                    txtnvalorcomi.Attributes.Remove("readonly");
                    txtnvalorcomi.Attributes.Remove("class");
                    txtnvalorcomi.CssClass = "generic-decimal-textbox";

                    txtnmontliqui.Attributes.Remove("readonly");
                    txtnmontliqui.Attributes.Remove("class");
                    txtnmontliqui.CssClass = "generic-decimal-textbox";

                }
                else
                {
                    txtnvalorresi.Attributes.Add("readonly", "readonly");
                    txtnvalorresi.CssClass = "generic-decimal-textbox disabled-textbox";

                    txtnvalorrepo.Attributes.Add("readonly", "readonly");
                    txtnvalorrepo.CssClass = "generic-decimal-textbox disabled-textbox";

                    txtndepreacum.Attributes.Add("readonly", "readonly");
                    txtndepreacum.CssClass = "generic-decimal-textbox disabled-textbox";

                    txtnvalorcom.Attributes.Add("readonly", "readonly");
                    txtnvalorcom.CssClass = "generic-decimal-textbox disabled-textbox";

                    txtnvalorcomi.Attributes.Add("readonly", "readonly");
                    txtnvalorcomi.CssClass = "generic-decimal-textbox disabled-textbox";

                    txtnmontliqui.Attributes.Add("readonly", "readonly");
                    txtnmontliqui.CssClass = "generic-decimal-textbox disabled-textbox";


                }



        }

        #endregion

        #region Método para Insertar un activo
        private bool Met_InsertarActivos()
        {
            if (!base.isRefresh)
            {
                MensajeCliente mc = null;

                if (ValidarCampos())
                {
                    try
                    {
                        BEactactivos estados = new BEactactivos();
                        estados = Met_ObtnerParametrosDeFormulario(estados, this.Met_CadenaConexion());
                        
                        if (estados != null)
                        {

                            Seractactivos Seractactivos = new Seractactivos(this.Met_CadenaConexion());
                            string msgUnidadExiste = "No se pueden incluir registros duplicados.";

                            estados.cfotoactiv = GuardarArchivo(estados);
                            string resultado = Seractactivos.Met_InsertarActivos(estados);


                            if (resultado != msgUnidadExiste)
                            {
                                ActivoSelect.cnumactivo = txtcnumactivo.Text;
                                mc = new MensajeCliente(MensajeCliente.TipoMensaje.Confirmacion, "Confirmación", resultado);
                                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                                Met_RegistrarBitacora(BEGeAcciones.Insertar, "Insertando activo", "actactivos", base.sesionActual.CCodUsuari);
                                return true;
                            }
                            else
                            {
                              
                                mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Advertencia", resultado);
                                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                                return false;
                            }

                        }
                        else return false;


                    }
                    catch (GenericException genericException)
                    {
                        mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", genericException.Mensaje + " <br /> " + ((genericException.NIdLogExcAG > 0) ? genericException.NIdLogExcAG.ToString() : ""));
                    }

                    catch (Exception)
                    {
                        mc = new MensajeCliente(MensajeCliente.TipoMensaje.Error, "Validando…", "Se ha producido un error al insertar.");
                    }

                    if (mc != null)
                    {
                        wucMensajePopUp.Met_SetearMensajeCliente(mc);
                        return false;
                    }
                }
            }

            return false;

        }
        #endregion

        #region Método para Actualizar un activo
        private bool Met_ActualizaActivos()
        {
            if (!base.isRefresh)
            {
                MensajeCliente mc = null;
                if (ValidarCampos())
                {

                    try
                    {
                        BEactactivos estados = new BEactactivos();
                        estados = Met_ObtnerParametrosDeFormulario(estados, this.Met_CadenaConexion());
                        if (estados != null)
                        {

                            Seractactivos Seractactivos = new Seractactivos(this.Met_CadenaConexion());
                            
                            estados.cfotoactiv = GuardarArchivo(estados);
                            string resultado = Seractactivos.Met_ActualizarActivos(estados);
                            
                            mc = new MensajeCliente(MensajeCliente.TipoMensaje.Confirmacion, "Confirmación", resultado);
                            wucMensajePopUp.Met_SetearMensajeCliente(mc);

                            ActivoSelect.cnumactivo = txtcnumactivo.Text;
                            Met_RegistrarBitacora(BEGeAcciones.Modificar, "Modificando activo", "actactivos", base.sesionActual.CCodUsuari);
                            return true;

                        }
                        else return false;


                    }
                    catch (GenericException genericException)
                    {
                        mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", genericException.Mensaje + " <br /> " + ((genericException.NIdLogExcAG > 0) ? genericException.NIdLogExcAG.ToString() : ""));
                    }

                    catch (Exception)
                    {
                        mc = new MensajeCliente(MensajeCliente.TipoMensaje.Error, "Validando…", "Se ha producido un error al actualizar.");
                    }

                    if (mc != null)
                    {
                        wucMensajePopUp.Met_SetearMensajeCliente(mc);
                        return false;
                    }
                }
               
            }
            return false;
        }
        #endregion


        #region Método para validar los campos del formulario
        private bool ValidarCampos()
        {
            string msj = "";
            bool listoSalvar = true;

            Seractparamet seractparamet=new Seractparamet(Met_CadenaConexion());
            var montominimo = seractparamet.Met_SelMontoMinimo().nmonminact;

            if (string.IsNullOrEmpty(txtcnumactivo.Text) || txtcnumactivo.Text=="000000")
            {
                msj = "Debe ingresar el número de placa.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (string.IsNullOrEmpty(txtcnomactiv.Text))
            {
                msj = "Debe ingresar el nombre del activo.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (string.IsNullOrEmpty(wucGenericBusquedaTipoActivo.GetTextField1().Text))
            {
                msj = "Debe ingresar el tipo de activo.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (string.IsNullOrEmpty(wucGenericBusquedaTipoReferencia.GetTextField1().Text))
            {
                msj = "Debe ingresar el código de referencia de tributación.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (string.IsNullOrEmpty(txtdfechaacti.Text))
            {
                msj = "Debe ingresar la fecha de compra.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (((ConvertirStringDecimal(txtnporcdepre.Text) <= 0) || (ConvertirStringDecimal(txtnporcdepre.Text) > 100)) && radcactdeprec.SelectedValue == "S" && radtipodepre.SelectedValue == "L")
            {
                msj = "El porcentaje de depreciación es incorrecto.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (ConvertirStringDecimal(txtnperiovida.Text) <= 0 && radcactdeprec.SelectedValue == "S" && radtipodepre.SelectedValue == "S")
            {
                msj = "El periodo de vida util es incorrecto.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (ConvertirStringDecimal(txtnvalorcom.Text) <= 0)
            {
                msj = "El monto de compra ingresado es incorrecto.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (ConvertirStringDecimal(txtndepreacum.Text) < 0)
            {
                msj = "El monto de depreciación acumulada es incorrecto.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (ConvertirStringDecimal(txtnvalorcom.Text) < montominimo)
            {
                msj = "El monto del activo debe ser mayor o igual a " + FormatearDecimalString(montominimo);
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (ConvertirStringDecimal(txtnvalorcom.Text) <= ConvertirStringDecimal(txtnvalorresi.Text))
            {
                msj = "El monto del activo debe ser mayor al valor residual.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if (ConvertirStringDecimal(txtmontact.Text) < ConvertirStringDecimal(txtnvalorresi.Text))
            {
                msj = "El valor en libros del activo debe ser mayor o igual al valor residual.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            else if ((ConvertirStringDecimal(txtnmontliqui.Text) < 0) || (ConvertirStringDecimal(txtnmontliqui.Text) > 0 && (ConvertirStringDecimal(txtnmontliqui.Text) != (ConvertirStringDecimal(txtmontact.Text) + ConvertirStringDecimal(txtmonmejo.Text) + ConvertirStringDecimal(txtmonreva.Text)))))
            {
                msj = "El monto de liquidación es incorrecto.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoSalvar = false;
            }
            
            
            return listoSalvar;

        }
        #endregion


        #region Método para validar la eliminación
        private bool ValidarEliminar()
        {
            string msj = "";
            bool listoEliminar = true;
           
           
            if (ConvertirStringDecimal(txtndepreacum.Text) > 0)
            {
                msj = "No se permite eliminar el activo, ya que la depreciación es mayor a 0.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoEliminar = false;
            }
            else if (ConvertirStringDecimal(txtnmontliqui.Text) > 0)
            {
                msj = "No se permite eliminar el activo, ya que ha sido liquidado.";
                MensajeCliente mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…", msj);
                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                listoEliminar = false;
            }


            return listoEliminar;

        }
        #endregion

        #region Método para eliminar un activo
        private bool Met_EliminarActivos()
        {
            if (!base.isRefresh)
            {
                MensajeCliente mc = null;

               
                    if (!string.IsNullOrEmpty(txtcnumactivo.Text))
                    {
                        if (ValidarEliminar())
                        {
                            try
                            {
                                Seractactivos Seractactivos = new Seractactivos(this.Met_CadenaConexion());
                                string resultado = Seractactivos.Met_EliminarActivos(txtcnumactivo.Text);
                                string msgActivosEliminado = "El registro se ha eliminado exitosamente";
                                mc = new MensajeCliente(MensajeCliente.TipoMensaje.Confirmacion, "Confirmación",
                                    resultado);
                                wucMensajePopUp.Met_SetearMensajeCliente(mc);
                                if (msgActivosEliminado == resultado)
                                {
                                    ActivoSelect.cnumactivo = "";
                                    Met_RegistrarBitacora(BEGeAcciones.Eliminar, "Eliminando activo", "actactivos",
                                        base.sesionActual.CCodUsuari);
                                    return true;
                                }
                                return false;
                            }
                            catch (GenericException genericException)
                            {
                                mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Validando…",
                                    genericException.Mensaje + " <br /> ");
                            }

                            catch (Exception)
                            {
                                mc = new MensajeCliente(MensajeCliente.TipoMensaje.Error, "Validando…",
                                    "Se ha producido un error al eliminar.");
                            }
                        }

                    }
                    else
                    {
                        string resultado = "Debe seleccionar un activo para eliminar el registro.";
                        mc = new MensajeCliente(MensajeCliente.TipoMensaje.Advertencia, "Confirmación", resultado);
                        wucMensajePopUp.Met_SetearMensajeCliente(mc);
 
                    } 
                    if (mc != null)
                    {
                        wucMensajePopUp.Met_SetearMensajeCliente(mc);
                        return false;
                    }               
                
            }

            return false;

        }
        #endregion


        protected void btnContinuar_Click(object sender, EventArgs e)
        {

        }

        protected void btnNoContinuar_Click(object sender, EventArgs e)
        {

        }

        protected void hlNuevo_OnClick(object sender, EventArgs e)
        {
            EventoSeleccionado(1);

        }

        protected void hlEditar_OnClick(object sender, EventArgs e)
        {
            EventoSeleccionado(2);
        }

        protected void hlBorrar_OnClick(object sender, EventArgs e)
        {
            EventoSeleccionado(3);

        }

        protected void hlAceptar_OnClick(object sender, EventArgs e)
        {
            EventoSeleccionado(4);
        }

        protected void hlCancelar_OnClick(object sender, EventArgs e)
        {
            EventoSeleccionado(5);
        }

       

        protected void Met_IngBitacora(string pDesc, int accion)
        {
            //Se instancia el objeto bitácora
            BEGebitacora beGebitacora = new BEGebitacora();
            //Se llena el objeto bitácora
            beGebitacora.CCODUSUARI = sesionActual.CCodUsuari;
            beGebitacora.CDETALLESI = pDesc;
            switch (accion)
            {
                case 1:
                    beGebitacora.CACCIONEJE = BEGeAcciones.Insertar;
                    break;
                case 2:
                    beGebitacora.CACCIONEJE = BEGeAcciones.Modificar;
                    break;
                case 3:
                    beGebitacora.CACCIONEJE = BEGeAcciones.Eliminar;
                    break;
                case 4:
                    beGebitacora.CACCIONEJE = BEGeAcciones.Consultar;
                    break;
                case 8:
                    beGebitacora.CACCIONEJE = BEGeAcciones.Reporte;
                    break;
                case 9:
                    beGebitacora.CACCIONEJE = BEGeAcciones.Procesos;
                    break;
            }

            beGebitacora.CCODAPLICA = Convert.ToString(Request.QueryString["cod"]);
            beGebitacora.CNOMBTABLA = "";
            beGebitacora.CIDASOCIAD = string.Empty;

            sesionActual.CodOpcionCodeas = beGebitacora.CCODAPLICA;

            //\FIN se llena el objeto bitácora

            //Se realiza el llamado al método para el insert del registro del evento
            CWBitacora.Met_InsertarEnBitacora(beGebitacora, sesionActual);
        }

        protected void Met_IngBitacoraError()
        {
            //Se instancia el objeto bitácora
            BEGebitacora beGebitacora = new BEGebitacora();
            //Se llena el objeto bitácora
            beGebitacora.CCODUSUARI = sesionActual.CCodUsuari;
            beGebitacora.CDETALLESI = "Error en el mantenimiento de estados de los empleados";
            beGebitacora.CACCIONEJE = BEGeAcciones.Cancelar;
            beGebitacora.CCODAPLICA = Convert.ToString(Request.QueryString["cod"]);
            beGebitacora.CNOMBTABLA = "";
            beGebitacora.CIDASOCIAD = string.Empty;

            sesionActual.CodOpcionCodeas = beGebitacora.CCODAPLICA;

            //\FIN se llena el objeto bitácora

            //Se realiza el llamado al método para el insert del registro del evento
            CWBitacora.Met_InsertarEnBitacora(beGebitacora, sesionActual);
        }


        protected void btn_poliza_OnClick(object sender, EventArgs e)
        {
            //llenarPolizas();
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/paginas/modulos/Menu.aspx#");
        }

        private void Met_CrearYouTubeLink()
        {
            if (string.IsNullOrEmpty(YoutubeLink)) return;
            var linkCtrl = new HyperLink
            {
                CssClass = "btn-md btn-youtube",
                NavigateUrl = YoutubeLink,
                Target = "_blank",
                ID = "youtubeLink",
                ToolTip = @"Conozca más sobre esta opción en Youtube"
            };
            pnlRightBtn.Controls.AddAt(0, linkCtrl);
        }
    }
}

    

  
