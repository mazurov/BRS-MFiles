using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MFilesAPI;

namespace MFilesLib
{
    public class VaultWrapper
    {
        public static string[] DocTypeKeys =
        {
            "Class",
            "Additional classes",
            "Class group"
        };

        public static readonly string[] ChemicalKeys =
        {
            "Term-Chemical",
            "Chemical",
            "Chemicals",
            "All Chemicals",
            "AIII Category",
            "Annex III - Chemical"
        };

        public static readonly string[] MeetingKeys =
        {
            "CRM-Meeting",
            "Meeting Acronym",
            "Meeting Acronym (list)"
        };

        public static readonly string[] MeetingTypeKeys =
        {
            "CRM-Meeting-type"
        };

        public static readonly string[] AuthorKeys =
        {
            "Author(s)"
        };



        public static readonly string[] CorporateAuthorKeys =
        {
            "Corporate Author"
        };

        public static readonly string[] ProgramKeys =
        {
            "Term-Programme",
            "Programme/Subject Matter"
        };

        public static readonly string[] TermKeys =
        {
            "Term-Topics",
            "Term-ScientificTechnicalPublications"
        };

        public static readonly string[] TagsKeys =
        {
            //"Term-Topics",
            //"Term-ScientificTechnicalPublications",
            "Keyphrases",
            "Keywords",
            "Keyword"
        };

        public static readonly string[] DescriptionKeys =
        {
            "Description",
            "Short description (in English)"
        };

        public static readonly string NameKey = "Name or title";
        public static readonly string UnNumberKey = "UN-number";
        public static readonly string LanguageKey = "Language";
        public static readonly string TitleKey = "Title";
        public static readonly string PlayerKey = "Player";
        public static readonly string CopyrightKey = "Photo Credits/Source";
        public static readonly string TransmissionDateKey = "TransmissionDate";
        public static readonly string DateIssuanceSignatureKey = "Date Issuance-Signature";
        public static readonly string DateIssuanceKey = "Date Issuance";
        public static readonly string DateOfCorrespondesKey = "Date of correspondence";
        public static readonly string DateStartKey = "Date Start";
        public static readonly string PublicationDateDisplayKey = "PublicationDateDisplay";
        public static readonly string PublicationDateMonthKey = "PublicationDate-Month";
        public static readonly string PublicationDateYearKey = "PublicationDate-Year";
        public static readonly string PeriodBienniumKey = "Period (Biennium or Year)";
        public static readonly string SourceKey = "Source";

        private Dictionary<string, PropertyDef> _definitions;

        public VaultWrapper(string name, IVault vault)
        {
            Name = name;
            Vault = vault;

        }

        public IVault Vault { get; }


        public string Name { get; }

        public Guid Guid => Guid.Parse(Vault.GetGUID());

        public Dictionary<string, PropertyDef> PropertyDefinitions
        {
            get
            {
                if (_definitions == null)
                {
                    _definitions = new Dictionary<string, PropertyDef>();
                    foreach (PropertyDef pdef in Vault.PropertyDefOperations.GetPropertyDefs())
                    {
                        _definitions.Add(pdef.Name, pdef);
                    }
                }
                return _definitions;
            }
        }

        public PropertyDef GetPropertyDef(int propertyDef)
        {
            return Vault.PropertyDefOperations.GetPropertyDef(propertyDef);
        }

        public PropertyValue GetPropertyValue(ObjVer objVer, string key)
        {
            if (PropertyDefinitions.ContainsKey(key))
            {
                try
                {
                    return Vault.ObjectPropertyOperations.GetProperty(objVer, PropertyDefinitions[key].ID);
                }
                catch (COMException)
                {
                }
            }
            return null;
        }

        public ObjectFile GetObjectFile(ObjVer objVer)
        {
            return Vault.ObjectFileOperations.GetFiles(objVer)[1];
        }

        public string[] GetListValues(string key)
        {
            var result = new List<string>();
            if (PropertyDefinitions.ContainsKey(key))
            {
                var pdef = PropertyDefinitions[key];
                if (pdef.ValueList <= 0) return result.ToArray();
                var items = Vault.ValueListItemOperations.GetValueListItems(pdef.ValueList);
                result.AddRange(from ValueListItem item in items select item.Name);
            }
            return result.ToArray();
        }

        public string[] GetListValues(string[] keys)
        {
            var result = new List<string>();
            foreach (var key in keys)
            {
                result.AddRange(GetListValues(key));
            }
            return result.ToArray();
        }
    }
}