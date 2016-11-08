using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Events;
using ODataTools.DtoGenerator.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ODataTools.DtoGenerator.Generator.Services
{
    public class DataSvcUtilGUIService : IDataSvcUtilGUIService
    {
        private IEventAggregator eventAggregator = null;
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="eventAggregator">The event aggregator</param>
        public DataSvcUtilGUIService(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Generate data classes
        /// </summary>
        public void Generate(DataSvcUtilGUISettings settings)
        {
            ProcessStartInfo psi = new ProcessStartInfo(settings.SelectedDataSvcUtil.FullName);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = this.GenerateArguments(settings);

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
                        generatedFiles.Add(new FileInfo(settings.OutputFile), File.ReadAllText(settings.OutputFile));
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
        private string GenerateArguments(DataSvcUtilGUISettings settings)
        {
            string source = settings.IsInFileMode ? $"/in:{settings.InputFile}" : $"/uri:{settings.URL}";
            string version = settings.Version == DataSvcUtilVersions.Version1 ? "1.0" : "2.0";
            string language = settings.Language.ToString();

            string arguments = $"{source} /out:{settings.OutputFile} /language:{language} /version:{version}";

            if (settings.GenerateBindableObjects)
                arguments += " /dataservicecollection";

            return arguments;
        }
    }
}