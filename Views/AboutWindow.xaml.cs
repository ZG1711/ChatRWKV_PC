﻿using ChatRWKV_PC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public string Version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public AboutWindow()
        {
            InitializeComponent();
            //初始化语言
            OtherUtil.ChangeLanguage(Properties.Settings.Default.Language);
        }
    }
}
