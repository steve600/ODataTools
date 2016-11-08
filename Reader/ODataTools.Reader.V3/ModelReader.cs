using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Validation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ODataTools.Reader.V3
{
    public static class ModelReader
    {
        public static IEdmModel Parse(string fileContent)
        {
            IEdmModel model = null;

            using (XmlReader reader = XmlReader.Create(new StringReader(fileContent)))
            {
                IEnumerable<EdmError> errors = null;

                EdmxReader.TryParse(reader, out model, out errors);

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
