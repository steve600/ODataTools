using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using Prism.Events;
using Prism.Regions;

namespace ODataTools.Shell.ViewModels
{
    public class RightTitlebarWindowCommandsViewModel : ViewModelBase
    {
        public RightTitlebarWindowCommandsViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
        }
    }
}
