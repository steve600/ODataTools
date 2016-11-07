using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Contracts.Interfaces;
using ODataTools.Reader.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace ODataTools.DtoGenerator.Services
{
    public class DtoGeneratorService : IDtoGenerator
    {
        public Dictionary<FileInfo, string> GenerateDtoClassesForModel(DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs")
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            IDtoGenerator dtoGenerator = this.GetGenerator(dtoGeneratorSettings.SourceEdmxFile);

            try
            {
                string outputFile = Path.GetFileName(Path.ChangeExtension(dtoGeneratorSettings.SourceEdmxFile, ".cs"));

                result = dtoGenerator.GenerateDtoClassesForModel(dtoGeneratorSettings, outputFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return result;
        }

        private IDtoGenerator GetGenerator(string sourceFile)
        {
            IDtoGenerator generator = null;

            EdmxVersion edmxVersion = ModelHelper.DetectEdmxVersion(sourceFile);
            ODataServiceVersion odataVersion = ModelHelper.DetectODataServiceVersion(sourceFile);

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
