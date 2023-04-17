namespace MigrateExec.Generator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            string code = @"
        class MyClass
        {
            void MyMethod()
            {
                string s = ""hola"";
                Console.WriteLine(s);
            }

            void MyOtherMethod()
            {
                Console.WriteLine(""otro método"");
            }
        }";

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);

            foreach (var member in tree.GetCompilationUnitRoot().Members)
            {
                if (member is ClassDeclarationSyntax classDeclaration)
                {
                    foreach (var method in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
                    {
                        var methodBody = method.Body;
                        if (methodBody != null)
                        {
                            foreach (var node in methodBody.DescendantNodes())
                            {
                                if (node is LiteralExpressionSyntax literal && literal.Token.ValueText == "hola")
                                {
                                    Console.WriteLine($"Encontrado 'hola' en el método '{method.Identifier.Text}'");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
