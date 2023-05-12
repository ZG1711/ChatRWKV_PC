using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace ChatRWKV_PC.Resources.Dictionarys
{
    public partial class JapaneseLanguage
    {
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //使用explorer来打开默认浏览器浏览指定网址
            Process.Start("explorer.exe", ((Hyperlink)sender).NavigateUri.ToString());
        }
    }
}
