using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace HRHL;

public class DownloadDatas
{
    public int DownloadNum = 0;
    public DownloadData[] Downloads = new DownloadData[64];

    public void ReadData()
    {
        string filePath = "./.rh/.settings/downloaddatas.json";
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"文件 {filePath} 不存在，无法读取数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
        string jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
        DownloadNum = data.DownloadNum;
        for (int i = 0; i < DownloadNum; i++)
        {
            Downloads[i] = new DownloadData(
                (string)data.Downloads[i].name,
                data.Downloads[i].links.ToObject<string[]>(),
                (string)data.Downloads[i].path
            );
        }
    }


}