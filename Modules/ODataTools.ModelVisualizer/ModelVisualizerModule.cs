using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.Infrastructure.Constants;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ODataTools.ModelVisualizer
{
    public class ModelVisualizerModule : PrismModuleBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        public ModelVisualizerModule(IUnityContainer unityContainer, RegionManager regionManager) :
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
            Prism.Unity.UnityExtensions.RegisterTypeForNavigation<Views.ModelVisualizer>(this.UnityContainer, ViewNames.ModelVisualizer);


        }

        /// <summary>
        /// Add Resource-Dictionaries
        /// </summary>
        public override void AddResourceDictionaries()
        {
            var rd = new ResourceDictionary();

            rd.Source = new Uri("/ODataTools.ModelVisualizer;component/Styling/LookAndFeel.xaml", UriKind.RelativeOrAbsolute);

            Application.Current.Resources.MergedDictionaries.Add(rd);
        }
    }
}
