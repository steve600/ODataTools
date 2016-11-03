using ODataTools.DtoGenerator.Contracts.Enums;
using System;

namespace ODataTools.DtoGenerator.Events
{
    public class DtoGeneratorLogEntryAddedEventArgs : EventArgs
    {
        public DtoGeneratorLogEntryAddedEventArgs(DtoGeneratorMode generatorMode, string logMessage)
        {
            this.GeneratorMode = generatorMode;
            this.LogMessage = logMessage;
        }

        public DtoGeneratorMode GeneratorMode { get; private set; }
        public string LogMessage { get; private set; }
    }
}
