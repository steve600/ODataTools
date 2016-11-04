using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAPICodePack.Dialogs;
using ODataTools.Core.Base;
using ODataTools.Infrastructure.Constants;
using ODataTools.Infrastructure.Interfaces;
using ODataTools.ModelVisualizer.Contracts.Model;
using ODataTools.ModelVisualizer.Graphs;
using ODataTools.Reader.Common.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ODataTools.ModelVisualizer.ViewModels
{
    public class ModelVisualizerViewModel : ViewModelBase
    {
        private IModelVisualizer modelVisualizer = null;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ModelVisualizerViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IEventAggregator eventAggregator) :
            base(unityContainer, regionManager, eventAggregator)
        {
            this.Title = this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.ModelVisualizer:Resources:ModelVisualizerTitle");

            this.InitializeCommands();

            this.InitializeGraph();

            this.modelVisualizer = this.Container.Resolve<IModelVisualizer>(ServiceNames.ModelVisualizerService);            
        }

        #region Commands

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitializeCommands()
        {
            this.OpenEdmxFileCommand = new DelegateCommand(this.OpenEdmxFile);
        }

        /// <summary>
        /// Open EDMX-File command
        /// </summary>
        public ICommand OpenEdmxFileCommand { get; private set; }

        /// <summary>
        /// Open EDMX-File
        /// </summary>
        private void OpenEdmxFile()
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.ModelVisualizer:Resources:ModelVisualizerSelectModelFile"));
            fileDialog.Filters.Add(new CommonFileDialogFilter(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.ModelVisualizer:Resources:ModelVisualizerModelFiles"), "*.edmx"));

            fileDialog.IsFolderPicker = false;
            fileDialog.Multiselect = false;
            fileDialog.Filters.Add(new CommonFileDialogFilter(this.Container?.Resolve<ILocalizerService>(ServiceNames.LocalizerService)?.GetLocalizedString("ODataTools.ModelVisualizer:Resources:ModelVisualizerModelFiles"), "*.edmx"));

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.Entities = this.modelVisualizer.GetEntitiesForVisualization(fileDialog.FileName);

                this.UpdateGraph();
            }
        }

        #endregion Commands

        /// <summary>
        /// Initalize the graph
        /// </summary>
        private void InitializeGraph()
        {
            this.EntityArea = new EntityArea();

            var dgLogic = new EntityCore();
            this.EntityArea.LogicCore = dgLogic;

            dgLogic.DefaultLayoutAlgorithm = GraphX.PCL.Common.Enums.LayoutAlgorithmTypeEnum.KK;
            dgLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            dgLogic.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            dgLogic.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;

            dgLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            dgLogic.EdgeCurvingEnabled = true;         

            //EntityVertex e1 = new EntityVertex("FAKIR_Station_List");
            //e1.Fields.Add(new EntityField("Test", "Edm.String", true));

            //EntityVertex e2 = new EntityVertex("FAKIR_Station");

            //this.EntityArea.AddVertexAndData(e1, new VertexControl(e1));
            //this.EntityArea.AddVertexAndData(e2, new VertexControl(e2));

            //this.EntityArea.AddEdge(new EntityEdge(e1, e2, 1), new EdgeControl(), true);

            //this.RelayoutGraph();            
        }

        /// <summary>
        /// Update graph
        /// </summary>
        private void UpdateGraph()
        {
            this.EntityArea.ClearLayout();

            foreach (var e in this.Entities)
            {
                this.EntityArea.AddVertexAndData(e, new VertexControl(e), true);

                //foreach (var r in e.References)
                //{
                //    this.EntityArea.AddEdge(new EntityEdge(e, this.Entities.First(e1 => e1.Name.Equals(r.EntityName))), new EdgeControl());
                //}                
            }

            foreach (var e in this.Entities)
            {
                foreach (var r in e.References)
                {

                    var targetEntity = this.Entities.Where(e1 => e1.Name.Equals(r.EntityName)).FirstOrDefault();

                    var sourceVertex = this.EntityArea.VertexList.Where(v => v.Key.Equals(e)).FirstOrDefault().Value;
                    var targetVertex = this.EntityArea.VertexList.Where(t => t.Key.Equals(targetEntity)).FirstOrDefault().Value;

                    var ee = new EntityEdge(e, targetEntity);
                    var ec = new EdgeControl(sourceVertex, targetVertex, ee);

                    this.EntityArea.InsertEdgeAndData(ee, ec);
                }
            }


            this.RelayoutGraph();
        }

        /// <summary>
        /// Relayout graph
        /// </summary>
        private void RelayoutGraph()
        {
            //we have to check if there is only one vertex and set coordinates manulay 
            //because layout algorithms skip all logic if there are less than two vertices
            if (EntityArea.VertexList.Count == 1)
            {
                EntityArea.VertexList.First().Value.SetPosition(0, 0);
                EntityArea.UpdateLayout(); //update layout to update vertex size
            }
            else
                EntityArea.RelayoutGraph(true);
        }

        private EntityArea entityArea;

        /// <summary>
        /// The entity area
        /// </summary>
        public EntityArea EntityArea
        {
            get { return entityArea; }
            set { this.SetProperty<EntityArea>(ref this.entityArea, value); }
        }

        private ObservableCollection<EntityVertex> entities;

        /// <summary>
        /// List with the entities
        /// </summary>
        public ObservableCollection<EntityVertex> Entities
        {
            get { return entities; }
            set { this.SetProperty<ObservableCollection<EntityVertex>>(ref this.entities, value); }
        }
    }
}
