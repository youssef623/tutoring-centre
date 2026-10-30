using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace TutoringCentre.Infrastructure.Persistence.Migrations;
[DbContext(typeof(AppDbContext))]
[Migration("20261030202957_AddPromptVersion")]
public sealed partial class AddPromptVersion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.AddColumn<string>(
                name: "prompt_version",
                schema: "assistant",
                table: "agent_runs",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
            migrationBuilder.DropColumn(
                name: "prompt_version",
                schema: "assistant",
                table: "agent_runs");
    }
}
