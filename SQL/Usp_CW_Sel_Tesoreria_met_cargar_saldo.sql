IF EXISTS (SELECT name FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Usp_CW_Sel_Tesoreria_met_cargar_saldo]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
                DROP PROCEDURE [dbo].[Usp_CW_Sel_Tesoreria_met_cargar_saldo]
GO



---- ===================================================================================== ----
-- Este script debe ser ejecutado en la base de datos del cliente
---- ===================================================================================== ----
-- Módulo				:  Tesoreria
-- Sub-módulo			: Procesos
-- Opción				: Transaccion de cajas
-- Cliente				: Quarzo
---- ===================================================================================== ----
-- Detalle				: Procedimiento almacenado que anula un recibo.
--
-- Autor.....			: Deivis Leal (@SanPanda)- para Ingenius
-- Creación..			: 
-- Modificado			: 27/06/2017
-- Modificado por		: 
-- Parámetros			: 
---- ===================================================================================== ----

CREATE PROCEDURE [dbo].[Usp_CW_Sel_Tesoreria_met_cargar_saldo]
(
@pccodigocaj varchar (4),

@nIdLogExcAG int output,
@cMensajeExc varchar(2000) output

)		

AS 
BEGIN
DECLARE

--VARIABLES EXCEPCIONES:
	@vlci_errorNumber int, @vlci_errorSeverity int, @vlci_errorState int, @vlcc_errorProcedure varchar(128),
	@vlci_errorLine int, @vlcc_errorMessage varchar(4000), @vlcd_fechaActual datetime, @TranName VARCHAR(200), @cantReg int
	,@ValorRetorno varchar(5)
