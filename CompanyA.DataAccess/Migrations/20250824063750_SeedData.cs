using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommissionProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    profile_name = table.Column<int>(type: "int", nullable: false),
                    commission_fixed = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    commission_percentage = table.Column<decimal>(type: "decimal(10,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionProfile", x => x.Id);
                    table.CheckConstraint("CK_CommissionProfile_Fixed", "commission_fixed >= 0");
                    table.CheckConstraint("CK_CommissionProfile_Percentage", "commission_percentage >= 0 AND commission_percentage <= 1");
                });

            migrationBuilder.CreateTable(
                name: "Personnel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    age = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    commission_profile_id = table.Column<int>(type: "int", nullable: false),
                    bank_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    bank_account_no = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.Id);
                    table.CheckConstraint("CK_Personnel_Age", "age >= 19");
                    table.CheckConstraint("CK_Personnel_Name", "LEN(LTRIM(RTRIM(name))) > 0");
                    table.CheckConstraint("CK_Personnel_Phone", "LEN(LTRIM(RTRIM(phone))) > 0");
                    table.ForeignKey(
                        name: "FK_Personnel_CommissionProfile_commission_profile_id",
                        column: x => x.commission_profile_id,
                        principalTable: "CommissionProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personnel_id = table.Column<int>(type: "int", nullable: false),
                    report_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sales_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.CheckConstraint("CK_Sales_Amount", "sales_amount >= 0");
                    table.CheckConstraint("CK_Sales_Date", "report_date <= GETDATE()");
                    table.ForeignKey(
                        name: "FK_Sales_Personnel_personnel_id",
                        column: x => x.personnel_id,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CommissionProfile",
                columns: new[] { "Id", "commission_fixed", "commission_percentage", "profile_name" },
                values: new object[,]
                {
                    { 1, 500.00m, 0.050000m, 1 },
                    { 2, 750.00m, 0.030000m, 2 },
                    { 3, 300.00m, 0.080000m, 3 }
                });

            migrationBuilder.InsertData(
                table: "Personnel",
                columns: new[] { "Id", "age", "bank_account_no", "bank_name", "commission_profile_id", "name", "phone" },
                values: new object[,]
                {
                    { 1, 25, "1234567890", "Chase Bank", 1, "John Smith", "555-0101" },
                    { 2, 28, "2345678901", "Wells Fargo", 2, "Sarah Johnson", "555-0102" },
                    { 3, 32, "3456789012", "Bank of America", 1, "Michael Brown", "555-0103" },
                    { 4, 24, "4567890123", "Citibank", 3, "Emily Davis", "555-0104" },
                    { 5, 29, "5678901234", "TD Bank", 2, "David Wilson", "555-0105" }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "personnel_id", "report_date", "sales_amount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1250.00m },
                    { 2, 1, new DateTime(2024, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 980.50m },
                    { 3, 2, new DateTime(2024, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2150.00m },
                    { 4, 2, new DateTime(2024, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1875.50m },
                    { 5, 3, new DateTime(2024, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 950.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_CommissionProfileId",
                table: "Personnel",
                column: "commission_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PersonnelId",
                table: "Sales",
                column: "personnel_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ReportDate",
                table: "Sales",
                column: "report_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Personnel");

            migrationBuilder.DropTable(
                name: "CommissionProfile");
        }
    }
}
