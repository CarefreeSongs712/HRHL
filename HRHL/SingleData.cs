namespace HRHL;


public class GameData
{
    public string name = "";
    public string path = "";
    public GameData(string name,string path) 
    {
        this.name = name;
        this.path = path;
    }
}
public class DownloadData
{
    public string name = "";
    public string[] links = new string[8];
    public string path = "";
    public DownloadData(string name , string[] links,string path)
    {
        this.name = name;
        this.links = links;
        this.path = path;
    }
}