using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TE4IT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectMembersModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMember",
                table: "ProjectMember");

            migrationBuilder.RenameTable(
                name: "ProjectMember",
                newName: "ProjectMembers");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMember_ProjectId_UserId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_ProjectId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.RenameTable(
                name: "ProjectMembers",
                newName: "ProjectMember");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_ProjectId_UserId",
                table: "ProjectMember",
                newName: "IX_ProjectMember_ProjectId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMember",
                table: "ProjectMember",
                column: "Id");
        }
    }
}
