using Microsoft.Practices.Unity;
using Microsoft.WindowsAPICodePack.Dialogs;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Contracts.Interfaces;
using ODataTools.DtoGenerator.Events;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.ExtensionMethods;
using ODataTools.Infrastructure.Interfaces;
using ODataTools.Reader.Common;
using ODataTools.Reader.Common.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ODataTools.DtoGenerator.ViewModels
{
    public class DtoGeneratorSettingsEditViewModel : ViewModelBase
    {
        PropertyChangedObserver<DtoGeneratorSettings> generatorSettingsObserver = null;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DtoGeneratorSettingsEditViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.DtoGenerator:Resources:DtoGeneratorViewTitle");

            this.InitializeCommands();

            this.UserCredentialsConfirmationRequest = new InteractionRequest<UserCredentialsConfirmation>();
            
            this.GeneratorSettings = new DtoGeneratorSettings()
            {
                GenerateAttributes = true,
                GenerateSingleFilePerDto = false,
                TargetNamespace = "MyNamespace"
            };

            this.generatorSettingsObserver = new PropertyChangedObserver<DtoGeneratorSettings>(this.GeneratorSettings)
                .RegisterHandler(nameof(this.GeneratorSettings.SourceEdmxFile), this.GeneratorSettingsChanged)
                .RegisterHandler(nameof(this.GeneratorSettings.ServiceBaseUrl), this.GeneratorSettingsChanged)
                .RegisterHandler(nameof(this.GeneratorSettings.OutputPath), this.GeneratorSettingsChanged);            
        }

        #region Event-Handler

        private void GeneratorSettingsChanged(DtoGeneratorSettings settings)
        {
            ((DelegateCommand)this.GenerateDataClassesCommand).RaiseCanExecuteChanged();
        }

        #endregion Event-Handler

        #region Interaction Requests

        public InteractionRequest<UserCredentialsConfirmation> UserCredentialsConfirmationRequest { get; private set; }

        #endregion Interaction Requests

        private async Task<string> ReadLocalFile(string filePath)
        {
            string result = string.Empty;

            if (System.IO.File.Exists(filePath))
            {
                result = await FileExtensions.ReadAllTextAsync(filePath);
            }
            else
            {
                // TODO: Message file not exists
            }

            return result;
        }

        private async Task<string> ReadMetadataFromService(Uri uri)
        {
            string result = string.Empty;

            Uri baseUrl = new Uri(generatorSettings.ServiceBaseUrl);

            if (generatorSettings.UserCredentials == null)
            {
                try
                {
                    result = await MetadataHelper.GetMetadata(baseUrl);
                    EventAggregator.GetEvent<EdmxFileChanged>().Publish(result);
                }
                catch (UnauthorizedAccessException ex)
                {
                    this.GetUserCredentials();
                    result = await MetadataHelper.GetMetadata(baseUrl, generatorSettings.UserCredentials);
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
            this.SelectOutputDirectoryCommand = new DelegateCommand(this.SelectOutputDirectory);
            this.OpenOutputDirectoryCommand = new DelegateCommand(this.OpenOutputDirectory);
            this.GenerateDataClassesCommand = DelegateCommand.FromAsyncHandler(this.GenerateDataClasses, this.GenerateDataClassesCanExecute);
            this.GetUserCredentialsCommand = new DelegateCommand(this.GetUserCredentials);
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
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.DtoGenerator:Resources:DtoGeneratorSelectModelFile"));
            fileDialog.IsFolderPicker = false;
            fileDialog.Multiselect = false;
            fileDialog.Filters.Add(new CommonFileDialogFilter(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.DtoGenerator:Resources:DtoGeneratorModelFiles"), "*.edmx"));

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.GeneratorSettings.SourceEdmxFile = fileDialog.FileName;

                if (System.IO.File.Exists(this.GeneratorSettings.SourceEdmxFile))
                {
                    // Fire-Event
                    EventAggregator.GetEvent<EdmxFileChanged>().Publish(File.ReadAllText(this.GeneratorSettings.SourceEdmxFile));
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
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog("Select output directory");
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
            {
                if (System.IO.Directory.Exists(this.GeneratorSettings.OutputPath))
                {
                    System.Diagnostics.Process.Start(this.GeneratorSettings.OutputPath);
                }
                else
                {
                    // TODO: Message directory not exists
                }
            }
        }

        public ICommand GenerateDataClassesCommand { get; private set; }

        /// <summary>
        /// Generate the DTOs
        /// </summary>
        /// <returns></returns>
        private async Task GenerateDataClasses()
        {
            var dtoGeneratorService = Container.Resolve<IDtoGenerator>(ServiceNames.DtoGeneratorService);

            string outputFile = Path.GetFileName(Path.ChangeExtension(generatorSettings.SourceEdmxFile, ".cs"));

            string fileContent = string.Empty;

            if (generatorSettings.IsFileModeEnabled)
            {
                fileContent = await ReadLocalFile(generatorSettings.SourceEdmxFile);
            }
            else
            {
                if (!String.IsNullOrEmpty(generatorSettings.ServiceBaseUrl))
                {
                    Uri baseUrl = new Uri(generatorSettings.ServiceBaseUrl);

                    if (generatorSettings.UserCredentials == null)
                    {
                        fileContent = await MetadataHelper.GetMetadata(baseUrl);
                        EventAggregator.GetEvent<EdmxFileChanged>().Publish(fileContent);
                    }
                }
            }

            // Task.Run Etiquette and Proper Usage
            // http://blog.stephencleary.com/2013/10/taskrun-etiquette-and-proper-usage.html
            await Task.Run(() =>
            {
                var result = dtoGeneratorService.GenerateDtoClassesForModel(fileContent, generatorSettings, outputFile);

                EventAggregator.GetEvent<DtoGeneratorFinished>().Publish(new DtoGeneratorFinishedEventArgs(DtoGeneratorMode.DtoGenerator, result));
            });
        }

        private bool GenerateDataClassesCanExecute()
        {
            return ((!String.IsNullOrEmpty(GeneratorSettings.SourceEdmxFile) || !String.IsNullOrEmpty(GeneratorSettings.ServiceBaseUrl)) && !String.IsNullOrEmpty(GeneratorSettings.OutputPath));
        }

        public ICommand GetUserCredentialsCommand { get; private set; }

        /// <summary>
        /// Get user credentials
        /// </summary>
        private void GetUserCredentials()
        {
            this.UserCredentialsConfirmationRequest.Raise(
                new UserCredentialsConfirmation { Title = "User-Credentials" },
                c => { this.GeneratorSettings.UserCredentials = c.Confirmed ? c.UserCredentials : null; });
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
