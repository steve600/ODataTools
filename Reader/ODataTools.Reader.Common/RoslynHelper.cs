using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Generic;
using System.IO;
using ODataTools.Infrastructure.ExtensionMethods;
using System;

namespace ODataTools.Reader.Common
{
    public static class RoslynHelper
    {
        /// <summary>
        /// Create class declaration
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="baseClass">The base class.</param>
        /// <returns>The generated class declaration syntax.</returns>
        public static ClassDeclarationSyntax CreateClass(string className, string baseClass = "")
        {
            ClassDeclarationSyntax result = null;

            // Create class
            if (string.IsNullOrEmpty(baseClass))
            {
                result = SyntaxFactory.ClassDeclaration(className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            }
            else
            {
                result = SyntaxFactory.ClassDeclaration(className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                         .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseClass)));
            }

            return result;
        }

        /// <summary>
        /// Create DTO proxy class
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="baseClass">The base class.</param>
        /// <param name="dtoType">The DTO type.</param>
        /// <returns></returns>
        public static ClassDeclarationSyntax CreateDtoProxyClass(string className)
        {
            ClassDeclarationSyntax result = null;

            string proxyClass = $"{className}Proxy";
            string baseClass = "DtoProxyBase";

            var ctor = SyntaxFactory.ConstructorDeclaration(new SyntaxList<AttributeListSyntax>(),
                                                            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                                                            SyntaxFactory.ParseToken(proxyClass),
                                                            SyntaxFactory.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier("model")).WithType(SyntaxFactory.ParseTypeName(className)))),
                                                            SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, SyntaxFactory.Token(SyntaxKind.ColonToken), SyntaxFactory.Token(SyntaxKind.BaseKeyword), SyntaxFactory.ParseArgumentList("(model)")),
                                                            SyntaxFactory.Block(SyntaxFactory.Token(SyntaxKind.OpenBraceToken), new SyntaxList<StatementSyntax>(), SyntaxFactory.Token(SyntaxKind.CloseBraceToken)));

            result = SyntaxFactory.ClassDeclaration(proxyClass)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"{baseClass}<{className}>")))
                        .AddMembers(ctor);

            return result;
        }
        /// <summary>
        /// Create a property
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The generated property.</returns>
        public static PropertyDeclarationSyntax CreateProperty(string propertyName, string propertyType)
        {
            PropertyDeclarationSyntax property = null;

            try
            {
                // Crate property
                property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyType), propertyName)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>()
                         .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.GetKeyword), null, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                         .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.SetKeyword), null, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));
            }
            catch (Exception ex)
            {
                // TODO: Logging, ...
                string errorMsg = $"Error while creating property with name {propertyName} and Type {propertyType}" + System.Environment.NewLine;
                errorMsg += "Exception was:" + Environment.NewLine;
                errorMsg += ex.StackTrace;

                System.Diagnostics.Debug.WriteLine(errorMsg);
            }

            return property;
        }

        /// <summary>
        /// Create a property
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The generated property.</returns>
        public static IList<MemberDeclarationSyntax> CreateBindableProperty(string propertyName, string propertyType)
        {
            IList<MemberDeclarationSyntax> result = new List<MemberDeclarationSyntax>();

            string privateIdentifier = $"_{propertyName.FirstCharacterToLower()}";
            string publicIdentifier = propertyName.FirstCharacterToUpper();

            // Create field
            FieldDeclarationSyntax field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(propertyType),
                                                                new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(privateIdentifier)))))
                                                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));

            result.Add(field);

            // Crate property
            var getExpression = SyntaxFactory.ParseExpression($"return this.{privateIdentifier}");
            var getBlock = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(getExpression));

            var setExpression = SyntaxFactory.ParseExpression($"this.SetProperty(ref this.{privateIdentifier}, value)");
            var setBlock = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(setExpression));

            var prop = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyType), publicIdentifier)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>()
                     .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.GetKeyword), getBlock, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                     .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.SetKeyword), setBlock, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

            result.Add(prop);

            return result;
        }

        /// <summary>
        /// Create a property
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The generated property.</returns>
        public static IList<MemberDeclarationSyntax> CreateDtoProxyProperty(string propertyName, string propertyType)
        {
            IList<MemberDeclarationSyntax> result = new List<MemberDeclarationSyntax>();

            string privateIdentifier = $"_{propertyName.FirstCharacterToLower()}";
            string publicIdentifier = propertyName.FirstCharacterToUpper();

            // Crate property
            var getExpression = SyntaxFactory.ParseExpression($"return GetProperty<{propertyType}>()");
            var getBlock = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(getExpression));

            var setExpression = SyntaxFactory.ParseExpression($"SetProperty<{propertyType}>(value)");
            var setBlock = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(setExpression));

            var prop = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyType), publicIdentifier)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>()
                     .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.GetKeyword), getBlock, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                     .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.SetKeyword), setBlock, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

            result.Add(prop);

            // Getter for orignal value
            var getExpressionOrg = SyntaxFactory.ParseExpression($"return GetOriginalValue<{propertyType}>(nameof({publicIdentifier}));");
            var getBlockOrg = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(getExpressionOrg));

            var propOrg = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyType), $"{publicIdentifier}OriginalValue")
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(new SyntaxList<AccessorDeclarationSyntax>()
                     .Add(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SyntaxFactory.Token(SyntaxKind.GetKeyword), getBlockOrg, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))));

            result.Add(propOrg);

            return result;
        }

        /// <summary>
        /// Create attribute syntax
        /// </summary>
        /// <param name="name">The attribute name, e.g. System.Data.Services.Common.DataServiceKeyAttribute</param>
        /// <param name="arguments">The attribute arguments, e.g. ("Key_1", "Key_2")</param>
        /// <returns>Returns arugment Syntax</returns>
        public static AttributeSyntax CreateAttribute(string name, string arguments)
        {
            // Add class attributes
            var attributeName = SyntaxFactory.ParseName(name);
            //var arguments = SyntaxFactory.ParseAttributeArgumentList("(\"some_param\")");
            var attributeArguments = SyntaxFactory.ParseAttributeArgumentList(arguments);
            var attribute = SyntaxFactory.Attribute(attributeName, attributeArguments); //MyAttribute("some_param")

            return attribute;
        }

        /// <summary>
        /// Create compilation unit
        /// </summary>
        /// <returns></returns>
        public static CompilationUnitSyntax CreateCompilationUnit()
        {
            return SyntaxFactory.CompilationUnit()
                                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")));
        }

        /// <summary>
        /// Create namespace
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        public static NamespaceDeclarationSyntax CreateNamespace(string namespaceName)
        {
            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName));
        }

        /// <summary>
        /// Edit and copy BindableBase file
        /// </summary>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="targetFolder">The target folder.</param>
        public static Dictionary<FileInfo, string> EditAndCopyNotifyPropertyChangedBaseClasses(string targetNamespace, string targetFolder)
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            var templateFile = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Templates\NotifyPropertyChanged\BindableBase.cs");

            var targetFile = Path.Combine(targetFolder, "BindableBase.cs");

            FileInfo file = CopyAndEditTemplateFile(templateFile, targetFile, targetNamespace);

            if (file != null)
                result.Add(file, File.ReadAllText(file.FullName));

            return result;
        }

        public static Dictionary<FileInfo, string> EditAndCopyDtoProxyClasses(string targetNamespace, string targetFolder)
        {
            Dictionary<FileInfo, string> result = new Dictionary<FileInfo, string>();

            var bindableBase = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Templates\DtoProxy\BindableBase.cs");
            var bindableBaseTargetFile = Path.Combine(targetFolder, "BindableBase.cs");
            var proxyBase = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Templates\DtoProxy\DtoProxyBase.cs");
            var proxyBaseTargetFile = Path.Combine(targetFolder, "DtoProxyBase.cs");

            FileInfo file = CopyAndEditTemplateFile(bindableBase, bindableBaseTargetFile, targetNamespace);

            if (file != null)
                result.Add(file, File.ReadAllText(file.FullName));

            file = CopyAndEditTemplateFile(proxyBase, proxyBaseTargetFile, targetNamespace);

            if (file != null)
                result.Add(file, File.ReadAllText(file.FullName));

            return result;

        }

        private static FileInfo CopyAndEditTemplateFile(string templateFile, string targetFile, string targetNamespace)
        {
            FileInfo result = null;

            if (File.Exists(templateFile))
            {
                var text = File.ReadAllText(templateFile);

                var cu = SyntaxFactory.ParseCompilationUnit(text);

                var oldNamespace = cu.Members[0] as NamespaceDeclarationSyntax;

                NamespaceDeclarationSyntax newNamespace =
                        oldNamespace.Update(oldNamespace.NamespaceKeyword,
                                            SyntaxFactory.ParseName(targetNamespace),
                                            oldNamespace.OpenBraceToken,
                                            oldNamespace.Externs,
                                            oldNamespace.Usings,
                                            oldNamespace.Members,
                                            oldNamespace.CloseBraceToken,
                                            oldNamespace.SemicolonToken);

                cu = cu.ReplaceNode(oldNamespace, newNamespace);
               
                FormatAndWriteToFile(targetFile, cu);

                result = new FileInfo(targetFile);
            }

            return result;
        }

        /// <summary>
        /// Format and write to a text file
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cu">The compilation unit.</param>
        public static string FormatAndWriteToFile(string filePath, CompilationUnitSyntax cu)
        {
            SyntaxNode node = Formatter.Format(cu, new AdhocWorkspace());

            using (TextWriter writer = new StreamWriter(File.Create(filePath, 1024, FileOptions.Asynchronous)))
            {
                writer.Write(node.ToFullString());
                writer.Close();

                //node.WriteTo(writer);
            }

            return node.ToFullString();
        }
    }
}
