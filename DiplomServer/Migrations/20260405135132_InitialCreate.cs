using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GroupDisciplines",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DisciplineId = table.Column<uint>(type: "int unsigned", nullable: false),
                    AttestTypeId = table.Column<uint>(type: "int unsigned", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    StudyYear = table.Column<string>(type: "varchar(95)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDisciplines", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RetakeDirections",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupDisciplineId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CreatedById = table.Column<uint>(type: "int unsigned", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RetakeDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetakeDirections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetakeDirections_GroupDisciplines_GroupDisciplineId",
                        column: x => x.GroupDisciplineId,
                        principalTable: "GroupDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RetakeDirectionStudents",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RetakeDirectionId = table.Column<uint>(type: "int unsigned", nullable: false),
                    StudentId = table.Column<uint>(type: "int unsigned", nullable: false),
                    RetakeGradeValue = table.Column<int>(type: "int", nullable: false),
                    RetakeIsPassed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RetakeGradeDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetakeDirectionStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetakeDirectionStudents_RetakeDirections_RetakeDirectionId",
                        column: x => x.RetakeDirectionId,
                        principalTable: "RetakeDirections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDisciplines_GroupId_DisciplineId_AttestTypeId_Semester_~",
                table: "GroupDisciplines",
                columns: new[] { "GroupId", "DisciplineId", "AttestTypeId", "Semester", "StudyYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RetakeDirections_GroupDisciplineId",
                table: "RetakeDirections",
                column: "GroupDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_RetakeDirectionStudents_RetakeDirectionId_StudentId",
                table: "RetakeDirectionStudents",
                columns: new[] { "RetakeDirectionId", "StudentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetakeDirectionStudents");

            migrationBuilder.DropTable(
                name: "RetakeDirections");

            migrationBuilder.DropTable(
                name: "GroupDisciplines");
        }
    }
}
