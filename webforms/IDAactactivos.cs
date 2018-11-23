using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeasWebEntities.Entities.Activos;
using CodeasWebEntities.Entities.Contabilidad;

namespace ICodeasWebDataAccess.IDataAccess.Activos
{
    public interface IDAactactivos
    {
        /// <summary>
        /// Buscar Activos
        /// </summary>
        /// <returns></returns>
        List<BEactactivos> Met_ObtenerActivosFilter(string valorFiltro, string campoFiltro, string operadorFiltro, int topFiltro);

        /// <summary>
        /// Inserta un Activo
        /// </summary>
        /// <param name="Activos"></param>
        /// <returns></returns>
        string Met_InsertarActivos(BEactactivos Activos);

        /// <summary>
        /// Acualiza un Activo
        /// </summary>
        /// <param name="Activos"></param>
        /// <returns></returns>
        string Met_ActualizarActivos(BEactactivos Activos);

        /// <summary>
        /// Elimina un Activo
        /// </summary>
        /// <param name="ctipoprove"></param>
        /// <returns></returns>
        string Met_EliminarActivos(string ctipoprove);

        /// <summary>
        /// Obtiene la Activo por cnumactivo
        /// </summary>
        /// <returns></returns>
        BEactactivos Met_ObtenerActivoXID(string cnumactivo);

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
        string Met_ProcesoParaReporteMejorasRevaluaciones(string cfecha1, string cfecha2, string cTipAct1,
            string cTipAct2, string cChkMej, string cChkRev, string cREmpI, string cREmpF, string corden,
            out List<BEactactivos> listRevMej);
        //Obteniendo los tipos para revaluacion o mejora
        List<BEactactivos> Met_ObtenerParamActivo();

        /// <summary>
        /// <summary>
        /// Proc. para reporte de depreciacion
        /// </summary>
        /// <param name="cTipAct1"></param>
        /// <param name="cTipAct2"></param>
        /// <param name="cChkMej"></param>
        /// <param name="listAct"></param>
        /// <returns></returns>
        string Met_ProcesoParaReporteDepreciacion(string cTipAct1, string cTipAct2, string cChkMej,
            out List<BEactactivos> listAct);
            
        /// <summary>
        /// Obtiene los activos para asignar a pólizas
        /// </summary>
        List<BEactactivos> Met_ObtenerActivosAsignarPolizas(string cnumpoliza, string ctippoliza, string dfechavige);

        /// <summary>
        /// Proc. almacenado que obtiene los centros de costo de activos fijos.
        /// </summary>
        List<BECoCentcDep> Met_ObtenerCentrosCosto();

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
        string Met_ProcesoParaReporteMovimDepreciacion(string cfecha1, string cfecha2, string cTipAct1, string cTipAct2,
            string cUbica1, string cUbica2, string cNumAct1, string cNumAct2, string cMarca, string cSerie,
            string cModelo, out List<BEactactivos> listRes);

        /// <summary>
        /// Proc. para  proceso de renumerar activos
        /// </summary>
        /// <param name="cNumActNew"></param>
        /// <param name="cNumActOld"></param>
        /// <returns></returns>
        string Met_ProcesoRenumerarActivos(string cNumActNew, string cNumActOld);
        
        ///
        /// Obtiene datos de activo para liquidacion del mismo
        /// 
        BEactactivos Met_ObtenerActivoLiquid(string cnumactivo, string ventgast, ref string tipasi_ctipasient, ref string tipasi_nconsecuti);

    }
}
