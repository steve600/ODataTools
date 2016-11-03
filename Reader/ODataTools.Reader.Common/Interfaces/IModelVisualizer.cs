using ODataTools.ModelVisualizer.Contracts.Model;
using System.Collections.ObjectModel;

namespace ODataTools.Reader.Common.Common
{
    public interface IModelVisualizer
    {
        ObservableCollection<EntityVertex> GetEntitiesForVisualization(string sourceFile);
    }
}
