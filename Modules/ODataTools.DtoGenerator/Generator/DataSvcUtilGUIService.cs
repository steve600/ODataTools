using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Events;
using ODataTools.DtoGenerator.Interfaces;
using ODataTools.Infrastructure.SystemInformation;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ODataTools.DtoGenerator.Generator
{
    public class DataSvcUtilGUIService : IDataSvcUtilGUIService
    {
        private IEventAggregator eventAggregator = null;
        private string DataSvcUtilExe = "DataSvcUtil.exe";

        private PropertyChangedObserver<DataSvcUtilGUISettings> settingsObserver = null;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="eventAggregator">The event aggregator</param>
        public DataSvcUtilGUIService(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.settingsObserver = new PropertyChangedObserver<DataSvcUtilGUISettings>(this.Settings)
                .RegisterHandler(nameof(this.Settings.Version), this.CanGenerateBindableObjects)
                .RegisterHandler(nameof(this.Settings.GenerateBindableObjects), this.CanGenerateBindableObjects);

            this.Settings.DetectedDataSvcUtils = CheckDataSvcUtil();

            if (this.Settings.DetectedDataSvcUtils != null && this.Settings.DetectedDataSvcUtils.Any())
            {
                this.Settings.SelectedDataSvcUtil = this.Settings.DetectedDataSvcUtils.First();
            }
        }

        private void CanGenerateBindableObjects(DataSvcUtilGUISettings settings)
        {
            if (settings.Version == DataSvcUtilVersions.Version1)
            {
                settings.GenerateBindableObjects = false;
                settings.CanGenerateBindableObjects = false;
            }
            else
            {
                settings.CanGenerateBindableObjects = true;
            }
        }

        /// <summary>
        /// Check if DataSvcUtil is installed
        /// </summary>
        /// <returns></returns>
        private IList<FileInfo> CheckDataSvcUtil()
        {
            IList<FileInfo> result = new List<FileInfo>();

            foreach (var f in DotNetFrameworkInfo.InstalledDotNetVersions())
            {
                if (!String.IsNullOrEmpty(f.InstallPath))
                {
                    FileInfo fi = new FileInfo(Path.Combine(f.InstallPath, DataSvcUtilExe));

                    if (fi.Exists && !result.Where(i => i.FullName.Equals(fi.FullName)).Any())
                        result.Add(fi);
                }
            }

            return result;
        }

        /// <summary>
        /// Generate data classes
        /// </summary>
        public void Generate()
        {
            ProcessStartInfo psi = new ProcessStartInfo(this.Settings.SelectedDataSvcUtil.FullName);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = this.GenerateArguments();

            try
            {
                using (Process process = Process.Start(psi))
                {
                    //
                    // Read in all the text from the process with the StreamReader.
                    //
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        this.eventAggregator.GetEvent<DtoGeneratorLogEntryAdded>().Publish(new DtoGeneratorLogEntryAddedEventArgs(DtoGeneratorMode.DataSvcUtil, result));
                    }

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Dictionary<FileInfo, string> generatedFiles = new Dictionary<FileInfo, string>();
                        generatedFiles.Add(new FileInfo(this.Settings.OutputFile), File.ReadAllText(this.Settings.OutputFile));
                        this.eventAggregator.GetEvent<DtoGeneratorFinished>().Publish(new DtoGeneratorFinishedEventArgs(DtoGeneratorMode.DataSvcUtil, generatedFiles));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        
        /// <summary>
        /// Generate argument list
        /// </summary>
        /// <returns></returns>
        private string GenerateArguments()
        {
            string source = this.Settings.IsInFileMode ? $"/in:{this.Settings.InputFile}" : $"/uri:{this.Settings.URL}";
            string version = this.Settings.Version == DataSvcUtilVersions.Version1 ? "1.0" : "2.0";
            string language = this.Settings.Language.ToString();

            string arguments = $"{source} /out:{this.Settings.OutputFile} /language:{language} /version:{version}";

            if (this.Settings.GenerateBindableObjects)
                arguments += " /dataservicecollection";

            return arguments;
        }

        #region Properties

        private DataSvcUtilGUISettings settings = new DataSvcUtilGUISettings();

        /// <summary>
        /// Settings
        /// </summary>
        public DataSvcUtilGUISettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        
        #endregion Properties
    }
}