using ODataTools.DtoGenerator.Contracts.Enums;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Contracts
{ 
    public class DtoGeneratorSettings : BindableBase
    {
        private string targetNamespace = "MyNamespace";

        /// <summary>
        /// The target namespace
        /// </summary>
        public string TargetNamespace
        {
            get { return targetNamespace; }
            set { this.SetProperty<string>(ref this.targetNamespace, value); }
        }

        private bool generatedAttributes = true;

        /// <summary>
        /// Flag if the attributes should be generated
        /// </summary>
        public bool GenerateAttributes
        {
            get { return generatedAttributes; }
            set { this.SetProperty<bool>(ref this.generatedAttributes, value); }
        }

        private bool generateSingleFilePerDto = false;

        /// <summary>
        /// Flag if a single file per DTO should be generated
        /// </summary>
        public bool GenerateSingleFilePerDto
        {
            get { return generateSingleFilePerDto; }
            set { this.SetProperty<bool>(ref this.generateSingleFilePerDto, value); }
        }

        private string outputPath;

        /// <summary>
        /// The output path
        /// </summary>
        public string OutputPath
        {
            get { return outputPath; }
            set { this.SetProperty<string>(ref this.outputPath, value); }
        }

        private string sourceEdmxFile;

        /// <summary>
        /// The source EDMX file
        /// </summary>
        public string SourceEdmxFile
        {
            get { return sourceEdmxFile; }
            set { this.SetProperty<string>(ref this.sourceEdmxFile, value); }
        }

        private DataClassOptions dataClassOptions = DataClassOptions.DTO;

        /// <summary>
        /// Data class options
        /// </summary>
        public DataClassOptions DataClassOptions
        {
            get { return dataClassOptions; }
            set { this.SetProperty<DataClassOptions>(ref this.dataClassOptions, value); }
        }        
    }
}
