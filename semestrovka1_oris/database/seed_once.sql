SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO

:r seed_data.sql

EXEC sys.sp_addextendedproperty @name = N'OrisSeedV1', @value = 1;
COMMIT TRANSACTION;
GO
