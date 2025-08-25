-- Marketing Personnel Management Database Schema
-- Company A - Production Database Setup

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MarketingPersonnelDB')
BEGIN
    CREATE DATABASE MarketingPersonnelDB;
END
GO

USE MarketingPersonnelDB;
GO

-- Create CommissionProfile table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CommissionProfile' AND xtype='U')
BEGIN
    CREATE TABLE CommissionProfile (
        Id int IDENTITY(1,1) PRIMARY KEY,
        ProfileName int NOT NULL UNIQUE,
        CommissionFixed decimal(10,2) NOT NULL CHECK (CommissionFixed >= 0),
        CommissionPercentage decimal(10,6) NOT NULL CHECK (CommissionPercentage >= 0 AND CommissionPercentage <= 1),
        CreatedDate datetime2 DEFAULT GETDATE(),
        UpdatedDate datetime2 DEFAULT GETDATE()
    );
    
    PRINT 'CommissionProfile table created successfully.';
END
ELSE
BEGIN
    PRINT 'CommissionProfile table already exists.';
END
GO

-- Create Personnel table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Personnel' AND xtype='U')
BEGIN
    CREATE TABLE Personnel (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(50) NOT NULL,
        Age int NOT NULL CHECK (Age >= 19),
        Phone nvarchar(20) NOT NULL,
        BankName nvarchar(20) NULL,
        BankAccountNo nvarchar(20) NULL,
        CommissionProfileId int NOT NULL,
        CreatedDate datetime2 DEFAULT GETDATE(),
        UpdatedDate datetime2 DEFAULT GETDATE(),
        FOREIGN KEY (CommissionProfileId) REFERENCES CommissionProfile(Id)
    );
    
    PRINT 'Personnel table created successfully.';
END
ELSE
BEGIN
    PRINT 'Personnel table already exists.';
END
GO

-- Create Sales table with CASCADE DELETE
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sales' AND xtype='U')
BEGIN
    CREATE TABLE Sales (
        Id int IDENTITY(1,1) PRIMARY KEY,
        PersonnelId int NOT NULL,
        ReportDate date NOT NULL CHECK (ReportDate <= CAST(GETDATE() AS date)),
        SalesAmount decimal(10,2) NOT NULL CHECK (SalesAmount >= 0),
        CreatedDate datetime2 DEFAULT GETDATE(),
        FOREIGN KEY (PersonnelId) REFERENCES Personnel(Id) ON DELETE CASCADE
    );
    
    PRINT 'Sales table created successfully.';
END
ELSE
BEGIN
    PRINT 'Sales table already exists.';
END
GO

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Personnel_CommissionProfileId')
BEGIN
    CREATE INDEX IX_Personnel_CommissionProfileId ON Personnel(CommissionProfileId);
    PRINT 'Index IX_Personnel_CommissionProfileId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_PersonnelId')
BEGIN
    CREATE INDEX IX_Sales_PersonnelId ON Sales(PersonnelId);
    PRINT 'Index IX_Sales_PersonnelId created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_ReportDate')
BEGIN
    CREATE INDEX IX_Sales_ReportDate ON Sales(ReportDate);
    PRINT 'Index IX_Sales_ReportDate created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_PersonnelId_ReportDate')
BEGIN
    CREATE INDEX IX_Sales_PersonnelId_ReportDate ON Sales(PersonnelId, ReportDate);
    PRINT 'Index IX_Sales_PersonnelId_ReportDate created.';
END
GO

PRINT 'Database schema creation completed successfully!';
GO
