using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Generator;

namespace ODataTools.DtoGenerator.Interfaces
{
    public interface IDataSvcUtilGUIService
    {
        DataSvcUtilGUISettings Settings { get; set; }

        void Generate();        

    }
}
