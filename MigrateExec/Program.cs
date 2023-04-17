using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MigrateExec.FileSave;
using MigrateExec.Generator;
using MigrateExec.Models;

class Program
{
    static void Main(string[] args)
    {
        string carpeta = @"C:\Desarrollo\NuevoKrikos\K360.Web.Backend\K360.Web.Backend\K360.Web.Backend\LegacyServices\CargaManual\Servicios\";
        string[] archivos = Directory.GetFiles(carpeta);
        //List<string> files = new List<string>();

        foreach (string archivo in archivos)
        {
            //files.Add(archivo);
            if (File.Exists(archivo) && Path.GetFileName(archivo).Substring(0, 1).ToLower() != "i")
            {

                //if (args.Length == 0)
                //{
                //    Console.WriteLine("Debe proporcionar una ruta de archivo para analizar.");
                //    return;
                //}
                //string filePath = args[0];

                //if (!File.Exists(filePath))
                //{
                //    Console.WriteLine($"El archivo '{filePath}' no existe.");
                //    return;
                //}

                try
                {
                    string code = File.ReadAllText(archivo);
                    string projectRoot = Path.GetDirectoryName(archivo);
                    string project = Path.GetDirectoryName(projectRoot);
                    Console.WriteLine($"Path Proyecto: {project}");


                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

                    var root = syntaxTree.GetRoot();

                    var classes = new List<ClassDeclarationSyntax>();
                    root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList().ForEach(classes.Add);

                    Console.WriteLine($"Archivo analizado: {archivo}");
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
                            Method method = null;
                            var methodName = methodSyntax.Identifier.Text;
                            var methodModifiers = string.Join(" ", methodSyntax.Modifiers.Select(m => m.Text));
                            var methodReturnType = methodSyntax.ReturnType.ToString();
                            var methodParameters = methodSyntax.ParameterList.ToString();
                            List<(string, string)> parameters = new List<(string, string)>();

                            foreach (var parameter in methodSyntax.ParameterList.Parameters)
                            {
                                var parameterType = parameter.Type.ToString();
                                var parameterName = parameter.Identifier.Text;
                                parameters.Add((parameterName, parameterType));
                            }

                            if (methodModifiers == "public")
                            {
                                string add = "";
                                if (methodName.ToLower().Contains("recuperar") || methodName.ToLower().Contains("listar") ||
                                    methodName.ToLower().Contains("get") || methodName.ToLower().Contains("traer") || methodName.ToLower().Contains("existe"))
                                {
                                    add = "Query";
                                }
                                else
                                {
                                    add = "Command";
                                    if (methodReturnType == "void")
                                    {
                                        methodReturnType = "Result";
                                    }
                                }
                                method = new Method
                                {
                                    Name = methodName,
                                    NameHandler = methodName + "Handler",
                                    NameMod = methodName + add,
                                    ReturnModel = methodReturnType,
                                    ParametersMethod = parameters
                                };
                            }

                            var codeQueryCommand = QueryOrCommand.GenerateClass(method);
                            var codeHandler = Handler.GenerateClass(method, className);

                            string nameFile = "";
                            string pathSave = "";
                            // Guardar el archivo generado
                            if (method.NameMod.ToLower().Contains("query"))
                            {
                                nameFile = method.NameMod + ".cs";
                                pathSave = @"C:\Users\LBeronne\Desktop\MediaTR\" + Path.GetFileName(archivo) + @"\Query\";
                                FileSaver.Save(pathSave, nameFile, codeQueryCommand);
                            }
                            else
                            {
                                nameFile = method.NameMod + ".cs";
                                pathSave = @"C:\Users\LBeronne\Desktop\MediaTR\" + Path.GetFileName(archivo) + @"\Command\";
                                FileSaver.Save(pathSave, nameFile, codeQueryCommand);
                            }

                            nameFile = method.NameHandler + ".cs";
                            pathSave = @"C:\Users\LBeronne\Desktop\MediaTR\" + Path.GetFileName(archivo) + @"\Handler\";
                            FileSaver.Save(pathSave, nameFile, codeHandler);

                            Console.WriteLine($"\tNombre del método: {methodName}");
                            Console.WriteLine($"\tModificadores: {methodModifiers}");
                            Console.WriteLine($"\tTipo de retorno: {methodReturnType}");
                            Console.WriteLine($"\tParámetros: {methodParameters}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al analizar el archivo: {ex.Message}");
                }
            }
        }
    }
}
