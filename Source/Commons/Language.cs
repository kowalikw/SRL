using System;
using System.Globalization;

namespace SRL.Commons
{
    /// <summary>
    /// UI languages supported by the application.
    /// </summary>
    public enum Language
    {
        English = 0,
        Polish
    }

    /// <summary>
    /// Helper methods for <see cref="Language"/> enumeration.
    /// </summary>
    public static class LanguageExtensions
    {
        /// <summary>
        /// Gets <see cref="CultureInfo"/> of the language.
        /// </summary>
        /// <param name="lang">Language.</param>
        /// <returns>Corresponding <see cref="CultureInfo"/></returns>
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

        /// <summary>
        /// Gets <see cref="Language"/> of the <see cref="CultureInfo"/> object.
        /// </summary>
        /// <param name="culture">Culture info object.</param>
        /// <returns>Corresponding <see cref="Language"/></returns>
        public static Language GetLanguage(this CultureInfo culture)
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
