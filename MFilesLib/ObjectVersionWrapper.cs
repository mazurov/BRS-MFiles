using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFilesAPI;

namespace MFilesLib
{
    public class ListProperty
    {
        public ListProperty(ILookup lookup)
        {
            IsStringProperty = false;
            if (lookup.DisplayIDAvailable)
            {
                Guid guid;
                if (System.Guid.TryParse(lookup.DisplayID, out guid))
                {
                    Guid = guid;
                }
            }
            Value = lookup.DisplayValue;
        }

        public ListProperty(string value)
        {
            IsStringProperty = true;
            Value = value;
        }
        public Guid? Guid { get; }

        public string Value { get; }

        public bool IsStringProperty { get; }
    }

    public class ObjectVersionWrapper
    {
        private readonly ObjectVersion _objVer;
        private readonly VaultWrapper _vault;
        private readonly Dictionary<string, PropertyValue> _propertieValues;
        private ObjectFileWrapper _file;

        public ObjectVersionWrapper(ObjectVersion obj, Vault vault, string vaultName)
        {
            _objVer = obj;
            _vault = new VaultWrapper(vaultName, vault);
            VaultName = vaultName;
            _propertieValues = new Dictionary<string, PropertyValue>();
            

        }

        public ObjectFileWrapper File => _file ??
                                         (_file =
                                             new ObjectFileWrapper(_vault.Vault.ObjectFileOperations.GetFiles(_objVer.ObjVer)[1],
                                                 VaultName == "Intranet" ? SourceUrl : null));

        public Guid Guid => Guid.Parse(_objVer.ObjectGUID);
        public string VaultName { get; }
        public string UnNumber => GetStringValue(VaultWrapper.UnNumberKey);
        public string Name => GetStringValue(VaultWrapper.NameKey);
        public string Title => GetStringValue(VaultWrapper.TitleKey);
        public string Description => GetStringValue(VaultWrapper.DescriptionKeys);
        public DateTime ModifiedDate => _objVer.LastModifiedUtc;
        public DateTime CreatedDate => _objVer.CreatedUtc;
        
        public string Language => GetStringValue(VaultWrapper.LanguageKey);
        public string Player => GetStringValue(VaultWrapper.PlayerKey);
        public string Author => GetStringValue(VaultWrapper.AuthorKeys);
        public string CorporateAuthor => GetStringValue(VaultWrapper.CorporateAuthorKeys);

        public string Copyright => GetStringValue(VaultWrapper.CopyrightKey);
        public string Country => GetStringValue(VaultWrapper.PlayerKey);




        public IList<ListProperty> Types => GetListValues(VaultWrapper.DocTypeKeys);
        public IList<ListProperty> Chemicals => GetListValues(VaultWrapper.ChemicalKeys);
        public IList<ListProperty> Programs => GetListValues(VaultWrapper.ProgramKeys);
        public IList<ListProperty> Terms => GetListValues(VaultWrapper.TermKeys);
        public IList<ListProperty> Tags => GetListValues(VaultWrapper.TagsKeys);
        public IList<ListProperty> Meetings => GetListValues(VaultWrapper.MeetingKeys);
        public IList<ListProperty> MeetingsTypes => GetListValues(VaultWrapper.MeetingTypeKeys);

        public string SourceUrl
        {
            get
            {
                var val = GetStringValue(VaultWrapper.SourceKey);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    val = val.Trim();
                    if (Uri.IsWellFormedUriString(val, UriKind.Absolute))
                    {
                        return val;
                    }
                }
                return null;
            }
        }

        public Tuple<DateTime, DateTime> GetPeriod()
        {
            var periods = GetStringValues(VaultWrapper.PeriodBienniumKey);

            var iperiods = new List<int>();
            foreach (var period in periods)
            {
                var startAndEnd = period.Split('-');
                foreach (var p in startAndEnd)
                {
                    var year = 0;
                    if (int.TryParse(p, out year))
                    {
                        iperiods.Add(year);
                    }
                }
            }

            DateTime? periodStartDate = null;
            DateTime? periodEndDate = null;
            if (iperiods.Count > 0)
            {
                periodStartDate = new DateTime(iperiods[0], 1, 1);
            }

            if (iperiods.Count > 1)
            {
                periodEndDate = new DateTime(iperiods[iperiods.Count - 1], 1, 1);
            }

            if (periodStartDate != null && periodEndDate != null)
            {
                return new Tuple<DateTime, DateTime>(periodStartDate.Value, periodEndDate.Value);
            }
            return null;
        }


