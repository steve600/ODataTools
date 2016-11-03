using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using Prism.Events;
using Prism.Regions;
using ODataTools.DtoGenerator.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Input;
using Prism.Commands;
using System.Xml.Linq;
using System.IO;
using ODataTools.DtoGenerator.Contracts.Enums;

namespace ODataTools.DtoGenerator.ViewModels
{
    public class DtoGeneratorViewModel : ViewModelBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DtoGeneratorViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DtoGeneratorViewTitle");

            this.RegionManager.RegisterViewWithRegion(RegionNames.DtoGeneratorSettingsRegion, typeof(Views.DtoGeneratorSettingsEdit));

            this.RegionManager.RegisterViewWithRegion(RegionNames.DtoGeneratorOutputRegion, CreateOutpuView);
        }

        private object CreateOutpuView()
        {
            var outputView = Container.Resolve<Views.DtoGeneratorOutput>();
            var vm = Container.Resolve<ViewModels.DtoGeneratorOutputViewModel>();
            vm.GeneratorMode = DtoGeneratorMode.DtoGenerator;
            outputView.DataContext = vm;

            return outputView;
        }
    }
}
