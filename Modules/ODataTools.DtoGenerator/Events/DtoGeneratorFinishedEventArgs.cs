using ODataTools.DtoGenerator.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Events
{
    public class DtoGeneratorFinishedEventArgs : EventArgs
    {
        public DtoGeneratorFinishedEventArgs(DtoGeneratorMode generatorMode, Dictionary<FileInfo, string> generatedFiles)
        {
            this.GeneratorMode = generatorMode;
            this.GeneratedFiles = generatedFiles;
        }

        public DtoGeneratorMode GeneratorMode { get; private set; }
        public Dictionary<FileInfo, string> GeneratedFiles { get; }
    }
}
