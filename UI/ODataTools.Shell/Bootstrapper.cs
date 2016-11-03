using Prism.Regions;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure;
using ODataTools.Infrastructure.Interfaces;
using ODataTools.Infrastructure.Services;
using Prism.Modularity;
using Prism.Events;
using ODataTools.Shell.Services;
using ODataTools.Infrastructure.Events;
using ODataTools.Shell.Views;
using Dragablz;
using ODataTools.Shell.RegionAdapter;

namespace ODataTools.Shell
{
    internal class Bootstrapper : UnityBootstrapper
    {
        private AutoResetEvent WaitForCreation { get; set; }

        /// <summary>
        /// The shell object
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// Initialize shell (MainWindow)
        /// </summary>
        protected override void InitializeShell()
        {
            // Register views
            var regionManager = this.Container.Resolve<IRegionManager>();
            if (regionManager != null)
            {
                regionManager.RegisterViewWithRegion(RegionNames.LeftWindowCommandsRegion, typeof(Views.LeftTitlebarWindowCommands));
                regionManager.RegisterViewWithRegion(RegionNames.RightWindowCommandsRegion, typeof(Views.RightTitlebarWindowCommands));
                regionManager.RegisterViewWithRegion(RegionNames.FlyoutRegion, typeof(Views.ApplicationSettings));
            }

            // Navigate to introduction page
            regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.OverviewPage);

            //Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Configure the DI-Container
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register views for navigation
            Prism.Unity.UnityExtensions.RegisterTypeForNavigation<Views.SystemInfo>(Container, ViewNames.SystemInformationView);

            //Prism.Unity.UnityExtensions.RegisterTypeForNavigation<object, >(Container, ViewNames.OverviewPage);
            Container.RegisterType<object, Views.OverviewPage>(ViewNames.OverviewPage);
            //Container.RegisterInstance(typeof(object), ViewNames.OverviewPage, new Model.TabContent(ViewNames.OverviewPage, new Views.IntroductionPage()));

            // Application commands
            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>();

            // Flyout service
            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());

            // Localizer service
            Container.RegisterInstance(typeof(ILocalizerService),
                ServiceNames.LocalizerService,
                new LocalizerService("en-US"),
                new Microsoft.Practices.Unity.ContainerControlledLifetimeManager());

        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            var regionBehaviorFactory = Container.Resolve<IRegionBehaviorFactory>();
            mappings.RegisterMapping(typeof(TabablzControl), new TabablzControlRegionAdapter(regionBehaviorFactory));
            return mappings;
        }

        /// <summary>
        /// Initialize application modules
        /// </summary>
        protected override void InitializeModules()
        {
            base.InitializeModules();

            // Show SplashScreen
            this.ShowSplashScreen();

            IModule prismModule = null;

            // DtoGenerator
            //Container.Resolve<IEventAggregator>().GetEvent<SplashScreenStatusMessageUpdateEvent>().Publish("FTP module ...");
            prismModule = Container.Resolve<ODataTools.DtoGenerator.DtoGeneratorModule>();
            prismModule.Initialize();

            // OData-Reader
            prismModule = Container.Resolve<ODataTools.Reader.ReaderModule>();
            prismModule.Initialize();

            // Model-Visualizer
            prismModule = Container.Resolve<ODataTools.ModelVisualizer.ModelVisualizerModule>();
            prismModule.Initialize();

            // Set StatusBarMessage
            var statusBarMessage = Container.Resolve<ILocalizerService>(ServiceNames.LocalizerService).GetLocalizedString("ODataTools.Shell:Resources:ApplicationReadyStatusBarMessage");
            Container.Resolve<IEventAggregator>().GetEvent<UpdateStatusBarMessageEvent>().Publish(statusBarMessage);

            // Show MainWindow
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Show SplashScreen
        /// </summary>
        private void ShowSplashScreen()
        {
            //Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
            //{
            //    Container.Resolve<IEventAggregator>().GetEvent<CloseSplashScreenEvent>().Publish(new CloseSplashScreenEvent());
            //}));

            //WaitForCreation = new AutoResetEvent(false);

            //Thread t = new Thread(new ThreadStart(
            //  () =>
            //  {
            //      Dispatcher.CurrentDispatcher.BeginInvoke(
            //         (Action)(() =>
            //         {
            //             // Register SplashScreen
            //             //Container.RegisterInstance<Views.SplashScreen>("SplashScreen", new Views.SplashScreen(), new ContainerControlledLifetimeManager());
            //             var splash = Container.Resolve<Views.SplashScreen>("SplashScreen");

            //             // Subscribe for Closing-Event
            //             Container.Resolve<IEventAggregator>().GetEvent<CloseSplashScreenEvent>().Subscribe(e => splash.Dispatcher.BeginInvoke((Action)(() => { splash.DataContext = null; splash.Dispatcher.InvokeShutdown(); splash.Close(); })), ThreadOption.PublisherThread, true);

            //             splash.Show();

            //             WaitForCreation.Set();
            //         }));

            //      Dispatcher.Run();
            //  }));

            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();

            //WaitForCreation.WaitOne();
        }
    }
}
