using System;
using System.Management.Automation;

namespace Octoposh.Cmdlets
{
    [Cmdlet("Get","RandomName")]
    public class GetRandomName : Cmdlet
    {
        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            WriteVerbose(Name);

            var nameCharacters = Name.ToCharArray();
            Array.Reverse(nameCharacters);

            WriteObject(new {
                ReversedName = new String(nameCharacters),
                NameLenght = nameCharacters.Length
            });
        }

        protected override void BeginProcessing()
        {
            Console.WriteLine($"In BeginProcessing");
        }

        protected override void EndProcessing()
        {
            Console.WriteLine($"In EndProcessing");
        }

        protected override void StopProcessing()
        {
            Console.WriteLine($"in StopProcessing");
        }
    }
}
