using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Uppdatera eventuella index
            migrationBuilder.RenameIndex(
                name: "IX_Matches_PlayerId1",
                table: "Matches",
                newName: "IX_Matches_Player1Id");

            // Uppdatera eventuella index
            migrationBuilder.RenameIndex(
                name: "IX_Matches_PlayerId2",
                table: "Matches",
                newName: "IX_Matches_Player2Id");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Uppdatera eventuella index
            migrationBuilder.RenameIndex(
                name: "IX_Matches_PlayerId1",
                table: "Matches",
                newName: "IX_Matches_Player1Id");

            // Uppdatera eventuella index
            migrationBuilder.RenameIndex(
                name: "IX_Matches_PlayerId2",
                table: "Matches",
                newName: "IX_Matches_Player2Id");
        }
    }
}
