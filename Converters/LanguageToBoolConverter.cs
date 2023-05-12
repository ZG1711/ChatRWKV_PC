using HandyControl.Properties.Langs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ChatRWKV_PC.Converters
{
    class LanguageToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rs = (string)value;
            string language = (string)parameter;
            if (language.Equals(rs))
            {
                ChangeLanguage(language);
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool rs = (bool)value;
            string language = (string)parameter;
            if (rs)
            {
                ChangeLanguage(language);
                return parameter;
            }
            else
            {
                return null;
            }
        }

        public void ChangeLanguage(string language)
        {
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

                for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                {
                    var dicts = Application.Current.Resources.MergedDictionaries;
                    if (dicts[i].ToString().Contains("Dictionarys") || dicts[i].Source.OriginalString.Contains("/Resources/Dictionarys"))
                    {
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                        Application.Current.Resources.MergedDictionaries.Add(langRd);
                    }
                }
                Properties.Settings.Default.Language = language;
                Properties.Settings.Default.Save();
            }
            
        }
    }
}
