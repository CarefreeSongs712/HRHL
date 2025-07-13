using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace HRHL;

public class ModDatas
{
    public int ModsNum = 0;
    public ModData[] Mods = new ModData[64];
    public void ReadData()
    {
        string filePath = "./.rh/.settings/gamedatas.json";
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"文件 {filePath} 不存在，无法读取数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //Environment.Exit(1);
            ModsNum = 0;
            return;
        }
        string jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<ModDatas>(jsonData);
        ModsNum = data.ModsNum;
        for (int i = 0; i < ModsNum; i++)
        {
            Mods[i] = new ModData(data.Mods[i].name, data.Mods[i].type, data.Mods[i].links, data.Mods[i].motd,data.Mods[i].rhversion,data.Mods[i].version,data.Mods[i].author);
        }
    }
}