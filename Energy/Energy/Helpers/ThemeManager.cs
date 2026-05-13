using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Energy.Helpers
{
    public static class ThemeManager
    {
        public enum ThemeType
        {
            Light,
            Dark
        }

        public static void ChangeTheme(ThemeType theme)
        {
            string themePath = theme == ThemeType.Light
                ? "/Themes/LightTheme.xaml"
                : "/Themes/DarkTheme.xaml";

            var newTheme = new ResourceDictionary()
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            var appResources = Application.Current.Resources;
            var mergedDictionaries = appResources.MergedDictionaries;

            // Находим старую тему
            ResourceDictionary oldTheme = null;
            foreach (var dict in mergedDictionaries)
            {
                if (dict.Source?.ToString().Contains("Theme") == true)
                {
                    oldTheme = dict;
                    break;
                }
            }

            // Удаляем старую
            if (oldTheme != null)
                mergedDictionaries.Remove(oldTheme);

            // Добавляем новую (после CommonStyles)
            int index = 0;
            for (int i = 0; i < mergedDictionaries.Count; i++)
            {
                if (mergedDictionaries[i].Source?.ToString().Contains("CommonStyles") == true)
                {
                    index = i;
                    break;
                }
            }

            mergedDictionaries.Insert(index, newTheme);
        }
    }
}
