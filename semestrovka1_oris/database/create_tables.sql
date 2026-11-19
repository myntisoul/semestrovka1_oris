USE [OrisDb];
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        ImageURL NVARCHAR(255) NOT NULL
    );
END;
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

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
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

select * from Users;