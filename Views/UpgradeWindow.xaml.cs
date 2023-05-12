using ChatRWKV_PC.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
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
using System.Windows.Shapes;

namespace ChatRWKV_PC.Views
{
    /// <summary>
    /// UpgradeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpgradeWindow : Window
    {
        public string PyPackageName = string.Empty;

        public string UrlPath = string.Empty;

        public string SelectVersion = string.Empty;

        public UpgradeWindow(string package,string param = "")
        {
            InitializeComponent();
            ConfirmBtn.IsEnabled = false;
            PyPackageName = package;
            UrlPath = param;
            Task.Run(() =>
            {
                string allVersion = PipUtils.GetAllVersion(package, string.IsNullOrEmpty(param) ? param : " -i " + param);
                if (allVersion.Equals("获取失败"))
                {
                    return;
                }
                else
                {
                    string[] list = allVersion.Replace("可更新版本：", "").Split(",");
                    
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        AllVersionComboBox.ItemsSource = list;
                        ConfirmBtn.IsEnabled = true;
                    });
                }
            });
            
        }

        private void AllVersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender != null)
            {
                ComboBox comboBox = (ComboBox)sender;
                SelectVersion = (string)comboBox.SelectedValue;
            }
        }

        private async void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(UrlPath))
                {
                    PipUtils.Upgrade(PyPackageName + "==" + SelectVersion.Trim());
                }
                else
                {
                    PipUtils.Upgrade(PyPackageName + "==" + SelectVersion.Trim() + " --extra-index-url " + UrlPath);
                }
                
            });
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
