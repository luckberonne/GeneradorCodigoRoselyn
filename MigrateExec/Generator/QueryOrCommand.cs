using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MigrateExec.Models;

namespace MigrateExec.Generator
{
    public static class QueryOrCommand
    {
        public static string GenerateClass(Method method)
        {
            // Definir el namespace y la clase base
            string namespaceName = "K360.Web.Backend.LegacyServices.CargaManual";
            string baseClassName = "IRequest<" + method.ReturnModel + ">";

            // Crear una unidad de compilación
            CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MediatR")),
                           SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Diagnostics.CodeAnalysis")));

            NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));

            // Crear la clase y agregar las propiedades
            ClassDeclarationSyntax classDeclaration = SyntaxFactory.ClassDeclaration(method.NameMod)
                .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseClassName)))));

            foreach ((string propertyName, string propertyType) in method.ParametersMethod)
            {
                classDeclaration = classDeclaration.AddMembers(
                 SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyType), "_" + propertyName)
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));
            }

            // Agregar el método con parámetros
            MethodDeclarationSyntax methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), method.NameMod)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(method.ParametersMethod.Select(p => SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Item1)).WithType(SyntaxFactory.ParseTypeName(p.Item2))).ToArray())
                .WithBody(SyntaxFactory.Block(method.ParametersMethod.Select(p => SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName($"_{p.Item1}"),
                        SyntaxFactory.IdentifierName(p.Item1)))).ToArray()));

            classDeclaration = classDeclaration.AddMembers(methodDeclaration)
                                               .AddAttributeLists(SyntaxFactory.AttributeList().AddAttributes(SyntaxFactory.Attribute(SyntaxFactory.ParseName("ExcludeFromCodeCoverage"))
                                                                                                                              .AddArgumentListArguments(SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression("\"Clase no testeable\"")))));

            // Agregar la clase al namespace y la unidad de compilación
            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);
            compilationUnit = compilationUnit.AddMembers(namespaceDeclaration);

            // Obtener la raíz de la sintaxis
            SyntaxNode syntaxNode = compilationUnit;

            var code = compilationUnit.NormalizeWhitespace().ToFullString();
            return code;

            // Guardar el archivo generado
            //string filePath = @"C:\ruta\al\archivo.cs";
            //using (var file = new System.IO.StreamWriter(filePath))
            //{
            //    file.Write(syntaxNode.ToFullString());
            //}
        }
    }
}