--VARIABLES LOCALES

  --declare
	  --@vlcc_nconsecuti varchar(200)

	  --ASIGNANDO VALOR POR DEFECTO A LAS VAR parametros

	--ASIGNANDO VALOR POR DEFECTO A LAS VAR EXCEPCIONES
	SET @nIdLogExcAG = 0
	SET @vlci_errorNumber    = 0
	SET @vlci_errorSeverity  = 0
	SET @vlci_errorState     = 0
	SET @vlcc_errorProcedure = ''
	SET @cMensajeExc = ''
	SET @vlci_errorLine      = 0
	SET @cantReg			 = 0
	SET @vlcc_errorMessage   = ''	
	set @vlcd_fechaActual = getdate()
	SET @ValorRetorno = 'False'
	SELECT @TranName = 'Usp_CW_Sel_Tesoreria_met_cargar_saldo'

	
	DECLARE @open_tran int = @@TRANCOUNT

	
		BEGIN TRY
		IF @open_tran > 0
			SAVE TRANSACTION @TranName
		ELSE
			BEGIN TRANSACTION @TranName
	BEGIN	

	---------Met Proceso----------------
	BEGIN
	
		    --*-*Cursor de Formas de Pago
			 IF OBJECT_ID(N'tempdb..#cur_moncaj', N'U') IS NULL  
			 CREATE TABLE #cur_moncaj ( id INT IDENTITY(1,1) Primary Key,
									    ccodmoneda varchar (3)									
									)

       INSERT INTO #cur_moncaj (ccodmoneda)
		SELECT ccodmoneda FROM temonecaja WHERE ccodigocaj = @pccodigocaj

		IF @@ROWCOUNT = 0
		 BEGIN 
			 SET @nIdLogExcAG=-1
			 SET @cMensajeExc='No fue posible obtener las monedas de la caja.'
			 SET @vlci_errorState = 1 																		
			 SET @vlci_errorSeverity = 16	
			 RAISERROR (@cMensajeExc, @vlci_errorSeverity, @vlci_errorState)
		  
		 END

		 DECLARE @i INT, @vlcc_ccodmoneda varchar (3), @vlcc_fecha datetime,@vlcc_nsaldoefec numeric (15,2),
         @vlcc_nsaldodocu numeric (15,2),@vlcc_nmontoinic numeric (15,2)
		 SET @i=1

		 WHILE EXISTS (SELECT * FROM #cur_moncaj WHERE id=@i)
		  BEGIN
		   SELECT @vlcc_ccodmoneda=ccodmoneda  FROM #cur_moncaj WHERE id=@i
		   SET @vlcc_fecha = getdate()
		   SELECT ccodigocaj, ccodmoneda, nsaldoefec, nsaldodocu, nmontoinic, dfechainic FROM tesaldocaj 
           WHERE ccodigocaj = @pccodigocaj and ccodmoneda = @vlcc_ccodmoneda
  
       
		  IF @@ROWCOUNT = 0
		   BEGIN
			--*!* BUSCO EL SALDO DESDE EL ULTIMO CIERRE
			SELECT TOP 1 ccodigocaj, ccodmoneda, dfeccierre, nsaldoefec, nsaldodocu,
			nmontoinic, dfechainic, dfechafina FROM tehsaldoca 
            WHERE ccodigocaj = @pccodigocaj AND ccodmoneda = @vlcc_ccodmoneda ORDER BY dfeccierre DESC
			 
			 IF @@ROWCOUNT > 0
			   BEGIN
			     --*!* SE HA UTILIZADO LA CAJA
				 SELECT TOP 1 @vlcc_nsaldoefec= nsaldoefec, @vlcc_nsaldodocu= nsaldodocu, @vlcc_nmontoinic=	nmontoinic
				 FROM tehsaldoca WHERE ccodigocaj = @pccodigocaj AND ccodmoneda = @vlcc_ccodmoneda ORDER BY dfeccierre DESC
       
     
				INSERT INTO tesaldocaj (ccodigocaj, ccodmoneda, nsaldoefec, nsaldodocu,nmontoinic, dfechainic) 
				VALUES (@pccodigocaj,@vlcc_ccodmoneda,@vlcc_nsaldoefec,@vlcc_nsaldodocu,@vlcc_nmontoinic,@vlcc_fecha)
			   END
			   ELSE
			    BEGIN
					--*!* NUNCA SE HA UTILIZADO LA CAJA
				   INSERT INTO tesaldocaj (ccodigocaj, ccodmoneda, nsaldoefec, nsaldodocu, nmontoinic, dfechainic)
				   VALUES (@pccodigocaj,@vlcc_ccodmoneda,0,0,0,@vlcc_fecha)
				END

		   END
       
      
      

		   SET @i= @i+1
		  END

  

      
		   
	END
	
	
	-------Fin Met Proceso-----------
	
	SET @ValorRetorno = 'True'
	END
	IF @open_tran = 0
	COMMIT TRANSACTION @TranName
	END TRY
	BEGIN CATCH
		IF Xact_state() <> -1 --SE HACE ROLLBACK EN TODO CASO
			ROLLBACK TRANSACTION @TranName
		ELSE IF @open_tran = 0 --SE HACE ROLLBACK SOLO SI ESTOY EN LA TRANSACCION
			ROLLBACK TRANSACTION @TranName
			
		IF Xact_state() <> -1 OR @open_tran = 0 
			EXEC Usp_cw_Ins_RegistExcepcion @vlci_errorNumber, @vlci_errorSeverity, @vlci_errorState,  @vlcc_errorProcedure, 
											@vlci_errorLine,   @vlcc_errorMessage,  @vlcd_fechaActual, @cMensajeExc,     
											@nIdLogExcAG OUT
		
		SET @ValorRetorno = 'False'
		SET @nIdLogExcAG = -1	
		SELECT @vlci_errorNumber = ERROR_NUMBER(),	@vlci_errorSeverity = ERROR_SEVERITY(), @vlci_errorState = ERROR_STATE()
	   ,@vlcc_errorProcedure  = ERROR_PROCEDURE(), @vlci_errorLine = ERROR_LINE(), @vlcc_errorMessage = ERROR_MESSAGE()
	   if(@cMensajeExc is null or @cMensajeExc = '')
	   begin
			SET @cMensajeExc = 'No fue Realizar el proceso.' + @vlcc_errorMessage			
		end
	
	END CATCH  
END--Procedure
GO

-- ===================================================================================== ----
-- Detalle		: Script para actualizar el diccionario DML.
-- Autor		: Victor M. Pelegrino - para Quarzo Innovación
-- Creacion		: 07/02/2017
-- Modificado por : 
-- Modificado   : 
-- ===================================================================================== ----

IF EXISTS (SELECT name FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_Ins_General_IngresaDML_Filter]'))
    EXEC Usp_Ins_General_IngresaDML_Filter '[dbo].[Usp_CW_Sel_Tesoreria_met_cargar_saldo]', 'Deivis', '2017/06/27', '', '', 
		'Procedimiento almacenado para ejecutar el proceso transacciones de cajas', 3
GO


