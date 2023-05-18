using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ultra.Core.WebApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedcityentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CITY",
                schema: "ult_crm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NAME = table.Column<string>(type: "text", nullable: false),
                    FOUNDATIONYEAR = table.Column<int>(name: "FOUNDATION_YEAR", type: "integer", nullable: true),
                    POPULATION = table.Column<int>(type: "integer", nullable: false),
                    LOCATION = table.Column<Point>(type: "geometry", nullable: true),
                    CREATEUSERID = table.Column<int>(name: "CREATE_USER_ID", type: "integer", nullable: false),
                    CREATEDATE = table.Column<DateTime>(name: "CREATE_DATE", type: "timestamp with time zone", nullable: false),
                    UPDATEUSERID = table.Column<int>(name: "UPDATE_USER_ID", type: "integer", nullable: true),
                    UPDATEDATE = table.Column<DateTime>(name: "UPDATE_DATE", type: "timestamp with time zone", nullable: true),
                    DELETEUSERID = table.Column<int>(name: "DELETE_USER_ID", type: "integer", nullable: true),
                    DELETEDATE = table.Column<DateTime>(name: "DELETE_DATE", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CITY", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CITY",
                schema: "ult_crm");
        }
    }
}
