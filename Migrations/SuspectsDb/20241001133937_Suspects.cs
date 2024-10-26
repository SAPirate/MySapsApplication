using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySapsApplication.Migrations.SuspectsDb
{
    /// <inheritdoc />
    public partial class Suspects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrimeRecordModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuspectNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Offence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sentence = table.Column<int>(type: "int", nullable: false),
                    IssuedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrimeRecordModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuspectNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuspectIdentity = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffenceModels",
                columns: table => new
                {
                    OffenceType = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffenceModels", x => x.OffenceType);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrimeRecordModels");

            migrationBuilder.DropTable(
                name: "IndexModels");

            migrationBuilder.DropTable(
                name: "OffenceModels");
        }
    }
}
