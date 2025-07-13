using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace HRHL;

public class DownloadDatas
{
    //public int DownloadNum = 0;
    //public DownloadData[] Downloads = new DownloadData[64];
    public List<DownloadData> Downloads = new List<DownloadData>();

    public void ReadData()
    {
        string filePath = "./.rh/.settings/downloaddatas.json";
        if (!File.Exists(filePath))
        {
            MessageBox.Show($"文件 {filePath} 不存在，无法读取数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
        string jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<List<DownloadData>>(jsonData);
        Downloads.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            Downloads.Add(new DownloadData(
                data[i].name,
                data[i].links,
                data[i].path
            ));
        }
    }


}