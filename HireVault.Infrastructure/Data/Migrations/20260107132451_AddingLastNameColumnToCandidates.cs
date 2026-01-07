using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireVault.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingLastNameColumnToCandidates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Candidates");
        }
    }
}
