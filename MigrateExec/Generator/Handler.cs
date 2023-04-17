using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MigrateExec.Models;

namespace MigrateExec.Generator
{
    public class Handler
    {
        public static string GenerateClass(Method method, string className)
        {
            // Definir el namespace y la clase base
            string namespaceName = "K360.Web.Backend.LegacyServices.CargaManual";
            string baseClassName = "IRequestHandler<" + method.NameMod + ", IEnumerable<" + method.ReturnModel + ">>";

            // Crear una unidad de compilación
            CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MediatR")),
                           SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading.Tasks")));

            NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName));

            // Crear la clase y agregar las propiedades
            ClassDeclarationSyntax classDeclaration = SyntaxFactory.ClassDeclaration(method.NameHandler)
                .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseClassName)))));


            classDeclaration = classDeclaration.AddMembers(
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("I" + className), "_" + className)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));


            // Agregar el método con parámetros
            MethodDeclarationSyntax methodDeclaration = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                method.NameHandler
            )
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(className[0].ToString().ToLower() + className.Substring(1)))
                .WithType(SyntaxFactory.ParseTypeName("I" + className))
            )
            .WithBody(SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName($"_{className}"),
                    SyntaxFactory.IdentifierName(className[0].ToString().ToLower() + className.Substring(1))

            ))));

            classDeclaration = classDeclaration.AddMembers(methodDeclaration);



            var arguments = new List<ArgumentSyntax>();

            foreach ((string propertyName, string propertyType) in method.ParametersMethod)
            {
                var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName("request._" + propertyName));
                arguments.Add(argument);
            }

            var argumentList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments, Enumerable.Repeat(SyntaxFactory.Token(SyntaxKind.CommaToken), arguments.Count - 1)));

            // Crea una nueva declaración de método con un tipo de retorno "Task" y el nombre del método especificado.
            MethodDeclarationSyntax methodDeclaration1 = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName(SyntaxFactory.Identifier("Task"))
                    .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword))))),
                "Handle"
            )

            // Agrega el modificador "public" al método.
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.AsyncKeyword))

            // Agrega los parámetros al método.
            .AddParameterListParameters(
                // El primer parámetro es el objeto `customModel` que se va a procesar.
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("request"))
                    .WithType(SyntaxFactory.ParseTypeName(method.NameMod)),

                // El segundo parámetro es el token de cancelación para cancelar la operación de procesamiento.
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("cancellationToken"))
                    .WithType(SyntaxFactory.ParseTypeName("CancellationToken"))
            )

            // Agrega el cuerpo del método. El cuerpo del método llama a un método asincrónico que realiza el procesamiento y devuelve una tarea.
            .WithBody(SyntaxFactory.Block(
                SyntaxFactory.ReturnStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("await " + $"_{className}." + method.Name)
                    )
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments)
                        )
                    )
                )
            ));

            // Agrega el método a la clase.
            classDeclaration = classDeclaration.AddMembers(methodDeclaration1);


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
