using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySapsApplication.Migrations.SuspectsDb
{
    /// <inheritdoc />
    public partial class AddStatusToCrimeRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedCases");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CrimeRecordModels",
                type: "nvarchar(max)",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CrimeRecordModels");

            migrationBuilder.CreateTable(
                name: "CompletedCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrimeRecordId = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedAt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Offence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sentence = table.Column<int>(type: "int", nullable: false),
                    SuspectNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedCases", x => x.Id);
                });
        }
    }
}
