using ChatRWKV_PC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace ChatRWKV_PC.Views
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            
            InitializeComponent();
            Task.Run(() =>
            {
                CheckEnv();
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                    Close();
                }));
            });
            
        }

        public void CheckEnv()
        {
            //检查必须的环境和文件
            string current = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                //多任务提高加载速度
                List<Task> tasks = new List<Task> 
                {
                        Task.Run(() =>
                        {
                            if (!Directory.Exists(current + "Python3.10"))
                            {
                                OtherUtil.RelativeZipFileRelease("/Resources/Other/Python3.10.zip", current);
                            }
                        }),
                        //Task.Run(() =>
                        //{
                        //    if (!Directory.Exists(current + "Git"))
                        //    {
                        //        OtherUtil.RelativeZipFileRelease("/Resources/Other/Git.zip", current);
                        //    }
                        //}),
                        Task.Run(() =>
                        {
                            if (!Directory.Exists(current + "rwkvcpp"))
                            {
                                OtherUtil.RelativeZipFileRelease("/Resources/Other/rwkvcpp.zip", current);
                            }
                        }),
                        Task.Run(() =>
                        {
                            if (!Directory.Exists(current + "prompt"))
                            {
                                OtherUtil.RelativeZipFileRelease("/Resources/Other/prompt.zip", current);
                            }
                        }),
                        Task.Run(() =>
                        {
                            if (!File.Exists(current + "20B_tokenizer.json"))
                            {
                                OtherUtil.RelativeFileRelease("/Resources/Other/20B_tokenizer.json", current + "20B_tokenizer.json");
                            }
                        }),

                        Task.Run(() =>
                        {
                            if(File.Exists("Run.py"))
                                File.Delete("Run.py");
                            OtherUtil.RelativeFileRelease("/Resources/PyFile/Run.py", current + "Run.py");
                        }),
                        Task.Run(() =>
                        {
                            if (!File.Exists(current + "convert_model.py"))
                            {
                                OtherUtil.RelativeFileRelease("/Resources/PyFile/convert_model.py", current + "convert_model.py");
                            }
                        })
                };
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化软件异异常:" + ex.Message, "软件错误");
                this.Close();
            }
            

        }
    }
}
