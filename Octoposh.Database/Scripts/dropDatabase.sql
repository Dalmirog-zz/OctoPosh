use master
go

DECLARE @Database varchar(30) = '$Database$';  

IF EXISTS(SELECT name from sys.databases
	WHERE name = @Database)
	PRINT 'Dropping database'
	EXEC ('DROP DATABASE ' + @Database);
GO