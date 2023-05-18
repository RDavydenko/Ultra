using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ultra.Core.WebApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedcitytohouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CITY_ID",
                schema: "ult_crm",
                table: "HOUSE",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HOUSE_CITY_ID",
                schema: "ult_crm",
                table: "HOUSE",
                column: "CITY_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HOUSE_CITY_CITY_ID",
                schema: "ult_crm",
                table: "HOUSE",
                column: "CITY_ID",
                principalSchema: "ult_crm",
                principalTable: "CITY",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HOUSE_CITY_CITY_ID",
                schema: "ult_crm",
                table: "HOUSE");

            migrationBuilder.DropIndex(
                name: "IX_HOUSE_CITY_ID",
                schema: "ult_crm",
                table: "HOUSE");

            migrationBuilder.DropColumn(
                name: "CITY_ID",
                schema: "ult_crm",
                table: "HOUSE");
        }
    }
}
