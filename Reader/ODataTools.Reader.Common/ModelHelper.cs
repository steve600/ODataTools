using ODataTools.DtoGenerator.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ODataTools.Reader.Common
{
    public static class ModelHelper
    {
        public static EdmxVersion DetectEdmxVersion(string fileContent)
        {
            EdmxVersion result = EdmxVersion.None;
            try
            {
                XDocument doc = XDocument.Parse(fileContent);

                string version = doc.Root.Attribute("Version").Value;

                switch (version)
                {
                    case "1.0":
                        result = EdmxVersion.V1;
                        break;
                    case "2.0":
                        result = EdmxVersion.V2;
                        break;
                    case "3.0":
                        result = EdmxVersion.V3;
                        break;
                    case "4.0":
                        result = EdmxVersion.V4;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static ODataServiceVersion DetectODataServiceVersion(string fileContent)
        {
            ODataServiceVersion result = ODataServiceVersion.None;
            try
            {
                XDocument doc = XDocument.Parse(fileContent);

                string version = doc.Descendants().Where(d => d.Name.LocalName.Equals("DataServices"))
                    .Attributes().Where(a => a.Name.LocalName.Equals("DataServiceVersion")).FirstOrDefault().Value;

                switch (version)
                {
                    case "1.0":
                        result = ODataServiceVersion.V1;
                        break;
                    case "2.0":
                        result = ODataServiceVersion.V2;
                        break;
                    case "3.0":
                        result = ODataServiceVersion.V3;
                        break;
                    case "4.0":
                        result = ODataServiceVersion.V4;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
