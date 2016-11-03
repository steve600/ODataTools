using System.Collections.Generic;
using System.IO;

namespace ODataTools.DtoGenerator.Contracts.Interfaces
{
    public interface IDtoGenerator
    {
        Dictionary<FileInfo, string> GenerateDtoClassesForModel(DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs");
    }
}
