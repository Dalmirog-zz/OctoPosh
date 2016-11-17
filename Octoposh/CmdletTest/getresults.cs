using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace CmdletTest
{
    [Alias("gr")]
    [Cmdlet("Get", "results")]
    public class Getresults : PSCmdlet
    {
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string Name { get; set; }

        private List<string> Output = new List<string>();

        private List<string> Names = new List<string>()
        {
            "Ana",
            "Anna",
            "Banana",
            "Empanada"
        };



        protected override void BeginProcessing()
        {
            if (WildcardPattern.ContainsWildcardCharacters(Name))
            {
                var pattern = new WildcardPattern(Name.ToLower());

                Output.AddRange(Names.Where(
                    x => pattern.IsMatch(x.ToLower()) 
                ));
            }

            foreach (var name in Output)
            {
                Console.WriteLine(name);
            }

        }
    }
}
