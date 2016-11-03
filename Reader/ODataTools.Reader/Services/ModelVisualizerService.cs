﻿using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.ModelVisualizer.Contracts.Model;
using ODataTools.Reader.Common.Common;
using System.Collections.ObjectModel;


namespace ODataTools.Reader.Services
{
    public class ModelVisualizerService : IModelVisualizer
    {
        public ObservableCollection<EntityVertex> GetEntitiesForVisualization(string sourceFile)
        {
            var modelVisualizer = this.GetModelVisualizer(sourceFile);

            return modelVisualizer?.GetEntitiesForVisualization(sourceFile);
        }

        private IModelVisualizer GetModelVisualizer(string sourceFile)
        {
            IModelVisualizer modelVisualizer = null;

            EdmxVersion edmxVersion = ModelHelper.DetectEdmxVersion(sourceFile);
            ODataServiceVersion odataVersion = ModelHelper.DetectODataServiceVersion(sourceFile);

            if (edmxVersion == EdmxVersion.V1 ||
                edmxVersion == EdmxVersion.V2 ||
                edmxVersion == EdmxVersion.V3)
            {
                modelVisualizer = new ODataTools.Reader.V3.Visualization.ModelVisualizer();
            }
            else
            {
                //modelVisualizer = new ODataTools.Reader.V4.Generator.DtoGenerator();
            }

            return modelVisualizer;
        }
    }
}
