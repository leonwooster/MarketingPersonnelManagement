-- Marketing Personnel Management Database Seed Data
-- Company A - Initial Data Setup

USE MarketingPersonnelDB;
GO

-- Insert Commission Profiles
IF NOT EXISTS (SELECT 1 FROM CommissionProfile WHERE ProfileName = 1)
BEGIN
    INSERT INTO CommissionProfile (ProfileName, CommissionFixed, CommissionPercentage)
    VALUES 
        (1, 500.00, 0.05),    -- Profile 1: $500 fixed + 5%
        (2, 750.00, 0.03),    -- Profile 2: $750 fixed + 3%
        (3, 300.00, 0.08);    -- Profile 3: $300 fixed + 8%
    
    PRINT 'Commission profiles seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Commission profiles already exist.';
END
GO

-- Insert Sample Personnel (2025 data)
IF NOT EXISTS (SELECT 1 FROM Personnel WHERE Name = 'John Smith')
BEGIN
    INSERT INTO Personnel (Name, Age, Phone, BankName, BankAccountNo, CommissionProfileId)
    VALUES 
        ('John Smith', 28, '555-0101', 'Chase Bank', 'ACC001234567', 1),
        ('Sarah Johnson', 32, '555-0102', 'Bank of America', 'ACC002345678', 2),
        ('Michael Brown', 25, '555-0103', 'Wells Fargo', 'ACC003456789', 1),
        ('Emily Davis', 29, '555-0104', 'Citibank', 'ACC004567890', 3),
        ('David Wilson', 35, '555-0105', 'Chase Bank', 'ACC005678901', 2),
        ('Lisa Anderson', 27, '555-0106', 'Bank of America', 'ACC006789012', 1),
        ('Robert Taylor', 31, '555-0107', 'Wells Fargo', 'ACC007890123', 3),
        ('Jennifer Martinez', 26, '555-0108', 'Citibank', 'ACC008901234', 2);
    
    PRINT 'Personnel seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Personnel already exist.';
END
GO

-- Insert Sample Sales Data for 2025 (Recent months)
IF NOT EXISTS (SELECT 1 FROM Sales WHERE PersonnelId = 1 AND ReportDate = '2025-01-15')
BEGIN
    DECLARE @PersonnelCount int = (SELECT COUNT(*) FROM Personnel);
    
    -- January 2025 Sales
    INSERT INTO Sales (PersonnelId, ReportDate, SalesAmount)
    VALUES 
        (1, '2025-01-15', 2500.00),
        (1, '2025-01-22', 1800.00),
        (2, '2025-01-10', 3200.00),
        (2, '2025-01-25', 2100.00),
        (3, '2025-01-12', 1900.00),
        (3, '2025-01-28', 2400.00),
        (4, '2025-01-08', 2800.00),
        (4, '2025-01-20', 1600.00),
        (5, '2025-01-18', 3500.00),
        (5, '2025-01-30', 2200.00),
        (6, '2025-01-14', 1700.00),
        (6, '2025-01-26', 2300.00),
        (7, '2025-01-16', 2900.00),
        (7, '2025-01-29', 1500.00),
        (8, '2025-01-11', 2600.00),
        (8, '2025-01-24', 1900.00);
    
    -- February 2025 Sales
    INSERT INTO Sales (PersonnelId, ReportDate, SalesAmount)
    VALUES 
        (1, '2025-02-05', 2200.00),
        (1, '2025-02-18', 2700.00),
        (2, '2025-02-12', 2900.00),
        (2, '2025-02-22', 1800.00),
        (3, '2025-02-08', 2100.00),
        (3, '2025-02-25', 2600.00),
        (4, '2025-02-14', 3100.00),
        (4, '2025-02-28', 1400.00),
        (5, '2025-02-10', 3800.00),
        (5, '2025-02-20', 2000.00),
        (6, '2025-02-16', 1900.00),
        (6, '2025-02-26', 2500.00),
        (7, '2025-02-06', 2700.00),
        (7, '2025-02-24', 1600.00),
        (8, '2025-02-13', 2400.00),
        (8, '2025-02-27', 2100.00);
    
    -- March 2025 Sales (Current month - partial data)
    INSERT INTO Sales (PersonnelId, ReportDate, SalesAmount)
    VALUES 
        (1, '2025-03-05', 2400.00),
        (2, '2025-03-08', 3100.00),
        (3, '2025-03-12', 1800.00),
        (4, '2025-03-15', 2900.00),
        (5, '2025-03-10', 3600.00),
        (6, '2025-03-14', 2200.00),
        (7, '2025-03-18', 2500.00),
        (8, '2025-03-20', 2000.00);
    
    -- December 2024 Sales (Previous year for comparison)
    INSERT INTO Sales (PersonnelId, ReportDate, SalesAmount)
    VALUES 
        (1, '2024-12-10', 2300.00),
        (1, '2024-12-20', 1900.00),
        (2, '2024-12-15', 2800.00),
        (2, '2024-12-28', 2200.00),
        (3, '2024-12-12', 2100.00),
        (3, '2024-12-22', 2400.00),
        (4, '2024-12-18', 2600.00),
        (4, '2024-12-30', 1700.00),
        (5, '2024-12-14', 3200.00),
        (5, '2024-12-25', 2500.00),
        (6, '2024-12-16', 1800.00),
        (6, '2024-12-29', 2300.00),
        (7, '2024-12-11', 2700.00),
        (7, '2024-12-24', 1600.00),
        (8, '2024-12-19', 2400.00),
        (8, '2024-12-31', 2000.00);
    
    PRINT 'Sales data seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Sales data already exists.';
END
GO

-- Display summary of seeded data
SELECT 
    'Commission Profiles' AS TableName,
    COUNT(*) AS RecordCount
FROM CommissionProfile

UNION ALL

SELECT 
    'Personnel' AS TableName,
    COUNT(*) AS RecordCount
FROM Personnel

UNION ALL

SELECT 
    'Sales Records' AS TableName,
    COUNT(*) AS RecordCount
FROM Sales;

PRINT 'Database seeding completed successfully!';
GO
