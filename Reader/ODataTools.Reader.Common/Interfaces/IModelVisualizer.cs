using ODataTools.ModelVisualizer.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.Reader.Common.Common
{
    public interface IModelVisualizer
    {
        ObservableCollection<EntityVertex> GetEntitiesForVisualization(string sourceFile);
    }
}
