namespace ODataTools.ModelVisualizer.Contracts.Model
{
    public class EntityReference
    {
        public EntityReference(string referenceName, string entityName)
        {
            this.ReferenceName = referenceName;
            this.EntityName = entityName;
        }

        public string ReferenceName { get; set; }

        public string EntityName { get; set; }
    }
}
