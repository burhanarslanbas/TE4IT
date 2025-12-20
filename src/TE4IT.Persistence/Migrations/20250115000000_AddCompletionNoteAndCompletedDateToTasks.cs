using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

using Microsoft.EntityFrameworkCore.Infrastructure;
using TE4IT.Persistence.Common.Contexts;

namespace TE4IT.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250115000000_AddCompletionNoteAndCompletedDateToTasks")]
    public partial class AddCompletionNoteAndCompletedDateToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletionNote",
                table: "Tasks",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionNote",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "Tasks");
        }
    }
}

