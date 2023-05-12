using ChatRWKV_PC.ViewModels;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatRWKV_PC.Models
{
    public class ChatInfoModel : BaseViewModel
    {
        
        private string message = string.Empty;
        /// <summary>
        /// 消息主体
        /// </summary>
        public string Message {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
        /// <summary>
        /// 发送者名字
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 角色
        /// </summary>
        public ChatRoleType Role { get; set; } = ChatRoleType.Sender;
        /// <summary>
        /// 消息类型,现在默认是字符串,后期如果有多模态可以修改
        /// </summary>
        public ChatMessageType Type { get; set; } = ChatMessageType.String;
        /// <summary>
        /// 气泡背景色
        /// </summary>
        public SolidColorBrush Background { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5db269"));
        /// <summary>
        /// 头像背景
        /// </summary>
        public SolidColorBrush ImagesBackground { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c75d40"));
        /// <summary>
        /// 头像
        /// </summary>
        public BitmapImage Icon { get; set; } = new BitmapImage();
        /// <summary>
        /// 字体颜色
        /// </summary>
        public SolidColorBrush FontColor { get; set; } = new SolidColorBrush(Colors.Black);
        /// <summary>
        /// 消息气泡宽度
        /// </summary>
        public float MaxWidth { get; set; } = 500;
    }
}
