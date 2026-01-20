using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamFlow.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStatus",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusLastUpdatedAt",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusNote",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StatusLastUpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StatusNote",
                table: "AspNetUsers");
        }
    }
}
