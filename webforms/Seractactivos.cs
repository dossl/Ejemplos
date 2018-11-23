using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeasWebDataAccess.DataAccess.Activos;
using CodeasWebEntities.Entities.Activos;
using CodeasWebEntities.Entities.Contabilidad;
using ICodeasWebDataAccess.IDataAccess.Activos;

namespace CodeasWebServices.Services.Activos
{
    public class Seractactivos
    {

        #region Variables privadas

        /// <summary>
        /// Contiene la cadena de conexión al origen de datos
        /// </summary>
        private string _cadenaConexion;

        /// <summary>
        /// Instancia de la clase de acceso a datos.
        /// </summary>
        private IDAactactivos _iDAactactivos;

        #endregion

        #region Constructores

        /// <summary>
        /// Nueva instancia de <see cref="Seractactivos"/>.
        /// </summary>
        /// <param name="cadenaConexión">Cadena de conexión.</param>
        public Seractactivos(string cadenaConexion)
        {

            this._cadenaConexion = cadenaConexion;
            this._iDAactactivos = new DAactactivos(this._cadenaConexion);

        }
        #endregion

        #region Métodos Públicos
        /// <summary>
        /// insertar un Activo
        /// </summary>
        /// <returns></returns>
        public string Met_InsertarActivos(BEactactivos Activos)
        {
           return _iDAactactivos.Met_InsertarActivos(Activos);
        }

       
        /// <summary>
        /// modificar un Activo
        /// </summary>
        /// <returns></returns>
        public string Met_ActualizarActivos(BEactactivos Activos)
        {
            return _iDAactactivos.Met_ActualizarActivos(Activos);
        }

        

        /// <summary>
        /// elimina el Activo
        /// </summary>
        /// <returns></returns>
        public string Met_EliminarActivos(string ctipoprove)
        {
            return _iDAactactivos.Met_EliminarActivos(ctipoprove);
        }

        /// <summary>
        /// Obtiene las Activos
        /// </summary>
        /// <returns></returns>
        public List<BEactactivos> Met_ObtenerActivosFilter(string valorFiltro, string campoFiltro,
            string operadorFiltro, int topFiltro)
        {
            return _iDAactactivos.Met_ObtenerActivosFilter(valorFiltro, campoFiltro, operadorFiltro, topFiltro);
        }

        /// <summary>
        /// Obtiene el Activo por cnumactivo
        /// </summary>
        /// <returns></returns>
        public BEactactivos Met_ObtenerActivoXID(string cnumactivo)
        {
            return _iDAactactivos.Met_ObtenerActivoXID(cnumactivo);
        }

        /// <summary>
        ///  Proc. para reporte de mejoras y revaluaciones
        /// </summary>
        /// <param name="cfecha1"></param>
        /// <param name="cfecha2"></param>
        /// <param name="cTipAct1"></param>
        /// <param name="cTipAct2"></param>
        /// <param name="cChkMej"></param>
        /// <param name="cChkRev"></param>
        /// <param name="cREmpI"></param>
        /// <param name="cREmpF"></param>
        /// <param name="corden"></param>
        /// <returns></returns>
        public string Met_ProcesoParaReporteMejorasRevaluaciones(string cfecha1, string cfecha2, string cTipAct1,
            string cTipAct2, string cChkMej, string cChkRev, string cREmpI, string cREmpF, string corden,
            out List<BEactactivos> listRevMej)
        {
            return _iDAactactivos.Met_ProcesoParaReporteMejorasRevaluaciones( cfecha1,  cfecha2,  cTipAct1,
             cTipAct2,  cChkMej,  cChkRev,  cREmpI,  cREmpF,  corden, out  listRevMej);
        }


        /// <summary>
        /// Proc. para reporte de depreciacion
        /// </summary>
        /// <param name="cTipAct1"></param>
        /// <param name="cTipAct2"></param>
        /// <param name="cChkMej"></param>
        /// <param name="listAct"></param>
        /// <returns></returns>
        public string Met_ProcesoParaReporteDepreciacion(string cTipAct1, string cTipAct2, string cChkMej,
            out List<BEactactivos> listAct)
        {
            return _iDAactactivos.Met_ProcesoParaReporteDepreciacion(cTipAct1,cTipAct2, cChkMej, out listAct);
        }
        
         /// <summary>
        /// Obtiene los activos para asignar a pólizas
        /// </summary>
       public List<BEactactivos> Met_ObtenerActivosAsignarPolizas(string cnumpoliza, string ctippoliza, string dfechavige)
        {
            return _iDAactactivos.Met_ObtenerActivosAsignarPolizas(cnumpoliza, ctippoliza, dfechavige);
        }

        /// <summary>
        /// Proc. almacenado que obtiene los centros de costo de activos fijos.
        /// </summary>
        public List<BECoCentcDep> Met_ObtenerCentrosCosto()
        {
            return _iDAactactivos.Met_ObtenerCentrosCosto();
        }
        public List<BEactactivos> Met_ObtenerParamActivo()
        {
            return _iDAactactivos.Met_ObtenerParamActivo();
        }


        /// <summary>
        /// Proc. para reporte de movimiento depreciacion
        /// </summary>
        /// <param name="cfecha1"></param>
        /// <param name="cfecha2"></param>
        /// <param name="cTipAct1"></param>
        /// <param name="cTipAct2"></param>
        /// <param name="cUbica1"></param>
        /// <param name="cUbica2"></param>
        /// <param name="cNumAct1"></param>
        /// <param name="cNumAct2"></param>
        /// <param name="cMarca"></param>
        /// <param name="cSerie"></param>
        /// <param name="cModelo"></param>
        /// <param name="listRes"></param>
        /// <returns></returns>
        public string Met_ProcesoParaReporteMovimDepreciacion(string cfecha1, string cfecha2, string cTipAct1, string cTipAct2, string cUbica1, string cUbica2, string cNumAct1, string cNumAct2, string cMarca, string cSerie,string cModelo, out  List<BEactactivos> listRes)
        {
            return _iDAactactivos.Met_ProcesoParaReporteMovimDepreciacion(cfecha1, cfecha2, cTipAct1, cTipAct2, cUbica1, cUbica2, cNumAct1, cNumAct2, cMarca, cSerie, cModelo, out  listRes);
        }

        /// <summary>
        /// Proc. para  proceso de renumerar activos
        /// </summary>
        /// <param name="cNumActNew"></param>
        /// <param name="cNumActOld"></param>
        /// <returns></returns>
        public string Met_ProcesoRenumerarActivos(string cNumActNew, string cNumActOld)
        {
            return _iDAactactivos.Met_ProcesoRenumerarActivos(cNumActNew, cNumActOld);
        }
        
         public BEactactivos Met_ObtenerActivoLiquid(string cnumactivo, string ventgast, ref string tipasi_ctipasient, ref string tipasi_nconsecuti)
        {
            return _iDAactactivos.Met_ObtenerActivoLiquid(cnumactivo, ventgast, ref tipasi_ctipasient, ref tipasi_nconsecuti);
        }


        #endregion

     
    }
}
