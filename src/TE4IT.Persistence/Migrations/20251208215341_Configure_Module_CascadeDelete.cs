using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TE4IT.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Configure_Module_CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UseCases_Modules_ModuleId",
                table: "UseCases");

            migrationBuilder.AddForeignKey(
                name: "FK_UseCases_Modules_ModuleId",
                table: "UseCases",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UseCases_Modules_ModuleId",
                table: "UseCases");

            migrationBuilder.AddForeignKey(
                name: "FK_UseCases_Modules_ModuleId",
                table: "UseCases",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
