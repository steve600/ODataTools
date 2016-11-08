using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Contracts.Interfaces;
using ODataTools.Reader.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ODataTools.DtoGenerator.Services
{
    public class DtoGeneratorService : IDtoGenerator
    {
        public Dictionary<FileInfo, string> GenerateDtoClassesForModel(string metaDataDocument, DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs")
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            try
            {
                IDtoGenerator dtoGenerator = this.GetGenerator(metaDataDocument);
                
                string outputFile = Path.GetFileName(Path.ChangeExtension(dtoGeneratorSettings.SourceEdmxFile, ".cs"));

                if (String.IsNullOrEmpty(outputFile))
                    outputFile = Path.Combine(dtoGeneratorSettings.OutputPath, "Generated.cs");           

                result = dtoGenerator.GenerateDtoClassesForModel(metaDataDocument, dtoGeneratorSettings, outputFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return result;
        }

        /// <summary>
        /// Get DTO generator based on version information
        /// </summary>
        /// <param name="fileContent">File content.</param>
        /// <returns></returns>
        private IDtoGenerator GetGenerator(string fileContent)
        {
            IDtoGenerator generator = null;

            EdmxVersion edmxVersion = ModelHelper.DetectEdmxVersion(fileContent);
            ODataServiceVersion odataVersion = ModelHelper.DetectODataServiceVersion(fileContent);

            if (edmxVersion == EdmxVersion.V1 ||
                edmxVersion == EdmxVersion.V2 ||
                edmxVersion == EdmxVersion.V3)
            {
                generator = new ODataTools.Reader.V3.Generator.DtoGenerator();
            }
            else
            {
                generator = new ODataTools.Reader.V4.Generator.DtoGenerator();
            }

            return generator;
        }
    }
}
