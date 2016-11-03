using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
