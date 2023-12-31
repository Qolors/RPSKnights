namespace LeagueStatusBot.RPGEngine.Helpers;

/// <summary>
/// FileManager is instantiated with GameManager, it handles all local folder/file operations
/// </summary>
public class FileManager
{
    private List<string> files;
    private string folderName;
    public string GetBasePath => folderName + "/";

    public FileManager(ulong folderName)
    {
        this.folderName = folderName.ToString();

        if (!Directory.Exists(this.folderName))
        {
            Directory.CreateDirectory(this.folderName);
        }

        files = new();
    }

    public void AddToCache(string file)
    {
        Console.WriteLine(file);
        files.Add(file);
    }

    public void DeleteAllFiles()
    {
        try
        {
            Directory.Delete(folderName, true);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        Console.WriteLine("Successfully Cleared GIFs and Folder");
    }

    public List<Image> LoadAllGifs()
    {
        List<Image> gifs = new();

        try
        {
            foreach (var file in files)
            {
                Image image = Image.Load<Rgba32>(file);
                gifs.Add(image);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return gifs;
    }

    public void DeleteInitialFile(string file)
    {
        string initialFilePath = $"{folderName}/{file}";

        if (File.Exists(initialFilePath))
        {
            File.Delete(initialFilePath);
        }
    }
    
}