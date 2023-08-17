using LeagueStatusBot.RPGEngine.Tooling.DatabaseTools;

namespace LeagueStatus.RPGEngine.Tooling
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Bonk RPG Engine Tooling. Please enter a command:\n add-items, add-itemeffects, add-beings");
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) { return; }

            var inputArgs = input.Split(" ");

            _ = new DatabasePopulator(inputArgs);

            Console.ReadLine();
        }
    }
}