using GraphX.PCL.Common.Models;
using System.Collections.ObjectModel;

namespace ODataTools.ModelVisualizer.Contracts.Model
{
    public class EntityVertex : VertexBase
    {      

        public EntityVertex(string entityName)
        {
            this.Name = entityName;
            this.Fields = new ObservableCollection<EntityField>();
            this.References = new ObservableCollection<EntityReference>();
        }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fields of the the entity
        /// </summary>
        public ObservableCollection<EntityField> Fields { get; set; }

        /// <summary>
        /// References
        /// </summary>
        public ObservableCollection<EntityReference> References { get; set; }
    }
}
