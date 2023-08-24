using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueStatusBot.RPGEngine.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLootTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loot",
                columns: table => new
                {
                    DiscordId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LootCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loot", x => x.DiscordId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loot");
        }
    }
}
