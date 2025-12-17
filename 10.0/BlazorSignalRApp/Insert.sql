DELETE [SignalR-Prototype].[dbo].[TheData]

SELECT * FROM [SignalR-Prototype].[dbo].[TheData]

INSERT INTO [SignalR-Prototype].[dbo].[TheData] ([Value]) VALUES('333')

--=======================================================================================================================================
USE master;
GO
ALTER DATABASE [SignalR-Prototype]
SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;
GO
-----------------------------------------------------------------------------------------------------------------------------------------
SELECT is_broker_enabled
FROM sys.databases
WHERE name = 'SignalR-Prototype';
