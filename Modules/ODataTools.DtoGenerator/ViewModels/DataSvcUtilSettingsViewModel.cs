using Microsoft.Practices.Unity;
using Microsoft.WindowsAPICodePack.Dialogs;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Events;
using ODataTools.DtoGenerator.Interfaces;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using ODataTools.Infrastructure.SystemInformation;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace ODataTools.DtoGenerator.ViewModels
{
    public class DataSvcUtilSettingsViewModel : ViewModelBase
    {
        #region Members and Constants

        private string DataSvcUtilExe = "DataSvcUtil.exe";
        private PropertyChangedObserver<DataSvcUtilGUISettings> settingsObserver = null;
        private PropertyChangedObserver<DataSvcUtilGUISettings> settingsCommandExecuteChangedObserver = null;

        #endregion Members and Constants

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

            this.Settings.DetectedDataSvcUtils = CheckDataSvcUtil();

            if (this.Settings.DetectedDataSvcUtils != null && this.Settings.DetectedDataSvcUtils.Any())
            {
                this.Settings.SelectedDataSvcUtil = this.Settings.DetectedDataSvcUtils.First();
            }

            this.settingsObserver = new PropertyChangedObserver<DataSvcUtilGUISettings>(this.Settings)
                .RegisterHandler(nameof(this.Settings.Version), this.CanGenerateBindableObjects)
                .RegisterHandler(nameof(this.Settings.GenerateBindableObjects), this.CanGenerateBindableObjects);

            this.settingsCommandExecuteChangedObserver = new PropertyChangedObserver<DataSvcUtilGUISettings>(this.Settings)
                .RegisterHandler(nameof(this.Settings.InputFile), this.RaiseCanExecuteChanged)
                .RegisterHandler(nameof(this.Settings.OutputFile), this.RaiseCanExecuteChanged);

            this.DataSvcUtilGUIService = Container.Resolve<IDataSvcUtilGUIService>(ServiceNames.DataSvcUtilGUIService);        
        }

        #region Event-Handler

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

        private void RaiseCanExecuteChanged(DataSvcUtilGUISettings settings)
        {
            ((DelegateCommand)this.GenerateDataClassesCommand).RaiseCanExecuteChanged();
        }

        #endregion Event-Handler

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

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.OpenEdmxFileCommand = new DelegateCommand(this.OpenEdmxFile);
            this.SelectOutputFileCommand = new DelegateCommand(this.SelectOutputFile);
            this.OpenOutputDirectoryCommand = new DelegateCommand(this.OpenOutputDirectory);
            this.GenerateDataClassesCommand = DelegateCommand.FromAsyncHandler(this.GenerateDataClasses, this.GenerateDataClassesCanExecute);
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
                Settings.InputFile = fileDialog.FileName;

                EventAggregator.GetEvent<EdmxFileChanged>().Publish(File.ReadAllText(fileDialog.FileName));

                Settings.OutputFile = Path.ChangeExtension(Settings.InputFile, "cs");
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
                    Settings.OutputFile = Path.ChangeExtension(fileDialog.FileName, "cs");
                }
                else
                {
                    Settings.OutputFile = fileDialog.FileName;
                }
            }
        }

        public ICommand OpenOutputDirectoryCommand { get; private set; }

        /// <summary>
        /// Open output directory
        /// </summary>
        private void OpenOutputDirectory()
        {
            if (!String.IsNullOrEmpty(Settings.OutputFile))
            {
                System.Diagnostics.Process.Start(Path.GetDirectoryName(Settings.OutputFile));
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
                this.dataSvcUtilGUIService.Generate(this.Settings);
            });            
        }

        /// <summary>
        /// Generate DTOs can execute handler
        /// </summary>
        /// <returns></returns>
        private bool GenerateDataClassesCanExecute()
        {
            return (!String.IsNullOrEmpty(this.Settings.InputFile) && !String.IsNullOrEmpty(this.Settings.OutputFile));
        }

        #endregion Commands

        #region Properties

        #region Properties

        private DataSvcUtilGUISettings settings = new DataSvcUtilGUISettings();

        /// <summary>
        /// Settings
        /// </summary>
        public DataSvcUtilGUISettings Settings
        {
            get { return settings; }
            set { this.SetProperty<DataSvcUtilGUISettings>(ref this.settings, value); }
        }

        #endregion Properties

        private IDataSvcUtilGUIService dataSvcUtilGUIService;

        /// <summary>
        /// The generator service
        /// </summary>
        public IDataSvcUtilGUIService DataSvcUtilGUIService
        {
            get { return dataSvcUtilGUIService; }
            set { this.SetProperty<IDataSvcUtilGUIService>(ref this.dataSvcUtilGUIService, value); }
        }

        #endregion Properties
    }
}
