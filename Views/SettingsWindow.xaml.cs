using ChatRWKV_PC.Utils;
using System;
using System.Collections.Generic;
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
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //作用于启动时选择字体
            for (int i = 0; i < FontFamilysComboBox.Items.Count; i++)
            {
                if (FontFamilysComboBox.Items[i].Equals(Properties.Settings.Default.FontFamilyName))
                    FontFamilysComboBox.SelectedIndex = i;
            }
        }
        private void FontFamilysComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.IsInitialized && comboBox.IsLoaded)
            {
                string fontName = comboBox.SelectedValue.ToString();
                OtherUtil.ChangeFontFamily(this, fontName);
                OtherUtil.ChangeFontFamily(Application.Current.MainWindow, fontName);
                Properties.Settings.Default.FontFamilyName = fontName;
                Properties.Settings.Default.Save();
            }

        }
    }
}
