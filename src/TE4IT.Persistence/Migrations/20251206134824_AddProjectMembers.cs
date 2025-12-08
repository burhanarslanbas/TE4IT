using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TE4IT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMember", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_ProjectId_UserId",
                table: "ProjectMember",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);

            // FK: ProjectMember.ProjectId -> Projects.Id (ON DELETE CASCADE)
            migrationBuilder.Sql(@"
                ALTER TABLE ""ProjectMember""
                ADD CONSTRAINT ""FK_ProjectMember_ProjectId_Projects""
                FOREIGN KEY (""ProjectId"")
                REFERENCES ""Projects""(""Id"")
                ON DELETE CASCADE;");

            // FK: ProjectMember.UserId -> AspNetUsers.Id (ON DELETE RESTRICT)
            migrationBuilder.Sql(@"
                ALTER TABLE ""ProjectMember""
                ADD CONSTRAINT ""FK_ProjectMember_UserId_AspNetUsers""
                FOREIGN KEY (""UserId"")
                REFERENCES ""AspNetUsers""(""Id"")
                ON DELETE RESTRICT;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""ProjectMember"" DROP CONSTRAINT IF EXISTS ""FK_ProjectMember_UserId_AspNetUsers"";");
            migrationBuilder.Sql(@"ALTER TABLE ""ProjectMember"" DROP CONSTRAINT IF EXISTS ""FK_ProjectMember_ProjectId_Projects"";");
            
            migrationBuilder.DropTable(
                name: "ProjectMember");
        }
    }
}
