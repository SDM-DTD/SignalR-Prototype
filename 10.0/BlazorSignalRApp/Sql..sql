DELETE [SignalR-Prototype].[dbo].[TheData]

SELECT top 1 * FROM [SignalR-Prototype].[dbo].[TheData] order by id desc

SELECT * FROM [SignalR-Prototype].[dbo].[TheData]


INSERT INTO [SignalR-Prototype].[dbo].[TheData] ([Value], [RunId]) VALUES('Happy', 999)
GO
WAITFOR DELAY '00:00:01';
INSERT INTO [SignalR-Prototype].[dbo].[TheData] ([Value], [RunId]) VALUES('Days', 77)
GO
WAITFOR DELAY '00:00:01';
INSERT INTO [SignalR-Prototype].[dbo].[TheData] ([Value], [RunId]) VALUES('are here.', 7)
GO


SELECT MAX([Id]) FROM dbo.TheData


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
