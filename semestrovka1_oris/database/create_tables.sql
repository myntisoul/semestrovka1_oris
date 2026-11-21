USE [OrisDb];
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TurnOrder INT NOT NULL CONSTRAINT CK_Roles_TurnOrder CHECK (TurnOrder > 0),
        Name NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        ImageURL NVARCHAR(255) NOT NULL
    );
END;
GO

-- Обновление существующей таблицы с сохранением заданной очерёдности.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH(N'dbo.Roles', N'TurnOrder') IS NULL
    ALTER TABLE dbo.Roles ADD TurnOrder INT NULL;

EXEC (N'
    UPDATE dbo.Roles
    SET TurnOrder = Id
    WHERE TurnOrder IS NULL OR TurnOrder <= 0;

    IF COLUMNPROPERTY(OBJECT_ID(N''dbo.Roles''), N''TurnOrder'', ''AllowsNull'') = 1
        ALTER TABLE dbo.Roles ALTER COLUMN TurnOrder INT NOT NULL;

    IF OBJECT_ID(N''dbo.CK_Roles_TurnOrder'', N''C'') IS NULL
        ALTER TABLE dbo.Roles ADD CONSTRAINT CK_Roles_TurnOrder CHECK (TurnOrder > 0);
');

COMMIT TRANSACTION;
GO

IF OBJECT_ID(N'dbo.Districts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Districts
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        Cost INT NOT NULL,
        Quantity INT NOT NULL,
        Color NVARCHAR(30) NOT NULL,
        ImageURL NVARCHAR(255) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.UsersWiki', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsersWiki
    (
        Id INT IDENTITY(1, 1) PRIMARY KEY,
        Login NVARCHAR(100) NOT NULL,
        Password NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL
    );
END;
GO

USE OrisDb;
GO

IF OBJECT_ID(N'dbo.GameRules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GameRules
    (
        Id INT IDENTITY(1, 1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Category NVARCHAR(100) NOT NULL
    );
END;
GO

