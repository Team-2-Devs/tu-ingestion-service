using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingestion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeToUploadSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "UploadSessions",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "UploadSessions");
        }
    }
}
