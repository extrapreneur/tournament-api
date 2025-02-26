using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ta bort den gamla FK:n
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_PlayerId1",
                table: "Matches");

            // Ta bort den gamla FK:n
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_PlayerId2",
                table: "Matches");

            // Ändra Player1Id och Player2Id till nullable
            migrationBuilder.AlterColumn<int>(
                name: "Player1Id",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Player2Id",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Skapa en ny FK med det nya namnet
            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_Player1Id",
                table: "Matches",
                column: "Player1Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Skapa en ny FK med det nya namnet
            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_Player2Id",
                table: "Matches",
                column: "Player2Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_Player1Id",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_Player2Id",
                table: "Matches");

            migrationBuilder.AlterColumn<int>(
                name: "Player2Id",
                table: "Matches",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_PlayerId1",
                table: "Matches",
                column: "PlayerId1",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_PlayerId2",
                table: "Matches",
                column: "PlayerId2",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Återställ eventuella index
            migrationBuilder.RenameIndex(
                name: "IX_Matches_Player1Id",
                table: "Matches",
                newName: "IX_Matches_PlayerId1");

            // Återställ eventuella index
            migrationBuilder.RenameIndex(
                name: "IX_Matches_Player2Id",
                table: "Matches",
                newName: "IX_Matches_PlayerId2");

        }
    }
}
