using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Model
{
    /// <summary>
    /// This class defines all the Environment Variables that will be used throught Octoposh, such as the one that references the folder where all the versions of Octo.exe are on, or the one that references the current Octo.exe in use.
    /// </summary>
    public static class OctoposhEnvVariables
    {
        /// <summary>
        /// Name of the variable that holds the path of the current Octo.exe version. In Powershell the equivalent would be $env:[te value of this var] like $env:OctoExe
        /// </summary>
        private static string OctoExeEnvVariable = "Octoexe";


        /// <summary>
        /// Name of the environment variable that references the directory where the folders with different versions of Octo.exe are sitting on.
        /// </summary>
        private static string OctopusToolsFolderEnvVariable = "OctopusToolsFolder";

        /// <summary>
        /// Gets/Sets the value of the environment variable that references the path of an Octo.exe version
        /// </summary>
        public static string Octoexe
        {
            get { return Environment.GetEnvironmentVariable(OctoExeEnvVariable); }
            set { Environment.SetEnvironmentVariable(OctoExeEnvVariable, value); }
        }

        /// <summary>
        /// Gets/Sets the value of the environment variable that references the directory where the folders with different versions of Octo.exe are sitting on.
        /// </summary>
        public static string OctopusToolsFolder
        {
            get { return Environment.GetEnvironmentVariable(OctopusToolsFolderEnvVariable); }
            set { Environment.SetEnvironmentVariable(OctopusToolsFolderEnvVariable, value); }
        }
    }
}
