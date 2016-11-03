using Microsoft.Practices.Unity;
using ODataTools.Core.Base;
using ODataTools.DtoGenerator.Contracts.Interfaces;
using ODataTools.Infrastructure.Constants;
using ODataTools.Reader.Common.Common;
using ODataTools.Reader.Services;
using Prism.Regions;

namespace ODataTools.Reader
{
    public class ReaderModule : PrismModuleBase
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        public ReaderModule(IUnityContainer unityContainer, RegionManager regionManager) :
            base(unityContainer, regionManager)
        {
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            this.UnityContainer.RegisterInstance<IDtoGenerator>(ServiceNames.DtoGeneratorService, new DtoGeneratorService(), new ContainerControlledLifetimeManager());
            this.UnityContainer.RegisterInstance<IModelVisualizer>(ServiceNames.ModelVisualizerService, new ModelVisualizerService(), new ContainerControlledLifetimeManager());
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
