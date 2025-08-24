-- Marketing Personnel Management System
-- Seed Data Script
-- SQL Server 2016+

USE [Marketing];
GO

-- Insert Commission Profiles
INSERT INTO dbo.CommissionProfile (profile_name, commission_fixed, commission_percentage) VALUES
(1, 500.00, 0.050000),  -- Profile 1: $500 fixed + 5%
(2, 750.00, 0.030000),  -- Profile 2: $750 fixed + 3%
(3, 300.00, 0.080000);  -- Profile 3: $300 fixed + 8%
GO

-- Insert Personnel
INSERT INTO dbo.Personnel (name, age, phone, commission_profile_id, bank_name, bank_account_no) VALUES
('John Smith', 25, '555-0101', 1, 'Chase Bank', '1234567890'),
('Sarah Johnson', 28, '555-0102', 2, 'Wells Fargo', '2345678901'),
('Michael Brown', 32, '555-0103', 1, 'Bank of America', '3456789012'),
('Emily Davis', 24, '555-0104', 3, 'Citibank', '4567890123'),
('David Wilson', 29, '555-0105', 2, 'TD Bank', '5678901234'),
('Lisa Anderson', 26, '555-0106', 1, 'PNC Bank', '6789012345'),
('Robert Taylor', 31, '555-0107', 3, 'US Bank', '7890123456'),
('Jennifer Martinez', 27, '555-0108', 2, 'Capital One', '8901234567'),
('Christopher Lee', 30, '555-0109', 1, NULL, NULL),
('Amanda White', 23, '555-0110', 3, 'HSBC', '0123456789');
GO

-- Insert Sales data (5-10 sales records per person for recent months)
DECLARE @PersonnelId int;
DECLARE @BaseDate datetime = DATEADD(month, -2, GETDATE());

-- Sales for John Smith (ID: 1)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(1, DATEADD(day, -45, GETDATE()), 1250.00),
(1, DATEADD(day, -38, GETDATE()), 980.50),
(1, DATEADD(day, -32, GETDATE()), 1450.75),
(1, DATEADD(day, -25, GETDATE()), 875.25),
(1, DATEADD(day, -18, GETDATE()), 1650.00),
(1, DATEADD(day, -12, GETDATE()), 1125.50),
(1, DATEADD(day, -5, GETDATE()), 1380.25);

-- Sales for Sarah Johnson (ID: 2)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(2, DATEADD(day, -42, GETDATE()), 2150.00),
(2, DATEADD(day, -35, GETDATE()), 1875.50),
(2, DATEADD(day, -28, GETDATE()), 2250.75),
(2, DATEADD(day, -21, GETDATE()), 1950.25),
(2, DATEADD(day, -14, GETDATE()), 2450.00),
(2, DATEADD(day, -7, GETDATE()), 2125.50),
(2, DATEADD(day, -2, GETDATE()), 2380.25);

-- Sales for Michael Brown (ID: 3)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(3, DATEADD(day, -40, GETDATE()), 950.00),
(3, DATEADD(day, -33, GETDATE()), 1150.50),
(3, DATEADD(day, -26, GETDATE()), 850.75),
(3, DATEADD(day, -19, GETDATE()), 1250.25),
(3, DATEADD(day, -13, GETDATE()), 1050.00),
(3, DATEADD(day, -6, GETDATE()), 925.50);

-- Sales for Emily Davis (ID: 4)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(4, DATEADD(day, -44, GETDATE()), 1750.00),
(4, DATEADD(day, -37, GETDATE()), 1425.50),
(4, DATEADD(day, -30, GETDATE()), 1850.75),
(4, DATEADD(day, -23, GETDATE()), 1650.25),
(4, DATEADD(day, -16, GETDATE()), 1950.00),
(4, DATEADD(day, -9, GETDATE()), 1725.50),
(4, DATEADD(day, -3, GETDATE()), 1580.25),
(4, DATEADD(day, -1, GETDATE()), 1325.00);

-- Sales for David Wilson (ID: 5)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(5, DATEADD(day, -41, GETDATE()), 1350.00),
(5, DATEADD(day, -34, GETDATE()), 1125.50),
(5, DATEADD(day, -27, GETDATE()), 1450.75),
(5, DATEADD(day, -20, GETDATE()), 1250.25),
(5, DATEADD(day, -15, GETDATE()), 1550.00),
(5, DATEADD(day, -8, GETDATE()), 1325.50);

-- Sales for Lisa Anderson (ID: 6)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(6, DATEADD(day, -43, GETDATE()), 875.00),
(6, DATEADD(day, -36, GETDATE()), 1025.50),
(6, DATEADD(day, -29, GETDATE()), 750.75),
(6, DATEADD(day, -22, GETDATE()), 950.25),
(6, DATEADD(day, -17, GETDATE()), 1150.00),
(6, DATEADD(day, -10, GETDATE()), 825.50),
(6, DATEADD(day, -4, GETDATE()), 1075.25);

-- Sales for Robert Taylor (ID: 7)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(7, DATEADD(day, -39, GETDATE()), 2250.00),
(7, DATEADD(day, -31, GETDATE()), 1975.50),
(7, DATEADD(day, -24, GETDATE()), 2150.75),
(7, DATEADD(day, -11, GETDATE()), 2450.25),
(7, DATEADD(day, -7, GETDATE()), 2350.00);

-- Sales for Jennifer Martinez (ID: 8)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(8, DATEADD(day, -46, GETDATE()), 1650.00),
(8, DATEADD(day, -38, GETDATE()), 1425.50),
(8, DATEADD(day, -30, GETDATE()), 1750.75),
(8, DATEADD(day, -22, GETDATE()), 1550.25),
(8, DATEADD(day, -14, GETDATE()), 1850.00),
(8, DATEADD(day, -6, GETDATE()), 1625.50),
(8, DATEADD(day, -1, GETDATE()), 1480.25);

-- Sales for Christopher Lee (ID: 9)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(9, DATEADD(day, -47, GETDATE()), 1125.00),
(9, DATEADD(day, -40, GETDATE()), 985.50),
(9, DATEADD(day, -33, GETDATE()), 1250.75),
(9, DATEADD(day, -26, GETDATE()), 1050.25),
(9, DATEADD(day, -19, GETDATE()), 1350.00),
(9, DATEADD(day, -12, GETDATE()), 1125.50);

-- Sales for Amanda White (ID: 10)
INSERT INTO dbo.Sales (personnel_id, report_date, sales_amount) VALUES
(10, DATEADD(day, -45, GETDATE()), 1950.00),
(10, DATEADD(day, -37, GETDATE()), 1725.50),
(10, DATEADD(day, -29, GETDATE()), 2050.75),
(10, DATEADD(day, -21, GETDATE()), 1850.25),
(10, DATEADD(day, -13, GETDATE()), 2150.00),
(10, DATEADD(day, -5, GETDATE()), 1925.50),
(10, DATEADD(day, -2, GETDATE()), 1780.25),
(10, DATEADD(day, -1, GETDATE()), 1625.00);

GO

PRINT 'Seed data inserted successfully.';
PRINT 'Commission Profiles: 3 records';
PRINT 'Personnel: 10 records';
PRINT 'Sales: 65+ records';
GO