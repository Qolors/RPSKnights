using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueStatusBot.RPGEngine.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialRealCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beings",
                columns: table => new
                {
                    DiscordId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Weapon = table.Column<int>(type: "INTEGER", nullable: false),
                    Helm = table.Column<int>(type: "INTEGER", nullable: false),
                    Chest = table.Column<int>(type: "INTEGER", nullable: false),
                    Gloves = table.Column<int>(type: "INTEGER", nullable: false),
                    Boots = table.Column<int>(type: "INTEGER", nullable: false),
                    Legs = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: false),
                    Strength = table.Column<int>(type: "INTEGER", nullable: false),
                    Luck = table.Column<int>(type: "INTEGER", nullable: false),
                    Endurance = table.Column<int>(type: "INTEGER", nullable: false),
                    Charisma = table.Column<int>(type: "INTEGER", nullable: false),
                    Intelligence = table.Column<int>(type: "INTEGER", nullable: false),
                    Agility = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Inventory = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beings", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "ItemEffects",
                columns: table => new
                {
                    EffectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EffectName = table.Column<string>(type: "TEXT", nullable: false),
                    EffectType = table.Column<string>(type: "TEXT", nullable: false),
                    EffectClass = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEffects", x => x.EffectId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<int>(type: "INTEGER", nullable: false),
                    Rarity = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemEffect = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beings");

            migrationBuilder.DropTable(
                name: "ItemEffects");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
