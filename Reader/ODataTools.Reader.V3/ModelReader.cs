using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ODataTools.Reader.V3
{
    public static class ModelReader
    {
        public static IEdmModel Parse(string modelFile)
        {
            IEdmModel model = null;

            using (XmlReader reader = XmlReader.Create(modelFile))
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
