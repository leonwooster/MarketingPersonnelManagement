-- Marketing Personnel Management System
-- Database Schema Creation Script
-- SQL Server 2016+

USE [Marketing];
GO

-- Drop tables if they exist (for clean re-creation)
IF OBJECT_ID('dbo.Sales', 'U') IS NOT NULL
    DROP TABLE dbo.Sales;
GO

IF OBJECT_ID('dbo.Personnel', 'U') IS NOT NULL
    DROP TABLE dbo.Personnel;
GO

IF OBJECT_ID('dbo.CommissionProfile', 'U') IS NOT NULL
    DROP TABLE dbo.CommissionProfile;
GO

-- Create CommissionProfile table
CREATE TABLE dbo.CommissionProfile (
    Id int IDENTITY(1,1) NOT NULL,
    profile_name int NOT NULL,
    commission_fixed decimal(10,2) NOT NULL DEFAULT 0.00,
    commission_percentage decimal(10,6) NOT NULL DEFAULT 0.000000,
    CONSTRAINT PK_CommissionProfile PRIMARY KEY (Id),
    CONSTRAINT CK_CommissionProfile_Fixed CHECK (commission_fixed >= 0),
    CONSTRAINT CK_CommissionProfile_Percentage CHECK (commission_percentage >= 0 AND commission_percentage <= 1)
);
GO

-- Create Personnel table
CREATE TABLE dbo.Personnel (
    Id int IDENTITY(1,1) NOT NULL,
    name varchar(50) NOT NULL,
    age int NOT NULL,
    phone varchar(20) NOT NULL,
    commission_profile_id int NOT NULL,
    bank_name varchar(20) NULL,
    bank_account_no varchar(20) NULL,
    CONSTRAINT PK_Personnel PRIMARY KEY (Id),
    CONSTRAINT FK_Personnel_CommissionProfile FOREIGN KEY (commission_profile_id) 
        REFERENCES dbo.CommissionProfile(Id),
    CONSTRAINT CK_Personnel_Age CHECK (age >= 19),
    CONSTRAINT CK_Personnel_Name CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT CK_Personnel_Phone CHECK (LEN(LTRIM(RTRIM(phone))) > 0)
);
GO

-- Create Sales table
CREATE TABLE dbo.Sales (
    Id int IDENTITY(1,1) NOT NULL,
    personnel_id int NOT NULL,
    report_date datetime NOT NULL,
    sales_amount decimal(10,2) NOT NULL,
    CONSTRAINT PK_Sales PRIMARY KEY (Id),
    CONSTRAINT FK_Sales_Personnel FOREIGN KEY (personnel_id) 
        REFERENCES dbo.Personnel(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Sales_Amount CHECK (sales_amount >= 0),
    CONSTRAINT CK_Sales_Date CHECK (report_date <= GETDATE())
);
GO

-- Create indexes for performance
CREATE INDEX IX_Sales_PersonnelId ON dbo.Sales(personnel_id);
GO

CREATE INDEX IX_Sales_ReportDate ON dbo.Sales(report_date);
GO

CREATE INDEX IX_Personnel_CommissionProfileId ON dbo.Personnel(commission_profile_id);
GO

PRINT 'Database schema created successfully.';