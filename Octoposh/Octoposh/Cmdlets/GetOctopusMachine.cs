﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">This cmdlet returns info about Octopus Targets (Tentacles, cloud regions, Offline deployment targets, SHH)</para>
    /// </summary>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -name "Database_Prod"</code>
    ///   <para>Gets the machine with the name "Database_Prod"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -name "*_Prod"</code>
    ///   <para>Gets all the machines which name is like "*_Prod"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -EnvironmentName "Staging","UAT""</code>
    ///   <para>Gets all the machines on the environments "Staging","UAT"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusMachine -URL "*:10933"</code>
    ///   <para>Gets all the machines with the string "*:10933" at the end of the URL</para>    
    /// </example>
    /// <example>   
    ///   <code>PS Get-OctopusMachine -Mode Listening</code>
    ///   <para>Gets all the machines registered in "Listening" mode. "Polling" is also a valid value</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/wiki">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusMachine", DefaultParameterSetName = All)]
    [OutputType(typeof(List<OutputOctopusMachine>))]
    [OutputType(typeof(List<MachineResource>))]
    public class GetOctopusMachine : PSCmdlet
    {
        private const string ByName = "ByName";
        private const string ByEnvironment = "ByEnvironment";
        private const string All = "All";
        private const string ByCommunicationStyle = "ByCommunicationStyle";
        private const string ByUrl = "ByURL";

        /// <summary>
        /// <para type="description">Name of the Machine to filter by</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true, ParameterSetName = ByName)]
        public List<string> MachineName { get; set; }

        /// <summary>
        /// <para type="description">Name of the Environment to filter by</para>
        /// </summary>
        [Alias("Environment")]
        [ValidateNotNullOrEmpty()]
        [Parameter(ValueFromPipeline = true, ParameterSetName = ByEnvironment)]
        public List<string> EnvironmentName { get; set; }

        /// <summary>
        /// <para type="description">Target URI to filter by</para>
        /// </summary>
        [Alias("URI")]
        [ValidateNotNullOrEmpty()]
        [Parameter(ValueFromPipeline = true, ParameterSetName = ByUrl)]
        public List<string> URL { get; set; }

        /// <summary>
        /// <para type="description">Target communication style to filter by</para>
        /// </summary>
        //todo: figure out how to print accepted values in help
        [Alias("Mode")]
        [ValidateSet("Listening", "Polling", "SSH", "CloudRegion", "OfflineDrop")]
        [Parameter(ValueFromPipeline = true, ParameterSetName = ByCommunicationStyle)]
        public string CommunicationStyle { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopur resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
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

            switch (ParameterSetName)
            {
                case All:
                    baseResourceList = _connection.Repository.Machines.FindAll();
                    break;

                case ByName:
                    MachineName = MachineName?.ConvertAll(s => s.ToLower());

                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                    if (MachineName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && MachineName.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("MachineName");
                    }
                    //Only 1 wildcarded value (ie -MachineName WebServer*)
                    else if (MachineName.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && MachineName.Count == 1))
                    {
                        var pattern = new WildcardPattern(MachineName.First());
                        baseResourceList = _connection.Repository.Machines.FindMany(x => pattern.IsMatch(x.Name.ToLower()));
                    }
                    //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                    else if (!MachineName.Any(item => WildcardPattern.ContainsWildcardCharacters(item)))
                    {
                        baseResourceList = _connection.Repository.Machines.FindMany(x => MachineName.Contains(x.Name.ToLower()));
                    }
                    break;
                case ByUrl:

                    URL = URL?.ConvertAll(s => s.ToLower());

                    //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -URL http://Tentacle*, http://Database1)
                    if (URL.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && URL.Count > 1))
                    {
                        throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("URL");
                    }
                    //Only 1 wildcarded value (ie -URL http://Tentacle*)
                    else if (URL.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && URL.Count == 1))
                    {
                        var pattern = new WildcardPattern(URL.First());
                        baseResourceList = _connection.Repository.Machines.FindMany(x => pattern.IsMatch(x.Uri));
                    }
                    //multiple non-wildcared values (i.e. -URL http://Tentacle ,http://Database)
                    else if (!URL.Any(item => WildcardPattern.ContainsWildcardCharacters(item)))
                    {
                        baseResourceList = _connection.Repository.Machines.FindMany(x => URL.Contains(x.Uri));
                    }
                    break;
                case ByCommunicationStyle:

                    var endpointtype = "";
                        
                    switch (CommunicationStyle)
                    {
                        case "Polling":
                            endpointtype = "Octopus.Client.Model.Endpoints.PollingTentacleEndpointResource";
                            break;
                        case "Listening":
                            endpointtype = "Octopus.Client.Model.Endpoints.ListeningTentacleEndpointResource";
                            break;
                        case "CloudRegion":
                            endpointtype = "Octopus.Client.Model.Endpoints.CloudRegionEndpointResource";
                            break;
                        case "OfflineDrop":
                            endpointtype = "Octopus.Client.Model.Endpoints.OfflineDropEndpointResource";
                            break;
                        case "SSH":
                            endpointtype = "Octopus.Client.Model.Endpoints.SSHEndpointResource";
                            break;
                    }

                    baseResourceList = _connection.Repository.Machines.FindMany(x => x.Endpoint.GetType().ToString().ToLower() == endpointtype.ToLower());

                    break;

                case ByEnvironment:

                    EnvironmentName = EnvironmentName.ConvertAll(s => s.ToLower());
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
