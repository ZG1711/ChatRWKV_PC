using ChatRWKV_PC.Commands;
using ChatRWKV_PC.Properties;
using ChatRWKV_PC.Utils;
using ChatRWKV_PC.Views;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;
using ChatRWKV_PC.Models;
using System.Windows.Documents;

namespace ChatRWKV_PC.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string modelName = Settings.Default.ModelName;
        private string strategy = Settings.Default.Strategy;
        private string ChatLang = Settings.Default.ChatLang;
        private string tokenCount = Settings.Default.TokenCount;
        private string temperature = Settings.Default.Temperature;
        private string topP = Settings.Default.TopP;
        private string presencePenalty = Settings.Default.PresencePenalty;
        private string countPenalty = Settings.Default.CountPenalty;
        private bool _IsSpeed = Settings.Default.IsSpeed;
        private int runStatus = 0;
        public static string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private string rwkvCurrentVersion = string.Empty;
        private string rwkvLastVersion = string.Empty;
        private string torchCurrentVersion = string.Empty;
        private string torchLastVersion = string.Empty;
        private string numpyCurrentVersion = string.Empty;
        private string numpyLastVersion = string.Empty;
        private string tokenizersCurrentVersion = string.Empty;
        private string tokenizersLastVersion = string.Empty;
        private string userName = string.Empty;
        private string botName = string.Empty;
        private string inputMsg = string.Empty;
        private string outMsg = string.Empty;
        private string language = Settings.Default.Language;
        private int outCount = 0;

        /// <summary>
        /// 模型路径
        /// </summary>
        public string ModelName
        {
            get => modelName;
            set
            {
                modelName = value;
                OnPropertyChanged("ModelName");
            }
        }
        /// <summary>
        /// 策略
        /// </summary>
        public string Strategy
        {
            get => strategy;
            set
            {
                strategy = value;
                OnPropertyChanged("Strategy");
            }
        }
        /// <summary>
        /// 语言
        /// </summary>
        public string CHAT_LANG
        {
            get => ChatLang;
            set
            {
                ChatLang = value;
                OnPropertyChanged("CHAT_LANG");
            }
        }
        /// <summary>
        /// 每次生成的长度
        /// </summary>
        public string TokenCount
        {
            get => tokenCount;
            set
            {
                tokenCount = value;
                OnPropertyChanged("TokenCount");
            }
        }
        /// <summary>
        /// 变化
        /// </summary>
        public string Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }
        /// <summary>
        /// 创新度
        /// </summary>
        public string TopP
        {
            get => topP;
            set
            {
                topP = value;
                OnPropertyChanged("TopP");
            }
        }
        /// <summary>
        /// 类似字
        /// </summary>
        public string PresencePenalty
        {
            get => presencePenalty;
            set
            {
                presencePenalty = value;
                OnPropertyChanged("PresencePenalty");
            }
        }
        /// <summary>
        /// 减少写类似字概率
        /// </summary>
        public string CountPenalty
        {
            get => countPenalty;
            set
            {
                countPenalty = value;
                OnPropertyChanged("CountPenalty");
            }
        }
        /// <summary>
        /// 是否开启加速
        /// 自动处理加速包的位置
        /// </summary>
        public bool IsSpeed
        {
            get => _IsSpeed;
            set
            {

                if (value)
                {
                    Wkv_CUDA();
                    if (File.Exists("Python3.10/Lib/site-packages/rwkv/model.py"))
                    {
                        SetSpeed();
                        _IsSpeed = true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_NotRWKVLibMsg") as string);
                        _IsSpeed = false;  
                    }
                }
                else
                {
                    ClearSpeed();
                    _IsSpeed = false;
                }
                OnPropertyChanged(nameof(IsSpeed));
            }
        }
        /// <summary>
        /// rwkv pip包当前安装版本
        /// </summary>
        public string RwkvCurrentVersion
        {
            get => rwkvCurrentVersion;
            set
            {
                rwkvCurrentVersion = value;
                OnPropertyChanged("RwkvCurrentVersion");
            }
        }
        /// <summary>
        /// rwkv pip包最新版本
        /// </summary>
        public string RwkvLastVersion
        {
            get => rwkvLastVersion;
            set
            {
                rwkvLastVersion = value;
                OnPropertyChanged("RwkvLastVersion");
            }
        }
        /// <summary>
        /// torch+cu117当前安装版本
        /// </summary>
        public string TorchCurrentVersion
        {
            get => torchCurrentVersion;
            set
            {
                torchCurrentVersion = value;
                OnPropertyChanged("TorchCurrentVersion");
            }
        }
        /// <summary>
        /// torch+cu117最新版本
        /// </summary>
        public string TorchLastVersion
        {
            get => torchLastVersion;
            set
            {
                torchLastVersion = value;
                OnPropertyChanged("TorchLastVersion");
            }
        }
        /// <summary>
        /// Numpy当前安装版本
        /// </summary>
        public string NumpyCurrentVersion
        {
            get => numpyCurrentVersion;
            set
            {
                numpyCurrentVersion = value;
                OnPropertyChanged("NumpyCurrentVersion");
            }
        }
        /// <summary>
        /// Numpy最新版本
        /// </summary>
        public string NumpyLastVersion
        {
            get => numpyLastVersion;
            set
            {
                numpyLastVersion = value;
                OnPropertyChanged("NumpyLastVersion");

            }
        }
        /// <summary>
        /// tokenizers当前安装版本
        /// </summary>
        public string TokenizersCurrentVersion
        {
            get => tokenizersCurrentVersion;
            set
            {

                tokenizersCurrentVersion = value;
                OnPropertyChanged("TokenizersCurrentVersion");
            }
        }
        /// <summary>
        /// tokenizers最新版本
        /// </summary>
        public string TokenizersLastVersion
        {
            get => tokenizersLastVersion;
            set
            {

                tokenizersLastVersion = value;
                OnPropertyChanged("TokenizersLastVersion");
            }
        }
        /// <summary>
        /// 运行状态
        /// -1 启动失败
        /// 0 未运行
        /// 1 运行中
        /// 2 启动中
        /// </summary>
        public int RunStatus
        {
            get => runStatus;
            set
            {
                runStatus = value;
                OnPropertyChanged("RunStatus");
            }
        }
        /// <summary>
        /// 对话中发送者名字
        /// </summary>
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        /// <summary>
        /// ChatRWKV名字
        /// </summary>
        public string BotName
        {
            get => botName;
            set
            {
                botName = value;
                OnPropertyChanged(nameof(BotName));
            }
        }
        /// <summary>
        /// RWKV进程是否运行中
        /// </summary>
        public bool IsRunPyProcess { get; set; } = false;
        /// <summary>
        /// RWKV cmd进程
        /// </summary>
        public Process? PyProcess { get; set; } = null;
        /// <summary>
        /// rwkv连接客户端
        /// </summary>
        public Socket? RWKV_PROCESS_CLIENT { get; set; }
        /// <summary>
        /// 要发送的消息
        /// </summary>
        public string InputMsg
        {
            get => inputMsg;
            set
            {
                inputMsg = value;
                OnPropertyChanged("InputMsg");
            }
        }
        /// <summary>
        /// rwkv输出的消息
        /// </summary>
        public string OutMsg
        {
            get => outMsg;
            set
            {
                outMsg = value;
                OnPropertyChanged("OutMsg");
            }
        }
        /// <summary>
        /// 消息列表
        /// </summary>
        public ObservableCollection<ChatInfoModel> ChatInfoModels { get; set; } = new ObservableCollection<ChatInfoModel>();
        /// <summary>
        /// 字数统计
        /// </summary>
        public int OutCount
        {
            get => outCount;
            set
            {
                outCount = value;
                OnPropertyChanged(nameof(OutCount));
            }
        }
        public BtnCommand SwitchSourceBtnCommand
        {
            get => new BtnCommand(param =>
                    {
                        if (param == null)
                            return;
                        string url = (string)param;
                        Task.Run(() =>
                        {
                            if (!Settings.Default.IsAutoCmd)
                                PipUtils.CreateType = "/k ";
                            else
                                PipUtils.CreateType = "/c ";

                            PipUtils.SwithSource(url);
                            GetLibVersion();
                        });
                    });
        }
        /// <summary>
        /// 显示策略按钮命令
        /// </summary>
        public BtnCommand ShowStrategyCommand { get; set; } = new BtnCommand(param =>
        {
            StrategyWindow strategyWindow = new StrategyWindow();
            strategyWindow.Owner = Application.Current.MainWindow;
            strategyWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            strategyWindow.Show();
        });
        /// <summary>
        /// 下载RWKV命令
        /// </summary>
        public BtnCommand GitHubRWKDownloadCommand { get; set; } = new BtnCommand(async param =>
        {
            //下载RWKV
            await Task.Run(() =>
            {
                string downStr = currentDirectory + "Git/bin/git.exe clone https://github.com/BlinkDL/ChatRWKV " + currentDirectory + "ChatRWKV";
                OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, downStr, true);
                if (File.Exists("ChatRWKV/v2/chat.py"))
                {
                    return;
                }
            });
        }, param =>
        {
            //检查目录是否存在
            if (Directory.Exists("ChatRWKV"))
            {
                return false;
            }
            return true;
        });
        /// <summary>
        /// 更新RWKV命令
        /// </summary>
        public BtnCommand GitHubRWKVUpdateCommand { get; set; } = new BtnCommand(async param =>
        {
            //强制更新RWKV
            await Task.Run(() =>
            {
                string updateStr = "cd ChatRWKV && " + currentDirectory + "Git/bin/git.exe fetch --all && " + currentDirectory + "Git/bin/git.exe reset --hard origin/main && " + currentDirectory + "Git/bin/git.exe pull";
                OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, updateStr, true);
                if (File.Exists("ChatRWKV/v2/chat.py"))
                {
                    return;
                }
            });
        }, param =>
        {
            //检查目录是否存在
            if (Directory.Exists("ChatRWKV"))
            {
                return true;
            }
            return false;
        });
        /// <summary>
        /// 更新依赖包命令
        /// </summary>
        public BtnCommand UpgradeBtnCommand { get => new BtnCommand(param =>
        {
            if (param == null)
                return;
            string package = (string)param;
            if (!Settings.Default.IsAutoCmd)
                PipUtils.CreateType = "/k ";
            else
                PipUtils.CreateType = "/c ";
            //需指定版本2.0目前不兼容
            if (package.Equals("torch"))
            {
                UpgradeWindow upgrade = new UpgradeWindow(package, "https://download.pytorch.org/whl/cu117");
                upgrade.Owner = Application.Current.MainWindow;
                upgrade.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                upgrade.ShowDialog();

            }
            else
            {
                UpgradeWindow upgrade = new UpgradeWindow(package);
                upgrade.Owner = Application.Current.MainWindow;
                upgrade.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                upgrade.ShowDialog();
                if (package.Equals("rwkv"))
                {
                    Wkv_CUDA();
                    SetSpeed();
                }
            }
            GetLibVersion();
        }, param =>
        {
            string package = (string)param;
            if (package == "rwkv" && (string.IsNullOrEmpty(RwkvCurrentVersion) || RwkvCurrentVersion.Equals("未安装")))
            {
                return false;
            }
            if (package == "torch" && (string.IsNullOrEmpty(TorchCurrentVersion) || TorchCurrentVersion.Equals("未安装")))
            {
                return false;
            }
            if (package == "numpy" && (string.IsNullOrEmpty(NumpyCurrentVersion) || NumpyCurrentVersion.Equals("未安装")))
            {
                return false;
            }
            if (package == "tokenizers" && (string.IsNullOrEmpty(TokenizersCurrentVersion) || TokenizersCurrentVersion.Equals("未安装")))
            {
                return false;
            }
            return true;
        });
        }
        /// <summary>
        /// 一键安装依赖命令
        /// </summary>
        public BtnCommand AllInstallBtnCommand { get => new BtnCommand(param =>
        {
            Task.Run(() =>
            {
                if (!Settings.Default.IsAutoCmd)
                    PipUtils.CreateType = "/k ";
                else
                    PipUtils.CreateType = "/c ";
                //目前需要特殊处理torch的版本,不能最新
                PipUtils.Installer("torch==1.13.1+cu117 --extra-index-url https://download.pytorch.org/whl/cu117");
                PipUtils.Installer("rwkv numpy prompt_toolkit ninja");
                GetLibVersion();
            });
        });
        }
        /// <summary>
        /// 编辑chat.py命令
        /// </summary>
        public BtnCommand EditBtnCommand { get => new BtnCommand(param =>
        {
            EditWindow editWindow = new EditWindow();
            editWindow.Owner = Application.Current.MainWindow;
            editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            editWindow.Show();
        },param =>
        {
            if (!File.Exists("ChatRWKV/v2/chat.py"))
                return false;
            return true;
        });
        }
        /// <summary>
        /// 启动GitHub版下的chat.py
        /// </summary>
        public BtnCommand StartGitHubChatPyCommand { get => new BtnCommand(param =>
        {
            Task.Run(() =>
            {
                string arguments = string.Format(PipUtils.PyPath + "python.exe " + currentDirectory + "/ChatRWKV/v2/chat.py");
                OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, arguments);
            });
        }, param =>
        {
            if (!File.Exists("ChatRWKV/v2/chat.py"))
                return false;
            return true;
        });
        }
        /// <summary>
        /// 转换模型命令
        /// </summary>
        public BtnCommand ConverterModelCommand { get => new BtnCommand(param =>
        {
            string modelName = ModelName;
            if (modelName.IndexOf(".pth") == -1)
                modelName += ".pth";

            if (!File.Exists(modelName))
            {
                System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_NotModelMsg") as string);
                return;
            }
            //检测转换文件是否存在
            if (!File.Exists("convert_model.py"))
            {
                Uri uri = new Uri("/convert_model.py", UriKind.Relative);
                StreamResourceInfo info = Application.GetResourceStream(uri);
                using (var stream = new FileStream("convert_model.py", FileMode.Create))
                {
                    info.Stream.CopyTo(stream);
                }
            }
            if (!Directory.Exists("ConverterModel"))
            {
                Directory.CreateDirectory("ConverterModel");
            }
            string args = string.Format("{0}python.exe convert_model.py --in {1} --out {2} --strategy \"{3}\"",
                PipUtils.PyPath,
                ModelName,
                AppDomain.CurrentDomain.BaseDirectory + "ConverterModel/" + Path.GetFileName(modelName),
                Strategy);
            OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, args);
        });
        }

        public BtnCommand SaveSettingsBtnCommand { get => new BtnCommand(param =>
        {
            Settings.Default.ModelName = ModelName;
            Settings.Default.Strategy = Strategy;
            Settings.Default.TokenCount = TokenCount;
            Settings.Default.Temperature = Temperature;
            Settings.Default.TopP = TopP;
            Settings.Default.PresencePenalty = PresencePenalty;
            Settings.Default.CountPenalty = CountPenalty;
            Settings.Default.IsSpeed = IsSpeed;
            Settings.Default.ChatLang = CHAT_LANG;
            try
            {
                Settings.Default.Save();
                System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_SaveSettingsSuccessMsg") as string);
            }
            catch
            {
                System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_SaveErrorMsg") as string);
            }
        });
        }
        /// <summary>
        /// 启动ChatRWKV进程
        /// </summary>
        public BtnCommand StartRWKVCommand { get => new BtnCommand(async param =>
        {
            await Task.Run(() =>
            {
                if (RunStatus == 0 || RunStatus == -1)
                {
                    if (string.IsNullOrEmpty(ModelName))
                    {
                        RunStatus = -1;
                        return;
                    }
                    RunStatus = 2;
                    StartSocket();
                    Thread.Sleep(Settings.Default.Socket_StartSleep);
                    if (RWKV_PROCESS_CLIENT == null)
                    {
                        try
                        {
                            RWKV_PROCESS_CLIENT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            IPEndPoint server_ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
                            RWKV_PROCESS_CLIENT.Connect(server_ip);
                        }
                        catch
                        {
                            RWKV_PROCESS_CLIENT = null;
                            RunStatus = -1;
                            return;
                        }
                    }
                    //不限制超时时间
                    RWKV_PROCESS_CLIENT.ReceiveTimeout = 0;
                    int buffer_size = 1024;
                    JObject jsonObj = new JObject();
                    jsonObj["operation"] = "start";
                    jsonObj["RWKV_CUDA_ON"] = Settings.Default.IsSpeed ? "1" : "0";
                    jsonObj["modelName"] = ModelName.Replace("\\", "/").Replace(".pth", "").Trim();
                    jsonObj["strategy"] = Strategy;
                    jsonObj["CHAT_LANG"] = CHAT_LANG;
                    jsonObj["Language"] = Settings.Default.Language.Equals("简体中文") ? "Chinese" : Settings.Default.Language;

                    if (SendMsgObject(RWKV_PROCESS_CLIENT, jsonObj).Equals("1"))
                    {
                        //获取prompt中设定的user和Bot
                        jsonObj = new JObject();
                        jsonObj["operation"] = "GetName";
                        string responseData = SendMsgObject(RWKV_PROCESS_CLIENT, jsonObj);
                        string[] names = responseData.Split(",");
                        if (names.Length == 2)
                        {
                            UserName = names[0];
                            BotName = names[1];
                        }
                        RunStatus = 1;
                    }
                    else
                    {
                        RWKV_PROCESS_CLIENT = null;
                        IsRunPyProcess = false;
                        RunStatus = -1;
                    }
                }
            });
        },param =>
        {
            if (RunStatus == 0 || RunStatus == -1)
                return true;
            else
                return false;
        });
        }
        /// <summary>
        /// 停止ChatRWKV进程
        /// </summary>
        public BtnCommand StopBtnCommand { get => new BtnCommand(async param =>
        {
            Button button = (Button)param;
            button.IsEnabled = false;
            JObject jsonObj = new JObject();
            jsonObj["operation"] = "stop";
            await Task.Run(() =>
            {
                string msg = "";
                //只允许启动单个socket进程,所以这里就这么处理可以了
                if (RunStatus == 1)
                    msg = SendMsgObject(RWKV_PROCESS_CLIENT, jsonObj);
                else if (RwkvCppRunStatus == 1)
                    msg = SendMsgObject(RWKV_CPP_PROCESS_CLIENT, jsonObj);
                else
                    return;
                if (msg.Equals("success"))
                {
                    RunStatus = 0;
                    RwkvCppRunStatus = 0;
                }
                else if (string.IsNullOrEmpty(msg))
                {
                    RunStatus = 0;
                    RwkvCppRunStatus = 0;
                }
                else
                {
                    RunStatus = -1;
                    RwkvCppRunStatus = 0;
                }

            });
            if (RunStatus == 0 || RwkvCppRunStatus == 0)
            {
                button.IsEnabled = true;
            }
            //直接干进程
            CloseRwkvProcess();
            CloseRwkvCppProcess();
        });
        }
        /// <summary>
        /// 发送消息命令
        /// </summary>
        public BtnCommand SendMessageCommand
        {
            get => new BtnCommand(async param =>
            {
                Button btn = (Button)param;
                Socket? socket = null;
                if (isSend)
                    return;
                if (RunStatus == 1)
                {
                    socket = RWKV_PROCESS_CLIENT;
                }
                else if (RwkvCppRunStatus == 1)
                {
                    socket = RWKV_CPP_PROCESS_CLIENT;
                }
                else
                {
                    System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_NotRunMsg") as string);
                    return;
                }

                if (string.IsNullOrEmpty(InputMsg) || socket == null || (InputMsg.IndexOf("+++") != -1 && ChatInfoModels.Count == 0))
                {
                    return;
                }
                IsSend = true;
                btn.IsEnabled = false;

                //特殊处理+++指令
                if (InputMsg.IndexOf("+++") == -1)
                {
                    ChatInfoModels.Add(new ChatInfoModel()
                    {
                        Role = HandyControl.Data.ChatRoleType.Sender,
                        Message = InputMsg,
                        ImagesBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c75d40")),
                        Name = UserName
                    });
                    ChatInfoModels.Add(new ChatInfoModel()
                    {
                        Role = HandyControl.Data.ChatRoleType.Receiver,
                        Message = "",
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2c2c2c")),
                        ImagesBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4ea181")),
                        Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/Images/icon.png", UriKind.Relative)),
                        FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                        MaxWidth = 4800,
                        Name = BotName
                    });
                }
                //发送消息部分代码
                await Task.Run(() =>
                {
                    JObject jsonObj = new JObject();
                    jsonObj["operation"] = "send";
                    jsonObj["token_count"] = int.Parse(TokenCount);
                    jsonObj["Language"] = Settings.Default.Language.Equals("简体中文") ? "Chinese" : Settings.Default.Language;
                    int index = ChatInfoModels.Count - 1;
                    //要传递的参数
                    jsonObj["ctx"] = InputMsg;
                    jsonObj["temperature"] = float.Parse(Temperature);
                    jsonObj["top_p"] = float.Parse(TopP);
                    jsonObj["alpha_frequency"] = float.Parse(CountPenalty);
                    jsonObj["alpha_presence"] = float.Parse(PresencePenalty);

                    //处理续写的指令,其它指令需要处理可以优化这部分
                    if (InputMsg.Contains("+gen"))
                    {
                        ChatInfoModels[index].Message = InputMsg.Replace("+gen ", "");
                    }

                    InputMsg = "";
                    if (SendMsgObject(socket, jsonObj, true).Equals("SendBack"))
                    {
                        index = ChatInfoModels.Count - 1;

                        MsgReceive(socket, ChatInfoModels[index]);
                    }
                });
                IsSend = false;
                btn.IsEnabled = true;
            });
        }
        /// <summary>
        /// 批量发送消息命令
        /// </summary>
        public BtnCommand BatchSendMessageCommand
        {
            get => new BtnCommand(async param =>
            {
                Button btn = (Button)param;
                Socket? socket = null;
                if (isSend)
                    return;
                if (RunStatus == 1)
                {
                    socket = RWKV_PROCESS_CLIENT;
                }
                else if (RwkvCppRunStatus == 1)
                {
                    socket = RWKV_CPP_PROCESS_CLIENT;
                }
                else
                {
                    System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_NotRunMsg") as string);
                    return;
                }

                if (string.IsNullOrEmpty(InputMsg) || socket == null || (InputMsg.IndexOf("+++") != -1 && ChatInfoModels.Count == 0))
                {
                    return;
                }
                IsSend = true;
                btn.IsEnabled = false;
                IsSend = true;
                //批量处理要输入的消息
                string key = "+frl ";
                string[] inputs = new string[0];
                if (InputMsg.StartsWith(key))
                {
                    string filename = InputMsg.Substring(key.Length);
                    if (File.Exists(filename))
                        inputs = File.ReadAllLines(filename);
                    else
                    {
                        System.Windows.MessageBox.Show($"Not File:{filename}");
                    }
                }
                else
                    inputs = InputMsg.Split("\n");

                //发送消息部分代码
                await Task.Run(() =>
                {
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        string sendMsg = inputs[i].Replace("\\n", "\n").Replace("\r", "");
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ChatInfoModels.Add(new ChatInfoModel()
                            {
                                Role = HandyControl.Data.ChatRoleType.Sender,
                                Message = sendMsg,
                                ImagesBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c75d40")),
                                Name = UserName
                            });
                            ChatInfoModels.Add(new ChatInfoModel()
                            {
                                Role = HandyControl.Data.ChatRoleType.Receiver,
                                Message = "",
                                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2c2c2c")),
                                ImagesBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4ea181")),
                                Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/Images/icon.png", UriKind.Relative)),
                                FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                                MaxWidth = 4800,
                                Name = BotName
                            });
                        });
                        JObject jsonObj = new JObject();
                        jsonObj["operation"] = "send";
                        jsonObj["token_count"] = int.Parse(TokenCount);
                        jsonObj["Language"] = Settings.Default.Language.Equals("简体中文") ? "Chinese" : Settings.Default.Language;
                        int index = ChatInfoModels.Count - 1;
                        //要传递的参数
                        jsonObj["ctx"] = sendMsg;
                        jsonObj["temperature"] = float.Parse(Temperature);
                        jsonObj["top_p"] = float.Parse(TopP);
                        jsonObj["alpha_frequency"] = float.Parse(CountPenalty);
                        jsonObj["alpha_presence"] = float.Parse(PresencePenalty);
                        //处理续写的指令,其它指令需要处理可以优化这部分
                        if (InputMsg.Contains("+gen"))
                        {
                            ChatInfoModels[index].Message = InputMsg.Replace("+gen ", "");
                        }
                        if (SendMsgObject(socket, jsonObj, true).Equals("SendBack"))
                        {
                            index = ChatInfoModels.Count - 1;

                            MsgReceive(socket, ChatInfoModels[index]);
                        }
                    }

                });
                IsSend = false;
                btn.IsEnabled = true;
            });
        }
        /// <summary>
        /// RWKV命令窗口
        /// </summary>
        public BtnCommand CommandDesc
        {
            get => new BtnCommand(param =>
            {
                string value = (string)Application.Current.FindResource("Lang_CommandDescStr");
                System.Windows.MessageBox.Show(@value);
            });
        }
        /// <summary>
        /// 保存对话记录窗口
        /// </summary>
        public BtnCommand SaveRecordCommand
        {
            get => new BtnCommand(param =>
            {
                string writeStr = string.Empty;
                foreach (ChatInfoModel chatInfo in ChatInfoModels)
                {
                    if (chatInfo.Role == HandyControl.Data.ChatRoleType.Sender)
                    {
                        writeStr += string.Format("{0}:{1}\n", UserName, chatInfo.Message);
                    }
                    else
                    {
                        writeStr += string.Format("{0}:{1}\n", BotName, chatInfo.Message);
                    }
                }
                if (string.IsNullOrEmpty(writeStr))
                {
                    return;
                }
                string filename = string.Format("SaveRecocd/{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                try
                {
                    if (!Directory.Exists("SaveRecocd"))
                        Directory.CreateDirectory("SaveRecocd");
                    File.WriteAllText(filename, writeStr);
                    System.Windows.MessageBox.Show((Application.Current.FindResource("Lang_SaveRecordlSuccessMsg") as string) + "\n" + filename);
                }
                catch
                {
                    System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_SaveErrorMsg") as string);
                }
            });
        }
        /// <summary>
        /// 打开指定目录
        /// </summary>
        public BtnCommand OpenFolderCommand
        {
            get => new BtnCommand(param =>
            {
                if (param == null)
                    return;
                string path = (string)param;
                Process.Start("explorer.exe", path);
            });
        }
        public BtnCommand ShowWindowCommand
        {
            get => new BtnCommand(param =>
            {
                try
                {
                    //根据传入的字符串打开窗口,限制在命名空间ChatRWKV_PC.Views下
                    //其它空间需要修改
                    string name = (string)param;
                    Type type = Type.GetType("ChatRWKV_PC.Views." + name);
                    System.Windows.Window window = (System.Windows.Window)Activator.CreateInstance(type);
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Show();
                }
                catch
                {

                }
            });
        }

        public BtnCommand ShowDialogWindowCommand
        {
            get => new BtnCommand(param =>
            {
                try
                {
                    //根据传入的字符串打开窗口,限制在命名空间ChatRWKV_PC.Views下
                    //其它空间需要修改
                    string name = (string)param;
                    Type type = Type.GetType("ChatRWKV_PC.Views." + name);
                    System.Windows.Window window = (System.Windows.Window)Activator.CreateInstance(type);
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();
                }
                catch
                {

                }
            });
        }
        public MainViewModel()
        {
            GetLibVersion();
        }
        ~MainViewModel()
        {
            if(PyProcess != null)
            {
                PyProcess.Kill();
                PyProcess.CloseMainWindow();
                PyProcess.Close();
                PyProcess.Dispose();
            }
            if(RWKV_PROCESS_CLIENT != null)
            {
                RWKV_PROCESS_CLIENT.Close();
                RWKV_PROCESS_CLIENT.Dispose();
            }
        }

        /// <summary>
        /// 查询依赖包信息
        /// </summary>
        public void GetLibVersion()
        {
            //rwkv
            Task.Run(() =>
            {
                RwkvCurrentVersion = PipUtils.GetLocationVersion("rwkv");
                RwkvLastVersion = PipUtils.GetLastVersion("rwkv");
            });
            //torch
            Task.Run(() =>
            {
                TorchCurrentVersion = PipUtils.GetLocationVersionList("torch");
                TorchLastVersion = PipUtils.GetLastVersion("torch", " -i https://download.pytorch.org/whl/cu117");
            });
            //numpy
            Task.Run(() =>
            {
                NumpyCurrentVersion = PipUtils.GetLocationVersion("numpy");
                NumpyLastVersion = PipUtils.GetLastVersion("numpy");
            });
            //tokenizers
            Task.Run(() =>
            {
                TokenizersCurrentVersion = PipUtils.GetLocationVersion("tokenizers");
                TokenizersLastVersion = PipUtils.GetLastVersion("tokenizers");
            });
        }
        /// <summary>
        /// 设置加速包
        /// </summary>
        public static void SetSpeed()
        {
            string[] lines = File.ReadAllLines("Python3.10/Lib/site-packages/rwkv/model.py");
            string[] writeLines = new string[lines.Length + 1];
            int j = 0;
            //遍历处理加速包需要注释的位置
            for (int i = 0; i < lines.Length; i++, j++)
            {
                if (lines[i].Contains("from torch.utils.cpp_extension import load"))
                {
                    if (!lines[i - 1].Contains("import wkv_cuda"))
                    {
                        writeLines[j] = lines[i].Replace("from torch.utils.cpp_extension import load", "import wkv_cuda");
                        j++;
                        while (!lines[i].Contains("@MyStatic"))
                        {
                            if (string.IsNullOrEmpty(lines[i]))
                            {
                                break;
                            }
                            else
                            {
                                writeLines[j] = "#" + lines[i];
                                i++; j++;
                            }

                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    writeLines[j] = lines[i];
                }
            }
            File.WriteAllLines("Python3.10/Lib/site-packages/rwkv/model.py", writeLines);
        }
        /// <summary>
        /// 启动RWKV进程
        /// </summary>
        public void StartSocket()
        {
            if (!IsRunPyProcess)
            {
                if (NumpyCurrentVersion.Equals("未安装")
                    || string.IsNullOrEmpty(NumpyCurrentVersion)
                    || TokenizersCurrentVersion.Equals("未安装")
                    || string.IsNullOrEmpty(TokenizersCurrentVersion)
                    || TorchCurrentVersion.Equals("未安装")
                    || string.IsNullOrEmpty(TorchCurrentVersion)
                    || RwkvCurrentVersion.Equals("未安装")
                    || string.IsNullOrEmpty(RwkvCurrentVersion))
                {
                    IsRunPyProcess = false;
                }
                else
                {
                    IsRunPyProcess = true;
                    //启动软件是开启加速不会自动检查是否将model.py的代码注释
                    //所以在这再做一次检查处理
                    if (IsSpeed)
                    {
                        Wkv_CUDA();
                        if (File.Exists("Python3.10/Lib/site-packages/rwkv/model.py"))
                        {
                            SetSpeed();
                        }
                    }
                    string arguments = string.Format(PipUtils.PyPath + "python.exe Run.py");
                    PyProcess = OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, arguments, isShow: Settings.Default.ShowRWKV);
                }
            }
        }
        /// <summary>
        /// 释放加速包依赖文件
        /// </summary>
        public static void Wkv_CUDA()
        {
            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
            //遍历显卡设备,只识别遍历到的第一张N卡,多卡的这里需要优化
            foreach (ManagementObject obj in objvide.Get())
            {
                string name = obj["Name"].ToString();
                if (name.IndexOf("NVIDIA") != -1)
                {
                    if (File.Exists("Python3.10/Lib/site-packages/wkv_cuda.pyd"))
                        return;
                    try
                    {
                        int number = int.Parse(Regex.Replace(name, @"[^0-9]+", ""));
                        Uri uri = null;
                        if (number > 0 && number < 2000)
                        {
                            uri = new Uri("/Resources/PyFile/wkv_cuda10.pyd", UriKind.Relative);
                        }
                        else if (number > 2000 && number < 3000)
                        {
                            uri = new Uri("/Resources/PyFile/wkv_cuda20.pyd", UriKind.Relative);
                        }
                        else if (number > 3000 && number < 4000)
                        {
                            uri = new Uri("/Resources/PyFile/wkv_cuda3+.pyd", UriKind.Relative);
                        }
                        else if (number > 4000 && number < 5000)
                        {
                            uri = new Uri("/Resources/PyFile/wkv_cuda3+.pyd", UriKind.Relative);
                        }
                        else
                        {
                            uri = new Uri("/Resources/PyFile/wkv_cuda10.pyd", UriKind.Relative);
                        }
                        StreamResourceInfo info = Application.GetResourceStream(uri);
                        using (var stream = new FileStream("Python3.10/Lib/site-packages/wkv_cuda.pyd", FileMode.OpenOrCreate))
                        {
                            info.Stream.CopyTo(stream);
                        }
                    }
                    catch
                    {
                        Uri uri = new Uri("/Resources/PyFile/wkv_cuda10.pyd", UriKind.Relative);
                        StreamResourceInfo info = Application.GetResourceStream(uri);
                        using (var stream = new FileStream("Python3.10/Lib/site-packages/wkv_cuda.pyd", FileMode.OpenOrCreate))
                        {
                            info.Stream.CopyTo(stream);
                        }
                    }
                }

            }
        }
        private string cppModelName = Settings.Default.CppModelName;
        private string promptType = Settings.Default.RwkvCpp_PromptType;
        private string quantizeFormat = Settings.Default.QuantizeFormat;
        private bool isRwkvCppRun = false;
        private bool isSend = false;
        private int rwkvCppRunStatus = 0;

        public int RwkvCppRunStatus
        {
            get => rwkvCppRunStatus;
            set
            {
                rwkvCppRunStatus = value;
                OnPropertyChanged(nameof(RwkvCppRunStatus));
            }
        }

        public Process? RwkvCppProcess { get; set; } = null;
        public Socket? RWKV_CPP_PROCESS_CLIENT { get; set; } = null;

        public bool IsRwkvCppRun
        {
            get => isRwkvCppRun;
            set
            {
                isRwkvCppRun = value;
                OnPropertyChanged(nameof(IsRwkvCppRun));
            }
        }
        public bool IsSend
        {
            get => isSend;
            set
            {
                isSend = value;
                OnPropertyChanged(nameof(IsSend));
            }
        }
        public string CppModelName
        {
            get => cppModelName;
            set
            {
                cppModelName = value;
                OnPropertyChanged("CppModelName");
            }
        }
        public string PromptType
        {
            get => promptType;
            set
            {
                promptType = value;
                OnPropertyChanged(nameof(PromptType));
            }
        }
        public string QuantizeFormat
        {
            get => quantizeFormat;
            set
            {
                quantizeFormat = value;
                OnPropertyChanged(nameof(QuantizeFormat));
            }
        }
        /// 接受消息
        /// </summary>
        /// <param name="chatInfoModel">聊天对象类</param>
        /// <param name="endStr">停止符</param>
        public void MsgReceive(Socket socket, ChatInfoModel chatInfoModel, string endStr = "{----end----}")
        {
            int buffer_size = 1024;
            try
            {
                socket.ReceiveTimeout = Settings.Default.Socket_RecvTimeout;
                while (true)
                {

                    //先接受服务端发过来的长度
                    byte[] lengthBuffer = new byte[buffer_size];
                    socket.Receive(lengthBuffer);
                    int length = int.Parse(Encoding.UTF8.GetString(lengthBuffer, 0, buffer_size));
                    //告诉服务端已经接收到了长度
                    socket.Send(Encoding.UTF8.GetBytes("ready"));
                    //接受指定长度的数据
                    int recv_size = 0;
                    byte[] bytes = new byte[length];
                    while (recv_size < length)
                    {
                        socket.Receive(bytes);
                        recv_size += bytes.Length;
                    }

                    string responseData = Encoding.UTF8.GetString(bytes, 0, length);
                    //遇到停止符后处理个别操作
                    if (responseData.Equals(endStr))
                    {
                        chatInfoModel.Message = chatInfoModel.Message.Replace("\n\n", "");
                        break;
                    }
                    else
                    {
                        chatInfoModel.Message += responseData;
                        if (!responseData.Equals("\n"))
                            OutCount += responseData.Length;
                    }

                }
            }
            catch (SocketException)
            {

            }

        }
        /// <summary>
        /// 清除加速
        /// </summary>
        public void ClearSpeed()
        {
            //清掉注释解除加速
            if (File.Exists("Python3.10/Lib/site-packages/rwkv/model.py"))
            {
                string[] lines = File.ReadAllLines("Python3.10/Lib/site-packages/rwkv/model.py");
                string[] writeLines = new string[lines.Length - 1];
                int j = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    //检测到相关行就开始处理
                    if (lines[i].Contains("import wkv_cuda"))
                    {

                        i += 1;
                        while (!lines[i].Contains("@MyStatic"))
                        {

                            if (string.IsNullOrEmpty(lines[i]))
                            {
                                break;
                            }
                            else
                            {
                                //取消注释
                                writeLines[j] = lines[i].Replace("#", "");
                                i++; j++;
                            }
                        }
                    }
                    else
                    {
                        //没找到会出现数组越界的情况,可以处理也可以不处理
                        //在Task中会自动抛出异常,不影响程序运行
                        try
                        {
                            writeLines[j] = lines[i];
                        }
                        catch
                        {

                        }
                    }
                    j++;
                }
                File.WriteAllLines("Python3.10/Lib/site-packages/rwkv/model.py", writeLines);
            }
        }
        /// <summary>
        /// 发送数据之前先发送长度
        /// </summary>
        /// <param name="sendObj">要发送的消息</param>
        /// <returns></returns>
        public bool SendMsgLength(JObject sendObj, Socket socket)
        {
            try
            {
                byte[] sendLength = Encoding.UTF8.GetBytes(sendObj.ToString());
                socket.Send((Encoding.UTF8.GetBytes(sendLength.Length.ToString())));
                byte[] buffer = new byte[5];
                int length = socket.Receive(buffer);
                string responseData = Encoding.UTF8.GetString(buffer, 0, length);
                if (responseData.Equals("ready"))
                    return true;
                else
                    return false;
            }
            catch (SocketException)
            {
                return false;
            }

        }
        /// <summary>
        /// 发送json对象至socket服务端
        /// </summary>
        /// <param name="sendObj">要发送的json对象</param>
        /// <param name="isSendBack">是否发送后直接返回</param>
        /// <returns>
        /// 空字符,服务端接收数据长度失败
        /// SendBack,不在本函数中处理服务端返回的数据
        /// 其它字符串,在本函数中处理服务端返回的数据并返回字符串
        /// </returns>
        public string SendMsgObject(Socket socket, JObject sendObj, bool isSendBack = false)
        {
            int buffer_size = 1024;
            //确定数据已经发送出去
            try
            {
                if (SendMsgLength(sendObj, socket))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(sendObj.ToString());
                    socket.Send(bytes);
                    if (isSendBack)
                    {
                        return "SendBack";
                    }
                    else
                    {
                        byte[] buffer = new byte[buffer_size];
                        int length = socket.Receive(buffer);
                        string responseData = Encoding.UTF8.GetString(buffer, 0, length);
                        return responseData;
                    }

                }
                else
                {
                    return string.Empty;
                }
            }
            catch (SocketException)
            {
                return string.Empty;
            }
        }
        public void CloseRwkvProcess()
        {
            if (PyProcess != null)
            {
                PyProcess.Kill();
                PyProcess.CloseMainWindow();
                PyProcess.Close();
                PyProcess.Dispose();
                PyProcess = null;
                IsRunPyProcess = false;
            }
            if (RWKV_PROCESS_CLIENT != null)
            {
                RWKV_PROCESS_CLIENT.Close();
                RWKV_PROCESS_CLIENT.Dispose();
                RWKV_PROCESS_CLIENT = null;
            }
        }
        public void CloseRwkvCppProcess()
        {
            if (RwkvCppProcess != null)
            {
                RwkvCppProcess.Kill();
                RwkvCppProcess.CloseMainWindow();
                RwkvCppProcess.Close();
                RwkvCppProcess.Dispose();
                RwkvCppProcess = null;
                IsRwkvCppRun = false;
            }
            if (RWKV_CPP_PROCESS_CLIENT != null)
            {
                RWKV_CPP_PROCESS_CLIENT.Close();
                RWKV_CPP_PROCESS_CLIENT.Dispose();
                RWKV_CPP_PROCESS_CLIENT = null;
            }
        }
        public BtnCommand GetStrategyCommand
        {
            get => new BtnCommand(param =>
            {
                Uri uri = new Uri("/Resources/PyFile/Check_Strategy.py", UriKind.Relative);
                StreamResourceInfo info = Application.GetResourceStream(uri);
                using (var stream = new FileStream(currentDirectory + "Check_Strategy.py", FileMode.Create))
                {
                    //输出文件
                    info.Stream.CopyTo(stream);
                }
                //创建进程
                string content = File.ReadAllText("Check_Strategy.py");
                File.WriteAllText("Check_Strategy.py", content.Replace("{Model_Name}", ModelName + ".pth"));
                OtherUtil.StartCmdProcess(false, PipUtils.PyPath + "python Check_Strategy.py");
            });
        }
        /// <summary>
        /// 量化模型命令
        /// </summary>
        public BtnCommand RwkvCppQuantizeCommand
        {
            get => new BtnCommand((param) =>
            {
                //rwkv.cpp根目录
                string root = currentDirectory + "rwkvcpp/rwkv/";
                //转换文件路径
                string converterFile = root + "QuantizeRun.py";
                //保存目录
                string savePath = root + "ConverterModels/";
                if (!File.Exists(converterFile))
                {
                    OtherUtil.RelativeFileRelease("/Resources/PyFile/QuantizeRun.py", converterFile);
                }

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                //创建进程
                string arg = string.Format("{0}python {1} {2} {3} {4}", PipUtils.PyPath, converterFile, CppModelName, savePath + Path.GetFileName(CppModelName), QuantizeFormat);
                OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, arg);
            });
        }
        public BtnCommand RwkvCppRunCommand
        {
            get => new BtnCommand(async param =>
            {
                //rwkv.cpp根目录
                string root = currentDirectory + "rwkvcpp/rwkv/";
                string runFile = root + "RwkvCppRun.py";

                OtherUtil.RelativeFileRelease("/Resources/PyFile/RwkvCppRun.py", runFile);
                await Task.Run(() =>
                {
                    if (RwkvCppRunStatus == 0 || RwkvCppRunStatus == -1)
                    {
                        if (string.IsNullOrEmpty(CppModelName))
                        {
                            RwkvCppRunStatus = -1;
                            return;
                        }
                        RwkvCppRunStatus = 2;
                        StartRwkvCppSocket();
                        Thread.Sleep(Settings.Default.Socket_StartSleep);
                        if (RWKV_CPP_PROCESS_CLIENT == null)
                        {
                            try
                            {
                                RWKV_CPP_PROCESS_CLIENT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPEndPoint server_ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
                                RWKV_CPP_PROCESS_CLIENT.Connect(server_ip);
                            }
                            catch
                            {
                                RWKV_CPP_PROCESS_CLIENT = null;
                                RwkvCppRunStatus = -1;
                                return;
                            }
                        }
                        //不限制超时时间
                        RWKV_CPP_PROCESS_CLIENT.ReceiveTimeout = 0;
                        int buffer_size = 1024;
                        JObject jsonObj = new JObject();
                        jsonObj["operation"] = "start";
                        jsonObj["PROMPT_TYPE"] = PromptType;
                        jsonObj["modelName"] = CppModelName.Replace("\\", "/").Trim();
                        jsonObj["CHAT_LANG"] = CHAT_LANG;
                        jsonObj["Language"] = Settings.Default.Language.Equals("简体中文") ? "Chinese" : Settings.Default.Language;

                        if (SendMsgObject(RWKV_CPP_PROCESS_CLIENT, jsonObj).Equals("1"))
                        {
                            //获取prompt中设定的user和Bot
                            jsonObj = new JObject();
                            jsonObj["operation"] = "GetName";
                            string responseData = SendMsgObject(RWKV_CPP_PROCESS_CLIENT, jsonObj);
                            string[] names = responseData.Split(",");
                            if (names.Length == 2)
                            {
                                UserName = names[0];
                                BotName = names[1];
                            }
                            RwkvCppRunStatus = 1;
                        }
                        else
                        {
                            RWKV_CPP_PROCESS_CLIENT = null;
                            RwkvCppRunStatus = -1;
                        }
                    }
                });
            }, param =>
            {
                if (RwkvCppRunStatus == 0 || RwkvCppRunStatus == -1)
                    return true;
                else
                    return false;
            });
        }

        /// <summary>
        /// 启动RWKV进程
        /// </summary>
        public void StartRwkvCppSocket()
        {
            if (RwkvCppRunStatus == 2)
            {
                OtherUtil.RwkvCppDllRelease(Settings.Default.CpuInstruction);
                string arguments = string.Format(PipUtils.PyPath + "python.exe {0}", currentDirectory + "rwkvcpp/rwkv/RwkvCppRun.py");
                RwkvCppProcess = OtherUtil.StartCmdProcess(Settings.Default.IsAutoCmd, arguments, isShow: Settings.Default.ShowRwkvCpp);
            }
        }
        public BtnCommand ModelLanguageCommand
        {
            get => new BtnCommand((param) =>
            {
                try
                {

                    Settings.Default.ChatLang = param.ToString();
                    Settings.Default.Save();
                }
                catch
                {

                }
            });
        }
        public BtnCommand PromptTypeCommand
        {
            get => new BtnCommand((param) =>
            {
                try
                {

                    Settings.Default.RwkvCpp_PromptType = param.ToString();
                    Settings.Default.Save();
                }
                catch
                {

                }
            });
        }
        public string cpp_LANGUAGE = Settings.Default.Cpp_LANGUAGE;

        public string CppLANGUAGE
        {
            get => cpp_LANGUAGE;
            set
            {
                cpp_LANGUAGE = value;
                OnPropertyChanged(nameof(CppLANGUAGE));
            }
        }
        public BtnCommand CppModelLanguageCommand
        {
            get => new BtnCommand((param) =>
            {
                try
                {

                    Settings.Default.Cpp_LANGUAGE = param.ToString();
                    Settings.Default.Save();
                }
                catch
                {

                }
            });
        }

        public BtnCommand ClearChatCommand
        {
            get => new BtnCommand(param =>
            {
                ChatInfoModels.Clear();
                OutCount = 0;
            });
        }

        public BtnCommand DialogCommand
        {
            get => new BtnCommand(param =>
            {
                System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_MainView_Dialog_ModelDesc") as string);
            });
        }
    }
}
