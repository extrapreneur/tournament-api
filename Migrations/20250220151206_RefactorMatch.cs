using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentApi.Migrations
{
    /// <inheritdoc />
    public partial class RefactorMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerId2",
                table: "Matches",
                newName: "Player2Id");

            migrationBuilder.RenameColumn(
                name: "PlayerId1",
                table: "Matches",
                newName: "Player1Id");

            migrationBuilder.AddColumn<int>(
                name: "NextMatchId",
                table: "Matches",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_NextMatchId",
                table: "Matches",
                column: "NextMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Matches_NextMatchId",
                table: "Matches",
                column: "NextMatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Matches_NextMatchId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_NextMatchId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "NextMatchId",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "Player2Id",
                table: "Matches",
                newName: "PlayerId2");

            migrationBuilder.RenameColumn(
                name: "Player1Id",
                table: "Matches",
                newName: "PlayerId1");
        }
    }
}
