using ChatRWKV_PC.Commands;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            DataContext = this;
            InitializeComponent();
            Title = "ChatRWKV/v2/chat.py";
            if (File.Exists("ChatRWKV/v2/chat.py"))
            {
                TextEditor.Load("ChatRWKV/v2/chat.py");
                //启用搜索
                ICSharpCode.AvalonEdit.Search.SearchPanel.Install(TextEditor);
            }
        }

        public BtnCommand SaveCommand { get => new BtnCommand(param =>
        {
            if (File.Exists("ChatRWKV/v2/chat.py"))
            {
                TextEditor.Save("ChatRWKV/v2/chat.py");
            }
        });
        }
    }
}
