using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    /// <summary>
    /// Single parameter that will be passed to the Powershell cmdlet call
    /// </summary>
    class CmdletParameter
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Array of values passed to the parameter. If you want to pass a single value to the parameter, use the property "SingleValue" instead.
        /// </summary>
        public string[] MultipleValue { get; set; }

        /// <summary>
        /// Single values passed to the parameter. If you want to pass an array of values to the parameter, use the property "MultipleValue" instead.
        /// </summary>
        public string SingleValue { get; set; }

        public Resource Resource { get; set; }
    }
}
