using ODataTools.DtoGenerator.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Interfaces
{
    public interface IDataSvcUtilGUIService
    {
        DataSvcUtilGUISettings Settings { get; set; }

        void Generate();        

    }
}
