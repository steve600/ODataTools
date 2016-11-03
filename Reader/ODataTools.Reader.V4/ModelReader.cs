using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ODataTools.Reader.V4
{
    public static class ModelReader
    {
        public static IEdmModel Parse(string modelFile)
        {
            IEdmModel model = null;

            using (XmlReader reader = XmlReader.Create(modelFile))
            {
                IEnumerable<EdmError> errors = null;

                CsdlReader.TryParse(reader, out model, out errors);

                if (errors.Count() == 0)
                {
                    return model;
                }
                else
                {
                    // TODO: Output errors
                    foreach (var e in errors)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ErrorMessage);
                    }
                }
            }

            return model;
        }
    }
}