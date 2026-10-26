using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCentreRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No DDL (Task 32.5, same reasoning as Task 29.2's AddMembershipRowVersion): "xmin" is PostgreSQL's
            // own system column on every table already, not something a migration creates — EF's generated
            // AddColumn for it is removed by hand. This migration exists only to keep the model snapshot in
            // step with Centre now mapping HasRowVersion().
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
