namespace HRHL;

public class GameData
{
    public string Name;
    public string Path;
    public GameData(string name,string path) 
    {
        this.Name = name;
        this.Path = path;
    }
}
public class DownloadData
{
    public string Name;
    public string[] Links;
    public string Path;
    public DownloadData(string name , string[] links,string path)
    {
        this.Name = name;
        this.Links = links;
        this.Path = path;
    }
}