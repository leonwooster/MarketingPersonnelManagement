using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyA.DataAccess;
using CompanyA.DataAccess.Models;
using System.Text;
using System.Globalization;

namespace CompanyA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly MarketingDbContext _context;

        public ReportsController(MarketingDbContext context)
        {
            _context = context;
        }

        [HttpGet("management-overview")]
        public async Task<IActionResult> GetManagementOverview(
            [FromQuery] int? year = null, 
            [FromQuery] int? month = null,
            [FromQuery] int? personnelId = null,
            [FromQuery] string format = "json")
        {
            try
            {
                var targetYear = year ?? DateTime.Now.Year;
                var targetMonth = month ?? DateTime.Now.Month;

                var startDate = new DateTime(targetYear, targetMonth, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // Get sales data for the month
                var salesQuery = _context.Sales
                    .Include(s => s.Personnel)
                    .Where(s => s.ReportDate >= startDate && s.ReportDate <= endDate);
                
                // Apply personnel filter if specified
                if (personnelId.HasValue)
                {
                    salesQuery = salesQuery.Where(s => s.PersonnelId == personnelId.Value);
                }
                
                var salesData = await salesQuery.ToListAsync();

                // Calculate totals by month
                var totalSales = salesData.Sum(s => s.SalesAmount);
                var totalTransactions = salesData.Count;

                // Top performers (top 5)
                var topPerformers = salesData
                    .GroupBy(s => new { s.PersonnelId, s.Personnel.Name })
                    .Select(g => new
                    {
                        PersonnelId = g.Key.PersonnelId,
                        PersonnelName = g.Key.Name,
                        TotalSales = g.Sum(s => s.SalesAmount),
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(p => p.TotalSales)
                    .Take(5)
                    .ToList();

                // Average per person
                var personnelCount = await _context.Personnel.CountAsync();
                var averagePerPerson = personnelCount > 0 ? totalSales / personnelCount : 0;

                // Days with no sales entries
                var salesDates = salesData.Select(s => s.ReportDate.Date).Distinct().ToHashSet();
                var daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);
                var daysWithNoSales = 0;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var checkDate = new DateTime(targetYear, targetMonth, day).Date;
                    if (!salesDates.Contains(checkDate))
                    {
                        daysWithNoSales++;
                    }
                }

                var report = new
                {
                    ReportPeriod = new
                    {
                        Year = targetYear,
                        Month = targetMonth,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(targetMonth),
                        StartDate = startDate,
                        EndDate = endDate
                    },
                    Summary = new
                    {
                        TotalSales = totalSales,
                        TotalTransactions = totalTransactions,
                        AveragePerPerson = Math.Round(averagePerPerson, 2),
                        DaysWithNoSales = daysWithNoSales,
                        DaysInMonth = daysInMonth,
                        ActivePersonnelCount = personnelCount
                    },
                    TopPerformers = topPerformers
                };

                if (format.ToLower() == "csv")
                {
                    var csv = GenerateManagementOverviewCsv(report);
                    return File(Encoding.UTF8.GetBytes(csv), "text/csv", 
                        $"management-overview-{targetYear}-{targetMonth:D2}.csv");
                }

                return Ok(new { success = true, data = report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("commission-payout")]
        public async Task<IActionResult> GetCommissionPayout(
            [FromQuery] int? year = null, 
            [FromQuery] int? month = null,
            [FromQuery] int? personnelId = null,
            [FromQuery] string format = "json")
        {
            try
            {
                var targetYear = year ?? DateTime.Now.Year;
                var targetMonth = month ?? DateTime.Now.Month;

                var startDate = new DateTime(targetYear, targetMonth, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // Get personnel with their commission profiles and sales
                var personnelBaseQuery = _context.Personnel.Include(p => p.CommissionProfile);
                
                // Apply personnel filter if specified
                var personnelQuery = personnelId.HasValue 
                    ? personnelBaseQuery.Where(p => p.Id == personnelId.Value)
                    : personnelBaseQuery;
                
                var payoutData = await personnelQuery
                    .Select(p => new
                    {
                        PersonnelId = p.Id,
                        PersonnelName = p.Name,
                        CommissionFixed = p.CommissionProfile.CommissionFixed,
                        CommissionPercentage = p.CommissionProfile.CommissionPercentage,
                        MonthlySales = _context.Sales
                            .Where(s => s.PersonnelId == p.Id && 
                                   s.ReportDate >= startDate && 
                                   s.ReportDate <= endDate)
                            .Sum(s => (decimal?)s.SalesAmount) ?? 0
                    })
                    .ToListAsync();

                var payoutReport = payoutData.Select(p => new
                {
                    PersonnelId = p.PersonnelId,
                    PersonnelName = p.PersonnelName,
                    MonthlySales = p.MonthlySales,
                    CommissionFixed = p.CommissionFixed,
                    CommissionPercentage = p.CommissionPercentage,
                    CommissionVariable = Math.Round(p.CommissionPercentage * p.MonthlySales, 2),
                    TotalPayout = Math.Round(p.CommissionFixed + (p.CommissionPercentage * p.MonthlySales), 2)
                }).ToList();

                var summary = new
                {
                    ReportPeriod = new
                    {
                        Year = targetYear,
                        Month = targetMonth,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(targetMonth)
                    },
                    TotalSales = payoutReport.Sum(p => p.MonthlySales),
                    TotalFixedCommissions = payoutReport.Sum(p => p.CommissionFixed),
                    TotalVariableCommissions = payoutReport.Sum(p => p.CommissionVariable),
                    TotalPayout = payoutReport.Sum(p => p.TotalPayout),
                    PersonnelCount = payoutReport.Count
                };

                var report = new
                {
                    Summary = summary,
                    PersonnelPayouts = payoutReport
                };

                if (format.ToLower() == "csv")
                {
                    var csv = GenerateCommissionPayoutCsv(report);
                    return File(Encoding.UTF8.GetBytes(csv), "text/csv", 
                        $"commission-payout-{targetYear}-{targetMonth:D2}.csv");
                }

                return Ok(new { success = true, data = report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private string GenerateManagementOverviewCsv(dynamic report)
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine($"Management Overview Report - {report.ReportPeriod.MonthName} {report.ReportPeriod.Year}");
            csv.AppendLine();
            
            // Summary
            csv.AppendLine("Summary");
            csv.AppendLine("Metric,Value");
            csv.AppendLine($"Total Sales,{report.Summary.TotalSales:C}");
            csv.AppendLine($"Total Transactions,{report.Summary.TotalTransactions}");
            csv.AppendLine($"Average Per Person,{report.Summary.AveragePerPerson:C}");
            csv.AppendLine($"Days with No Sales,{report.Summary.DaysWithNoSales}");
            csv.AppendLine($"Days in Month,{report.Summary.DaysInMonth}");
            csv.AppendLine($"Active Personnel Count,{report.Summary.ActivePersonnelCount}");
            csv.AppendLine();
            
            // Top Performers
            csv.AppendLine("Top Performers");
            csv.AppendLine("Rank,Personnel Name,Total Sales,Transaction Count");
            
            int rank = 1;
            foreach (var performer in report.TopPerformers)
            {
                csv.AppendLine($"{rank},{performer.PersonnelName},{performer.TotalSales:C},{performer.TransactionCount}");
                rank++;
            }
            
            return csv.ToString();
        }

        private string GenerateCommissionPayoutCsv(dynamic report)
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine($"Commission Payout Report - {report.Summary.ReportPeriod.MonthName} {report.Summary.ReportPeriod.Year}");
            csv.AppendLine();
            
            // Summary
            csv.AppendLine("Summary");
            csv.AppendLine("Metric,Value");
            csv.AppendLine($"Total Sales,{report.Summary.TotalSales:C}");
            csv.AppendLine($"Total Fixed Commissions,{report.Summary.TotalFixedCommissions:C}");
            csv.AppendLine($"Total Variable Commissions,{report.Summary.TotalVariableCommissions:C}");
            csv.AppendLine($"Total Payout,{report.Summary.TotalPayout:C}");
            csv.AppendLine($"Personnel Count,{report.Summary.PersonnelCount}");
            csv.AppendLine();
            
            // Personnel Details
            csv.AppendLine("Personnel Payouts");
            csv.AppendLine("Personnel Name,Monthly Sales,Fixed Commission,Commission %,Variable Commission,Total Payout");
            
            foreach (var payout in report.PersonnelPayouts)
            {
                csv.AppendLine($"{payout.PersonnelName},{payout.MonthlySales:C},{payout.CommissionFixed:C},{payout.CommissionPercentage:P},{payout.CommissionVariable:C},{payout.TotalPayout:C}");
            }
            
            return csv.ToString();
        }
    }
}
