using Dragablz;
using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.Infrastructure;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Events;
using Prism.Events;
using Prism.Regions;

namespace ODataTools.Shell.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggrgator">The event aggregator.</param>
        public MainWindowViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggrgator) :
            base(unityContainer, regionManager, eventAggrgator)
        {
            this.Title = "OData Tools";

            // Register to events
            EventAggregator.GetEvent<UpdateStatusBarMessageEvent>().Subscribe(OnUpdateStatusBarMessageEventHandler);

            _interTabClient = new DefaultInterTabClient();

            ApplicationCommands.NavigateToViewCommand = new Prism.Commands.DelegateCommand<string>(NavigateToView);

            // Load application config
            //this.LoadApplicationConfigFile();
        }

        #region Event-Handler

        private void OnUpdateStatusBarMessageEventHandler(string statusBarMessage)
        {
            this.StatusBarMessage = statusBarMessage;
        }

        #endregion Event-Handler

        #region Commands

        private void NavigateToView(string viewName)
        {
            RegionManager.RequestNavigate(RegionNames.MainRegion, viewName);
        }

        #endregion Commands

        #region TabControl

        private readonly IInterTabClient _interTabClient;

        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }

        /// <summary>
        /// Callback to handle tab closing.
        /// </summary>
        public ItemActionCallback ClosingTabItemHandler
        {
            get { return ClosingTabItemHandlerImpl; }
        }

        /// <summary>
        /// Callback to handle tab closing.
        /// </summary>
        private static void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            //in here you can dispose stuff or cancel the close

            //here's your view model:
            var viewModel = args.DragablzItem.DataContext as HeaderedItemViewModel;
            //Debug.Assert(viewModel != null);

            //here's how you can cancel stuff:
            //args.Cancel();
        }

        #endregion TabControl

        #region Properties

        private string statusBarMessage;

        /// <summary>
        /// Status-Bar message
        /// </summary>
        public string StatusBarMessage
        {
            get { return statusBarMessage; }
            private set { this.SetProperty<string>(ref this.statusBarMessage, value); }
        }

        #endregion Properties
    }
}
