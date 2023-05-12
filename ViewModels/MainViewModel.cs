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
        /// 是否自动关闭命令行
        /// </summary>
        public static bool IsAutoCmd { get; set; } = Properties.Settings.Default.IsAutoCmd;
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
                    //清掉注释解除加速
                    if (File.Exists("Python3.10/Lib/site-packages/rwkv/model.py"))
                    {
                        string[] lines = File.ReadAllLines("Python3.10/Lib/site-packages/rwkv/model.py");
                        string[] writeLines = new string[lines.Length - 1];
                        int j = 0;
                        for (int i = 0; i < lines.Length; i++)
                        {
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
                                        writeLines[j] = lines[i].Replace("#", "");
                                        i++; j++;
                                    }
                                }

                            }
                            else
                            {
                                writeLines[j] = lines[i];
                            }
                            j++;
                        }
                        File.WriteAllLines("Python3.10/Lib/site-packages/rwkv/model.py", writeLines);
                    }
                    _IsSpeed = false;
                }
                OnPropertyChanged(nameof(IsSpeed));
            }
        }
        /// <summary>
        /// 是否显示RWKV进程
        /// </summary>
        public bool ShowRWKV { get; set; } = Properties.Settings.Default.ShowRWKV;
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
        /// 系统字体列表
        /// </summary>
        public ObservableCollection<string> FontFamilys { get; set; }
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
        public string Language { get => language; set => language = value; }
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
                            if (!IsAutoCmd)
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
                string arguments = downStr;
                Process gitProcess = new Process();
                gitProcess.StartInfo.FileName = "cmd";
                if (!IsAutoCmd)
                    gitProcess.StartInfo.Arguments = "/k " + arguments;
                else
                    gitProcess.StartInfo.Arguments = "/c " + arguments;
                gitProcess.StartInfo.UseShellExecute = false;
                gitProcess.StartInfo.CreateNoWindow = false;
                gitProcess.Start();
                gitProcess.WaitForExit();
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
                string downStr = "cd ChatRWKV && " + currentDirectory + "Git/bin/git.exe fetch --all && " + currentDirectory + "Git/bin/git.exe reset --hard origin/main && " + currentDirectory + "Git/bin/git.exe pull";
                Debug.WriteLine(downStr);
                string arguments = downStr;
                Process gitProcess = new Process();
                gitProcess.StartInfo.FileName = "cmd";
                if (!IsAutoCmd)
                    gitProcess.StartInfo.Arguments = "/k " + arguments;
                else
                    gitProcess.StartInfo.Arguments = "/c " + arguments;
                gitProcess.StartInfo.UseShellExecute = false;
                gitProcess.StartInfo.CreateNoWindow = false;
                gitProcess.Start();
                gitProcess.WaitForExit();
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
            if (!IsAutoCmd)
                PipUtils.CreateType = "/k ";
            else
                PipUtils.CreateType = "/c ";
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
                if (!IsAutoCmd)
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
                Process ChatPyProcess = new Process();
                ChatPyProcess.StartInfo.FileName = "cmd";
                ChatPyProcess.StartInfo.Arguments = "/k " + arguments;
                ChatPyProcess.StartInfo.UseShellExecute = false;
                ChatPyProcess.StartInfo.CreateNoWindow = false;
                ChatPyProcess.Start();
            });
        }, param =>
        {
            if (!File.Exists("ChatRWKV/v2/chat.py"))
                return false;
            return true;
        });
        }

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
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.UseShellExecute = false;
            string args = string.Format("{0}python.exe convert_model.py --in {1} --out {2} --strategy \"{3}\"",
                PipUtils.PyPath,
                ModelName,
                AppDomain.CurrentDomain.BaseDirectory + "ConverterModel/" + Path.GetFileName(modelName),
                Strategy);
            if (IsAutoCmd)
                process.StartInfo.Arguments = "/c " + args;
            else
                process.StartInfo.Arguments = "/k " + args;
            process.Start();
            process.WaitForExit();
        });
        }

        public BtnCommand SaveSettingsBtnCommand { get => new BtnCommand(param =>
        {
            Properties.Settings.Default.ModelName = ModelName;
            Properties.Settings.Default.Strategy = Strategy;
            Properties.Settings.Default.TokenCount = TokenCount;
            Properties.Settings.Default.Temperature = Temperature;
            Properties.Settings.Default.TopP = TopP;
            Properties.Settings.Default.PresencePenalty = PresencePenalty;
            Properties.Settings.Default.CountPenalty = CountPenalty;
            Properties.Settings.Default.IsAutoCmd = IsAutoCmd;
            Properties.Settings.Default.ShowRWKV = ShowRWKV;
            Properties.Settings.Default.IsSpeed = IsSpeed;
            Properties.Settings.Default.ChatLang = CHAT_LANG;
            try
            {
                Properties.Settings.Default.Save();
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
                    JObject jsonObj = new JObject();
                    jsonObj["operation"] = "start";
                    jsonObj["RWKV_CUDA_ON"] = IsSpeed ? "1" : "0";
                    jsonObj["modelName"] = ModelName.Replace("\\", "/").Replace(".pth", "").Trim();
                    jsonObj["strategy"] = Strategy;
                    jsonObj["CHAT_LANG"] = CHAT_LANG;
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonObj.ToString());
                    RWKV_PROCESS_CLIENT.Send(bytes);
                    //接收数据
                    byte[] buffer = new byte[1024];
                    int length = RWKV_PROCESS_CLIENT.Receive(buffer);
                    string responseData = Encoding.UTF8.GetString(buffer, 0, length);
                    if (responseData.Equals("1"))
                    {

                        jsonObj = new JObject();
                        jsonObj["operation"] = "GetName";
                        bytes = Encoding.UTF8.GetBytes(jsonObj.ToString());
                        RWKV_PROCESS_CLIENT.Send(bytes);
                        //接收数据
                        buffer = new byte[1024];
                        length = RWKV_PROCESS_CLIENT.Receive(buffer);
                        responseData = Encoding.UTF8.GetString(buffer, 0, length);
                        string[] names = responseData.Split(",");
                        if (names.Length == 2)
                        {
                            UserName = names[0];
                            BotName = names[1];
                        }
                        RunStatus = 1;
                    }
                    else
                        RunStatus = -1;
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
            byte[] bytes = Encoding.UTF8.GetBytes(jsonObj.ToString());
            RWKV_PROCESS_CLIENT.Send(bytes);
            await Task.Run(() =>
            {
                //接收数据
                byte[] buffer = new byte[1024];
                int length = RWKV_PROCESS_CLIENT.Receive(buffer);
                string responseData = Encoding.UTF8.GetString(buffer, 0, length);
                if (responseData.Equals("success"))
                    RunStatus = 0;
                else
                    RunStatus = -1;
            });
            if (RunStatus == 0)
            {
                button.IsEnabled = true;
            }
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
                if (RunStatus != 1)
                {
                    System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_NotRunMsg") as string);
                    return;
                }
                if (string.IsNullOrEmpty(InputMsg) || RWKV_PROCESS_CLIENT == null || (InputMsg.IndexOf("+++") != -1 && ChatInfoModels.Count == 0))
                {
                    return;
                }
                btn.IsEnabled = false;
                
                //特殊处理+++指令
                if (InputMsg.IndexOf("+++") == -1)
                {
                    ChatInfoModels.Add(new ChatInfoModel()
                    {
                        Role = HandyControl.Data.ChatRoleType.Sender,
                        Message = inputMsg,
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
                    
                    int index = ChatInfoModels.Count - 1;
                    //要传递的参数
                    jsonObj["ctx"] = InputMsg;
                    jsonObj["temperature"] = float.Parse(Temperature);
                    jsonObj["top_p"] = float.Parse(TopP);
                    jsonObj["alpha_frequency"] = float.Parse(CountPenalty);
                    jsonObj["alpha_presence"] = float.Parse(PresencePenalty);
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonObj.ToString());
                    RWKV_PROCESS_CLIENT.Send(bytes);
                    //处理续写的指令,其它指令需要处理可以优化这部分
                    if (InputMsg.Contains("+gen"))
                    {
                        ChatInfoModels[index].Message = InputMsg.Replace("+gen ", "");
                    }
                    index = ChatInfoModels.Count - 1;
                    while (true)
                    {
                        try
                        {
                            //接收数据
                            byte[] buffer = new byte[1024];
                            int length = RWKV_PROCESS_CLIENT.Receive(buffer);
                            string responseData = Encoding.UTF8.GetString(buffer, 0, length);
                            //遇到结束符跳出
                            if (responseData.Equals("----end----"))
                            {
                                ChatInfoModels[index].Message = ChatInfoModels[index].Message.Replace("\n\n", "");
                                OutCount += ChatInfoModels[index].Message.TrimStart().Length;
                                break;
                            }
                            else
                            {
                                OutMsg += responseData;
                                ChatInfoModels[index].Message += responseData;
                            }
                        }
                        catch
                        {
                            break;
                        }

                    }
                });
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
                if (RunStatus != 1)
                {
                    System.Windows.MessageBox.Show(Application.Current.FindResource("Lang_NotRunMsg") as string);
                    return;
                }
                if (string.IsNullOrEmpty(InputMsg) || RWKV_PROCESS_CLIENT == null || (InputMsg.IndexOf("+++") != -1 && ChatInfoModels.Count == 0))
                {
                    return;
                }
                btn.IsEnabled = false;
                //批量处理要输入的消息
                string[] inputs = InputMsg.Split("\n");
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

                        int index = ChatInfoModels.Count - 1;
                        //要传递的参数
                        jsonObj["ctx"] = sendMsg;
                        jsonObj["temperature"] = float.Parse(Temperature);
                        jsonObj["top_p"] = float.Parse(TopP);
                        jsonObj["alpha_frequency"] = float.Parse(CountPenalty);
                        jsonObj["alpha_presence"] = float.Parse(PresencePenalty);
                        byte[] bytes = Encoding.UTF8.GetBytes(jsonObj.ToString());
                        RWKV_PROCESS_CLIENT.Send(bytes);
                        //处理续写的指令,其它指令需要处理可以优化这部分
                        if (InputMsg.Contains("+gen"))
                        {
                            ChatInfoModels[index].Message = InputMsg.Replace("+gen ", "");
                        }
                        index = ChatInfoModels.Count - 1;
                        while (true)
                        {
                            try
                            {
                                //接收数据
                                byte[] buffer = new byte[1024];
                                int length = RWKV_PROCESS_CLIENT.Receive(buffer);
                                string responseData = Encoding.UTF8.GetString(buffer, 0, length);
                                //遇到结束符跳出
                                if (responseData.Equals("----end----"))
                                {
                                    ChatInfoModels[index].Message = ChatInfoModels[index].Message.Replace("\n\n", "");
                                    OutCount += ChatInfoModels[index].Message.TrimStart().Length;
                                    break;
                                }
                                else
                                {
                                    OutMsg += responseData;
                                    ChatInfoModels[index].Message += responseData;
                                }
                            }
                            catch
                            {
                                break;
                            }

                        }
                    }

                });
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
            FontFamilys = GetSystemFontFamilys();
            Task.Run(() =>
            {
                while (!IsRunPyProcess)
                {
                    StartSocket();
                    Thread.Sleep(100);
                }
            });
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
                    string arguments = string.Format(PipUtils.PyPath + "python.exe Run.py");
                    PyProcess = new Process();
                    PyProcess.StartInfo.FileName = "cmd";
                    if (!IsAutoCmd)
                        PyProcess.StartInfo.Arguments = "/k " + arguments;
                    else
                        PyProcess.StartInfo.Arguments = "/c " + arguments;
                    PyProcess.StartInfo.UseShellExecute = false;
                    if (ShowRWKV)
                    {
                        PyProcess.StartInfo.CreateNoWindow = false;
                    }
                    else
                    {
                        PyProcess.StartInfo.CreateNoWindow = true;
                    }
                    PyProcess.Start();
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
                        if (number > 0 && number < 2000)
                        {
                            Uri uri = new Uri("/Resources/PyFile/wkv_cuda10.pyd", UriKind.Relative);
                            StreamResourceInfo info = Application.GetResourceStream(uri);
                            using (var stream = new FileStream("Python3.10/Lib/site-packages/wkv_cuda.pyd", FileMode.OpenOrCreate))
                            {
                                info.Stream.CopyTo(stream);
                            }
                        }
                        else if (number > 2000 && number < 3000)
                        {
                            Uri uri = new Uri("/Resources/PyFile/wkv_cuda20.pyd", UriKind.Relative);
                            StreamResourceInfo info = Application.GetResourceStream(uri);
                            using (var stream = new FileStream("Python3.10/Lib/site-packages/wkv_cuda.pyd", FileMode.OpenOrCreate))
                            {
                                info.Stream.CopyTo(stream);
                            }
                        }
                        else
                        {
                            Uri uri = new Uri("/Resources/PyFile/wkv_cuda3+.pyd", UriKind.Relative);
                            StreamResourceInfo info = Application.GetResourceStream(uri);
                            using (var stream = new FileStream("Python3.10/Lib/site-packages/wkv_cuda.pyd", FileMode.OpenOrCreate))
                            {
                                info.Stream.CopyTo(stream);
                            }
                        }
                    }
                    catch
                    {
                        Uri uri = new Uri("/Resources/PyFile/wkv_cuda3+.pyd", UriKind.Relative);
                        StreamResourceInfo info = Application.GetResourceStream(uri);
                        using (var stream = new FileStream("Python3.10/Lib/site-packages/wkv_cuda.pyd", FileMode.OpenOrCreate))
                        {
                            info.Stream.CopyTo(stream);
                        }
                    }
                }

            }
        }
    }
}
