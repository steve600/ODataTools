using ODataTools.DtoGenerator.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.IO;

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
