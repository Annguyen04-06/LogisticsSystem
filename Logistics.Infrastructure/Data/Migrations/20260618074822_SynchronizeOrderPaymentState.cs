using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SynchronizeOrderPaymentState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE [Payments]
                SET [PaidAt] = COALESCE([PaidAt], [CreatedAt])
                WHERE [Status] = 1;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Orders]
                SET [PaymentMethod] = latestPayment.[Method]
                FROM [Orders]
                CROSS APPLY
                (
                    SELECT TOP (1) [Method]
                    FROM [Payments]
                    WHERE [Payments].[OrderId] = [Orders].[Id]
                    ORDER BY [Payments].[CreatedAt] DESC, [Payments].[Id] DESC
                ) AS latestPayment;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");
        }
    }
}
