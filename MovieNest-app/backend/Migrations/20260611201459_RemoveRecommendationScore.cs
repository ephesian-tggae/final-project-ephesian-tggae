using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieNest.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecommendationScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Recommendations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Recommendations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
