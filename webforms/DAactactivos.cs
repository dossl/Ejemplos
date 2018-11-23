using CodeasWebDataAccess.HelperClasses;
using CodeasWebEntities.Entities.Activos;
using CodeasWebEntities.Entities.Contabilidad;
using CodeasWebExceptions.Exceptions;
using ICodeasWebDataAccess.IDataAccess.Activos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace CodeasWebDataAccess.DataAccess.Activos
{
    public class DAactactivos : IDAactactivos
    {
        #region Miembros privados
        /// <summary>
        /// Almacena la cadena de conexión inyectada mediante el constructor.
        /// </summary>
        private string _cadenaConexion;
        #endregion

        #region Nombres de Proc. Almacenados

        /// <summary>
        /// Proc. almacenado que busca los Activos
        /// </summary>
        private string _Usp_CW_Sel_Activos_Filter = "Usp_CW_Sel_Activos_Filter";
        private string _Usp_cw_Sel_wucBuscarActivos_Filter = "Usp_cw_Sel_wucBuscarActivos_Filter";


        /// <summary>
        /// Obtiene el Activo por cnumactivo
        /// </summary>
        private string _Usp_CW_Sel_Activos_X_ID = "Usp_CW_Sel_Activos_X_ID";


        /// <summary>
        /// Proc. almacenado que inserta Activo
        /// </summary>
        private string _Usp_CW_Ins_Activos = "Usp_CW_Ins_Activos";

        /// <summary>
        /// Proc. almacenado que actualiza Activo
        /// </summary>
        private string _Usp_CW_Upd_Activos = "Usp_CW_Upd_Activos";

        /// <summary>
        /// Proc. almacenado que elimina Activo
        /// </summary>
        private string _Usp_CW_Del_Activos = "Usp_CW_Del_Activos";

        private string _Usp_CW_Activos_Sel_Activo_All = "Usp_CW_Activos_Sel_Activo_All";

        /// <summary>
        /// Proc. almacenado Para el reporte de revaluaciones y mejoras
        /// </summary>
        private string _Usp_CW_Sel_Activo_ReporteMejorasRevaluaciones = "Usp_CW_Sel_Activo_ReporteMejorasRevaluaciones";

        /// <summary>
        /// Proc. almacenado para cargar los parmetros de activo, es decir, para decidir si sera una revaluacion o mejora.
        /// </summary>
        private string _Usp_cw_Sel_CargarActparamet = "Usp_cw_Sel_CargarActparamet";
        private const string _Usp_cw_Sel_wucBuscarActivo_Filter = "Usp_cw_Sel_wucBuscarActivo_Filter";



        /// <summary>       
        /// Obtiene los activos para asignar a pólizas
        /// </summary>
        private string _Usp_CW_Sel_Activos_AsignarPolizas = "Usp_CW_Sel_Activos_AsignarPolizas";




        /// <summary>
        /// Proc. almacenado Para el reporte de depreciacion
        /// </summary>
        private string _Usp_CW_Sel_Activo_ReporteDepreciacion = "Usp_CW_Sel_Activo_ReporteDepreciacion";

        /// <summary>
        /// Proc. almacenado que obtiene los centros de costo de activos fijos.
        /// </summary>
        private string _Usp_CW_Sel_VR_modulos_ccostoxdeptoActivosFijos = "Usp_CW_Sel_VR_modulos_ccostoxdeptoActivosFijos";

        /// <summary>
        /// Proc. almacenado Para el reporte de movimiento depreciacion
        /// </summary>
        private string _Usp_CW_Sel_Activo_ReporteMovimientoDepreciacion = "Usp_CW_Sel_Activo_ReporteMovimientoDepreciacion";

        /// <summary>
        /// Proc. almacenado Para el proceso de renumerar activos
        /// </summary>
        private string _Usp_CW_Upd_Activos_RenumeracionActivos = "Usp_CW_Upd_Activos_RenumeracionActivos";

        /// <summary>
        /// Proc. almacenado que carga los datos de un activo para su liquidacion
        /// </summary>
        private string _Usp_CW_Sel_Activos_DatosAct = "Usp_CW_Sel_Activos_DatosAct";

        ///<summary>
        /// Procedimiento que selecciona los activos para su liquidacion
        /// </summary>
        private string _Usp_cw_Sel_wucBuscarActivosLiq_Filter = "Usp_cw_Sel_wucBuscarActivosLiq_Filter";
        #endregion

        #region Constructores
        /// <summary>
        /// Nueva instancia de <see cref="DAactactivos"/>
        /// </summary>
        /// <param name="cadenaConexion">Cadena de conexión SQL</param>
        public DAactactivos(string cadenaConexion)
        {
            this._cadenaConexion = cadenaConexion;
        }
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Obtiene los Activos
        /// </summary>
        /// <returns></returns>
        public List<BEactactivos> Met_ObtenerActivosFilter(string valorFiltro, string campoFiltro, string operadorFiltro, int topFiltro)
        {
            string filtro = (operadorFiltro == "=") ? "IGUAL" : operadorFiltro;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var list = new List<BEactactivos>();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activos_Filter, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@cvalorfilt", SqlDbType.VarChar, 100).Value = valorFiltro ?? string.Empty;
                    sqlCommand.Parameters.Add("@ccolumna", SqlDbType.VarChar, 10).Value = campoFiltro ?? string.Empty;
                    sqlCommand.Parameters.Add("@coperador", SqlDbType.VarChar, 5).Value = filtro ?? string.Empty;
                    sqlCommand.Parameters.Add("@topselect", SqlDbType.Int).Value = topFiltro;

                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var result = new BEactactivos
                                {
                                    cnumactivo = reader["cnumactivo"].ToString().Trim(),
                                    cnomactiv = reader["cnomactiv"].ToString().Trim()


                                };
                                list.Add(result);

                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return list;

                }
            };

        }


        /// <summary>
        /// Obtiene el Activo por cnumactivo
        /// </summary>
        /// <returns></returns>
        public BEactactivos Met_ObtenerActivoXID(string cnumactivo)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var list = new BEactactivos();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activos_X_ID, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@cnumactivo", SqlDbType.Char).Value = cnumactivo;


                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var result = new BEactactivos
                                {
                                    cnumactivo = reader["cnumactivo"].ToString().Trim(),
                                    ctipoactiv = reader["ctipoactiv"].ToString().Trim(),
                                    cdescactiv = reader["cdescactiv"].ToString().Trim(),
                                    cnomactiv = reader["cnomactiv"].ToString().Trim(),
                                    cmarcaacti = reader["cmarcaacti"].ToString().Trim(),
                                    cserieacti = reader["cserieacti"].ToString().Trim(),
                                    cmodeloact = reader["cmodeloact"].ToString().Trim(),
                                    dfechaacti = Convert.ToDateTime(reader["dfechaacti"].ToString().Trim()),
                                    nvalorcom = Convert.ToDecimal(reader["nvalorcom"].ToString().Trim()),
                                    nporcdepre = Convert.ToDecimal(reader["nporcdepre"].ToString().Trim()),
                                    ndepreacum = Convert.ToDecimal(reader["ndepreacum"].ToString().Trim()),
                                    ccodproved = reader["ccodproved"].ToString().Trim(),
                                    cnumckcomp = reader["cnumckcomp"].ToString().Trim(),
                                    ccoddepart = reader["ccoddepart"].ToString().Trim(),
                                    cactdeprec = reader["cactdeprec"].ToString().Trim(),
                                    nmontomejo = Convert.ToDecimal(reader["nmontomejo"].ToString().Trim()),
                                    nmontoreva = Convert.ToDecimal(reader["nmontoreva"].ToString().Trim()),
                                    nmontdpmej = Convert.ToDecimal(reader["nmontdpmej"].ToString().Trim()),
                                    nmontdprev = Convert.ToDecimal(reader["nmontdprev"].ToString().Trim()),
                                    nmontliqui = Convert.ToDecimal(reader["nmontliqui"].ToString().Trim()),
                                    nvalorresi = Convert.ToDecimal(reader["nvalorresi"].ToString().Trim()),
                                    nvalorrepo = Convert.ToDecimal(reader["nvalorrepo"].ToString().Trim()),
                                    cestactivo = reader["cestactivo"].ToString().Trim(),
                                    ccodrefere = reader["ccodrefere"].ToString().Trim(),
                                    ctipodepre = reader["ctipodepre"].ToString().Trim(),
                                    nperiovida = Convert.ToDecimal(reader["nperiovida"].ToString().Trim()),
                                    nmontdesva = Convert.ToDecimal(reader["nmontdesva"].ToString().Trim()),
                                    cfotoactiv = reader["cfotoactiv"].ToString().Trim(),
                                    cidubicaci = reader["cidubicaci"].ToString().Trim(),
                                    cnumgarant = reader["cnumgarant"].ToString().Trim(),
                                    cdescrigar = reader["cdescrigar"].ToString().Trim(),
                                    nvalorcom1 = Convert.ToDecimal(reader["nvalorcom1"].ToString().Trim()),
                                    nvalorcomi = Convert.ToDecimal(reader["nvalorcomi"].ToString().Trim()),
                                    CCODCENTRO = reader["CCODCENTRO"].ToString().Trim(),
                                    cnumeorden = reader["cnumeorden"].ToString().Trim()



                                };
                                list = result;

                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return list;

                }
            };

        }


        /// <summary>
        /// Inserta Activo
        /// </summary>
        /// <param name="Activos"></param>
        /// <returns></returns>
        public string Met_InsertarActivos(BEactactivos Activos)
        {
            // Mensaje de excepción
            string cMensajeExc = "";
            string valorRetorno = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Ins_Activos, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        sqlCommand.Parameters.Add("@cnumactivo", SqlDbType.VarChar).Value = Activos.cnumactivo;
                        sqlCommand.Parameters.Add("@ctipoactiv", SqlDbType.VarChar).Value = Activos.ctipoactiv;
                        sqlCommand.Parameters.Add("@cdescactiv", SqlDbType.VarChar).Value = Activos.cdescactiv;
                        sqlCommand.Parameters.Add("@cnomactiv ", SqlDbType.VarChar).Value = Activos.cnomactiv;
                        sqlCommand.Parameters.Add("@cmarcaacti", SqlDbType.VarChar).Value = Activos.cmarcaacti;
                        sqlCommand.Parameters.Add("@cserieacti", SqlDbType.VarChar).Value = Activos.cserieacti;
                        sqlCommand.Parameters.Add("@cmodeloact", SqlDbType.VarChar).Value = Activos.cmodeloact;
                        sqlCommand.Parameters.Add("@dfechaacti", SqlDbType.DateTime).Value = Activos.dfechaacti;
                        sqlCommand.Parameters.Add("@nvalorcom ", SqlDbType.Decimal).Value = Activos.nvalorcom;
                        sqlCommand.Parameters.Add("@nporcdepre", SqlDbType.Decimal).Value = Activos.nporcdepre;
                        sqlCommand.Parameters.Add("@ndepreacum", SqlDbType.Decimal).Value = Activos.ndepreacum;
                        sqlCommand.Parameters.Add("@ccodproved", SqlDbType.VarChar).Value = Activos.ccodproved;
                        sqlCommand.Parameters.Add("@cnumckcomp", SqlDbType.VarChar).Value = Activos.cnumckcomp;
                        sqlCommand.Parameters.Add("@ccoddepart", SqlDbType.VarChar).Value = Activos.ccoddepart;
                        sqlCommand.Parameters.Add("@cactdeprec", SqlDbType.VarChar).Value = Activos.cactdeprec;
                        sqlCommand.Parameters.Add("@nmontomejo", SqlDbType.Decimal).Value = Activos.nmontomejo;
                        sqlCommand.Parameters.Add("@nmontoreva", SqlDbType.Decimal).Value = Activos.nmontoreva;
                        sqlCommand.Parameters.Add("@nmontdpmej", SqlDbType.Decimal).Value = Activos.nmontdpmej;
                        sqlCommand.Parameters.Add("@nmontdprev", SqlDbType.Decimal).Value = Activos.nmontdprev;
                        sqlCommand.Parameters.Add("@nmontliqui", SqlDbType.Decimal).Value = Activos.nmontliqui;
                        sqlCommand.Parameters.Add("@nvalorresi", SqlDbType.Decimal).Value = Activos.nvalorresi;
                        sqlCommand.Parameters.Add("@nvalorrepo", SqlDbType.Decimal).Value = Activos.nvalorrepo;
                        sqlCommand.Parameters.Add("@cestactivo", SqlDbType.VarChar).Value = Activos.cestactivo;
                        sqlCommand.Parameters.Add("@ccodrefere", SqlDbType.VarChar).Value = Activos.ccodrefere;
                        sqlCommand.Parameters.Add("@ctipodepre", SqlDbType.VarChar).Value = Activos.ctipodepre;
                        sqlCommand.Parameters.Add("@nperiovida", SqlDbType.Decimal).Value = Activos.nperiovida;
                        sqlCommand.Parameters.Add("@nmontdesva", SqlDbType.Decimal).Value = Activos.nmontdesva;
                        sqlCommand.Parameters.Add("@cfotoactiv", SqlDbType.VarChar).Value = Activos.cfotoactiv;
                        if (Activos.cidubicaci != string.Empty)
                        {
                            sqlCommand.Parameters.Add("@cidubicaci", SqlDbType.VarChar).Value = Activos.cidubicaci;
                        }
                        else
                        {
                            sqlCommand.Parameters.Add("@cidubicaci", SqlDbType.VarChar).Value = DBNull.Value;
                        }
                        sqlCommand.Parameters.Add("@cnumgarant", SqlDbType.VarChar).Value = Activos.cnumgarant;
                        sqlCommand.Parameters.Add("@cdescrigar", SqlDbType.VarChar).Value = Activos.cdescrigar;
                        sqlCommand.Parameters.Add("@nvalorcom1", SqlDbType.Decimal).Value = Activos.nvalorcom1;
                        sqlCommand.Parameters.Add("@nvalorcomi", SqlDbType.Decimal).Value = Activos.nvalorcomi;
                        sqlCommand.Parameters.Add("@CCODCENTRO", SqlDbType.VarChar).Value = Activos.CCODCENTRO;
                        sqlCommand.Parameters.Add("@cnumeorden", SqlDbType.VarChar).Value = Activos.cnumeorden;

                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);


                        sqlConnection.Open();

                        sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0 && resulIdExcep != 2)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {

                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        throw ex;
                    }
                }
            }


        }


        /// <summary>
        /// Actualiza Activo
        /// </summary>
        /// <param name="Activos"></param>
        /// <returns></returns>
        public string Met_ActualizarActivos(BEactactivos Activos)
        {
            // Mensaje de excepción
            string cMensajeExc = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Upd_Activos, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        sqlCommand.Parameters.Add("@cnumactivo", SqlDbType.VarChar).Value = Activos.cnumactivo;
                        sqlCommand.Parameters.Add("@ctipoactiv", SqlDbType.VarChar).Value = Activos.ctipoactiv;
                        sqlCommand.Parameters.Add("@cdescactiv", SqlDbType.VarChar).Value = Activos.cdescactiv;
                        sqlCommand.Parameters.Add("@cnomactiv ", SqlDbType.VarChar).Value = Activos.cnomactiv;
                        sqlCommand.Parameters.Add("@cmarcaacti", SqlDbType.VarChar).Value = Activos.cmarcaacti;
                        sqlCommand.Parameters.Add("@cserieacti", SqlDbType.VarChar).Value = Activos.cserieacti;
                        sqlCommand.Parameters.Add("@cmodeloact", SqlDbType.VarChar).Value = Activos.cmodeloact;
                        sqlCommand.Parameters.Add("@dfechaacti", SqlDbType.DateTime).Value = Activos.dfechaacti;
                        sqlCommand.Parameters.Add("@nvalorcom ", SqlDbType.Decimal).Value = Activos.nvalorcom;
                        sqlCommand.Parameters.Add("@nporcdepre", SqlDbType.Decimal).Value = Activos.nporcdepre;
                        sqlCommand.Parameters.Add("@ndepreacum", SqlDbType.Decimal).Value = Activos.ndepreacum;
                        sqlCommand.Parameters.Add("@ccodproved", SqlDbType.VarChar).Value = Activos.ccodproved;
                        sqlCommand.Parameters.Add("@cnumckcomp", SqlDbType.VarChar).Value = Activos.cnumckcomp;
                        sqlCommand.Parameters.Add("@ccoddepart", SqlDbType.VarChar).Value = Activos.ccoddepart;
                        sqlCommand.Parameters.Add("@cactdeprec", SqlDbType.VarChar).Value = Activos.cactdeprec;
                        sqlCommand.Parameters.Add("@nmontomejo", SqlDbType.Decimal).Value = Activos.nmontomejo;
                        sqlCommand.Parameters.Add("@nmontoreva", SqlDbType.Decimal).Value = Activos.nmontoreva;
                        sqlCommand.Parameters.Add("@nmontdpmej", SqlDbType.Decimal).Value = Activos.nmontdpmej;
                        sqlCommand.Parameters.Add("@nmontdprev", SqlDbType.Decimal).Value = Activos.nmontdprev;
                        sqlCommand.Parameters.Add("@nmontliqui", SqlDbType.Decimal).Value = Activos.nmontliqui;
                        sqlCommand.Parameters.Add("@nvalorresi", SqlDbType.Decimal).Value = Activos.nvalorresi;
                        sqlCommand.Parameters.Add("@nvalorrepo", SqlDbType.Decimal).Value = Activos.nvalorrepo;
                        sqlCommand.Parameters.Add("@cestactivo", SqlDbType.VarChar).Value = Activos.cestactivo;
                        sqlCommand.Parameters.Add("@ccodrefere", SqlDbType.VarChar).Value = Activos.ccodrefere;
                        sqlCommand.Parameters.Add("@ctipodepre", SqlDbType.VarChar).Value = Activos.ctipodepre;
                        sqlCommand.Parameters.Add("@nperiovida", SqlDbType.Decimal).Value = Activos.nperiovida;
                        sqlCommand.Parameters.Add("@nmontdesva", SqlDbType.Decimal).Value = Activos.nmontdesva;
                        sqlCommand.Parameters.Add("@cfotoactiv", SqlDbType.VarChar).Value = Activos.cfotoactiv;
                        if (Activos.cidubicaci != string.Empty)
                        {
                            sqlCommand.Parameters.Add("@cidubicaci", SqlDbType.VarChar).Value = Activos.cidubicaci;
                        }
                        else
                        {
                            sqlCommand.Parameters.Add("@cidubicaci", SqlDbType.VarChar).Value = DBNull.Value;
                        }
                        sqlCommand.Parameters.Add("@cnumgarant", SqlDbType.VarChar).Value = Activos.cnumgarant;
                        sqlCommand.Parameters.Add("@cdescrigar", SqlDbType.VarChar).Value = Activos.cdescrigar;
                        sqlCommand.Parameters.Add("@nvalorcom1", SqlDbType.Decimal).Value = Activos.nvalorcom1;
                        sqlCommand.Parameters.Add("@nvalorcomi", SqlDbType.Decimal).Value = Activos.nvalorcomi;
                        sqlCommand.Parameters.Add("@CCODCENTRO", SqlDbType.VarChar).Value = Activos.CCODCENTRO;
                        sqlCommand.Parameters.Add("@cnumeorden", SqlDbType.VarChar).Value = Activos.cnumeorden;

                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);

                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {

                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        throw ex;
                    }
                }
            }

        }


        /// <summary>
        /// Eliminaun Activo
        /// </summary>
        /// <param name="cnumactivo"></param>
        /// <returns></returns>
        public string Met_EliminarActivos(string cnumactivo)
        {
            string cMensajeExc = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Del_Activos, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        sqlCommand.Parameters.Add("@cnumactivo", SqlDbType.Char).Value = cnumactivo;

                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);

                        sqlConnection.Open();
                        var consulta = sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0 && resulIdExcep != 2)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {

                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        throw ex;
                    }
                }
            }

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
        public string Met_ProcesoParaReporteMejorasRevaluaciones(string cfecha1, string cfecha2, string cTipAct1, string cTipAct2, string cChkMej, string cChkRev, string cREmpI, string cREmpF, string corden, out List<BEactactivos> listRevMej)
        {
            // Mensaje de excepción
            string cMensajeExc = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activo_ReporteMejorasRevaluaciones, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        sqlCommand.Parameters.Add("@cfecha1", SqlDbType.VarChar, 20).Value = cfecha1;
                        sqlCommand.Parameters.Add("@cfecha2", SqlDbType.VarChar, 20).Value = cfecha2;
                        sqlCommand.Parameters.Add("@cTipAct1", SqlDbType.VarChar, 3).Value = cTipAct1;
                        sqlCommand.Parameters.Add("@cTipAct2", SqlDbType.VarChar).Value = cTipAct2;
                        sqlCommand.Parameters.Add("@cChkMej", SqlDbType.VarChar).Value = cChkMej;
                        sqlCommand.Parameters.Add("@cChkRev", SqlDbType.VarChar).Value = cChkRev;
                        sqlCommand.Parameters.Add("@cREmpI", SqlDbType.VarChar).Value = cREmpI;
                        sqlCommand.Parameters.Add("@cREmpF", SqlDbType.VarChar).Value = cREmpF;
                        sqlCommand.Parameters.Add("@corden", SqlDbType.VarChar).Value = corden;




                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);

                        sqlConnection.Open();
                        DataSet dsData = new DataSet();
                        List<BEactactivos> res = new List<BEactactivos>();

                        try
                        {

                            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                            da.Fill(dsData, "tmp");
                            if (dsData.Tables != null && dsData.Tables.Count > 0)
                            {
                                var cantTablasretornadas = dsData.Tables.Count;

                                DataTable tablaResultados = dsData.Tables[cantTablasretornadas - 1];

                                foreach (DataRow rowRes in tablaResultados.Rows)
                                {
                                    BEactactivos b = new BEactactivos();

                                    b.cnumactivo = rowRes["cnumactivo"] != null ? rowRes["cnumactivo"].ToString().Trim() : "";
                                    b.cnomactiv = rowRes["cnomactiv"] != null ? rowRes["cnomactiv"].ToString().Trim() : "";
                                    b.nvallibros = Convert.ToDecimal(rowRes["nvallibros"] != null ? rowRes["nvallibros"].ToString().Trim() : "0");
                                    b.nvalorcom = Convert.ToDecimal(rowRes["nvalorcom"] != null ? rowRes["nvalorcom"].ToString().Trim() : "0");
                                    b.dfechaacti = Convert.ToDateTime(rowRes["dfechaacti"] != null ? rowRes["dfechaacti"].ToString().Trim() : null);
                                    b.cnumtransa = rowRes["cnumtransa"] != null ? rowRes["cnumtransa"].ToString().Trim() : "";
                                    b.ctipotrans = rowRes["ctipotrans"] != null ? rowRes["ctipotrans"].ToString().Trim() : "";
                                    b.dfechamovi = Convert.ToDateTime(rowRes["dfechamovi"] != null ? rowRes["dfechamovi"].ToString().Trim() : null);
                                    b.nvalorreva = Convert.ToDecimal(rowRes["nvalorreva"] != null ? rowRes["nvalorreva"].ToString().Trim() : "0");
                                    b.ndepreacum = Convert.ToDecimal(rowRes["ndepreacum"] != null ? rowRes["ndepreacum"].ToString().Trim() : "0");
                                    res.Add(b);
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                        sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {
                            listRevMej = res;
                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        throw ex;
                    }
                }
            }

        }


        /// <summary>
        /// Obtiene los activos para asignar a pólizas
        /// </summary>
        /// <returns></returns>
        public List<BEactactivos> Met_ObtenerActivosAsignarPolizas(string cnumpoliza, string ctippoliza, string dfechavige)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var list = new List<BEactactivos>();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activos_AsignarPolizas, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@cnumpoliza", SqlDbType.VarChar).Value = cnumpoliza;
                    sqlCommand.Parameters.Add("@ctippoliza", SqlDbType.VarChar).Value = ctippoliza;
                    sqlCommand.Parameters.Add("@dfechavige", SqlDbType.VarChar).Value = dfechavige;


                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var result = new BEactactivos
                                {
                                    cnumactivo = reader["cnumactivo"].ToString().Trim(),
                                    cnomactiv = reader["cnomactiv"].ToString().Trim(),
                                    dfechaacti = Convert.ToDateTime(reader["dfechaacti"].ToString().Trim()),
                                    nvalorcom = Convert.ToDecimal(reader["nvalorcom"].ToString().Trim()),
                                    nseleccion = reader["nseleccion"].ToString().Trim()



                                };
                                list.Add(result);

                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return list;

                }
            };

        }


        public List<BEactactivos> Met_BuscarActivos_Filter(string val, string col, string op, int top)

        {
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var listResult = new List<BEactactivos>();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_cw_Sel_wucBuscarActivos_Filter, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@val", SqlDbType.VarChar).Value = val;
                    sqlCommand.Parameters.Add("@col", SqlDbType.VarChar).Value = col;
                    sqlCommand.Parameters.Add("@op", SqlDbType.VarChar).Value = op;
                    sqlCommand.Parameters.Add("@top", SqlDbType.Int).Value = top;
                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                var result =
                                    new BEactactivos
                                    {
                                        cnumactivo = sqlDataReader["cnumactivo"].ToString().Trim(),
                                        cnomactiv = sqlDataReader["cnomactiv"].ToString().Trim(),
                                        ctipodepre = sqlDataReader["ctipodepre"].ToString().Trim(),
                                        ctipoactiv = sqlDataReader["ctipoactiv"].ToString().Trim()
                                    };

                                listResult.Add(result);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return listResult;
                }
            }
        }


        ///Busca activos para hacerles liquidacion

        public List<BEactactivos> Met_BuscarActivosLiq_Filter(string val, string col, string op, int top, string ventGastBaj)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var listResult = new List<BEactactivos>();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_cw_Sel_wucBuscarActivosLiq_Filter, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add("@val", SqlDbType.VarChar).Value = val;
                    sqlCommand.Parameters.Add("@col", SqlDbType.VarChar).Value = col;
                    sqlCommand.Parameters.Add("@op", SqlDbType.VarChar).Value = op;
                    sqlCommand.Parameters.Add("@top", SqlDbType.Int).Value = top;
                    sqlCommand.Parameters.Add("@ventGastBaj", SqlDbType.VarChar).Value = ventGastBaj;
                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                var result =
                                    new BEactactivos
                                    {
                                        cnumactivo = sqlDataReader["cnumactivo"].ToString().Trim(),
                                        cnomactiv = sqlDataReader["cnomactiv"].ToString().Trim(),
                                        ctipodepre = sqlDataReader["ctipodepre"].ToString().Trim(),
                                        ctipoactiv = sqlDataReader["ctipoactiv"].ToString().Trim()
                                    };

                                listResult.Add(result);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return listResult;
                }
            }
        }
        ////
        /// Procedimiento almacenado para cargar los parmetros de activo, es decir, para decidir si sera una revaluacion o mejora.
        /// 
        public List<BEactactivos> Met_ObtenerParamActivo()
        {

            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var list = new List<BEactactivos>();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_cw_Sel_CargarActparamet, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read()
                                )
                            {
                                BEactactivos result = new BEactactivos()
                                {
                                    ctranreval = reader["ctranreval"].ToString().Trim(),
                                    ctranmejor = reader["ctranmejor"].ToString().Trim(),
                                    ctipasiact = reader["ctipasiact"].ToString().Trim(),
                                    ctipasirev = reader["ctipasirev"].ToString().Trim(),
                                    ctipasimej = reader["ctipasimej"].ToString().Trim()

                                };

                                list.Add(result);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();


                }
                return list;

            }
        }

        /// <summary>
        /// Obtiene los activos con cestactivo IN ('D','V')
        /// </summary>
        /// <returns></returns>
        public List<BEactactivos> Met_wucCargarActivo_Filter(string valorFiltro, string campoFiltro,
            string operadorFiltro, int topFiltro)
        {
            using (var sqlConnection = new SqlConnection(_cadenaConexion))
            {
                var list = new List<BEactactivos>();
                using (var sqlCommand = new SqlCommand(_Usp_cw_Sel_wucBuscarActivo_Filter, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@val", SqlDbType.VarChar, 100).Value = valorFiltro ?? string.Empty;
                    sqlCommand.Parameters.Add("@col", SqlDbType.VarChar, 10).Value = campoFiltro ?? string.Empty;
                    sqlCommand.Parameters.Add("@op", SqlDbType.VarChar, 5).Value = operadorFiltro ?? string.Empty;
                    sqlCommand.Parameters.Add("@top", SqlDbType.Int).Value = topFiltro;

                    try
                    {
                        sqlConnection.Open();
                        var reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var result = new BEactactivos
                                {
                                    cnumactivo = reader["cnumactivo"].ToString().Trim(),
                                    cnomactiv = reader["cnomactiv"].ToString().Trim(),
                                    dfechaacti = DateTime.Parse(reader["dfechaacti"].ToString().Trim()),
                                    nvalorcom = decimal.Parse(reader["nvalorcom"].ToString().Trim()),
                                    nporcdepre = decimal.Parse(reader["nporcdepre"].ToString().Trim()),
                                    ndepreacum = decimal.Parse(reader["ndepreacum"].ToString().Trim()),
                                    nmontomejo = decimal.Parse(reader["nmontomejo"].ToString().Trim()),
                                    nmontoreva = decimal.Parse(reader["nmontoreva"].ToString().Trim()),
                                    nmontdpmej = decimal.Parse(reader["nmontdpmej"].ToString().Trim()),
                                    nmontdprev = decimal.Parse(reader["nmontdprev"].ToString().Trim()),
                                    ctipoactiv = reader["ctipoactiv"].ToString().Trim(),
                                    ctipodepre = reader["ctipodepre"].ToString().Trim(),
                                    nperiovida = decimal.Parse(reader["nperiovida"].ToString().Trim()),
                                    nmontliqui = decimal.Parse(reader["nmontliqui"].ToString().Trim()),
                                    CCODCENTRO = reader["ccodcentro"].ToString().Trim(),
                                };

                                list.Add(result);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return list;

                }
            }
        }




        /// <summary>
        /// Proc. para reporte de depreciacion
        /// </summary>
        /// <param name="cTipAct1"></param>
        /// <param name="cTipAct2"></param>
        /// <param name="cChkMej"></param>
        /// <param name="listAct"></param>
        /// <returns></returns>
        public string Met_ProcesoParaReporteDepreciacion(string cTipAct1, string cTipAct2, string cChkMej, out List<BEactactivos> listAct)
        {
            // Mensaje de excepción
            string cMensajeExc = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activo_ReporteDepreciacion, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        sqlCommand.Parameters.Add("@cTipAct1", SqlDbType.VarChar, 3).Value = cTipAct1;
                        sqlCommand.Parameters.Add("@cTipAct2", SqlDbType.VarChar, 3).Value = cTipAct2;
                        sqlCommand.Parameters.Add("@cChkMej", SqlDbType.VarChar).Value = cChkMej;


                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);

                        sqlConnection.Open();
                        DataSet dsData = new DataSet();
                        List<BEactactivos> res = new List<BEactactivos>();

                        try
                        {

                            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                            da.Fill(dsData, "tmp");
                            if (dsData.Tables != null && dsData.Tables.Count > 0)
                            {
                                var cantTablasretornadas = dsData.Tables.Count;

                                DataTable tablaResultados = dsData.Tables[cantTablasretornadas - 1];

                                foreach (DataRow rowRes in tablaResultados.Rows)
                                {
                                    BEactactivos b = new BEactactivos();

                                    b.cnumactivo = rowRes["cnumactivo"] != null ? rowRes["cnumactivo"].ToString().Trim() : "";
                                    b.cnomactiv = rowRes["cnomactiv"] != null ? rowRes["cnomactiv"].ToString().Trim() : "";

                                    res.Add(b);
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                        sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {
                            listAct = res;
                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        throw ex;
                    }
                }
            }

        }

        /// <summary>
        /// Proc. almacenado que obtiene los centros de costo de activos fijos.
        /// </summary>
        /// <returns></returns>
        public List<BECoCentcDep> Met_ObtenerCentrosCosto()
        {
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var list = new List<BECoCentcDep>();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_VR_modulos_ccostoxdeptoActivosFijos, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                BECoCentcDep result = new BECoCentcDep()
                                {
                                    CCodCentro = reader["ccodcentro"].ToString().Trim(),
                                    CDesCentro = reader["cdescentro"].ToString().Trim(),

                                };

                                list.Add(result);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();


                }
                return list;

            }
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
        public string Met_ProcesoParaReporteMovimDepreciacion(string cfecha1, string cfecha2, string cTipAct1, string cTipAct2, string cUbica1, string cUbica2, string cNumAct1, string cNumAct2, string cMarca, string cSerie, string cModelo, out List<BEactactivos> listRes)
        {
            // Mensaje de excepción
            string cMensajeExc = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activo_ReporteMovimientoDepreciacion, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        sqlCommand.Parameters.Add("@cfecha1", SqlDbType.VarChar, 20).Value = cfecha1;
                        sqlCommand.Parameters.Add("@cfecha2", SqlDbType.VarChar, 20).Value = cfecha2;
                        sqlCommand.Parameters.Add("@cTipAct1", SqlDbType.VarChar, 3).Value = cTipAct1;
                        sqlCommand.Parameters.Add("@cTipAct2", SqlDbType.VarChar).Value = cTipAct2;
                        sqlCommand.Parameters.Add("@cUbica1", SqlDbType.VarChar).Value = cUbica1;
                        sqlCommand.Parameters.Add("@cUbica2", SqlDbType.VarChar).Value = cUbica2;
                        sqlCommand.Parameters.Add("@cNumAct1", SqlDbType.VarChar).Value = cNumAct1;
                        sqlCommand.Parameters.Add("@cNumAct2", SqlDbType.VarChar).Value = cNumAct2;
                        sqlCommand.Parameters.Add("@cMarca", SqlDbType.VarChar).Value = cMarca;
                        sqlCommand.Parameters.Add("@cSerie", SqlDbType.VarChar).Value = cSerie;
                        sqlCommand.Parameters.Add("@cModelo", SqlDbType.VarChar).Value = cModelo;




                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);

                        sqlConnection.Open();
                        DataSet dsData = new DataSet();
                        List<BEactactivos> res = new List<BEactactivos>();

                        try
                        {

                            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                            da.Fill(dsData, "tmp");
                            if (dsData.Tables != null && dsData.Tables.Count > 0)
                            {
                                var cantTablasretornadas = dsData.Tables.Count;

                                DataTable tablaResultados = dsData.Tables[cantTablasretornadas - 1];

                                foreach (DataRow rowRes in tablaResultados.Rows)
                                {
                                    BEactactivos b = new BEactactivos();

                                    b.cnumactivo = rowRes["cnumactivo"] != null ? rowRes["cnumactivo"].ToString().Trim() : "";
                                    b.cnomactiv = rowRes["cnomactiv"] != null ? rowRes["cnomactiv"].ToString().Trim() : "";

                                    res.Add(b);
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                        sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {
                            listRes = res;
                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        throw ex;
                    }
                }
            }

        }

        /// <summary>
        /// Obtiene el Activo por cnumactivo
        /// </summary>
        /// <returns></returns>
        public BEactactivos Met_ObtenerActivoLiquid(string cnumactivo, string ventgast, ref string tipasi_ctipasient, ref string tipasi_nconsecuti)
        {
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                var list = new BEactactivos();
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Sel_Activos_DatosAct, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@form_cnumactivo", SqlDbType.VarChar, 6).Value = cnumactivo;
                    sqlCommand.Parameters.Add("@form_ventgast", SqlDbType.Int).Value = Convert.ToInt32(ventgast);
                    sqlCommand.Parameters.Add("@nIdLogExcAG", SqlDbType.Int).Direction = ParameterDirection.Output;
                    sqlCommand.Parameters.Add("@cMensajeExc", SqlDbType.VarChar, 2000).Direction = ParameterDirection.Output;
                    sqlCommand.Parameters.Add("@tipasi_ctipasient", SqlDbType.VarChar, 3).Direction = ParameterDirection.Output;
                    sqlCommand.Parameters.Add("@tipasi_nconsecuti", SqlDbType.Int).Direction = ParameterDirection.Output;

                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                //tipasi_ctipasient = reader["@tipasi_ctipasient"].ToString().Trim();
                                var result = new BEactactivos
                                {
                                    cnomactiv = reader["cnomactiv"].ToString().Trim(),
                                    dfechaacti = Convert.ToDateTime(reader["dfechaacti"].ToString().Trim()),
                                    nvalorcom = Convert.ToDecimal(reader["nvalorcom"].ToString().Trim()),
                                    nporcdepre = Convert.ToDecimal(reader["nporcdepre"].ToString().Trim()),
                                    ndepreacum = Convert.ToDecimal(reader["ndepreacum"].ToString().Trim()),
                                    nmontomejo = Convert.ToDecimal(reader["nmontomejo"].ToString().Trim()),
                                    nmontoreva = Convert.ToDecimal(reader["nmontoreva"].ToString().Trim()),
                                    nmontdpmej = Convert.ToDecimal(reader["nmontdpmej"].ToString().Trim()),
                                    nmontdprev = Convert.ToDecimal(reader["nmontdprev"].ToString().Trim()),
                                    ctipoactiv = reader["ctipoactiv"].ToString().Trim(),
                                    ctipodepre = reader["ctipodepre"].ToString().Trim(),
                                    nperiovida = Convert.ToDecimal(reader["nperiovida"].ToString().Trim()),
                                    nmontdesva = Convert.ToDecimal(reader["nmontdesva"].ToString().Trim()),
                                    nmontliqui = Convert.ToDecimal(reader["nmontliqui"].ToString().Trim()),
                                    nvalorcomi = Convert.ToDecimal(reader["nvalorcomi"].ToString().Trim())
                                };
                                list = result;

                            }
                        }
                        sqlConnection.Close();
                        sqlConnection.Open();
                        //Ver como sobreescribo esto para  no ejecutar la consulta dos veces
                        sqlCommand.ExecuteNonQuery();
                        tipasi_ctipasient = string.Empty;
                        tipasi_nconsecuti = string.Empty;
                        tipasi_ctipasient = sqlCommand.Parameters["@tipasi_ctipasient"].Value.ToString();
                        tipasi_nconsecuti = sqlCommand.Parameters["@tipasi_nconsecuti"].Value.ToString();
                    }
                    catch
                    {
                        throw;
                    }
                    sqlConnection.Close();
                    return list;

                }
            };

        }


        /// <summary>
        /// Proc. para  proceso de renumerar activos
        /// </summary>
        /// <param name="cNumActNew"></param>
        /// <param name="cNumActOld"></param>
        /// <returns></returns>
        public string Met_ProcesoRenumerarActivos(string cNumActNew, string cNumActOld)
        {
            // Mensaje de excepción
            string cMensajeExc = "";
            int resulIdExcep = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._cadenaConexion))
            {
                using (SqlCommand sqlCommand = new SqlCommand(_Usp_CW_Upd_Activos_RenumeracionActivos, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        sqlCommand.Parameters.Add("@cnumeOld", SqlDbType.VarChar, 6).Value = cNumActOld;
                        sqlCommand.Parameters.Add("@cnumeNew", SqlDbType.VarChar, 6).Value = cNumActNew;



                        // Parámetros para recuperar excepciones
                        // Parámetro que contendrá el mensaje de la excepción
                        SqlParameter sqlParCMensajeExc = new SqlParameter("@cMensajeExc", SqlDbType.VarChar, 2000);
                        sqlParCMensajeExc.Direction = ParameterDirection.Output;
                        sqlParCMensajeExc.Value = String.Empty;
                        sqlCommand.Parameters.Add(sqlParCMensajeExc);
                        // Parámetro que contendrá el ID generado de la excepción

                        SqlParameter sqlParNIdLogExcAG = new SqlParameter("@nIdLogExcAG", 0);
                        sqlParNIdLogExcAG.Direction = ParameterDirection.Output;
                        sqlParNIdLogExcAG.Value = 0;
                        sqlCommand.Parameters.Add(sqlParNIdLogExcAG);

                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();

                        if (int.TryParse(sqlCommand.Parameters["@nIdLogExcAG"].Value.ToString(), out resulIdExcep) && resulIdExcep != 0)
                        {
                            cMensajeExc = sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                            throw new GenericException(resulIdExcep, cMensajeExc);

                        }
                        else
                        {

                            return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MethodBase fuente = MethodBase.GetCurrentMethod();
                        LogFile.RegistraLog(ex.ToString().Trim(), fuente);

                        return sqlCommand.Parameters["@cMensajeExc"].Value.ToString();
                    }
                }
            }

        }


        #endregion


    }
}
