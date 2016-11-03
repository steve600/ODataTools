using ODataTools.DtoGenerator.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
