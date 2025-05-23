using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entites.Migrations
{
    /// <inheritdoc />
    public partial class ModifyOrderItemV7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Drop the bad index on TicketTypeId alone
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_TicketTypeId",
                table: "OrderItems");

            // 2) Create the correct unique index on (OrderId, TicketTypeId)
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_TicketTypeId",
                table: "OrderItems",
                columns: new[] { "OrderId", "TicketTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // reverse: drop the composite index…
            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_TicketTypeId",
                table: "OrderItems");

            // …and restore the old unique index on TicketTypeId (if you really want to)
            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_TicketTypeId",
                table: "OrderItems",
                column: "TicketTypeId",
                unique: true);
        }
    }
}
