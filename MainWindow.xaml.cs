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
            //作用于启动时选择
            for (int i = 0; i < FontFamilysComboBox.Items.Count; i++)
            {
                if (FontFamilysComboBox.Items[i].Equals(Properties.Settings.Default.FontFamilyName))
                    FontFamilysComboBox.SelectedIndex = i;
            }
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
                }

            }
        }

        private void ModelName_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void FontFamilysComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            
            if (comboBox != null && comboBox.IsInitialized && comboBox.IsLoaded && comboBox.SelectedValue != null)
            {
                string fontName = comboBox.SelectedValue.ToString();
                FontFamily = new FontFamily(fontName);
                Properties.Settings.Default.FontFamilyName = fontName;
                Properties.Settings.Default.Save();
            }
            
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
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (sender as RadioButton);
            if (radioButton.IsLoaded)
            {
                string language = radioButton.Content.ToString();
                Properties.Settings.Default.Language = language;
                Properties.Settings.Default.Save();
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
                    if (this.Resources.MergedDictionaries.Count > 0)
                    {
                        Resources.MergedDictionaries.Clear();
                    }
                    Resources.MergedDictionaries.Add(langRd);
                }
            }
        }
    }
}
