using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Octoposh.Model;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octoposh.Infrastructure
{
    public abstract class GetOctoposhCmdlet : OctoposhConnection
    {
        /// <summary>
        /// <para type="description">If set to TRUE the cmdlet will return the basic Octopus resource. If not set or set to FALSE, the cmdlet will return a human friendly Octoposh output object</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ResourceOnly { get; set; }

        /// <summary>
        /// Gets Octopus resources filtering them by name
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="filterNames">Collection of resource names</param>
        /// <param name="parameterName">Name of the cmdlet parameter where the collection of resource names are coming from. This is only used for the exception message in case the values passed are invalid</param>
        /// <returns></returns>
        protected List<TResource> FilterByName<TResource>(IEnumerable<string> filterNames, IFindByName<TResource> repository, string parameterName)
            where TResource:INamedResource
        {
            var results = new List<TResource>();

            var projectGroupNameList = filterNames.ToList().ConvertAll(s => s.ToLower());

            //Multiple values but one of them is wildcarded, which is not an accepted scenario (I.e -MachineName WebServer*, Database1)
            if (projectGroupNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && projectGroupNameList.Count > 1))
            {
                throw OctoposhExceptions.ParameterCollectionHasRegularAndWildcardItem(parameterName);
            }
            //Only 1 wildcarded value (ie -MachineName WebServer*)
            else if (projectGroupNameList.Any(item => WildcardPattern.ContainsWildcardCharacters(item) && projectGroupNameList.Count == 1))
            {
                var pattern = new WildcardPattern(projectGroupNameList.First());
                results.AddRange(repository.FindMany(x => pattern.IsMatch(x.Name.ToLower())));
            }
            //multiple non-wildcared values (i.e. -MachineName WebServer1,Database1)
            else if (!projectGroupNameList.Any(WildcardPattern.ContainsWildcardCharacters))
            {
                results.AddRange(repository.FindMany(x => projectGroupNameList.Contains(x.Name.ToLower())));
            }

            return results;
        }
    }
}