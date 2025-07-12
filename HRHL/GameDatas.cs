using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace HRHL;

public class GameDatas
{
    public int GamesNum = 0;
    public GameData[] Games = new GameData[64];

    public void ReadData()
    {
        string filePath = "./.rh/.settings/gamedatas.json";
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"文件 {filePath} 不存在，无法读取数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //Environment.Exit(1);
            GamesNum = 0;
            return;
        }
        string jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
        GamesNum = data.GamesNum;
        for (int i = 0; i < GamesNum; i++)
        {
            Games[i] = new GameData((string)data.Games[i].name, (string)data.Games[i].path);
        }
    }
    private bool IsValidGame(string path)
    {
        var files = Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            if (file.Contains("PlantsVsZombiesRH.exe"))
            {
                //MessageBox.Show(file);
                return true;
            }
        }
        return false;
    }
    public void ReadDataFromDisk()
    {
        string gamedatasPath = "./.rh/.settings/gamedatas.json";
        if (!File.Exists(gamedatasPath))
        {
            MessageBox.Show($"文件 {gamedatasPath} 不存在，无法写入数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }


        string[] subFolders = Directory.GetDirectories(@"./.rh", "*", SearchOption.TopDirectoryOnly);
        GamesNum=0;
        foreach (var fld in subFolders)
        {
            string name = fld.Split(@"\")[1];
            if(name.StartsWith("."))
                continue;
            if (IsValidGame(fld))
            {
                if (File.Exists($"{fld}/.name"))
                {
                    using (StreamReader reader = new StreamReader($"{fld}/.name"))
                    {
                        string line = reader.ReadLine();
                        name = line.Trim();
                    }
                }
                Games[GamesNum] = new GameData(name, fld);
                GamesNum++;
            }
        }
        return;
        
        
        var gamedatas = new
        {
            GamesNum = GamesNum,
            Games = Games.Take(GamesNum).Select(g => new { g.name, g.path }).ToArray()
        };
        string jsonData = JsonConvert.SerializeObject(gamedatas, Formatting.Indented);
        File.WriteAllText(gamedatasPath, jsonData);
    }
    public void WriteData()
    {
        string gamedatasPath = "./.rh/.settings/gamedatas.json";
        if (!File.Exists(gamedatasPath))
        {
            MessageBox.Show($"文件 {gamedatasPath} 不存在，无法写入数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
        var gamedatas = new
        {
            GamesNum = GamesNum,
            Games = Games.Take(GamesNum).Select(g => new { g.name, g.path }).ToArray()
        };
        string jsonData = JsonConvert.SerializeObject(gamedatas, Formatting.Indented);
        File.WriteAllText(gamedatasPath, jsonData);
    }
    public void StartGame(int status,string path)
    {
        if (!path.EndsWith("/") && !path.EndsWith("\\"))
        {
            path += "/";
        }

        if (status == 1)
        {// 原版
            File.Delete($"{path}winhttp.dll");
            File.Delete($"{path}version.dll");
            Process.Start($"{path}PlantsVsZombiesRH.exe");
        }
        if (status == 2)
        {// B
            File.Delete($"{path}version.dll");
            File.Copy($"{path}.bepinex/winhttp.dll", $"{path}winhttp.dll", true);
            Process.Start($"{path}PlantsVsZombiesRH.exe");
        }
        if(status == 3)
        {// M
            File.Delete($"{path}winhttp.dll");
            File.Copy($"{path}.melonloader/version.dll", $"{path}version.dll", true);
            Process.Start($"{path}PlantsVsZombiesRH.exe");
        }
    }

    public void RemoveGame(int index,bool force = false)
    {
        if (index != -1 && index<GamesNum)
        {
            if (force)
            {
                var folderPath = Games[index].path;
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, recursive: true);
                }
            }
            var gamesList = Games.ToList();
            gamesList.RemoveAt(index);
            Games = gamesList.ToArray();
            GamesNum--;
            WriteData();
        }
        else
        {
            MessageBox.Show("未选择游戏");
        }
    }
}