using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleIdToAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_ClassId_StudentId_Date",
                table: "Attendances");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassId_StudentId_Date_ScheduleId",
                table: "Attendances",
                columns: new[] { "ClassId", "StudentId", "Date", "ScheduleId" },
                unique: true,
                filter: "[ScheduleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ScheduleId",
                table: "Attendances",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Schedules_ScheduleId",
                table: "Attendances",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Schedules_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ClassId_StudentId_Date_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ScheduleId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassId_StudentId_Date",
                table: "Attendances",
                columns: new[] { "ClassId", "StudentId", "Date" },
                unique: true);
        }
    }
}
