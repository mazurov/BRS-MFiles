using System;
using System.Collections.Generic;
using TreatiesService.Treaties;
using TreatiesService.Countries;

namespace TreatiesService
{
    public class CountriesClient
    {
        private readonly PlayersEntities _ctx;
        private Dictionary<string, string> _isoCodes2;

        public CountriesClient(string serviceUri)
        {
            _ctx = new PlayersEntities(new Uri(serviceUri));
            _ctx.IgnoreMissingProperties = true;
        }

        private Dictionary<string, string> IsoCodes2
        {
            get
            {
                if (_isoCodes2 == null)
                {
                    _isoCodes2 = new Dictionary<string, string>();
                    foreach (var country in _ctx.Countries)
                    {
                        if (!string.IsNullOrEmpty(country.countryNameEn) && !_isoCodes2.ContainsKey(country.countryNameEn.ToLower()))
                        {
                            _isoCodes2.Add(country.countryNameEn.ToLower(), country.countryCode2);
                        }
                    }
                }
                return _isoCodes2;
            }
        }

        public string GetCountryIsoCode2(string countryName)
        {
            if (countryName == null)
            {
                return null;
            }
            var key = countryName.ToLower();
            return IsoCodes2.ContainsKey(key) ? IsoCodes2[key] : null;
        }
    }
}
