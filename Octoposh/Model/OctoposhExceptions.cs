﻿using System;
using System.Management.Automation;

namespace Octoposh.Model
{
    public class OctoposhExceptions
    {
        internal static Exception ParameterCollectionHasRegularAndWildcardItem(string cmdlet)
        {
            throw new Exception(
                $"If you pass more than one value to a parameter, none of them should include a Wildcard. Parameter: {cmdlet}");
        }

        public static Exception ResourceNotFound(string resourceName,string resourceType)
        {
            throw new Exception(
            $"{resourceType} not found: {resourceName}");
        }

        public static Exception ToolsFolderNotSet()
        {
            throw new Exception("The Octopus Tools folder has not been set yet. Run [Get-help Set-OctopusToolsFolder] to learn how to do this");
        }

        public static Exception ToolVersionNotfound(string version,string toolsFolderEnvVarName)
        {
            var toolsFolder = Environment.GetEnvironmentVariable(toolsFolderEnvVarName);
            throw new Exception($"Octo.exe version [{version}] not found in any of the child folder of [{toolsFolder}]. If this is not the right path Octoposh should be looking for versions of Octo.exe, please fix it using Set-OctopusToolsFolder");
        }
    }
}