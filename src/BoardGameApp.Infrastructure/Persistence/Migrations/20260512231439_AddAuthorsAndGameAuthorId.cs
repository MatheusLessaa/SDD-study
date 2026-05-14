using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorsAndGameAuthorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Michael Kiesling" },
                    { 2, "Klaus Teuber" },
                    { 3, "Jacob Fryxelius" }
                });

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE Games
                SET AuthorId = COALESCE(
                    (SELECT TOP 1 Id FROM Authors WHERE Authors.Name = Games.Author),
                    1)
                """);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Games",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Games");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AuthorId",
                table: "Games",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Authors_AuthorId",
                table: "Games",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Authors_AuthorId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_AuthorId",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE Games
                SET Author = COALESCE(
                    (SELECT TOP 1 Name FROM Authors WHERE Authors.Id = Games.AuthorId),
                    '')
                """);

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
