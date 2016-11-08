using Microsoft.Data.Edm;
using ODataTools.ModelVisualizer.Contracts.Interfaces;
using ODataTools.ModelVisualizer.Contracts.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace ODataTools.Reader.V3.Visualization
{
    public class ModelVisualizer : IModelVisualizer
    {
        public ObservableCollection<EntityVertex> GetEntitiesForVisualization(string sourceFile)
        {
            ObservableCollection<EntityVertex> result = new ObservableCollection<EntityVertex>();

            IEdmModel model = ModelReader.Parse(sourceFile);

            if (model != null)
            {
                // Get entities
                var entities = model.SchemaElements.Where(e => e.SchemaElementKind == Microsoft.Data.Edm.EdmSchemaElementKind.TypeDefinition &&
                                                               e is Microsoft.Data.Edm.IEdmEntityType)
                                                   .Cast<IEdmEntityType>();

                if (entities != null && entities.Any())
                {
                    foreach (var e in entities)
                    {                       
                        result.Add(this.CreateEntity(e));
                    }
                }                
            }

            return result;
        }

        private EntityVertex CreateEntity(IEdmEntityType entity)
        {
            EntityVertex newEntity = new EntityVertex(entity.Name);

            // Create properties
            foreach (var p in entity.DeclaredProperties)
            {
                if (p.PropertyKind != EdmPropertyKind.Navigation)
                {
                    newEntity.Fields.Add(new EntityField(p.Name, p.Type.Definition.ToString(), (entity.DeclaredKey == null) ? false : entity.DeclaredKey.Contains(p)));
                }
            }

            // References
            foreach (var np in entity.NavigationProperties())
            {
                newEntity.References.Add(new EntityReference(np.Name, np.ToEntityType().Name));
            }

            return newEntity;
        }
    }
}
