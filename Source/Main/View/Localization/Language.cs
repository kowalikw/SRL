using System;
using System.Globalization;

namespace SRL.Main.View.Localization
{
    public enum Language
    {
        English = 0,
        Polish
    }

    public static class LanguageExtensions
    {
        public static CultureInfo GetCultureInfo(this Language lang)
        {
            switch (lang)
            {
                case Language.Polish:
                    return new CultureInfo("pl-PL");
                case Language.English:
                default:
                    return new CultureInfo("en");
            }
        }

        public static Language GetLanguage(CultureInfo culture)
        {
            switch (culture.TwoLetterISOLanguageName)
            {
                case "pl":
                    return Language.Polish;
                case "en":
                    return Language.English;
                default:
                    throw new ArgumentException("Language not supported.");
            }
        }
    }
}
