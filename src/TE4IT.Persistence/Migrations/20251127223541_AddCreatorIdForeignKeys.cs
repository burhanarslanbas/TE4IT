using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TE4IT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorIdForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FK: Projects.CreatorId -> AspNetUsers.Id
            migrationBuilder.Sql(@"
                ALTER TABLE ""Projects""
                ADD CONSTRAINT ""FK_Projects_CreatorId_AspNetUsers""
                FOREIGN KEY (""CreatorId"")
                REFERENCES ""AspNetUsers""(""Id"")
                ON DELETE RESTRICT;");

            // FK: Modules.CreatorId -> AspNetUsers.Id
            migrationBuilder.Sql(@"
                ALTER TABLE ""Modules""
                ADD CONSTRAINT ""FK_Modules_CreatorId_AspNetUsers""
                FOREIGN KEY (""CreatorId"")
                REFERENCES ""AspNetUsers""(""Id"")
                ON DELETE RESTRICT;");

            // FK: UseCases.CreatorId -> AspNetUsers.Id
            migrationBuilder.Sql(@"
                ALTER TABLE ""UseCases""
                ADD CONSTRAINT ""FK_UseCases_CreatorId_AspNetUsers""
                FOREIGN KEY (""CreatorId"")
                REFERENCES ""AspNetUsers""(""Id"")
                ON DELETE RESTRICT;");

            // FK: Tasks.CreatorId -> AspNetUsers.Id
            migrationBuilder.Sql(@"
                ALTER TABLE ""Tasks""
                ADD CONSTRAINT ""FK_Tasks_CreatorId_AspNetUsers""
                FOREIGN KEY (""CreatorId"")
                REFERENCES ""AspNetUsers""(""Id"")
                ON DELETE RESTRICT;");

            // FK: Tasks.AssigneeId -> AspNetUsers.Id
            migrationBuilder.Sql(@"
                ALTER TABLE ""Tasks""
                ADD CONSTRAINT ""FK_Tasks_AssigneeId_AspNetUsers""
                FOREIGN KEY (""AssigneeId"")
                REFERENCES ""AspNetUsers""(""Id"")
                ON DELETE RESTRICT;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Tasks"" DROP CONSTRAINT IF EXISTS ""FK_Tasks_AssigneeId_AspNetUsers"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Tasks"" DROP CONSTRAINT IF EXISTS ""FK_Tasks_CreatorId_AspNetUsers"";");
            migrationBuilder.Sql(@"ALTER TABLE ""UseCases"" DROP CONSTRAINT IF EXISTS ""FK_UseCases_CreatorId_AspNetUsers"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Modules"" DROP CONSTRAINT IF EXISTS ""FK_Modules_CreatorId_AspNetUsers"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Projects"" DROP CONSTRAINT IF EXISTS ""FK_Projects_CreatorId_AspNetUsers"";");
        }
    }
}
