using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using Prism.Events;
using Prism.Regions;


namespace ODataTools.DtoGenerator.ViewModels
{
    public class DataSvcUtilGUIViewModel : ViewModelBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DataSvcUtilGUIViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("DataSvcUtilGUITitle");
            
            this.RegionManager.RegisterViewWithRegion(RegionNames.DataSvcUtilGUISettingsRegion, typeof(Views.DataSvcUtilSettings));
            this.RegionManager.RegisterViewWithRegion(RegionNames.DataSvcUtilGUIOutputRegion, CreateOutpuView);
        }

        private object CreateOutpuView()
        {
            var outputView = Container.Resolve<Views.DtoGeneratorOutput>();
            var vm = Container.Resolve<ViewModels.DtoGeneratorOutputViewModel>();
            vm.GeneratorMode = DtoGeneratorMode.DataSvcUtil;
            outputView.DataContext = vm;

            return outputView;
        }
    }
}
