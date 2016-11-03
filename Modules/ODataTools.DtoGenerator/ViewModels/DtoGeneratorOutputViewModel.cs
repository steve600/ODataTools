using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Events;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ODataTools.DtoGenerator.ViewModels
{
    public class DtoGeneratorOutputViewModel : ViewModelBase
    {
        private StringBuilder log = new StringBuilder();
        private ILocalizerService localizerService = null;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DtoGeneratorOutputViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.localizerService = Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService);

            // Register for events
            EventAggregator.GetEvent<EdmxFileChanged>().Subscribe(this.EdmxFileChangedEventHandler);          
            EventAggregator.GetEvent<DtoGeneratorLogEntryAdded>().Subscribe(this.DtoGeneratorLogEntryAddedEventHandler);
            EventAggregator.GetEvent<DtoGeneratorFinished>().Subscribe(this.DtoGeneratorFinishedEventHandler);
        }

        #region Event-Handler

        /// <summary>
        /// Event-Handler for EDMX-File changed
        /// </summary>
        /// <param name="edmxFile">The new filepath.</param>
        private void EdmxFileChangedEventHandler(string edmxFile)
        {
            if (System.IO.File.Exists(edmxFile))
            {
                try
                {
                    this.EdmxFileContent = XDocument.Load(edmxFile).ToString();

                    var logMessage = String.Format(localizerService.GetLocalizedString("DtoGeneratorEdmxSuccessfullyLoaded"), edmxFile);
                    EventAggregator.GetEvent<DtoGeneratorLogEntryAdded>().Publish(new DtoGeneratorLogEntryAddedEventArgs(DtoGeneratorMode.DtoGenerator, logMessage));

                    this.GeneratedSourceFiles = null;
                }
                catch (Exception ex)
                {
                    var logMessage = String.Format(localizerService.GetLocalizedString("DtoGeneratorFailedToLoadEdmxFile"), edmxFile, ex.ToString());
                    EventAggregator.GetEvent<DtoGeneratorLogEntryAdded>().Publish(new DtoGeneratorLogEntryAddedEventArgs(DtoGeneratorMode.DtoGenerator, logMessage));
                }
            }
        }

        /// <summary>
        /// Write log entry.
        /// </summary>
        /// <param name="args">The EventArgs.</param>
        private void DtoGeneratorLogEntryAddedEventHandler(DtoGeneratorLogEntryAddedEventArgs args)
        {
            if (args.GeneratorMode == GeneratorMode)
            {
                if (GeneratorMode == DtoGeneratorMode.DtoGenerator)
                {
                    this.log.AppendLine($"{DateTime.Now.ToLongTimeString()} - {args.LogMessage}");
                }
                else
                {
                    this.log.Append(args.LogMessage);
                }
            }

            OnPropertyChanged(() => this.Log);
        }

        /// <summary>
        /// Event handler for the generater finished event
        /// </summary>
        /// <param name="args">The EventArgs.</param>
        private void DtoGeneratorFinishedEventHandler(DtoGeneratorFinishedEventArgs args)
        {
            if (this.GeneratorMode == args.GeneratorMode)
            {
                this.GeneratedSourceFiles = args.GeneratedFiles;
                var logMessage = String.Format(localizerService.GetLocalizedString("DtoGeneratorGeneratorFinished"), args.GeneratedFiles.Count);
                EventAggregator.GetEvent<DtoGeneratorLogEntryAdded>().Publish(new DtoGeneratorLogEntryAddedEventArgs(DtoGeneratorMode.DtoGenerator, logMessage));
            }
        }

        #endregion Event-Handler

        #region Properties

        private DtoGeneratorMode generatorMode = DtoGeneratorMode.None;

        /// <summary>
        /// The generator mode
        /// </summary>
        public DtoGeneratorMode GeneratorMode
        {
            get { return generatorMode; }
            set { this.SetProperty<DtoGeneratorMode>(ref this.generatorMode, value); }
        }
        
        private string edmxFileContent;

        /// <summary>
        /// EDMX-File content
        /// </summary>
        public string EdmxFileContent
        {
            get { return edmxFileContent; }
            set { this.SetProperty<string>(ref this.edmxFileContent, value); }
        }

        private Dictionary<FileInfo, string> generatedSourceFiles;

        /// <summary>
        /// Generated source files
        /// </summary>
        public Dictionary<FileInfo, string> GeneratedSourceFiles
        {
            get { return generatedSourceFiles; }
            set { this.SetProperty<Dictionary<FileInfo, string>>(ref this.generatedSourceFiles, value); }
        }

        /// <summary>
        /// The log messages
        /// </summary>
        public string Log
        {
            get
            {
                return this.log.ToString();
            }
        }

        #endregion Properties
    }
}
