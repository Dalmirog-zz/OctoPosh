using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Model
{
    interface IOctoposhCmdlet
    {
        /// <summary>
        /// Validates if a collection of strings has a wildcard value or not. When the user passes multiple values to parameter, there shoudn't be any wildcarded value on the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        bool CollectionHasWildcard(List<string> collection);
    }
}
