using ODataTools.DtoGenerator.Contracts.Enums;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator.Generator
{
    public class DataSvcUtilGUISettings : BindableBase
    {
        private IList<FileInfo> detectedDataSvcUtils;

        /// <summary>
        /// Detected DataSvcUtils
        /// </summary>
        public IList<FileInfo> DetectedDataSvcUtils
        {
            get { return detectedDataSvcUtils; }
            set { this.SetProperty<IList<FileInfo>>(ref this.detectedDataSvcUtils, value); }
        }

        private FileInfo selectedDataSvcUtil;

        /// <summary>
        /// Selected DataSvcUtil
        /// </summary>
        public FileInfo SelectedDataSvcUtil
        {
            get { return selectedDataSvcUtil; }
            set { this.SetProperty<FileInfo>(ref this.selectedDataSvcUtil, value); }
        }

        private bool isInFileMode = true;

        /// <summary>
        /// Flag if is in file mode
        /// </summary>
        public bool IsInFileMode
        {
            get { return isInFileMode; }
            set { this.SetProperty<bool>(ref this.isInFileMode, value); }
        }
        
        private string inputFile;

        /// <summary>
        /// The input file
        /// </summary>
        public string InputFile
        {
            get { return inputFile; }
            set { this.SetProperty<string>(ref this.inputFile, value); }
        }

        private string outputFile;

        /// <summary>
        /// The ouput file
        /// </summary>
        public string OutputFile
        {
            get { return outputFile; }
            set { this.SetProperty<string>(ref this.outputFile, value); }
        }

        private string url;

        /// <summary>
        /// The URL to the OData service
        /// </summary>
        public string URL
        {
            get { return url; }
            set { this.SetProperty<string>(ref this.url, value); }
        }

        private Languages language = Languages.CSharp;

        /// <summary>
        /// The language to generate
        /// </summary>
        public Languages Language
        {
            get { return language; }
            set { this.SetProperty<Languages>(ref this.language, value); }
        }

        private DataSvcUtilVersions version = DataSvcUtilVersions.Version2;

        /// <summary>
        /// Version
        /// </summary>
        public DataSvcUtilVersions Version
        {
            get { return version; }
            set { this.SetProperty<DataSvcUtilVersions>(ref this.version, value); }
        }

        private bool generateBindableObjects = true;

        /// <summary>
        /// Generate bindable ojects
        /// </summary>
        public bool GenerateBindableObjects
        {
            get { return generateBindableObjects; }
            set { this.SetProperty<bool>(ref this.generateBindableObjects, value); }
        }

        private bool canGenerateBindableObjects = true;

        /// <summary>
        /// Flag if bindable objects can be generated
        /// </summary>
        public bool CanGenerateBindableObjects
        {
            get { return canGenerateBindableObjects; }
            set { this.SetProperty<bool>(ref this.canGenerateBindableObjects, value); }
        }
    }
}
