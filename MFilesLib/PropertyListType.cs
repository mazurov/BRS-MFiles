using System;
using System.Collections.Generic;

namespace MFilesLib
{
    public class PropertyListItem
    {
        public PropertyListItem(Guid guid, string value)
        {
            Guid = guid;
            Value = value;
        }

        public Guid Guid { get; set; }
        public string Value { get; set; }
    }

    public class PropertyListType
    {
        public PropertyListType(string vaultName, string type, string propertyName)
        {
            VaultName = vaultName;
            Type = type;
            PropertyName = propertyName;

            Items = new List<PropertyListItem>();

        }

        public string VaultName { get; }
        public string Type { get; }
        public string PropertyName { get; }

        public IList<PropertyListItem> Items { get; set; }

    }
}
