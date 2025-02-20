using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentApi.Migrations
{
    /// <inheritdoc />
    public partial class MakePlayerIdsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_Player1Id",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_Player2Id",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Player1Id",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Player2Id",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "Player2Id",
                table: "Matches",
                newName: "PlayerId1");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId1",
                table: "Matches",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId2",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerId1",
                table: "Matches",
                column: "PlayerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerId2",
                table: "Matches",
                column: "PlayerId2");

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
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_PlayerId1",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_PlayerId2",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_PlayerId1",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_PlayerId2",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "PlayerId2",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "PlayerId1",
                table: "Matches",
                newName: "Player2Id");

            migrationBuilder.AddColumn<int>(
                name: "Player1Id",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Player1Id",
                table: "Matches",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Player2Id",
                table: "Matches",
                column: "Player2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_Player1Id",
                table: "Matches",
                column: "Player1Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_Player2Id",
                table: "Matches",
                column: "Player2Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
