using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace HRHL;

public class GameDatas
{
    public int GamesNum;
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
        Debug.Assert(data != null, nameof(data) + " != null");
        GamesNum = data.GamesNum;
        for (int i = 0; i < GamesNum; i++)
        {
            Games[i] = new GameData((string)data.Games[i].name, (string)data.Games[i].path);
        }
    }

    public void StartGame(int status,string path)
    {
        if (status == 1)
        {// 原版
            File.Delete($"{path}winhttp.dll");
            File.Delete($"{path}version.dll");
            Process.Start($"{path}/PlantsVsZombiesRH.exe");
        }
        if (status == 2)
        {// B
            File.Delete($"{path}version.dll");
            File.Copy($"{path}.bepinex/winhttp.dll", $"{path}winhttp.dll", true);
            Process.Start($"{path}/PlantsVsZombiesRH.exe");
        }
        if(status == 3)
        {// M
            File.Delete($"{path}winhttp.dll");
            File.Copy($"{path}.melonloader/version.dll", $"{path}version.dll", true);
            Process.Start($"{path}/PlantsVsZombiesRH.exe");
        }
    }
}