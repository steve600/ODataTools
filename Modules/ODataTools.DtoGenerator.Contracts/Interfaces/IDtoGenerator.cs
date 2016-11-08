using System.Collections.Generic;
using System.IO;

namespace ODataTools.DtoGenerator.Contracts.Interfaces
{
    /// <summary>
    /// Interface for the DTO-Generator
    /// </summary>
    public interface IDtoGenerator
    {
        Dictionary<FileInfo, string> GenerateDtoClassesForModel(string metaDataDocument, DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs");
    }
}
