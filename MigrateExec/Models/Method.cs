namespace MigrateExec.Models
{
    public class Method
    {
        public string Name { get; set; }
        public string NameHandler { get; set; }
        public string NameMod { get; set; }
        public string ReturnModel { get; set; }
        public List<(string, string)> ParametersMethod { get; set; }

    }
}
