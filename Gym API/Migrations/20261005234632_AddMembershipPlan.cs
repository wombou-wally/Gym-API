using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gym_API.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembershipPlanId",
                table: "Members",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    DurationDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Plans_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Plans_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Members_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembershipPlanId",
                table: "Members");
        }
    }
}
