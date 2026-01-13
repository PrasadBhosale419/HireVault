using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireVault.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCandidateDocumentAttributeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "CandidateDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CandidateId",
                table: "CandidateDocuments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
