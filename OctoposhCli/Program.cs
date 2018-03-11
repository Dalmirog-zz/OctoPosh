using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Octoposh.Model;
using Octopus.Client.Model;

namespace OctoposhCli
{
    internal class Program
    {
        private static string groupByValue;

        static int Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Utility should be run like:OctoposhCli <apiKey> <serverUrl> <command> <parameter(s)>");
                return 1;
            }

            var apiKey = args[0];
            var serverUrl = args[1];
            var cmd = args[2];
            var variables = args[3];

            // Set connection info 
            SetConnectionInfo(apiKey, serverUrl);

            List<CmdletParameter> parameters;
            var returnCount = ParseParameters(variables, out parameters);

            string json = RunCommandGetJson(cmd, parameters, returnCount);
            Console.WriteLine(json);

            // Remove connection info
            SetConnectionInfo("", "");

            return 0;
        }

        private static int ParseParameters(string variables, out List<CmdletParameter> parameters)
        {
            var varsSplit = variables.Split(',');

            parameters = new List<CmdletParameter>();
            var returnCount = 0;

            foreach (var varPair in varsSplit)
            {
                var keyValue = varPair.Split('=');
                var splitValues = keyValue[1].Split(';');

                if (string.CompareOrdinal(keyValue[0].ToLower(), "count") == 0)
                {
                    returnCount = int.Parse(keyValue[1]);
                    continue;
                }

                if (string.CompareOrdinal(keyValue[0].ToLower(), "groupby") == 0)
                {
                    groupByValue = keyValue[1];
                    continue;
                }

                if (splitValues.Count() > 1)
                {
                    parameters.Add(new CmdletParameter()
                    {
                        Name = keyValue[0],
                        MultipleValue = splitValues
                    });
                }
                else
                {
                    parameters.Add(new CmdletParameter()
                    {
                        Name = keyValue[0],
                        SingleValue = keyValue[1]
                    });
                }
            }

            return returnCount;
        }

        private static void SetConnectionInfo(string apiKey, string serverUrl)
        {
            Environment.SetEnvironmentVariable("OctopusURL", serverUrl);
            Environment.SetEnvironmentVariable("OctopusAPIKey", apiKey);
        }

        private static string RunCommandGetJson(string command, List<CmdletParameter> parameters, int returnCount)
        {
            var cmdType = CmdTypeConverter.Convert(command);
            var jss = new JavaScriptSerializer();
            string output;

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(command, cmdType, parameters);
            
            switch (command)
            {
                case "Get-OctopusChannel":
                    var channelResults = powershell.Invoke<OutputOctopusChannel>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(channelResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : channelResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(channelResults.Take(returnCount > 0 ? returnCount : channelResults.Count));
                    }
                    break;
                case "Get-OctopusDashboard":
                    var dashboardResults = powershell.Invoke<OutputOctopusDashboardEntry>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(dashboardResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : dashboardResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(dashboardResults.Take(returnCount > 0 ? returnCount : dashboardResults.Count));
                    }
                    break;
                case "Get-OctopusDeployment":
                    var deploymentResults = powershell.Invoke<OutputOctopusDeployment>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(deploymentResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : deploymentResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(deploymentResults.Take(returnCount > 0 ? returnCount : deploymentResults.Count));
                    }
                    break;
                case "Get-OctopusEnvironment":
                    var environmentResults = powershell.Invoke<OutputOctopusEnvironment>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(environmentResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : environmentResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(environmentResults.Take(returnCount > 0 ? returnCount : environmentResults.Count));
                    }
                    break;
                case "Get-OctopusFeed":
                    var feedResults = powershell.Invoke<OutputOctopusFeed>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(feedResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : feedResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(feedResults.Take(returnCount > 0 ? returnCount : feedResults.Count));
                    }
                    break;
                case "Get-OctopusLifecycle":
                    var lifecycleResults = powershell.Invoke<OutputOctopusLifecycle>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(lifecycleResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : lifecycleResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(lifecycleResults.Take(returnCount > 0 ? returnCount : lifecycleResults.Count));
                    }
                    break;
                case "Get-OctopusMachine":
                    var machineResults = powershell.Invoke<OutputOctopusMachine>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(machineResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : machineResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(machineResults.Take(returnCount > 0 ? returnCount : machineResults.Count));
                    }
                    break;
                case "Get-OctopusProject":
                    var projectResults = powershell.Invoke<OutputOctopusProject>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(projectResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : projectResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(projectResults.Take(returnCount > 0 ? returnCount : projectResults.Count));
                    }
                    break;
                case "Get-OctopusProjectGroup":
                    var projectGroupResults = powershell.Invoke<OutputOctopusProjectGroup>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(projectGroupResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : projectGroupResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(projectGroupResults.Take(returnCount > 0 ? returnCount : projectGroupResults.Count));
                    }
                    break;
                case "Get-OctopusRelease":
                    var releaseResults = powershell.Invoke<OutputOctopusRelease>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(releaseResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : releaseResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(releaseResults.Take(returnCount > 0 ? returnCount : releaseResults.Count));
                    }
                    break;
                case "Get-OctopusTagSet":
                    var tagSetResults = powershell.Invoke<OutputOctopusTagSet>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(tagSetResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : tagSetResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(tagSetResults.Take(returnCount > 0 ? returnCount : tagSetResults.Count));
                    }
                    break;
                case "Get-OctopusTeam":
                    var teamResults = powershell.Invoke<OutputOctopusTeam>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(teamResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : teamResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(teamResults.Take(returnCount > 0 ? returnCount : teamResults.Count));
                    }
                    break;
                case "Get-OctopusTenant":
                    var tenantResults = powershell.Invoke<OutputOctopusTenant>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(tenantResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : tenantResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(tenantResults.Take(returnCount > 0 ? returnCount : tenantResults.Count));
                    }
                    break;
                case "Get-OctopusToolPath":
                    var toolPathResults = powershell.Invoke<string>()[0];
                    output = jss.Serialize(toolPathResults);
                    break;
                case "Get-OctopusToolVersion":
                    var toolVersionResults = powershell.Invoke<List<OctopusToolVersion>>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(toolVersionResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : toolVersionResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(toolVersionResults.Take(returnCount > 0 ? returnCount : toolVersionResults.Count));
                    }
                    break;
                case "Get-OctopusToolsFolder":
                    var toolsFolderResults = powershell.Invoke<string>()[0];
                    output = jss.Serialize(toolsFolderResults);
                    break;
                case "Get-OctopusUser":
                    var userResults = powershell.Invoke<OutputOctopusUser>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(userResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : userResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(userResults.Take(returnCount > 0 ? returnCount : userResults.Count));
                    }
                    break;
                case "Get-OctopusUserRole":
                    var userRoleResults = powershell.Invoke<UserRoleResource>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(userRoleResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : userRoleResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(userRoleResults.Take(returnCount > 0 ? returnCount : userRoleResults.Count));
                    }
                    break;
                case "Get-OctopusVariableSet":
                    var variableSetResults = powershell.Invoke<OutputOctopusVariableSet>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(variableSetResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : variableSetResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(variableSetResults.Take(returnCount > 0 ? returnCount : variableSetResults.Count));
                    }
                    break;
                case "Install-OctopusTool":
                    var toolResults = powershell.Invoke();
                    output = jss.Serialize(toolResults);
                    break;
                case "Set-OctopusReleaseStatus":
                    var releaseStatusResults = powershell.Invoke<List<OutputOctopusTeam>>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(releaseStatusResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : releaseStatusResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(releaseStatusResults.Take(returnCount > 0 ? returnCount : releaseStatusResults.Count));
                    }
                    break;
                case "Update-OctopusResource":
                    var resourceResults = powershell.Invoke<ProjectGroupResource>();
                    if (groupByValue != null)
                    {
                        output = jss.Serialize(resourceResults.GroupBy(dr => dr.GetType().GetProperty(groupByValue)).Take(returnCount > 0 ? returnCount : resourceResults.Count));
                    }
                    else
                    {
                        output = jss.Serialize(resourceResults.Take(returnCount > 0 ? returnCount : resourceResults.Count));
                    }
                    break;
                default:
                    return null;
            }

            return output;
        }
    }
}
