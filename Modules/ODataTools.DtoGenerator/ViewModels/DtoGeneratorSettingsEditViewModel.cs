﻿using Microsoft.Practices.Unity;
using Microsoft.WindowsAPICodePack.Dialogs;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Contracts.Interfaces;
using ODataTools.DtoGenerator.Events;
using ODataTools.DtoGenerator.Generator;
using ODataTools.DtoGenerator.Interfaces;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using ODataTools.Reader.Common;
using ODataTools.Reader.Common.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace ODataTools.DtoGenerator.ViewModels
{
    public class DtoGeneratorSettingsEditViewModel : ViewModelBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DtoGeneratorSettingsEditViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DtoGeneratorViewTitle");

            this.GeneratorSettings = new DtoGeneratorSettings()
            {
                OutputPath = @"C:\Temp\OData\Generated",
                GenerateAttributes = true,
                GenerateSingleFilePerDto = false,
                TargetNamespace = "MyNamespace"
            };

            this.InitializeCommands();
        }

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.OpenEdmxFileCommand = new DelegateCommand(this.OpenEdmxFile);
            this.SelectOutputDirectoryCommand = new DelegateCommand(this.SelectOutputDirectory);
            this.OpenOutputDirectoryCommand = new DelegateCommand(this.OpenOutputDirectory);
            this.GenerateDataClassesCommand = DelegateCommand.FromAsyncHandler(this.GenerateDataClasses);
        }

        /// <summary>
        /// Open EDMX-File command
        /// </summary>
        public ICommand OpenEdmxFileCommand { get; private set; }

        /// <summary>
        /// Open EDMX-File
        /// </summary>
        private void OpenEdmxFile()
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DtoGeneratorSelectModelFile"));
            fileDialog.IsFolderPicker = false;
            fileDialog.Multiselect = false;
            fileDialog.Filters.Add(new CommonFileDialogFilter(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DtoGeneratorModelFiles"), "*.edmx"));

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.GeneratorSettings.SourceEdmxFile = fileDialog.FileName;

                if (System.IO.File.Exists(this.GeneratorSettings.SourceEdmxFile))
                {
                    // Fire-Event
                    EventAggregator.GetEvent<EdmxFileChanged>().Publish(this.GeneratorSettings.SourceEdmxFile);
                }
            }
        }

        /// <summary>
        /// Select output directory command
        /// </summary>
        public ICommand SelectOutputDirectoryCommand { get; private set; }

        /// <summary>
        /// Select output directory
        /// </summary>
        private void SelectOutputDirectory()
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog("Select otuput directory");
            fileDialog.IsFolderPicker = true;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.GeneratorSettings.OutputPath = fileDialog.FileName;
            }
        }

        public ICommand OpenOutputDirectoryCommand { get; private set; }

        /// <summary>
        /// Open output directory
        /// </summary>
        private void OpenOutputDirectory()
        {
            if (!String.IsNullOrEmpty(this.GeneratorSettings.OutputPath))
                System.Diagnostics.Process.Start(this.GeneratorSettings.OutputPath);
        }

        public ICommand GenerateDataClassesCommand { get; private set; }

        /// <summary>
        /// Generate the DTOs
        /// </summary>
        /// <returns></returns>
        private async Task GenerateDataClasses()
        {
            // Task.Run Etiquette and Proper Usage
            // http://blog.stephencleary.com/2013/10/taskrun-etiquette-and-proper-usage.html
            await Task.Run(() =>
            {
                var dtoGeneratorService = Container.Resolve<IDtoGenerator>(ServiceNames.DtoGeneratorService);

                string outputFile = Path.GetFileName(Path.ChangeExtension(generatorSettings.SourceEdmxFile, ".cs"));            

                var result = dtoGeneratorService.GenerateDtoClassesForModel(generatorSettings, outputFile);

                EventAggregator.GetEvent<DtoGeneratorFinished>().Publish(new DtoGeneratorFinishedEventArgs(DtoGeneratorMode.DtoGenerator, result));
            });
        }

        #endregion Commands

        #region Properties

        private DtoGeneratorSettings generatorSettings;

        /// <summary>
        /// The DTO generator settings
        /// </summary>
        public DtoGeneratorSettings GeneratorSettings
        {
            get { return generatorSettings; }
            set { this.SetProperty<DtoGeneratorSettings>(ref this.generatorSettings, value); }
        }

        #endregion Properties
    }
}
