using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieNest.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecommendationReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Recommendations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Recommendations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
