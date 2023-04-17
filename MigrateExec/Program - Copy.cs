using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MigrateExec.Models;

class Program
{
    static void Main(string[] args)
    {
        string[] files = null;
        if (args.Length == 0)
        {
            Console.WriteLine("Debe proporcionar una ruta de archivo para analizar.");
            return;
        }
        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"El archivo '{filePath}' no existe.");
            return;
        }

        try
        {
            string code = File.ReadAllText(filePath);
            string projectRoot = Path.GetDirectoryName(filePath);
            string project = Path.GetDirectoryName(projectRoot);
            Console.WriteLine($"Path Proyecto: {project}");


            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            var root = syntaxTree.GetRoot();

            var classes = new List<ClassDeclarationSyntax>();
            root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList().ForEach(classes.Add);

            Console.WriteLine($"Archivo analizado: {filePath}");
            //Console.WriteLine($"Tamaño del archivo: {new FileInfo(filePath).Length} bytes");
            //Console.WriteLine($"Fecha de creación: {File.GetCreationTime(filePath)}");
            //Console.WriteLine($"Fecha de última modificación: {File.GetLastWriteTime(filePath)}");
            Console.WriteLine($"Clases encontradas: {classes.Count}\n");

            foreach (var classSyntax in classes)
            {
                var className = classSyntax.Identifier.Text;
                var classModifiers = string.Join(" ", classSyntax.Modifiers.Select(m => m.Text));
                var classFields = classSyntax.Members.OfType<FieldDeclarationSyntax>().Count();
                var classMethods = classSyntax.Members.OfType<MethodDeclarationSyntax>().ToList();
                var classProperties = classSyntax.Members.OfType<PropertyDeclarationSyntax>().Count();
                var classConstructors = classSyntax.Members.OfType<ConstructorDeclarationSyntax>().Count();

                Console.WriteLine($"Nombre de la clase: {className}");
                Console.WriteLine($"Modificadores: {classModifiers}");
                Console.WriteLine($"Campos: {classFields}");
                Console.WriteLine($"Métodos: {classMethods.Count}");

                foreach (var methodSyntax in classMethods)
                {
                    var methodName = methodSyntax.Identifier.Text;
                    var methodModifiers = string.Join(" ", methodSyntax.Modifiers.Select(m => m.Text));
                    var methodReturnType = methodSyntax.ReturnType.ToString();
                    var methodParameters = methodSyntax.ParameterList.ToString();

                    var method = new Method
                    {
                        Name = methodName,
                        NameQuery = methodName + "Query",
                        NameCommand = methodName + "Command",
                        NameHandler = methodName + "Handler",
                        ReturnModel = methodReturnType,
                        ParametersMethod = new ParameterMethod
                        {
                            Type = methodSyntax.ParameterList.Parameters.,
                            Name = methodName
                        }
                    };

                    Console.WriteLine($"\tNombre del método: {methodName}");
                    //Console.WriteLine($"\tModificadores: {methodModifiers}");
                    Console.WriteLine($"\tTipo de retorno: {methodReturnType}");
                    Console.WriteLine($"\tParámetros: {methodParameters}");

                    //    var localVariables = methodSyntax.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();

                    //    if (localVariables.Any())
                    //    {
                    //        Console.WriteLine("\tVariables locales:");

                    //        foreach (var variableSyntax in localVariables)
                    //        {
                    //            var variableName = variableSyntax.Declaration.Variables.First().Identifier.Text;
                    //            var variableType = variableSyntax.Declaration.Type.ToString();

                    //            Console.WriteLine($"\t\tNombre: {variableName}");
                    //            Console.WriteLine($"\t\tTipo: {variableType}");
                    //        }
                    //    }

                    //    var nestedClasses = methodSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>();

                    //    if (nestedClasses.Any())
                    //    {
                    //        Console.WriteLine("\tClases anidadas:");

                    //        foreach (var nestedClassSyntax in nestedClasses)
                    //        {
                    //            var className2 = nestedClassSyntax.Identifier.Text;
                    //            var classModifiers2 = string.Join(" ", nestedClassSyntax.Modifiers.Select(m => m.Text));
                    //            var classFields2 = nestedClassSyntax.Members.OfType<FieldDeclarationSyntax>().Count();
                    //            var classMethods2 = nestedClassSyntax.Members.OfType<MethodDeclarationSyntax>().ToList();
                    //            var classProperties2 = nestedClassSyntax.Members.OfType<PropertyDeclarationSyntax>().Count();
                    //            var classConstructors2 = nestedClassSyntax.Members.OfType<ConstructorDeclarationSyntax>().Count();

                    //            Console.WriteLine($"\t\tNombre de la clase: {className2}");
                    //            Console.WriteLine($"\t\tModificadores: {classModifiers2}");
                    //            Console.WriteLine($"\t\tCampos: {classFields2}");
                    //            Console.WriteLine($"\t\tMétodos: {classMethods2.Count}");
                    //            Console.WriteLine($"\t\tPropiedades: {classProperties2}");
                    //            Console.WriteLine($"\t\tConstructores: {classConstructors2}\n");
                    //        }
                    //    }


                    //    var objectCreationExpressions = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();

                    //    if (objectCreationExpressions.Any())
                    //    {
                    //        Console.WriteLine("\tObjetos creados de Clases:");
                    //        foreach (var objectCreationExpression in objectCreationExpressions)
                    //        {
                    //            var objectCreation = objectCreationExpression.Type.ToString();
                    //            Console.WriteLine($"\t\tObjecto creado: {objectCreation}");

                    //            files = Directory.GetFiles(project, objectCreation + ".cs", SearchOption.AllDirectories);

                    //            if (files.Length == 0)
                    //            {
                    //                Console.WriteLine($"\t\tFile not found.");
                    //            }
                    //            else
                    //            {
                    //                //Console.WriteLine($"Found {files.Length} matching file(s):");
                    //                foreach (string file in files)
                    //                {
                    //                    Console.WriteLine($"\t\tUbicacion: {file}");
                    //                }

                    //            }
                    //        }
                    //    }


                    //    var methodInvocations = methodSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>();

                    //    if (methodInvocations.Any())
                    //    {
                    //        Console.WriteLine("\tMétodos invocados:");

                    //        foreach (var invocationSyntax in methodInvocations)
                    //        {
                    //            var invocationName = invocationSyntax.ToString();

                    //            Console.WriteLine($"\t\t{invocationName}");
                    //        }
                    //    }
                }

                //Console.WriteLine($"Propiedades: {classProperties}");
                //Console.WriteLine($"Constructores: {classConstructors}\n");
            }
            //genera una prorpiedad recursiva para los objetos creados de clases
            //Console.WriteLine($"----------------------------------------------");

            //Main(files);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al analizar el archivo: {ex.Message}");
        }
    }
}
