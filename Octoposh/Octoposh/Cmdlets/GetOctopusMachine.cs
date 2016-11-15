using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Cmdlets
{
    [Cmdlet("Get", "OctopusMachine", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusMachine>))]
    [OutputType(typeof(List<MachineResource>))]
    public class GetOctopusMachine : PSCmdlet
    {
        private const string ByName = "ByName";
        private const string ByEnvironment = "ByEnvironment";
        private const string All = "All";

        [Alias("Name")]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public List<string> MachineName { get; set; }

        [Alias("Environment")]
        [Parameter(ValueFromPipeline = true, ParameterSetName = ByEnvironment)]
        public List<string> EnvironmentName { get; set; }

        [Parameter]
        public SwitchParameter ResourceOnly {get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
            
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<MachineResource>();
            

            switch (this.ParameterSetName)
            {
                case All:
                    baseResourceList = _connection.Repository.Machines.FindAll();
                    break;

                case ByName:
                    foreach (var name in MachineName)
                    {
                        if (name.StartsWith("*"))
                        {
                            var searchname = name.Remove(0,1);
                            baseResourceList.AddRange(
                                _connection.Repository.Machines.FindMany(x => x.Name.ToLower().EndsWith(searchname.ToLower())));
                        }
                        else if (name.EndsWith("*"))
                        {
                            var searchname = name.Remove(name.Length -1, 1);
                            baseResourceList.AddRange(
                                _connection.Repository.Machines.FindMany(x => x.Name.ToLower().StartsWith(searchname.ToLower())));
                        }
                        else
                        {
                            baseResourceList.AddRange(_connection.Repository.Machines.FindMany(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase)));
                        }
                    }
                    break;

                case ByEnvironment:
                    foreach (var name in EnvironmentName)
                    {
                        var environment = _connection.Repository.Environments.FindByName(name);
                        baseResourceList.AddRange(_connection.Repository.Environments.GetMachines(environment));
                    }
                    break;
            }

            if (ResourceOnly)
            {
                WriteObject(baseResourceList);
            }
            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusMachine(baseResourceList);

                WriteObject(outputList);
            }
            
        }
    }
}
