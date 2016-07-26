using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFilesAPI;

namespace MFilesLib
{
    public class ObjectFileWrapper
    {
        private readonly ObjectFile _file;
        private readonly string _sourceUrl;

        public ObjectFileWrapper(ObjectFile file, string sourceUrl = null)
        {
            _file = file;
            _sourceUrl = sourceUrl;
        }
        public string Name => _file.Title;
        public string Extension => _file.Extension;
        public long Size => _file.LogicalSize;

        public string GetUrl(string prefix = null)
        {
            if (_sourceUrl != null)
            {
                return _sourceUrl;
            }
            return $"{prefix ?? ""}{Name}.{Extension}";
        }
    }
}
