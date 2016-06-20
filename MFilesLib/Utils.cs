using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesLib
{

    public class StringUtils
    {
        public delegate string Indexer<T>(T obj);
        public static string Concatenate<T>(IEnumerable<T> collection, Indexer<T> indexer, char separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in collection) sb.Append(indexer(t)).Append(separator);
            if (sb.Length > 0)
            {
                return sb.Remove(sb.Length - 1, 1).ToString();
            }
            return "";
        }
    }

    public class CultureUtils
    {
        private static IEnumerable<RegionInfo> _regionsInfo;
        private static IEnumerable<CultureInfo> _culturesInfo;
        private static ICountries _countriesSource = null;

        private static Dictionary<string, int> _months = new Dictionary<string, int>()
        {
            {"January", 1},
            {"February", 2},
            {"March", 3},
            {"April", 4},
            {"May", 5},
            {"June", 6},
            {"July", 7},
            {"August", 8},
            {"September", 9},
            {"October", 10},
            {"November", 11},
            {"December", 12}
        };

        public static void SetCountriesSource(ICountries countries)
        {
            _countriesSource = countries;
        }

        public static string GetLangTwoLetterCode(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            if (null == _culturesInfo)
            {
                _culturesInfo = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            }

            CultureInfo culture = _culturesInfo.FirstOrDefault(c => c.EnglishName == name);
            return culture != null ? culture.TwoLetterISOLanguageName : null;

        }

        public static string GetCountryTwoLetterCode(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }
            if (null == _regionsInfo)
            {
                _regionsInfo =
                    CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
            }

            var region = _regionsInfo.FirstOrDefault(r => r.EnglishName.Contains(name));

            if (region != null)
            {
                return region.TwoLetterISORegionName;
            }


            return _countriesSource != null ? _countriesSource.GetCountryIsoCode2(name) : null;
        }


    }
}
