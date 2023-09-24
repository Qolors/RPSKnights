using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueStatusBot.RPGEngine.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlayerRanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArmorEffects");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "ItemEffects");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Loot");

            migrationBuilder.DropTable(
                name: "SuperMonsters");

            migrationBuilder.DropColumn(
                name: "Agility",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Boots",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Charisma",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Chest",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Gloves",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Helm",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Intelligence",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Inventory",
                table: "Beings");

            migrationBuilder.DropColumn(
                name: "Legs",
                table: "Beings");

            migrationBuilder.RenameColumn(
                name: "Weapon",
                table: "Beings",
                newName: "Wins");

            migrationBuilder.RenameColumn(
                name: "Strength",
                table: "Beings",
                newName: "ServerId");

            migrationBuilder.RenameColumn(
                name: "MaxHitPoints",
                table: "Beings",
                newName: "Losses");

            migrationBuilder.RenameColumn(
                name: "Luck",
                table: "Beings",
                newName: "EloRating");

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ServerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beings_ServerId",
                table: "Beings",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Beings_Servers_ServerId",
                table: "Beings",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beings_Servers_ServerId",
                table: "Beings");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Beings_ServerId",
                table: "Beings");

            migrationBuilder.RenameColumn(
                name: "Wins",
                table: "Beings",
                newName: "Weapon");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "Beings",
                newName: "Strength");

            migrationBuilder.RenameColumn(
                name: "Losses",
                table: "Beings",
                newName: "MaxHitPoints");

            migrationBuilder.RenameColumn(
                name: "EloRating",
                table: "Beings",
                newName: "Luck");

            migrationBuilder.AddColumn<int>(
                name: "Agility",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Boots",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Charisma",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Chest",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Beings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Endurance",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Gloves",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Helm",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Intelligence",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Inventory",
                table: "Beings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Legs",
                table: "Beings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ArmorEffects",
                columns: table => new
                {
                    EffectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EffectFor = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArmorEffects", x => x.EffectId);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    MonsterName = table.Column<string>(type: "TEXT", nullable: false),
                    IntroductionImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IntroductionPost = table.Column<string>(type: "TEXT", nullable: false),
                    MidPost = table.Column<string>(type: "TEXT", nullable: false),
                    MidPostImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    PreFightPost = table.Column<string>(type: "TEXT", nullable: false),
                    PreFightPostImageUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.MonsterName);
                });

            migrationBuilder.CreateTable(
                name: "ItemEffects",
                columns: table => new
                {
                    EffectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EffectClass = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectName = table.Column<string>(type: "TEXT", nullable: false),
                    EffectType = table.Column<string>(type: "TEXT", nullable: false)
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
                    ItemEffect = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<int>(type: "INTEGER", nullable: false),
                    Rarity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

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

            migrationBuilder.CreateTable(
                name: "SuperMonsters",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Damage = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstSuper_Description = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Effect = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Name = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Targets = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondSuper_Damage = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondSuper_Description = table.Column<string>(type: "TEXT", nullable: false),
                    SecondSuper_Effect = table.Column<string>(type: "TEXT", nullable: false),
                    SecondSuper_Name = table.Column<string>(type: "TEXT", nullable: false),
                    SecondSuper_Targets = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperMonsters", x => x.Name);
                });
        }
    }
}
