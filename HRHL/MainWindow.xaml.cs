using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Schema;
namespace HRHL
{    
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
    public partial class MainWindow : Window
    {
        public static GameDatas? gameDatas = new GameDatas();
        public static DownloadDatas? downloadDatas = new DownloadDatas();
        private bool HasReaded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化选项卡按钮
            TabButton_Click(TabGames, null);

            Refresh();
        }

        private void Refresh()
        {
            MainGrid.Children.Clear();
            if (HasReaded)
            {
                string gamedatasPath = "./.rh/.settings/gamedatas.json";
                if (!File.Exists(gamedatasPath))
                {
                    MessageBox.Show($"文件 {gamedatasPath} 不存在，无法写入数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
                var gamedatas = new
                {
                    GamesNum = gameDatas.GamesNum,
                    Games = gameDatas.Games.Take(gameDatas.GamesNum).Select(g => new { g.name, g.path }).ToArray()
                };
                string jsonData = JsonConvert.SerializeObject(gamedatas, Formatting.Indented);
                File.WriteAllText(gamedatasPath, jsonData);
            }
            gameDatas.ReadData();
            downloadDatas.ReadData();
            HasReaded = true;
            for (int i = 0; i < gameDatas.GamesNum; i++)
            {
                AddGridItem(i);
            }
            if (gameDatas.GamesNum == 0)
            {
                var label = new Label
                {
                    Content = $"你看起来还没有游戏，去下载界面看看吧",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = 30,
                };
                var border = new Border
                {
                    Child = label,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                GamesView.Content = border;
            }
            else
            {
                GamesView.Content = MainGrid;
            }
            
            DownloadGrid.Children.Clear();
            for (int i = 0; i < downloadDatas.DownloadNum; i++)
            {
                AddDownloadGridItem(i);
            }
            AddDownloadGridItem(-1,special:true);
        }

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有选项卡样式
            TabHome.Background = Brushes.Transparent;
            TabGames.Background = Brushes.Transparent;
            TabDownloads.Background = Brushes.Transparent;
            TabSettings.Background = Brushes.Transparent;
            TabHome.Foreground = Brushes.White;
            TabGames.Foreground = Brushes.White;
            TabDownloads.Foreground = Brushes.White;
            TabSettings.Foreground = Brushes.White;

            // 隐藏所有视图
            HomeView.Visibility = Visibility.Collapsed;
            GamesView.Visibility = Visibility.Collapsed;
            DownloadsView.Visibility = Visibility.Collapsed;
            SettingsView.Visibility = Visibility.Collapsed;

            Button clickedButton = sender as Button;

            // 设置选中样式
            if (clickedButton != null)
            {
                clickedButton.Background = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                clickedButton.Foreground = Brushes.White;
            }

            Refresh();
            // 显示对应的视图
            switch ((sender as Button)?.Tag.ToString())
            {
                case "home":
                    HomeView.Visibility = Visibility.Visible;
                    break;
                case "games":
                    GamesView.Visibility = Visibility.Visible;
                    break;
                case "downloads":
                    DownloadsView.Visibility = Visibility.Visible;
                    break;
                case "settings":
                    SettingsView.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void AddGridItem(int index)
        {
            var border = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Padding = new Thickness(8),
                MaxHeight = 100,
                Tag = index
            };

            var verticalPanel = new StackPanel();

            var label = new Label
            {
                Content = $"{gameDatas.Games[index].name}",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 18
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            for (int j = 1; j <= 4; j++)
            {
                string content;
                switch (j)
                {
                    case 1: content = "原游戏"; break;
                    case 2: content = "B版"; break;
                    case 3: content = "M版"; break;
                    case 4: content = "配置"; break;
                    default: content = "null"; break;
                }
                var button = new Button
                {
                    Content = content,
                    Padding = new Thickness(10),
                    Margin = new Thickness(2, 0, 2, 0),
                    Background = (j == 4) ? new SolidColorBrush(Color.FromRgb(0, 200, 200)) : GetButtonColor(index), // 获取按钮颜色
                    Foreground = Brushes.White,
                    Tag = $"2|{index}|{j}" // 存储唯一标识
                };

                button.Click += Button_Click; // 添加点击事件
                buttonPanel.Children.Add(button);
            }

            // 将控件添加到布局
            verticalPanel.Children.Add(label);
            verticalPanel.Children.Add(buttonPanel);
            border.Child = verticalPanel;

            // 添加到主网格
            MainGrid.Children.Add(border);
        }

        private void AddDownloadGridItem(int index,bool special = false)
        {
            var border = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Padding = new Thickness(8),
                MaxHeight = 100,
                Tag = index
            };

            var verticalPanel = new StackPanel();

            var label = new Label
            {
                Content = special ? "从本地安装" : $"{downloadDatas.Downloads[index].name}",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 18
            };


            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            for (int j = 1; j <= 2; j++)
            {
                string content;
                if (special)
                {
                    switch (j)
                    {
                        case 1: content = "选择zip"; break;
                        case 2: content = "选择文件夹"; break;
                        default: content = "null"; break;
                    }
                }
                else
                {
                    switch (j)
                    {
                        case 1: content = "下载源1"; break;
                        case 2: content = "下载源2"; break;
                        default: content = "null"; break;
                    }
                }
                
                
                var button = new Button
                {
                    Content = content,
                    Padding = new Thickness(10),
                    Margin = new Thickness(2, 0, 2, 0),
                    Background = GetButtonColor(index), // 获取按钮颜色
                    Foreground = Brushes.White,
                    Tag = $"3|{index}|{j}" // 存储唯一标识
                };

                button.Click += Button_Click; // 添加点击事件
                buttonPanel.Children.Add(button);
            }

            // 将控件添加到布局
            verticalPanel.Children.Add(label);
            verticalPanel.Children.Add(buttonPanel);
            border.Child = verticalPanel;

            // 添加到主网格
            DownloadGrid.Children.Add(border);
            
        }

        // 不同索引使用不同的按钮颜色
        private Brush GetButtonColor(int index)
        {
            switch (index % 4)
            {
                case 1: return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // #4CAF50
                case 2: return new SolidColorBrush(Color.FromRgb(33, 150, 243)); // #2196F3
                case 3: return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // #FF9800
                default: return new SolidColorBrush(Color.FromRgb(156, 39, 176)); // #9C27B0
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tag)
            {
                var parts = tag.Split('|');
                if (parts.Length == 3)
                {
                    var t = int.Parse(parts[0]);
                    if (t == 2)
                    {
                        int itemIndex = int.Parse(parts[1]);
                        int buttonIndex = int.Parse(parts[2]);
                        if (buttonIndex <= 3)
                        {
                            gameDatas.StartGame(buttonIndex, gameDatas.Games[itemIndex].path);
                        }
                        else
                        {
                            MessageBox.Show($"你点击了第 {itemIndex} 组的第 {buttonIndex} 个按钮",
                                            "操作通知",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                        }
                    }
                    else if (t == 3)
                    {
                        int itemIndex = int.Parse(parts[1]);
                        int buttonIndex = int.Parse(parts[2]);
                        if (itemIndex != -1)
                        {
                            
                            string DownloadLink = downloadDatas.Downloads[itemIndex].links[buttonIndex - 1];
                            MessageBox.Show($"马上程序会弹出一个黑窗口，开始下载文件和解压，请勿关闭窗口，请耐心等待操作完成");
                            Tools.DownloadFileAndUnZip($"{downloadDatas.Downloads[itemIndex].path}/", DownloadLink,
                                downloadDatas.Downloads[itemIndex].name);

                        }
                        else
                        {
                            if (buttonIndex == 1)
                            {
                                string? zippath = Tools.SelectFile("选择游戏的 ZIP 文件","ZIP 压缩文件|*.zip|所有文件|*.*");
                                if (zippath is not null)
                                {
                                    Tools.DownloadFileAndUnZip("./.rh/newgame/",null,"新的游戏",zippath);
                                }
                            }
                            else
                            {
                                MessageBox.Show("还在开发...");
                            }
                           
                            
                        }
                        Refresh();
                        
                    }
                }
            }
        }
    }
}
