-- Marketing Personnel Management Database Reporting Views
-- Company A - Optional Reporting Views and Stored Procedures

USE MarketingPersonnelDB;
GO

-- View: Monthly Sales Summary by Personnel
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_MonthlySalesSummary')
    DROP VIEW vw_MonthlySalesSummary;
GO

CREATE VIEW vw_MonthlySalesSummary AS
SELECT 
    p.Id AS PersonnelId,
    p.Name AS PersonnelName,
    YEAR(s.ReportDate) AS SalesYear,
    MONTH(s.ReportDate) AS SalesMonth,
    DATENAME(MONTH, s.ReportDate) AS MonthName,
    COUNT(s.Id) AS TransactionCount,
    SUM(s.SalesAmount) AS TotalSales,
    AVG(s.SalesAmount) AS AverageSale,
    MIN(s.SalesAmount) AS MinSale,
    MAX(s.SalesAmount) AS MaxSale
FROM Personnel p
LEFT JOIN Sales s ON p.Id = s.PersonnelId
GROUP BY 
    p.Id, p.Name, 
    YEAR(s.ReportDate), MONTH(s.ReportDate), DATENAME(MONTH, s.ReportDate);
GO

-- View: Commission Calculation Summary
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_CommissionSummary')
    DROP VIEW vw_CommissionSummary;
GO

CREATE VIEW vw_CommissionSummary AS
SELECT 
    p.Id AS PersonnelId,
    p.Name AS PersonnelName,
    cp.ProfileName,
    cp.CommissionFixed,
    cp.CommissionPercentage,
    YEAR(s.ReportDate) AS SalesYear,
    MONTH(s.ReportDate) AS SalesMonth,
    DATENAME(MONTH, s.ReportDate) AS MonthName,
    ISNULL(SUM(s.SalesAmount), 0) AS MonthlySales,
    cp.CommissionFixed AS FixedCommission,
    ISNULL(SUM(s.SalesAmount), 0) * cp.CommissionPercentage AS VariableCommission,
    cp.CommissionFixed + (ISNULL(SUM(s.SalesAmount), 0) * cp.CommissionPercentage) AS TotalPayout
FROM Personnel p
INNER JOIN CommissionProfile cp ON p.CommissionProfileId = cp.Id
LEFT JOIN Sales s ON p.Id = s.PersonnelId
GROUP BY 
    p.Id, p.Name, cp.ProfileName, cp.CommissionFixed, cp.CommissionPercentage,
    YEAR(s.ReportDate), MONTH(s.ReportDate), DATENAME(MONTH, s.ReportDate);
GO

-- View: Top Performers by Month
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_TopPerformers')
    DROP VIEW vw_TopPerformers;
GO

CREATE VIEW vw_TopPerformers AS
WITH RankedSales AS (
    SELECT 
        p.Id AS PersonnelId,
        p.Name AS PersonnelName,
        YEAR(s.ReportDate) AS SalesYear,
        MONTH(s.ReportDate) AS SalesMonth,
        SUM(s.SalesAmount) AS TotalSales,
        COUNT(s.Id) AS TransactionCount,
        ROW_NUMBER() OVER (
            PARTITION BY YEAR(s.ReportDate), MONTH(s.ReportDate) 
            ORDER BY SUM(s.SalesAmount) DESC
        ) AS SalesRank
    FROM Personnel p
    INNER JOIN Sales s ON p.Id = s.PersonnelId
    GROUP BY 
        p.Id, p.Name, 
        YEAR(s.ReportDate), MONTH(s.ReportDate)
)
SELECT 
    PersonnelId,
    PersonnelName,
    SalesYear,
    SalesMonth,
    DATENAME(MONTH, DATEFROMPARTS(SalesYear, SalesMonth, 1)) AS MonthName,
    TotalSales,
    TransactionCount,
    SalesRank
FROM RankedSales
WHERE SalesRank <= 5;
GO

-- View: Daily Sales Activity
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_DailySalesActivity')
    DROP VIEW vw_DailySalesActivity;
GO

CREATE VIEW vw_DailySalesActivity AS
SELECT 
    s.ReportDate,
    YEAR(s.ReportDate) AS SalesYear,
    MONTH(s.ReportDate) AS SalesMonth,
    DAY(s.ReportDate) AS SalesDay,
    DATENAME(WEEKDAY, s.ReportDate) AS WeekdayName,
    COUNT(DISTINCT s.PersonnelId) AS ActivePersonnel,
    COUNT(s.Id) AS TotalTransactions,
    SUM(s.SalesAmount) AS DailySales,
    AVG(s.SalesAmount) AS AverageSaleAmount
FROM Sales s
GROUP BY s.ReportDate
GO

PRINT 'Reporting views created successfully!';

-- Sample queries to test the views
PRINT 'Sample query results:';

-- Top 3 performers for current month
SELECT TOP 3 
    PersonnelName,
    TotalSales,
    TransactionCount,
    SalesRank
FROM vw_TopPerformers 
WHERE SalesYear = YEAR(GETDATE()) 
  AND SalesMonth = MONTH(GETDATE())
ORDER BY SalesRank;

-- Commission summary for current month
SELECT 
    PersonnelName,
    MonthlySales,
    FixedCommission,
    VariableCommission,
    TotalPayout
FROM vw_CommissionSummary 
WHERE SalesYear = YEAR(GETDATE()) 
  AND SalesMonth = MONTH(GETDATE())
  AND MonthlySales > 0
ORDER BY TotalPayout DESC;

GO
