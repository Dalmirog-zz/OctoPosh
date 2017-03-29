using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Tests
{
    static class TestsUtilities
    {
        public static string TestsPath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty); }
            set { }
        }
    }
}
