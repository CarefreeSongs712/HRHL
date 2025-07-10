using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace HRHL
{
    public class Tools
    {
        public static void DownloadFile(string name, string url)
        {
            Process.Start($"curl -o {name} \"{url}\"");
        }

        public static void DownloadFileAndUnZip(string path, string? url, string name, string? zippath = null)
        {
            if (url == "")
            {
                MessageBox.Show("该链接无效！");
                return;
            }

            try
            {
                string startupPath = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists($"{startupPath}{path}"))
                {
                    Directory.CreateDirectory($"{startupPath}{path}");
                }

                //  下载
                string gamezippath;
                if (zippath is null)
                {
                    Process process1 = new Process();
                    //MessageBox.Show($"-o \"{startupPath}{name}\" \"{url}\"");
                    process1.StartInfo = new ProcessStartInfo
                    {
                        FileName = "curl.exe",
                        Arguments = $"-o {startupPath}{path}/.game.zip \"{url}\"",
                        UseShellExecute = false,
                        // RedirectStandardOutput = true,  // 重定向标准输出
                        CreateNoWindow = false // 创建窗口
                    };

                    process1.Start();
                    process1.WaitForExit();

                    gamezippath = $"{startupPath}{path}/.game.zip";
                }
                else
                {
                    gamezippath = zippath;
                }


                // 解压
                Process process2 = new Process();
                process2.StartInfo = new ProcessStartInfo
                {
                    FileName = $"7za.exe",
                    Arguments = $"x \"{gamezippath}\" -o\"{startupPath}{path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false, // 重定向标准输出
                    CreateNoWindow = false // 创建窗口
                };
                process2.Start();
                process2.WaitForExit();

                //移动
                string? FolderName = null;
                // 获取所有符合条件的文件夹名称
                var matchingFolders = Directory.EnumerateDirectories($"{startupPath}{path}")
                    .Select(dir => new DirectoryInfo(dir))
                    .Where(dir => dir.Name.Contains("融合版"))
                    .Select(dir => dir.Name); // 获取文件夹名称而不是完整路径
                if (matchingFolders.Count() > 0)
                {
                    foreach (var folder in matchingFolders)
                    {
                        FolderName = folder;
                        Console.WriteLine(folder);
                        break;
                    }
                }

                if (FolderName is not null)
                {
                    MoveDirectory($"{startupPath}{path}/{FolderName}", $"{startupPath}{path}");
                }

                //删除
                File.Delete($"{startupPath}{path}/.game.zip");
                MainWindow.gameDatas.Games[MainWindow.gameDatas.GamesNum] = new GameData(name, $"{path}");
                MainWindow.gameDatas.GamesNum++;

                // 安装B&M
                Process process3 = new Process();
                process3.StartInfo = new ProcessStartInfo
                {
                    FileName = $"7za.exe",
                    Arguments = $"x \"{startupPath}.rh/.apps/modsloader.7z\" -o\"{startupPath}{path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false, // 重定向标准输出
                    CreateNoWindow = false // 创建窗口
                };
                process3.Start();
                process3.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void MoveDirectory(string sourcePath, string targetPath)
        {
            // 递归移动所有文件和子目录
            foreach (string filePath in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
            {
                // 计算目标路径
                string relativePath = filePath.Substring(sourcePath.Length).TrimStart('\\');
                string targetFilePath = Path.Combine(targetPath, relativePath);

                // 确保目标目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

                // 移动文件（覆盖模式）
                File.Move(filePath, targetFilePath, true);
            }

            // 递归删除源目录（可选）
            Directory.Delete(sourcePath, true);
        }


        public static string? SelectFile(string title,string filter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = false,
                CheckFileExists = true
            };

            return openFileDialog.ShowDialog() == true
                ? openFileDialog.FileName
                : null;
        }
    }
}