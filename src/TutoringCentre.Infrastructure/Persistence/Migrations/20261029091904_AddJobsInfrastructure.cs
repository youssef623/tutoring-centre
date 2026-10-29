using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobsInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create schema hangfire authorization tutoring_owner;");
            migrationBuilder.Sql("create schema jobs authorization tutoring_owner;");
            migrationBuilder.GrantJobStorageAccess();
            migrationBuilder.GrantRuntimeAccess("jobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RevokeRuntimeAccess("jobs");
            migrationBuilder.Sql("drop schema jobs;");
            migrationBuilder.Sql("drop schema hangfire cascade;");
        }
    }
}
