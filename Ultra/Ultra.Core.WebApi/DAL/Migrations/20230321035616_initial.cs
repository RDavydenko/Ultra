using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ultra.Core.WebApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ult_crm");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "ENTITY_TYPE",
                schema: "ult_crm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SYSTEMNAME = table.Column<string>(name: "SYSTEM_NAME", type: "text", nullable: false),
                    DISPLAYNAME = table.Column<string>(name: "DISPLAY_NAME", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENTITY_TYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MAN",
                schema: "ult_crm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NAME = table.Column<string>(type: "text", nullable: true),
                    AGE = table.Column<int>(type: "integer", nullable: true),
                    ASSIGNEDUSERID = table.Column<int>(name: "ASSIGNED_USER_ID", type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAN", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FAVORITE_ENTITY",
                schema: "ult_crm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    USERID = table.Column<int>(name: "USER_ID", type: "integer", nullable: false),
                    ENTITYTYPEID = table.Column<int>(name: "ENTITY_TYPE_ID", type: "integer", nullable: false),
                    ENTITYID = table.Column<int>(name: "ENTITY_ID", type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAVORITE_ENTITY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FAVORITE_ENTITY_ENTITY_TYPE_ENTITY_TYPE_ID",
                        column: x => x.ENTITYTYPEID,
                        principalSchema: "ult_crm",
                        principalTable: "ENTITY_TYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HOUSE",
                schema: "ult_crm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ADDRESS = table.Column<string>(type: "text", nullable: true),
                    BUILDINGYEAR = table.Column<int>(name: "BUILDING_YEAR", type: "integer", nullable: true),
                    MAGICNUMBER = table.Column<int>(name: "MAGIC_NUMBER", type: "integer", nullable: false),
                    OWNERID = table.Column<int>(name: "OWNER_ID", type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_HOUSE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HOUSE_MAN_OWNER_ID",
                        column: x => x.OWNERID,
                        principalSchema: "ult_crm",
                        principalTable: "MAN",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FAVORITE_ENTITY_ENTITY_TYPE_ID",
                schema: "ult_crm",
                table: "FAVORITE_ENTITY",
                column: "ENTITY_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_HOUSE_OWNER_ID",
                schema: "ult_crm",
                table: "HOUSE",
                column: "OWNER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAVORITE_ENTITY",
                schema: "ult_crm");

            migrationBuilder.DropTable(
                name: "HOUSE",
                schema: "ult_crm");

            migrationBuilder.DropTable(
                name: "ENTITY_TYPE",
                schema: "ult_crm");

            migrationBuilder.DropTable(
                name: "MAN",
                schema: "ult_crm");
        }
    }
}
