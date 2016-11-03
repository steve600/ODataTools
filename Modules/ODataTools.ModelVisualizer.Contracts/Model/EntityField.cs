namespace ODataTools.ModelVisualizer.Contracts.Model
{
    public class EntityField
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">The data type.</param>
        /// <param name="isKey">Is key flag.</param>
        public EntityField(string name, string dataType, bool isKey)
        {
            this.Name = name;
            this.DataType = dataType;
            this.IsKey = isKey;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The data type
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Flag if is key field
        /// </summary>
        public bool IsKey { get; set; }
    }
}