using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletTransactionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Payments_PaymentId",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentTransactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE transactionItem
                SET
                    transactionItem.[UserId] = payment.[UserId],
                    transactionItem.[OrderId] = payment.[OrderId]
                FROM [PaymentTransactions] AS transactionItem
                INNER JOIN [Payments] AS payment
                    ON payment.[Id] = transactionItem.[PaymentId];
                """);

            migrationBuilder.Sql(
                """
                UPDATE [PaymentTransactions]
                SET
                    [TransactionCode] = N'Payment',
                    [Note] = N'Thanh toán đơn hàng #' + CONVERT(nvarchar(20), [OrderId])
                WHERE [TransactionCode] = N'WalletPayment';
                """);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_UserId",
                table: "PaymentTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Orders_OrderId",
                table: "PaymentTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Payments_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Users_UserId",
                table: "PaymentTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Orders_OrderId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Payments_PaymentId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Users_UserId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_UserId",
                table: "PaymentTransactions");

            migrationBuilder.Sql(
                """
                DELETE FROM [PaymentTransactions]
                WHERE [PaymentId] IS NULL;
                """);

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Payments_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
