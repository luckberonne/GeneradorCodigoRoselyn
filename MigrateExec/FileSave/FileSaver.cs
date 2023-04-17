namespace MigrateExec.FileSave
{
    internal class FileSaver
    {
        public static void Save(string path, string nombreArchivo, string code)
        {
            // verificar si la carpeta existe
            if (!Directory.Exists(path))
            {
                // si la carpeta no existe, crearla
                Directory.CreateDirectory(path);
            }

            // guardar el archivo en la carpeta
            string rutaCompleta = Path.Combine(path, nombreArchivo);
            using (StreamWriter archivo = new StreamWriter(rutaCompleta))
            {
                archivo.Write(code);
            }
        }
    }
}
