IF EXISTS (SELECT name FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Usp_CW_Sel_Tesoreria_Proceso_AjustePago_met_det_recibo_aju]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
                DROP PROCEDURE [dbo].[Usp_CW_Sel_Tesoreria_Proceso_AjustePago_met_det_recibo_aju]
GO



IF NOT EXISTS (SELECT name FROM sys.types WHERE is_table_type = 1 AND name = 'cur_tedetatran')
	CREATE TYPE cur_tedetatran AS TABLE(   id INT ,--IDENTITY(1,1) Primary Key,
									 cdetipotra varchar (100), cdetransac varchar(100),cdocurefer varchar (25),nmontotran numeric (15,2),
									 namortizac numeric (15,2),	 nintereses numeric(15,2), nmoratorio numeric(15,2),ccuentacon varchar(35),
									 ccodprovee varchar(4), ctipotrans varchar(1), cidtransac varchar(6), cobservaci varchar(250),
									 cutiajupag varchar (1) default 'N',cllave varchar(25)
									
									)

GO
 
 IF NOT EXISTS (SELECT name FROM sys.types WHERE is_table_type = 1 AND name = 'cur_tranaju')
	CREATE TYPE cur_tranaju AS TABLE(   id INT ,--IDENTITY(1,1) Primary Key,
									ccodigocaj varchar (4), cidconsecu varchar(4), nnumrecibo numeric(18,0), cidtransac varchar(6)
									
									)

GO

---- ===================================================================================== ----
-- Este script debe ser ejecutado en la base de datos del cliente
---- ===================================================================================== ----
-- Módulo				: Tesoreria
-- Sub-módulo			: Procesos
-- Opción				: Anulacion De recibos
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

CREATE PROCEDURE [dbo].[Usp_CW_Sel_Tesoreria_Proceso_AjustePago_met_det_recibo_aju]
(

@pctipasient varchar (3),
@pcnumasient varchar (30),
@pcdecliente varchar (80),
@pcidconsecu varchar (4) ,
@pnnumereci numeric (18,2),
@pccodigocaj varchar (4),
@pdfechatran datetime,
@vpbc_codusuario varchar (30),
@pnmontotran numeric (18,2),
@pcnota varchar (1000),
@pcidcliente varchar (10),
@pctiprecibo varchar (3),
@pccodmoneda varchar (3),
@ccuentacaj varchar (35),
@ntipocambi float,
@cur_tedetatran dbo.cur_tedetatran READONLY,
@cur_tranaju dbo.cur_tranaju READONLY,
@cidtransac varchar (6) output,
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
	SELECT @TranName = 'Usp_CW_Sel_Tesoreria_Proceso_AjustePago_met_det_recibo_aju'

	
	DECLARE @open_tran int = @@TRANCOUNT

	
		BEGIN TRY
		IF @open_tran > 0
			SAVE TRANSACTION @TranName
		ELSE
			BEGIN TRANSACTION @TranName
	BEGIN	

	---------Met Proceso----------------
	BEGIN
	  DECLARE @i int
	  SET @i=1

	  DECLARE @ctipotrans varchar (1), @cdocurefer varchar (35),  @nmontotran numeric(18,2)

	  WHILE EXISTS(SELECT * FROM @cur_tedetatran WHERE id= @i)
	   BEGIN
	  	  
  		 
	     SELECT @ctipotrans=ctipotrans,@cdocurefer=cdocurefer, @cidtransac=cidtransac,@nmontotran=nmontotran
		 FROM @cur_tedetatran WHERE id=@i
		 
		 IF @ctipotrans='C'
		  BEGIN
		   ---------------------------------------tran_cxc----------------------------------------------------------------------------------------------
		    EXEC Usp_CW_Sel_Tesoreria_Proceso_AjustePago_met_tran_cxc_aju @ccuentacaj, @pcidcliente, @pctipasient, @pcnumasient, @pccodigocaj,@pdfechatran,
			                                       @pnnumereci, @pcidconsecu, @vpbc_codusuario, @i, @cur_tedetatran,@cur_tranaju,
												   @cidtransac output,@nIdLogExcAG output, @cMensajeExc output
			  
			  IF @nIdLogExcAG=-1
			   BEGIN
			    SET @vlci_errorState = 1 																		
			    SET @vlci_errorSeverity = 16																	
			    SET @nIdLogExcAG=-1					
			    RAISERROR (@cMensajeExc, @vlci_errorSeverity, @vlci_errorState)
				
			   END
		  
		   ---------------------------------------FIN tran_cxc---------------------------------------------------------------------------------------------------
		  END
		  ELSE
		    BEGIN
			 IF @ctipotrans='E'
			  BEGIN
			      ---------------------------------------tran_ext----------------------------------------------------------------------------------------------
		    EXEC Usp_CW_Sel_Tesoreria_met_tran_ext @ccuentacaj, @pcidcliente, @pctipasient, @pcnumasient, @pccodigocaj, @pdfechatran,
			                                       @pnnumereci, @pcidconsecu, @vpbc_codusuario,@i, @pccodmoneda, @ntipocambi,
												   @pctiprecibo, @cur_tedetatran, @nIdLogExcAG output, @cMensajeExc output

			  IF @nIdLogExcAG=-1
			   BEGIN
			    SET @vlci_errorState = 1 																		
				SET @vlci_errorSeverity = 16																	
				SET @nIdLogExcAG=-1	
				RAISERROR (@cMensajeExc, @vlci_errorSeverity, @vlci_errorState)
			   END
		  
		   ---------------------------------------FIN tran_ext---------------------------------------------------------------------------------------------------
			  END
			END

		

	     




	    SET @i = @i+1
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
    EXEC Usp_Ins_General_IngresaDML_Filter '[dbo].[Usp_CW_Sel_Tesoreria_Proceso_AjustePago_met_det_recibo_aju]', 'Deivis', '2017/06/27', '', '', 
		'Procedimiento almacenado para ejecutar el procedimiento anular recibos',20
GO