        public DateTime PublicationDate
        {
            get
            {

                var d = GetDateTimeValue(VaultWrapper.TransmissionDateKey);
                if (d.HasValue)
                {
                    return d.Value;
                }

                d = GetDateTimeValue(VaultWrapper.DateIssuanceKey);
                if (d.HasValue)
                {
                    return d.Value;
                }

                d = GetDateTimeValue(VaultWrapper.DateIssuanceSignatureKey);
                if (d.HasValue)
                {
                    return d.Value;
                }
                d = GetDateTimeValue(VaultWrapper.DateOfCorrespondesKey);
                if (d.HasValue)
                {
                    return d.Value;
                }

                d = GetDateTimeValue(VaultWrapper.DateStartKey);
                if (d.HasValue)
                {
                    return d.Value;
                }

                var sd = GetStringValue(VaultWrapper.PublicationDateDisplayKey);
                try
                {
                    var sdDate = DateTime.ParseExact(sd, "MMMM yyyy", CultureInfo.CurrentCulture);
                    return sdDate;
                }
                catch (FormatException)
                {

                }

                var pubMonth = GetStringValue(VaultWrapper.PublicationDateMonthKey);
                var pubYear = GetStringValue(VaultWrapper.PublicationDateYearKey);

                if (!(string.IsNullOrWhiteSpace(pubMonth) || string.IsNullOrWhiteSpace(pubYear)))
                {
                    try
                    {
                        var sdDate = DateTime.ParseExact(pubMonth + " " + pubYear, "MMMM yyyy", CultureInfo.CurrentCulture);
                        return sdDate;
                    }
                    catch (FormatException)
                    {

                    }
                }

                return CreatedDate;
            }
        }




        private PropertyValue GetPropertyValue(string key)
        {
            if (!_propertieValues.ContainsKey(key))
            {
                _propertieValues[key] = _vault.GetPropertyValue(_objVer.ObjVer, key);
            }
            return _propertieValues[key];
        }

        public IList<ListProperty> GetListValues(string key)
        {
            var result = new List<ListProperty>();

            var propertyValue = GetPropertyValue(key);
            if (null != propertyValue)
            {
                var propertyDef = _vault.PropertyDefinitions[key];

                if (propertyDef.ValueList > 0)
                {
                    result.AddRange(from Lookup lookup in propertyValue.Value.GetValueAsLookups()
                                    where !String.IsNullOrWhiteSpace(lookup.DisplayValue) && !lookup.Deleted
                                    group lookup by lookup.DisplayValue into g
                                    select new ListProperty(g.First()));
                }
                else
                {
                    if (!String.IsNullOrEmpty(propertyValue.Value.DisplayValue))
                    {
                        result.Add(new ListProperty(propertyValue.Value.DisplayValue));
                    }
                }
            }

            return result;
        }

        public IList<ListProperty> GetListValues(string[] keys)
        {
            var result = new List<ListProperty>();
            foreach (var k in keys)
            {
                result.AddRange(GetListValues(k));
            }
            return result;
        }

        public ListProperty GetListValue(string key)
        {
            var values = GetListValues(key);
            return values.Count > 0 ? values[0] : null;
        }

        public ListProperty GetListValue(string[] keys)
        {
            return keys.Select(GetListValue).FirstOrDefault(value => value != null);
        }

        public string GetStringValue(string key)
        {
            var values = GetListValues(key);
            return StringUtils.Concatenate(values, (ListProperty p) => p.Value, ',');
        }

        private string GetStringValue(string[] keys)
        {
            var values = GetListValues(keys);
            foreach (var v in values)
            {
                return v.Value;
            }
            return "";
        }

        private IEnumerable<string> GetStringValues(string key)
        {
            return GetListValues(key).Select(v => v.Value).ToList();
        }

        private IList<string> GetStringValues(string[] keys)
        {
            var result = new List<string>();
            foreach (var key in keys)
            {
                result.AddRange(GetStringValues(key));
            }
            return result;
        }

        private TypedValue GetTypedValue(string key)
        {
            PropertyValue propertyValue = GetPropertyValue(key);
            return propertyValue?.TypedValue;
        }

        public int? GetIntegerValue(string key)
        {
            TypedValue typedValue = GetTypedValue(key);
            return typedValue?.Value;
        }

        public DateTime? GetDateTimeValue(string key)
        {
            var typedValue = GetTypedValue(key);
            if (null != typedValue && !typedValue.IsNULL())
            {
                return typedValue.Value;
            }
            return null;
        }
    }
}
