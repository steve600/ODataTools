using ODataTools.ModelVisualizer.Contracts.Model;
using System.Collections.ObjectModel;

namespace ODataTools.ModelVisualizer.Contracts.Interfaces
{
    public interface IModelVisualizer
    {
        ObservableCollection<EntityVertex> GetEntitiesForVisualization(string sourceFile);
    }
}
