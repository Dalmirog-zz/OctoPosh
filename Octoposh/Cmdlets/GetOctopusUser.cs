
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
    /// <summary>
    /// <para type="description">This cmdlet returns info about Octopus Targets (Tentacles, cloud regions, Offline deployment targets, SHH)</para>
    /// </summary>
    /// <example>
    ///   <code>PS C:\> Get-OctopusUser</code>
    ///   <para>Gets all the Users on the Octopus instance</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUser -Username "Jotaro Kujo"</code>
    ///   <para>Gets the user with the Username "Jotaro Kujo"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUser -Username "Jotaro Kujo","Dio Brando"</code>
    ///   <para>Gets the users with the Usernames "Jotaro Kujo" and "Dio Brando"</para>    
    /// </example>
    /// <example>   
    ///   <code>PS C:\> Get-OctopusUser -Username "*Joestar"</code>
    ///   <para>Gets all the users whose username ends with "Joestar"</para>    
    /// </example>
    /// <para type="link" uri="http://Octoposh.net">WebSite: </para>
    /// <para type="link" uri="https://github.com/Dalmirog/OctoPosh/">Github Project: </para>
    /// <para type="link" uri="http://octoposh.readthedocs.io">Wiki: </para>
    /// <para type="link" uri="https://gitter.im/Dalmirog/OctoPosh#initial">QA and Feature requests: </para>
    [Cmdlet("Get", "OctopusUser")]
    [OutputType(typeof(List<OutputOctopusMachine>))]
    [OutputType(typeof(List<MachineResource>))]
    public class GetOctopusUser : PSCmdlet
    {
        /// <summary>
        /// <para type="description">User Name. Accepts wildcard</para>
        /// </summary>
        [Alias("Name")]
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] UserName { get; set; }

        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopus resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        private OctopusConnection _connection;

        protected override void BeginProcessing()
        {
            _connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }

        protected override void ProcessRecord()
        {
            var baseResourceList = new List<UserResource>();

            var userNameList = UserName?.ToList().ConvertAll(s => s.ToLower());

            if (userNameList == null)
            {
                baseResourceList = _connection.Repository.Users.FindAll();
            }
            else
            {
                //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
                if (userNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && userNameList.Count > 1))
                {
                    throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem("UserName");
                }
                //Only 1 wildcarded value (ie -MachineName WebServer*)
                else if (userNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && userNameList.Count == 1))
                {
                    var pattern = new WildcardPattern(userNameList.First());
                    baseResourceList = _connection.Repository.Users.FindMany(u => pattern.IsMatch(u.Username.ToLower()));
                }
                //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
                else if (!userNameList.Any(WildcardPattern.ContainsWildcardCharacters))
                {
                    baseResourceList = _connection.Repository.Users.FindMany(u => userNameList.Contains(u.Username.ToLower()));
                }
            }

            if (ResourceOnly)
            {
                if (baseResourceList.Count == 1)
                {
                    WriteObject(baseResourceList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(baseResourceList,true);
                }
            }
            else
            {
                var converter = new OutputConverter();
                var outputList = converter.GetOctopusUser(baseResourceList);

                if (outputList.Count == 1)
                {
                    WriteObject(outputList.FirstOrDefault(),true);
                }
                else
                {
                    WriteObject(outputList,true);
                }
            }

        }
    }
}
