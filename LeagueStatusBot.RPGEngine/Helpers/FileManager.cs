namespace LeagueStatusBot.RPGEngine.Helpers;


public class FileManager
{
    private List<string> files;

    public FileManager()
    {
        files = new();
    }

    public void AddToCache(string file)
    {
        files.Add(file);
    }

    public void DeleteAllFiles()
    {
        foreach (var file in files)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        files.Clear();

        Console.WriteLine("Successfully Cleared GIFs");
    }
    
}