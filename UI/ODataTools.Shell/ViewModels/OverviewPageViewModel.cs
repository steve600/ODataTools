using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using Prism.Events;
using Prism.Regions;

namespace ODataTools.Shell.ViewModels
{
    public class OverviewPageViewModel : ViewModelBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggrgator">The event aggregator.</param>
        public OverviewPageViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("OverviewPageTitle");
        }
    }
}
