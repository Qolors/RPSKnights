namespace LeagueStatusBot.RPGEngine.Helpers;


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
        files.Add(folderName + "/" + file);
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

        foreach (var file in files)
        {
            Image image = Image.Load<Rgba32>(file);
            gifs.Add(image);
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