using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace HRHL;

public class ModDatas
{
    //public int ModsNum = 0;
    //public ModData[] Mods = new ModData[64];
    public List<ModData> Mods = new List<ModData>();
    public void ReadData()
    {
        string filePath = "./.rh/.settings/gamedatas.json";
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"文件 {filePath} 不存在，无法读取数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        string jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<ModDatas>(jsonData);
        for (int i = 0; i < Mods.Count; i++)
        {
            Mods[i] = new ModData(data.Mods[i].name, data.Mods[i].type, data.Mods[i].links, data.Mods[i].motd,data.Mods[i].rhversion,data.Mods[i].version,data.Mods[i].author);
        }
    }
}