using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_C_4.Migrations
{
    /// <inheritdoc />
    public partial class newForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SeatingCharts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatingCharts_UserId",
                table: "SeatingCharts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeatingCharts_Users_UserId",
                table: "SeatingCharts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeatingCharts_Users_UserId",
                table: "SeatingCharts");

            migrationBuilder.DropIndex(
                name: "IX_SeatingCharts_UserId",
                table: "SeatingCharts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SeatingCharts");
        }
    }
}
