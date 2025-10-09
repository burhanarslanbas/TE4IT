using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TE4IT.Relational.Db.Migrations
{
    /// <inheritdoc />
    public partial class RenameRefreshTokensPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "ix_refreshtoken_user_revoked",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId_RevokedAt");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_Token",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId_RevokedAt",
                table: "refresh_tokens",
                newName: "ix_refreshtoken_user_revoked");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Token",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_Token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "Id");
        }
    }
}
