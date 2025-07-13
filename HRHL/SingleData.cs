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
    //public string[] links = new string[8];
    public List<string> links = new List<string>();
    public string path = "";
    public DownloadData(string name , List<String> links,string path)
    {
        this.name = name;
        this.links = links;
        this.path = path;
    }
}

public enum ModType
{
    BepInEx,
    MelonLoader
}
public class ModData
{
    public string name = "";
    public ModType type;
    //public string[] links = new string[8];
    public List<string> links = new List<string>();
    public string motd = "";
    public string rhversion = "";
    public string version = "";
    public string author = "";
    public ModData(string name,ModType type,List<string> links,string motd,string rhversion,string version,string author)
    {
        this.name = name;
        this.type = type;
        this.links = links;
        this.motd = motd;
        this.rhversion = rhversion;
        this.version = version;
        this.author = author;
    }
}