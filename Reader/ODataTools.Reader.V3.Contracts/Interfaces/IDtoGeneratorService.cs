using Microsoft.Data.Edm;
using ODataTools.DtoGenerator.Contracts;
using ODataTools.Reader.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
