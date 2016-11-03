using Microsoft.Practices.Unity;
using Microsoft.WindowsAPICodePack.Dialogs;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Events;
using ODataTools.DtoGenerator.Interfaces;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using ODataTools.Infrastructure.SystemInformation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ODataTools.DtoGenerator.ViewModels
{
    public class DataSvcUtilSettingsViewModel : ViewModelBase
    {

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DataSvcUtilSettingsViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.InitializeCommands();

            this.DataSvcUtilGUIService = Container.Resolve<IDataSvcUtilGUIService>(ServiceNames.DataSvcUtilGUIService);        
        }

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.OpenEdmxFileCommand = new DelegateCommand(this.OpenEdmxFile);
            this.SelectOutputFileCommand = new DelegateCommand(this.SelectOutputFile);
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
            fileDialog.Filters.Add(new CommonFileDialogFilter(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DtoGeneratorModelFiles"), "*.edmx"));

            fileDialog.IsFolderPicker = false;
            fileDialog.Multiselect = false;
            fileDialog.Filters.Add(new CommonFileDialogFilter(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DtoGeneratorModelFiles"), "*.edmx"));

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.dataSvcUtilGUIService.Settings.InputFile = fileDialog.FileName;

                EventAggregator.GetEvent<EdmxFileChanged>().Publish(fileDialog.FileName);

                this.dataSvcUtilGUIService.Settings.OutputFile = Path.ChangeExtension(this.dataSvcUtilGUIService.Settings.InputFile, "cs");
            }
        }

        /// <summary>
        /// Select output directory command
        /// </summary>
        public ICommand SelectOutputFileCommand { get; private set; }

        /// <summary>
        /// Select output directory
        /// </summary>
        private void SelectOutputFile()
        {
            string fileName = string.Empty;

            CommonSaveFileDialog fileDialog = new CommonSaveFileDialog("Select otuput file");
            fileDialog.Filters.Add(new CommonFileDialogFilter("CSharp-File", "*.cs"));

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (String.IsNullOrEmpty(Path.GetExtension(fileDialog.FileName)))
                {
                    this.dataSvcUtilGUIService.Settings.OutputFile = Path.ChangeExtension(fileDialog.FileName, "cs");
                }
                else
                {
                    this.dataSvcUtilGUIService.Settings.OutputFile = fileDialog.FileName;
                }
            }
        }

        public ICommand OpenOutputDirectoryCommand { get; private set; }

        /// <summary>
        /// Open output directory
        /// </summary>
        private void OpenOutputDirectory()
        {
            if (!String.IsNullOrEmpty(this.dataSvcUtilGUIService.Settings.OutputFile))
            {
                System.Diagnostics.Process.Start(Path.GetDirectoryName(this.dataSvcUtilGUIService.Settings.OutputFile));
            }
        }

        public ICommand GenerateDataClassesCommand { get; private set; }

        /// <summary>
        /// Generate the DTOs
        /// </summary>
        /// <returns></returns>
        private async Task GenerateDataClasses()
        {
            await Task.Run(() =>
            {
                this.dataSvcUtilGUIService.Generate();
            });            
        }

        #endregion Commands

        #region Properties

        private IDataSvcUtilGUIService dataSvcUtilGUIService;

        public IDataSvcUtilGUIService DataSvcUtilGUIService
        {
            get { return dataSvcUtilGUIService; }
            set { this.SetProperty<IDataSvcUtilGUIService>(ref this.dataSvcUtilGUIService, value); }
        }


        #endregion Properties
    }
}
