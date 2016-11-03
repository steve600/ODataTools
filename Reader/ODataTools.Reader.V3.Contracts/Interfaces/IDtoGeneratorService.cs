using Microsoft.Data.Edm;
using ODataTools.DtoGenerator.Contracts;
using System.Collections.Generic;
using System.IO;

namespace ODataTools.DtoGenerator.Interfaces
{
    /// <summary>
    /// Interface for the DTO generator service
    /// </summary>
    public interface IDtoGeneratorService
    {
        Dictionary<FileInfo, string> GenerateDtoClassesForModel(IEdmModel model, DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs");
    }
}
