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
                                Uri uri = new Uri("/Resources/Other/Python3.10.zip", UriKind.Relative);
                                StreamResourceInfo info = Application.GetResourceStream(uri);
                                using (ZipArchive archive = new ZipArchive(info.Stream))
                                {
                                    archive.ExtractToDirectory(current);
                                }
                            }
                        }),
                        //Task.Run(() =>
                        //{
                        //    if (!Directory.Exists(current + "Git"))
                        //    {
                        //        Uri uri = new Uri("/Resources/Other/Git.zip", UriKind.Relative);
                        //        StreamResourceInfo info = Application.GetResourceStream(uri);
                        //        using (ZipArchive archive = new ZipArchive(info.Stream))
                        //        {
                        //            archive.ExtractToDirectory(current);
                        //        }
                        //    }
                        //}),
                        Task.Run(() =>
                        {
                            if (!Directory.Exists(current + "prompt"))
                            {
                                Uri uri = new Uri("/Resources/Other/prompt.zip", UriKind.Relative);
                                StreamResourceInfo info = Application.GetResourceStream(uri);
                                using (ZipArchive archive = new ZipArchive(info.Stream))
                                {
                                    archive.ExtractToDirectory(current);
                                }
                            }
                        }),
                        Task.Run(() =>
                        {
                            if (!File.Exists(current + "20B_tokenizer.json"))
                            {
                                Uri uri = new Uri("/Resources/Other/20B_tokenizer.json", UriKind.Relative);
                                StreamResourceInfo info = Application.GetResourceStream(uri);
                                using (var stream = new FileStream(current + "20B_tokenizer.json", FileMode.OpenOrCreate))
                                {
                                    info.Stream.CopyTo(stream);
                                }
                            }
                        }),

                        Task.Run(() =>
                        {
                            
                                Uri uri = new Uri("/Resources/PyFile/Run.py", UriKind.Relative);
                                StreamResourceInfo info = Application.GetResourceStream(uri);
                                using (var stream = new FileStream(current + "Run.py", FileMode.Create))
                                {
                                    info.Stream.CopyTo(stream);
                                }
                            
                        }),
                        Task.Run(() =>
                        {
                            if (!File.Exists(current + "convert_model.py"))
                            {
                                Uri uri = new Uri("/Resources/PyFile/convert_model.py", UriKind.Relative);
                                StreamResourceInfo info = Application.GetResourceStream(uri);
                                using (var stream = new FileStream(current + "convert_model.py", FileMode.OpenOrCreate))
                                {
                                    info.Stream.CopyTo(stream);
                                }
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
