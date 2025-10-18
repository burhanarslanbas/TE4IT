using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TE4IT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTaskRelationsAndRefreshTokenFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskRelations_Tasks_TaskId",
                table: "TaskRelations");

            migrationBuilder.DropIndex(
                name: "IX_TaskRelations_TaskId",
                table: "TaskRelations");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "TaskRelations");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRelations_SourceTaskId",
                table: "TaskRelations",
                column: "SourceTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRelations_SourceTaskId_TargetTaskId_RelationType",
                table: "TaskRelations",
                columns: new[] { "SourceTaskId", "TargetTaskId", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskRelations_TargetTaskId",
                table: "TaskRelations",
                column: "TargetTaskId");

            // Mevcutta yetim RefreshToken kayıtları varsa migration sırasında doğrulama hatası almamak için NOT VALID ekle
            migrationBuilder.Sql("ALTER TABLE \"RefreshTokens\" ADD CONSTRAINT \"FK_RefreshTokens_AspNetUsers_UserId\" FOREIGN KEY (\"UserId\") REFERENCES \"AspNetUsers\"(\"Id\") ON DELETE CASCADE NOT VALID;");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRelations_Tasks_SourceTaskId",
                table: "TaskRelations",
                column: "SourceTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRelations_Tasks_TargetTaskId",
                table: "TaskRelations",
                column: "TargetTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskRelations_Tasks_SourceTaskId",
                table: "TaskRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskRelations_Tasks_TargetTaskId",
                table: "TaskRelations");

            migrationBuilder.DropIndex(
                name: "IX_TaskRelations_SourceTaskId",
                table: "TaskRelations");

            migrationBuilder.DropIndex(
                name: "IX_TaskRelations_SourceTaskId_TargetTaskId_RelationType",
                table: "TaskRelations");

            migrationBuilder.DropIndex(
                name: "IX_TaskRelations_TargetTaskId",
                table: "TaskRelations");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "TaskRelations",
                type: "uuid",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_TaskRelations_TaskId",
                table: "TaskRelations",
                column: "TaskId");


            migrationBuilder.AddForeignKey(
                name: "FK_TaskRelations_Tasks_TaskId",
                table: "TaskRelations",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
