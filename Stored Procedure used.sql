USE [databasename]
GO

/****** Object:  Table [dbo].[Invoice]    Script Date: 1/30/2023 2:46:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Invoice](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Invoice_ID] [nvarchar](20) NOT NULL,
	[Date_Served] [datetime] NOT NULL,
	[Total_Billable] [money] NOT NULL,
	[Total_Taxable] [money] NOT NULL,
	[Served_By] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


GO

/****** Object:  Table [dbo].[InvoiceHistory]    Script Date: 1/30/2023 2:48:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceHistory](
	[Invoice_ID] [nvarchar](20) NOT NULL,
	[Date_Served] [datetime] NOT NULL,
	[Total_Billable] [money] NOT NULL,
	[Total_Taxable] [money] NOT NULL,
	[Served_By] [nvarchar](250) NOT NULL
) ON [PRIMARY]
GO


GO
/****** Object:  StoredProcedure [dbo].[InsterttoInvoice]    Script Date: 1/24/2023 5:39:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Mwita Joseph
-- Create date: Tuesday 24 2023 1227hrs
-- Description:	Create a sample procedure to insert to/update sample invoice table
-- =============================================
CREATE PROCEDURE [dbo].[InsterttoInvoice] -- 'inv-001', 320, 280, 'Alexandar Joseph'
	-- Add the parameters for the stored procedure here
		@Invoice_ID nvarchar(20)
        ,@Total_Billable money
        ,@Total_Taxable money
		,@Served_By nvarchar(250)
AS
BEGIN -- Start of procedure
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY -- Before any transaction we enclose it in a catch statement
		
		BEGIN TRANSACTION -- Start of our transaction

		IF (NOT (EXISTS(SELECT TOP 1 * FROM Invoice WHERE Invoice.Invoice_ID = @Invoice_ID)))
			BEGIN -- Start of an if statement
				INSERT INTO [Invoice]
					   ([Invoice_ID]
					   ,[Date_Served]
					   ,[Total_Billable]
					   ,[Total_Taxable]
					   ,[Served_By])
				 VALUES
					   (@Invoice_ID
					   ,GetDate()
					   ,@Total_Billable
					   ,@Total_Taxable
					   ,@Served_By)
			END -- End of an if statement
		ELSE -- Perform an update
			BEGIN
				INSERT INTO [InvoiceHistory] -- Create history before updating Original table
					   ([Invoice_ID]
					   ,[Date_Served]
					   ,[Total_Billable]
					   ,[Total_Taxable]
					   ,[Served_By])
				SELECT Invoice_ID, Date_Served, Total_Billable, Total_Taxable, Served_By FROM invoice WHERE Invoice.Invoice_ID = @Invoice_ID;
				UPDATE [dbo].[Invoice]
				   SET
					  [Total_Billable] = @Total_Billable
					  ,[Total_Taxable] = @Total_Taxable
					  ,[Served_By] = @Served_By
				 WHERE Invoice.Invoice_ID = @Invoice_ID;
				
			END -- end of else 


		COMMIT TRANSACTION -- End of our transaction
	END TRY

	BEGIN CATCH --Rollback transaction in case of error
		ROLLBACK TRANSACTION 
		declare @ErrorMessage nvarchar(500)        
		declare @ErrorSeverity nvarchar(50)        
		declare @ErrorState nvarchar(50)        
		SET @ErrorMessage  = ERROR_MESSAGE()        
		SET @ErrorSeverity = ERROR_SEVERITY()        
		SET @ErrorState    = ERROR_STATE()        
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)   
	END CATCH

    
END --End of InsterttoInvoice procedure


GO
/****** Object:  StoredProcedure [dbo].[InvoiceByID]    Script Date: 1/30/2023 2:49:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Joseph
-- Create date: Wen 25 2023 0824hrs
-- Description:	A procedure to select an invoice by invoice ID
-- =============================================
CREATE PROCEDURE [dbo].[InvoiceByID]
	-- Add the parameters for the stored procedure here
	@InvoiceID nvarchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Invoice WHERE Invoice.Invoice_ID = @InvoiceID;
END

GO
/****** Object:  StoredProcedure [dbo].[Delete_Invoice]    Script Date: 1/30/2023 2:49:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Mwita Joseph
-- Create date: Tuesday 24 2023 1227hrs
-- Description:	Create a sample procedure to insert to/update sample invoice table
-- =============================================
CREATE PROCEDURE [dbo].[Delete_Invoice] -- 'inv-001'
	-- Add the parameters for the stored procedure here
		@Invoice_ID nvarchar(20)

AS
BEGIN -- Start of procedure
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @return_String varchar(300); -- Holds a return value
	DECLARE @affected_queries int;

	BEGIN TRY -- Before any transaction we enclose it in a catch statement
		
		BEGIN TRANSACTION -- Start of our transaction


		--Only delete when an invoice exists
		IF (EXISTS(SELECT TOP 1 * FROM Invoice WHERE Invoice.Invoice_ID = @Invoice_ID))
		BEGIN
			-- Start of a delete statement
			DELETE FROM Invoice WHERE Invoice.Invoice_ID = @Invoice_ID

			SET @return_String = @Invoice_ID + ' Deleted was successful';
			
		END
		ELSE 
		BEGIN
			SET @return_String = 'Invoice no. ' + @Invoice_ID + ' Does not exist';
		END
			
		COMMIT TRANSACTION -- End of our transaction
	END TRY

	BEGIN CATCH --Rollback transaction in case of error
		ROLLBACK TRANSACTION 
		declare @ErrorMessage nvarchar(500)        
		declare @ErrorSeverity nvarchar(50)        
		declare @ErrorState nvarchar(50)        
		SET @ErrorMessage  = ERROR_MESSAGE()        
		SET @ErrorSeverity = ERROR_SEVERITY()        
		SET @ErrorState    = ERROR_STATE()        
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)   
	END CATCH
	
	SELECT @return_String;
    
END --End of procedure


