using ChatRWKV_PC.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;

namespace ChatRWKV_PC.Utils
{
    public class OtherUtil
    { 
        public static void RelativeFileRelease(string resourcesPath,string targPath = "",FileMode mode = FileMode.Create)
        {
            Uri uri = new Uri(resourcesPath, UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            using (var stream = new FileStream(targPath, mode))
            {
                info.Stream.CopyTo(stream);
            }
        }
        public static void RelativeZipFileRelease(string resourcesPath, string targPath = "", FileMode mode = FileMode.Create)
        {
            Uri uri = new Uri(resourcesPath, UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            using (ZipArchive archive = new ZipArchive(info.Stream))
            {
                archive.ExtractToDirectory(targPath);
            }
        }
        /// <summary>
        /// 启动一个cmd进程
        /// </summary>
        /// <param name="isAutoClose">是否自动关闭命令行</param>
        /// <param name="arg">命令行参数</param>
        /// <param name="isWait">为真只有关闭了命令行才能操作界面</param>
        /// <param name="isShow">是否显示窗口</param>
        /// <returns>返回进程</returns>
        public static Process StartCmdProcess(bool isAutoClose,string arg,bool isWait = false,bool isShow = true)
        {
            //创建进程
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            if (isAutoClose)
                process.StartInfo.Arguments = "/c " + arg;
            else
                process.StartInfo.Arguments = "/k " + arg;
            process.StartInfo.UseShellExecute = false;
            if(!isShow)
                process.StartInfo.CreateNoWindow = true;
            else 
                process.StartInfo.CreateNoWindow = false;
            process.Start();
            if(isWait)
                process.WaitForExit();
            return process;
        }
        /// <summary>
        /// 释放rwkv.dll
        /// </summary>
        /// <param name="value">释放的CPU指令集类型</param>
        public static void RwkvCppDllRelease(int value)
        {
            Uri uri = null;
            switch (value)
            {
                case 0:
                    uri = new Uri("Resources/Dll/rwkv_avx-x64.dll", UriKind.Relative);
                    break;
                case 1:
                    uri = new Uri("Resources/Dll/rwkv_avx2-x64.dll", UriKind.Relative);
                    break;
                case 2:
                    uri = new Uri("Resources/Dll/rwkv_avx512-x64.dll", UriKind.Relative);
                    break;
                default:
                    break;
            }
            StreamResourceInfo info = Application.GetResourceStream(uri);
            using (var stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "rwkvcpp/rwkv.dll", FileMode.Create))
            {
                info.Stream.CopyTo(stream);
            }
        }
        /// <summary>
        /// 改变字体
        /// </summary>
        /// <param name="window"></param>
        /// <param name="fontName"></param>
        public static void ChangeFontFamily(Window window,string fontName)
        {
            FontFamily fontFamily = new FontFamily(fontName);
            window.FontFamily = fontFamily;
            Properties.Settings.Default.FontFamilyName = fontName;
            Properties.Settings.Default.Save();

        }
        /// <summary>
        /// 改变软件语言,不应该在这写,后面再挪了
        /// </summary>
        /// <param name="language"></param>
        public static void ChangeLanguage(string language)
        {
            ResourceDictionary? langRd = null;
            try
            {
                if (language.Equals("简体中文"))
                {
                    langRd = Application.LoadComponent(new Uri(@"Resources\Dictionarys\ChineseLanguage.xaml", UriKind.Relative)) as ResourceDictionary;
                }
                else
                {
                    string xamlName = string.Format("Resources\\Dictionarys\\{0}Language.xaml", language);
                    langRd = Application.LoadComponent(new Uri(xamlName, UriKind.Relative)) as ResourceDictionary;
                }
            }
            catch
            {

            }
            if (langRd != null)
            {

                for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                {
                    var dicts = Application.Current.Resources.MergedDictionaries;
                    if (dicts[i].ToString().Contains("Dictionarys") || dicts[i].Source.OriginalString.Contains("/Resources/Dictionarys"))
                    {
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                        Application.Current.Resources.MergedDictionaries.Add(langRd);
                    }
                }
                Properties.Settings.Default.Language = language;
                Properties.Settings.Default.Save();
            }
        }
    }
}
