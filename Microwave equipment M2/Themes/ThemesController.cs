﻿using System;
using System.Windows;

namespace Microwave_equipment_M2.Themes
{
    public static class ThemesController
    {
        public static ThemeTypes CurrentTheme { get; set; }

        public enum ThemeTypes
        {
            Light, Dark
        }

        public static ResourceDictionary ThemeDictionary
        {
            get { return Application.Current.Resources.MergedDictionaries[0]; }
            set { Application.Current.Resources.MergedDictionaries[0] = value; }
        }

        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }

        public static void SetTheme(ThemeTypes theme)
        {
            string themeName = string.Empty;
            CurrentTheme = theme;
            switch (theme)
            {
                case ThemeTypes.Light:
                    themeName = "LightTheme";
                    break;
                case ThemeTypes.Dark:
                    themeName = "DarkTheme";
                    break;
                default:
                    break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                {
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
                }
            }
            catch (Exception) { }

        }
    }
}
