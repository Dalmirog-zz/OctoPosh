using Octopus.Client.Model;

namespace OctoposhCli
{
    public class CmdletParameter
    {
        public string Name { get; set; }
        public string[] MultipleValue { get; set; }
        public string SingleValue { get; set; }
        public Resource Resource { get; set; }
    }
}
