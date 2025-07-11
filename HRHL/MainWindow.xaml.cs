using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HRHL
{
    public partial class MainWindow
    {
        public static GameDatas? GameDatas = new GameDatas();
        public static DownloadDatas? DownloadDatas = new DownloadDatas();
        private bool _hasReaded;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化选项卡按钮
            TabButton_Click(TabGames, null!);

            Refresh();
        }

        private void Refresh()
        {
            #region Assert
            Debug.Assert(GameDatas != null, nameof(GameDatas) + " != null");
            Debug.Assert(DownloadDatas != null, nameof(DownloadDatas) + " != null");
            #endregion
            MainGrid.Children.Clear();
            if (_hasReaded)
            {
                string gamedatasPath = "./.rh/.settings/gamedatas.json";
                if (!File.Exists(gamedatasPath))
                {
                    MessageBox.Show($"文件 {gamedatasPath} 不存在，无法写入数据。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }

                var gamedatas = new
                {
                    GameDatas.GamesNum,
                    Games = GameDatas.Games.Take(GameDatas.GamesNum).Select(g => new { name = g.Name, path = g.Path }).ToArray()
                };
                string jsonData = JsonConvert.SerializeObject(gamedatas, Formatting.Indented);
                File.WriteAllText(gamedatasPath, jsonData);
            }
            GameDatas.ReadData();
            DownloadDatas.ReadData();
            _hasReaded = true;
            for (int i = 0; i < GameDatas.GamesNum; i++)
            {
                AddGridItem(i);
            }
            if (GameDatas.GamesNum == 0)
            {
                var label = new Label
                {
                    Content = $"你看起来还没有游戏，去下载界面看看吧",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = 30,
                };
                var button = new Button
                {
                    Content = $"→ 前往下载界面 ←",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = 20,
                    Padding = new Thickness(10),
                    Margin = new Thickness(2, 0, 2, 0),
                    Background = new SolidColorBrush(Color.FromRgb(0, 200, 200)), // 获取按钮颜色
                    Foreground = Brushes.White,
                    Tag = $"gotodownload" // 存储唯一标识
                };
                var stackPanel = new StackPanel();
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(button);
                var border = new Border
                {
                    Child = stackPanel,
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
            for (int i = 0; i < DownloadDatas.DownloadNum; i++)
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
            TabGameSettings.Background = Brushes.Transparent;
            TabDownloads.Background = Brushes.Transparent;
            TabDownloadMods.Background = Brushes.Transparent;
            TabSettings.Background = Brushes.Transparent;
            TabHome.Foreground = Brushes.White;
            TabGames.Foreground = Brushes.White;
            TabGameSettings.Foreground = Brushes.White;
            TabDownloads.Foreground = Brushes.White;
            TabDownloadMods.Foreground = Brushes.White;
            TabSettings.Foreground = Brushes.White;

            // 隐藏所有视图
            HomeView.Visibility = Visibility.Collapsed;
            GamesView.Visibility = Visibility.Collapsed;
            GameSettingsView.Visibility = Visibility.Collapsed;
            DownloadsView.Visibility = Visibility.Collapsed;
            DownloadModsView.Visibility = Visibility.Collapsed;
            SettingsView.Visibility = Visibility.Collapsed;

            Button? clickedButton = sender as Button;

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
                case "gamesettings":
                    GameSettingsView.Visibility = Visibility.Visible;
                    break;
                case "downloads":
                    DownloadsView.Visibility = Visibility.Visible;
                    break;
                case "downloadmods":
                    DownloadModsView.Visibility = Visibility.Visible;
                    break;
                case "settings":
                    SettingsView.Visibility = Visibility.Visible;
                    break;
            }
        }
        
        private void AddGridItem(int index)
        {
            #region Assert
            Debug.Assert(GameDatas != null, nameof(GameDatas) + " != null");
            #endregion
            
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
                Content = $"{GameDatas.Games[index].Name}",
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
            #region Assert
            Debug.Assert(DownloadDatas != null, nameof(DownloadDatas) + " != null");
            #endregion
            
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
                Content = special ? "从本地安装" : $"{DownloadDatas.Downloads[index].Name}",
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

        private void AddGameSettingsGrid(int index)
        {
            #region Assert
            Debug.Assert(GameDatas != null, nameof(GameDatas) + " != null");
            #endregion
            var label = new Label
            {
                Content = $"{GameDatas.Games[index].Name}",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 18
            };

            GameSettingsContainer.Children.Add(label);
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
                #region Assert
                Debug.Assert(GameDatas != null, nameof(GameDatas) + " != null");
                Debug.Assert(DownloadDatas != null, nameof(DownloadDatas) + " != null");
                #endregion
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
                            GameDatas.StartGame(buttonIndex, GameDatas.Games[itemIndex].Path);
                        }
                        else
                        {
                            //MessageBox.Show($"你点击了第 {itemIndex} 组的第 {buttonIndex} 个按钮",
                            //                "操作通知",
                            //                MessageBoxButton.OK,
                            //                MessageBoxImage.Information);
                            GameSettingsContainer.Children.Clear(); //gamesettingsview
                            AddGameSettingsGrid(itemIndex);
                            TabButton_Click(TabGameSettings, null!); // 跳转到“管理游戏”选项卡
                        }
                    }
                    else if (t == 3)
                    {
                        int itemIndex = int.Parse(parts[1]);
                        int buttonIndex = int.Parse(parts[2]);
                        if (itemIndex != -1)
                        {
                            string downloadLink = DownloadDatas.Downloads[itemIndex].Links[buttonIndex - 1];
                            MessageBox.Show($"马上程序会弹出一个黑窗口，开始下载文件和解压，请勿关闭窗口，请耐心等待操作完成");
                            Tools.DownloadFileAndUnZip($"{DownloadDatas.Downloads[itemIndex].Path}/", downloadLink,
                                DownloadDatas.Downloads[itemIndex].Name);

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
                else if (parts.Length == 0)
                {
                    if (tag == "gotodownload")
                    {
                        TabButton_Click(TabDownloads,null!);
                    }
                }
            }
        }
    }
}
