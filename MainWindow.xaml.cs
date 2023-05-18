using ChatRWKV_PC.Utils;
using ChatRWKV_PC.ViewModels;
using HandyControl.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatRWKV_PC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        /// <summary>
        /// 当前输出框的滚动条高度
        /// </summary>
        public double CurrentScrollableHeight = 0;
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            InitView();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            //处理没有关闭的进程
            MainViewModel mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel != null)
            {
                if (mainViewModel.PyProcess != null)
                {
                    mainViewModel.PyProcess.Kill();
                    mainViewModel.PyProcess.CloseMainWindow();
                    mainViewModel.PyProcess.Close();
                    mainViewModel.PyProcess.Dispose();
                }
                mainViewModel.RWKV_PROCESS_CLIENT?.Dispose();
            }
        }
        public void InitView()
        {

            //初始化语言
            OtherUtil.ChangeLanguage(Properties.Settings.Default.Language);
        }

        private void ModelName_PreviewDrop(object sender, DragEventArgs e)
        {
            //文件拖入事件处理
            TextBox textBox = (TextBox)sender;

            if (textBox.IsLoaded)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string filename = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                    if (filename.IndexOf(".pth") != -1)
                    {
                        MainViewModel mainViewModel = DataContext as MainViewModel;
                        mainViewModel.ModelName = filename.Replace("\\", "/").Replace(".pth", "");
                    }
                    if (filename.IndexOf(".bin") != -1)
                    {
                        MainViewModel mainViewModel = DataContext as MainViewModel;
                        mainViewModel.CppModelName = filename.Replace("\\", "/");
                    }
                }

            }
        }

        private void ModelName_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //使用explorer来打开默认浏览器浏览指定网址
            Process.Start("explorer.exe", ((Hyperlink)sender).NavigateUri.ToString());
        }

        private void ListBoxChat_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //处理滚动条自动滚动
            ScrollViewer sv = e.OriginalSource as ScrollViewer;
            if (sv != null)
            {
                if (CurrentScrollableHeight < sv.ScrollableHeight)
                {
                    CurrentScrollableHeight = sv.ScrollableHeight;
                    sv.ScrollToEnd();
                }


            }
        }
        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            //文件拖入事件处理
            TextBox textBox = (TextBox)sender;

            if (textBox.IsLoaded)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    textBox.Text += ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString().Replace("\\", "/");
                }

            }
        }
    }
}
