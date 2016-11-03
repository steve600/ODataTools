using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Generator;
using ODataTools.DtoGenerator.Interfaces;
using ODataTools.Infrastructure.Constants;
using ODataTools.Reader.Common.Common;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.DtoGenerator
{
    public class DtoGeneratorModule : PrismModuleBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        public DtoGeneratorModule(IUnityContainer unityContainer, RegionManager regionManager) :
            base(unityContainer, regionManager)
        {
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            //this.RegionManager.RegisterViewWithRegion(RegionNames.RightWindowCommandsRegion, typeof(Views.FtpRightTitlebarWindowCommands));
            //this.RegionManager.RegisterViewWithRegion(RegionNames.FlyoutRegion, typeof(Views.FtpConnectionsFlyout));

            // Tiles
            this.RegionManager.RegisterViewWithRegion(RegionNames.TilesRegion, typeof(Views.HomeTiles));

            // Register views for navigation
            Prism.Unity.UnityExtensions.RegisterTypeForNavigation<Views.DtoGenerator>(this.UnityContainer, ViewNames.DtoGenerator);
            Prism.Unity.UnityExtensions.RegisterTypeForNavigation<Views.DataSvcUtilGUI>(this.UnityContainer, ViewNames.DataSvcUtilGUI);

            // Regsiter DTO-Generator services
            //this.UnityContainer.RegisterInstance<IDtoGenerator>(ServiceNames.DtoGeneratorService_V3, new DtoGeneratorService_V3(), new ContainerControlledLifetimeManager());
            this.UnityContainer.RegisterInstance<IDataSvcUtilGUIService>(ServiceNames.DataSvcUtilGUIService, UnityContainer.Resolve<DataSvcUtilGUIService>(), new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Add Resource-Dictionaries
        /// </summary>
        public override void AddResourceDictionaries()
        {
            //var rd = new ResourceDictionary();

            //rd.Source = new Uri("/MetroFtpExplorer.Ftp;component/Styling/LookAndFeel.xaml", UriKind.RelativeOrAbsolute);

            //Application.Current.Resources.MergedDictionaries.Add(rd);
        }
    }
}
