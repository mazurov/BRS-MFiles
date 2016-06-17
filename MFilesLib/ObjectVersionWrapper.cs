using System;
using System.Collections.Generic;
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

        public ObjectVersionWrapper(ObjectVersion obj, Vault vault, string vaultName)
        {
            _objVer = obj;
            _vault = new VaultWrapper(vaultName, vault);
            VaultName = vaultName;

            _propertieValues = new Dictionary<string, PropertyValue>();
        }

        public Guid Guid => Guid.Parse(_objVer.ObjectGUID);
        public string Title => _objVer.Title;
        public string VaultName { get; }
        public string UnNumber => GetStringValue(VaultWrapper.UnNumberKey);

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
                                    where !String.IsNullOrWhiteSpace(lookup.DisplayValue)
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

        private IList<string> GetStringValues(string key)
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
            if (null != propertyValue)
            {
                return propertyValue.TypedValue;
            }
            return null;
        }

        public int? GetIntegerValue(string key)
        {
            TypedValue typedValue = GetTypedValue(key);
            if (null != typedValue)
            {
                return typedValue.Value;
            }
            return null;
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
