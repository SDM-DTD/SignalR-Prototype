DELETE [SignalR-Prototype].[dbo].[TheData]

SELECT top 1 * FROM [SignalR-Prototype].[dbo].[TheData] order by id desc

INSERT INTO [SignalR-Prototype].[dbo].[TheData] ([Value]) VALUES('Ex-Cellent')





SELECT TOP 1 [id], [Value] FROM [SignalR-Prototype].[dbo].[TheData] WHERE [id]=242 ORDER BY [Id] DESC

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
