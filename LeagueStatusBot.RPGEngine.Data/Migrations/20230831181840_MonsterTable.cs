using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueStatusBot.RPGEngine.Data.Migrations
{
    /// <inheritdoc />
    public partial class MonsterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    MonsterName = table.Column<string>(type: "TEXT", nullable: false),
                    IntroductionPost = table.Column<string>(type: "TEXT", nullable: false),
                    IntroductionImageUrl = table.Column<string>(type: "TEXT", nullable: false),
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
                name: "SuperMonsters",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Name = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Description = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Damage = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstSuper_Effect = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSuper_Targets = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondSuper_Name = table.Column<string>(type: "TEXT", nullable: false),
                    SecondSuper_Description = table.Column<string>(type: "TEXT", nullable: false),
                    SecondSuper_Damage = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondSuper_Effect = table.Column<string>(type: "TEXT", nullable: false),
                    SecondSuper_Targets = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperMonsters", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "SuperMonsters");
        }
    }
}
