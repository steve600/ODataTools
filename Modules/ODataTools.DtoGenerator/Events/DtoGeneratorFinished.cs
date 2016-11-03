using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Events
{
    public class DtoGeneratorFinished : PubSubEvent<DtoGeneratorFinishedEventArgs>
    {
    }
}
