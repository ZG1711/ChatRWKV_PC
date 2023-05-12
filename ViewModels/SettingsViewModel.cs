using ChatRWKV_PC.Commands;
using ChatRWKV_PC.Properties;
using ChatRWKV_PC.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;

namespace ChatRWKV_PC.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            //系统字体设置
            FontFamilys = GetSystemFontFamilys();
        }
        private string language = Settings.Default.Language;
        private bool isAutoCmd = Settings.Default.IsAutoCmd;
        private bool showRWKV = Settings.Default.ShowRWKV;
        private string socketStartSleep = Settings.Default.Socket_StartSleep.ToString();
        private string socketRecvTimeout = Settings.Default.Socket_RecvTimeout.ToString();
        private int cpuInstruction = Settings.Default.CpuInstruction;
        private bool showRwkvCpp = Settings.Default.ShowRwkvCpp;

        /// <summary>
        /// 是否显示RWKV进程
        /// </summary>
        public bool ShowRWKV {
            get => showRWKV;
            set 
            {
                showRWKV = value;
                Settings.Default.ShowRWKV = value;
                Settings.Default.Save();
                OnPropertyChanged("ShowRWKV");
            }
        }         /// <summary>
                                                                                   /// 语言
                                                                                   /// </summary>
        public string Language
        {
            get => language;
            set {
                language = value; 
                OnPropertyChanged("Language");
            }
        }
        /// <summary>
        /// 系统字体列表
        /// </summary>
        public ObservableCollection<string> FontFamilys { get; set; }

        /// <summary>
        /// 是否自动关闭命令行
        /// </summary>
        public bool IsAutoCmd
        {
            get => isAutoCmd;
            set
            {
                isAutoCmd = value;
                Settings.Default.IsAutoCmd = value;
                Settings.Default.Save();
                OnPropertyChanged("IsAutoCmd");
            }
        }

        public string SocketStartSleep
        {
            get => socketStartSleep;
            set
            {
                socketStartSleep = value;
                try
                {
                    Settings.Default.Socket_StartSleep = int.Parse(value);
                    Settings.Default.Save();
                }
                catch
                {

                }
                OnPropertyChanged(nameof(SocketStartSleep));
            }
        }

        public string SocketRecvTimeout
        {
            get => socketRecvTimeout;
            set
            {
                socketRecvTimeout = value;
                try
                {
                    Settings.Default.Socket_RecvTimeout = int.Parse(value);
                    Settings.Default.Save();
                }
                catch
                {

                }
                OnPropertyChanged(nameof(SocketRecvTimeout));
            }
        }

        public int CpuInstruction
        {
            get => cpuInstruction;
            set
            {
                cpuInstruction = value;
                OnPropertyChanged(nameof(CpuInstruction));
            }
        }

        public bool ShowRwkvCpp { 
            get => showRwkvCpp; 
            set
            {
                showRwkvCpp = value;
                Settings.Default.ShowRwkvCpp = value;
                Settings.Default.Save();
                OnPropertyChanged("ShowRwkvCpp");
            }
        }
        public BtnCommand CpuRadioBtnCommand
        {
            get => new BtnCommand(param =>
            {
                try
                {
                    int value = int.Parse(param.ToString());
                    Settings.Default.CpuInstruction = value;
                    Settings.Default.Save();
                    OtherUtil.RwkvCppDllRelease(value);
                }
                catch
                {

                }
            });
        }

        public BtnCommand LanguageChangeCommand
        {
            get => new BtnCommand(param =>
            {
                Language = param.ToString();
                OtherUtil.ChangeLanguage(param.ToString());
            });
        }

        /// <summary>
        /// 获取系统字体
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetSystemFontFamilys()
        {
            var list = new ObservableCollection<string>();
            foreach (FontFamily family in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary _fontDic = family.FamilyNames;
                if (_fontDic.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    string _fontName = null;
                    if (_fontDic.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out _fontName))
                    {
                        list.Add(_fontName);
                    }
                }
                else
                {
                    list.Add(family.ToString());
                }
            }
            return list;
        }

    }
}
