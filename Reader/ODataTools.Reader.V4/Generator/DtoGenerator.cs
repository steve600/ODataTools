﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.OData.Edm;
using ODataTools.DtoGenerator.Contracts;
using ODataTools.DtoGenerator.Contracts.Enums;
using ODataTools.DtoGenerator.Contracts.Interfaces;
using ODataTools.Infrastructure.ExtensionMethods;
using ODataTools.Reader.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ODataTools.Reader.V4.Generator
{
    public class DtoGenerator : IDtoGenerator
    {
        private readonly Dictionary<EdmPrimitiveTypeKind, string> ClrDictionary = new Dictionary<EdmPrimitiveTypeKind, string>
        {
            {EdmPrimitiveTypeKind.Int32, "int"},
            {EdmPrimitiveTypeKind.String, "string"},
            {EdmPrimitiveTypeKind.Binary, "byte[]"},
            {EdmPrimitiveTypeKind.Decimal, "decimal"},
            {EdmPrimitiveTypeKind.Int16, "short"},
            {EdmPrimitiveTypeKind.Single, "float"},
            {EdmPrimitiveTypeKind.Boolean, "bool"},
            {EdmPrimitiveTypeKind.Double, "double"},
            {EdmPrimitiveTypeKind.Guid, "Guid"},
            {EdmPrimitiveTypeKind.Byte, "byte"},
            {EdmPrimitiveTypeKind.Int64, "long"},
            {EdmPrimitiveTypeKind.SByte, "sbyte"},
            {EdmPrimitiveTypeKind.Stream, "Stream"},
            {EdmPrimitiveTypeKind.Geography, "Geography"},
            {EdmPrimitiveTypeKind.GeographyPoint, "GeographyPoint"},
            {EdmPrimitiveTypeKind.GeographyLineString, "GeographyLineString"},
            {EdmPrimitiveTypeKind.GeographyPolygon, "GeographyMultiPolygon"},
            {EdmPrimitiveTypeKind.GeographyCollection, "GeographyCollection"},
            {EdmPrimitiveTypeKind.GeographyMultiPolygon, "GeographyMultiPolygon"},
            {EdmPrimitiveTypeKind.GeographyMultiLineString, "GeographyMultiLineString"},
            {EdmPrimitiveTypeKind.GeographyMultiPoint, "GeographyMultiPoint"},
            {EdmPrimitiveTypeKind.Geometry, "Geometry"},
            {EdmPrimitiveTypeKind.GeometryPoint, "GeometryPoint"},
            {EdmPrimitiveTypeKind.GeometryLineString, "GeometryLineString"},
            {EdmPrimitiveTypeKind.GeometryPolygon, "GeometryPolygon"},
            {EdmPrimitiveTypeKind.GeometryCollection, "GeometryCollection"},
            {EdmPrimitiveTypeKind.GeometryMultiPolygon, "GeometryMultiPolygon"},
            {EdmPrimitiveTypeKind.GeometryMultiLineString, "GeometryMultiLineString"},
            {EdmPrimitiveTypeKind.GeometryMultiPoint, "GeometryMultiPoint"},
            {EdmPrimitiveTypeKind.DateTimeOffset, "DateTimeOffset"}
        };

        #region CTOR

        public DtoGenerator()
        {

        }

        #endregion CTOR      

        /// <summary>
        /// Generate the DTO classes for a given model
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="dtoGeneratorSettings">The DTO generator settings.</param>
        /// <returns></returns>
        public Dictionary<FileInfo, string> GenerateDtoClassesForModel(string metaDataDocument, DtoGeneratorSettings dtoGeneratorSettings, string outputFileName = "Generated.cs")
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            IEdmModel model = ModelReader.Parse(metaDataDocument);

            if (model != null)
            {
                // Get entities
                var entities = model.SchemaElements.Where(e => e.SchemaElementKind == Microsoft.OData.Edm.EdmSchemaElementKind.TypeDefinition &&
                                                               e is Microsoft.OData.Edm.IEdmEntityType)
                                                   .Cast<IEdmEntityType>();

                if (entities != null && entities.Any())
                {
                    // Edit and copy template files
                    result.AddRange(this.EditAndCopyTemplateFiles(dtoGeneratorSettings));

                    switch (dtoGeneratorSettings.DataClassOptions)
                    {
                        case DataClassOptions.DTO:
                        case DataClassOptions.INotifyPropertyChanged:
                            result.AddRange(this.GenerateClasses(entities, dtoGeneratorSettings.DataClassOptions, dtoGeneratorSettings.GenerateAttributes, dtoGeneratorSettings.GenerateSingleFilePerDto,
                                                                 dtoGeneratorSettings.TargetNamespace, dtoGeneratorSettings.OutputPath, outputFileName));
                            break;
                        case DataClassOptions.Proxy:
                            result.AddRange(this.GenerateClasses(entities, dtoGeneratorSettings.DataClassOptions, dtoGeneratorSettings.GenerateAttributes, dtoGeneratorSettings.GenerateSingleFilePerDto,
                                                                 dtoGeneratorSettings.TargetNamespace, dtoGeneratorSettings.OutputPath, outputFileName));
                            result.AddRange(this.GenerateClasses(entities, DataClassOptions.DTO, dtoGeneratorSettings.GenerateAttributes, dtoGeneratorSettings.GenerateSingleFilePerDto,
                                                                 dtoGeneratorSettings.TargetNamespace, dtoGeneratorSettings.OutputPath, outputFileName));
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private Dictionary<FileInfo, string> GenerateClasses(IEnumerable<IEdmEntityType> entities, DataClassOptions dataClassOption, bool generateWithAttributes,
                                                             bool generateSinglePerDto, string targetNamespace, string outputPath, string outputFileName = "Generated.cs")
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            CompilationUnitSyntax cu = RoslynHelper.CreateCompilationUnit();

            // Create Namespace
            var ns = RoslynHelper.CreateNamespace(targetNamespace);

            foreach (var e in entities)
            {
                // Create class
                ClassDeclarationSyntax cls = CreateClass(e, dataClassOption, generateWithAttributes);

                // Create properties
                foreach (var p in e.DeclaredProperties)
                {
                    if (p.PropertyKind != EdmPropertyKind.Navigation)
                    {
                        foreach (var newProperty in this.CreateProperty(p, dataClassOption))
                            cls = cls.AddMembers(newProperty);
                    }
                }

                // Add class to Namespace
                ns = ns.AddMembers(cls);

                if (generateSinglePerDto)
                {
                    cu = cu.AddMembers(ns);
                    string fileName = System.IO.Path.Combine(outputPath, $"{e.Name}.cs");

                    string generatedSource = RoslynHelper.FormatAndWriteToFile(fileName, cu);
                    result.Add(new FileInfo(fileName), generatedSource);

                    // Reset CompilationUnit
                    cu = RoslynHelper.CreateCompilationUnit();

                    // Reset namespace
                    ns = RoslynHelper.CreateNamespace(targetNamespace);
                }
            }

            if (!generateSinglePerDto)
            {
                if (dataClassOption == DataClassOptions.Proxy)
                {
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName) + "Proxy.cs";
                }

                // Add Namespace to CompilationUnit
                cu = cu.AddMembers(ns);
                string fileName = Path.Combine(outputPath, outputFileName);

                string generatedSource = RoslynHelper.FormatAndWriteToFile(fileName, cu);
                result.Add(new FileInfo(fileName), generatedSource);
            }

            return result;
        }

        /// <summary>
        /// Edit and copy the template files
        /// </summary>
        /// <param name="dtoGeneratorSettings">The generator settings.</param>
        /// <returns></returns>
        private Dictionary<FileInfo, string> EditAndCopyTemplateFiles(DtoGeneratorSettings dtoGeneratorSettings)
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            // Create BindableBase class
            switch (dtoGeneratorSettings.DataClassOptions)
            {
                case DataClassOptions.INotifyPropertyChanged:
                    result.AddRange(RoslynHelper.EditAndCopyNotifyPropertyChangedBaseClasses(dtoGeneratorSettings.TargetNamespace, dtoGeneratorSettings.OutputPath));
                    break;
                case DataClassOptions.Proxy:
                    result.AddRange(RoslynHelper.EditAndCopyDtoProxyClasses(dtoGeneratorSettings.TargetNamespace, dtoGeneratorSettings.OutputPath));
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Create the DTO class
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private ClassDeclarationSyntax CreateClass(IEdmEntityType entity, DataClassOptions dataClassOption, bool generateWithAttributes)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null!");

            ClassDeclarationSyntax cls = null;

            switch (dataClassOption)
            {
                case DataClassOptions.DTO:
                    cls = RoslynHelper.CreateClass(entity.Name);
                    break;
                case DataClassOptions.INotifyPropertyChanged:
                    cls = RoslynHelper.CreateClass(entity.Name, "BindableBase");
                    break;
                case DataClassOptions.Proxy:
                    cls = RoslynHelper.CreateDtoProxyClass(entity.Name);
                    break;
                default:
                    cls = RoslynHelper.CreateClass(entity.Name);
                    break;
            }

            if (generateWithAttributes && dataClassOption != DataClassOptions.Proxy)
            {
                // Get attributes
                AttributeListSyntax attributes = GenerateAttributes(entity);

                if (attributes != null)
                    cls = cls.AddAttributeLists(attributes);
            }

            return cls;
        }

        /// <summary>
        /// Create property
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="generatorSettings">The gernator settings.</param>
        /// <returns></returns>
        private IList<MemberDeclarationSyntax> CreateProperty(IEdmProperty property, DataClassOptions dataClassOptions)
        {
            IList<MemberDeclarationSyntax> result = new List<MemberDeclarationSyntax>();

            switch (dataClassOptions)
            {
                case DataClassOptions.DTO:
                    result.Add(RoslynHelper.CreateProperty(property.Name, GetPropertyType(property)));
                    break;
                case DataClassOptions.INotifyPropertyChanged:
                    var props = RoslynHelper.CreateBindableProperty(property.Name, GetClrDataType(property.Type.PrimitiveKind()));
                    foreach (var p in props)
                        result.Add(p);
                    break;
                case DataClassOptions.Proxy:
                    var proxyProps = RoslynHelper.CreateDtoProxyProperty(property.Name, GetClrDataType(property.Type.PrimitiveKind()));
                    foreach (var p in proxyProps)
                        result.Add(p);
                    break;
                default:
                    result.Add(RoslynHelper.CreateProperty(property.Name, GetClrDataType(property.Type.PrimitiveKind())));
                    break;
            }

            return result;
        }

        private string GetPropertyType(IEdmProperty property)
        {
            string result = string.Empty;

            result = GetClrDataType(property.Type.PrimitiveKind());

            if (String.IsNullOrEmpty(result))
            {
                switch (property.Type.Definition.TypeKind)
                {
                    case EdmTypeKind.Enum:
                        // TODO: Create enum
                        var test = property.Type.AsEnum();
                        result = property.Type.ShortQualifiedName();
                        break;
                    case EdmTypeKind.Collection:
                        // TODO: Create collection
                        result = property.Type.ShortQualifiedName();
                        break;
                    default:
                        result = property.Type.ShortQualifiedName();
                        break;
                }                
            }

            return result;
        }

        /// <summary>
        /// Generate attributes
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The generated attributes.</returns>
        private AttributeListSyntax GenerateAttributes(IEdmEntityType entity)
        {
            // Add class attributes
            AttributeListSyntax attributes = null;

            // Attribute for the key fields
            var keyFields = GetKeyFieldsAsString(entity);
            if (!String.IsNullOrEmpty(keyFields))
            {
                var keyList = new SeparatedSyntaxList<AttributeSyntax>();
                keyList = keyList.Add(RoslynHelper.CreateAttribute("System.Data.Services.Common.DataServiceKeyAttribute", keyFields));
                attributes = SyntaxFactory.AttributeList(keyList);
            }

            return attributes;
        }

        /// <summary>
        /// Get key fields as string
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the key fields as comma separated string.</returns>
        private string GetKeyFieldsAsString(IEdmEntityType entity)
        {
            string keyList = string.Empty;

            if (entity.DeclaredKey.Count() > 0)
            {
                keyList = string.Join(",", entity.DeclaredKey.Select(k => $"\"{k.Name}\""));

                keyList = keyList.Insert(0, "(");
                keyList = keyList.Insert(keyList.Length, ")");
            }

            return keyList;
        }

        /// <summary>
        /// Get the CLR data type.
        /// </summary>
        /// <param name="type">The EMD data type.</param>
        /// <returns></returns>
        private string GetClrDataType(EdmPrimitiveTypeKind type)
        {
            string value;

            ClrDictionary.TryGetValue(type, out value);

            return value;
        }
    }
}