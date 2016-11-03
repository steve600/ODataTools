using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Contracts.Interfaces
{
    public interface IDtoGenerator
    {
        Dictionary<FileInfo, string> GenerateDtoClassesForModel(DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs");
    }
}
