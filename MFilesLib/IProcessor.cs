using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFilesAPI;
namespace MFilesLib
{
    public interface IProcessor
    {
        IProcessorContext CreateContext();
    }
    public interface IProcessorContext
    {
        void ProcessObject(ObjectVersionWrapper obj);
        void ProcessListProperty(PropertyListType obj);
    }
}
