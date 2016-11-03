using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ODataTools.Core.Base
{
    public abstract class ViewModelBase : BindableBase
    {
        protected ViewModelBase(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.Container = unityContainer;
            this.RegionManager = regionManager;
            this.EventAggregator = eventAggregator;
        }

        #region Properties

        private IUnityContainer unityContainer;

        /// <summary>
        /// The unity container
        /// </summary>
        public IUnityContainer Container
        {
            get { return unityContainer; }
            private set { this.SetProperty<IUnityContainer>(ref this.unityContainer, value); }
        }

        private IRegionManager regionManager;

        /// <summary>
        /// The region manager
        /// </summary>
        public IRegionManager RegionManager
        {
            get { return regionManager; }
            private set { this.SetProperty<IRegionManager>(ref this.regionManager, value); }
        }

        private IEventAggregator eventAggregator;

        /// <summary>
        /// The EventAggregator
        /// </summary>
        public IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
            private set { this.SetProperty<IEventAggregator>(ref this.eventAggregator, value); }
        }

        private string title;

        /// <summary>
        /// View title
        /// </summary>
        public string Title
        {
            get { return title; }
            set { this.SetProperty<string>(ref this.title, value); }
        }

        #endregion Properties
    }
}
