using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Tests
{
    /// <summary>
    /// Helper class whose purpose is to provie some properties/methods that will aid during unit tests.
    /// </summary>
    static class TestsUtilities
    {
        /// <summary>
        /// Gets the path where the tests are currently running
        /// </summary>
        public static string TestsPath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty); }
            set { }
        }
    }
}
