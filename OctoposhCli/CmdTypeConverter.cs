using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Cmdlets;

namespace OctoposhCli
{
    static class CmdTypeConverter
    {
        public static Type Convert(string cmdName)
        {		
            switch (cmdName)
            {
                case "Get-OctopusChannel":
                    return typeof(GetOctopusChannel);
                case "Get-OctopusConnectionInfo":
                    return typeof(GetOctopusConnectionInfo);
                case "Get-OctopusDashboard":
                    return typeof(GetOctopusDashboard);
                case "Get-OctopusDeployment":
                    return typeof(GetOctopusDeployment);
                case "Get-OctopusEnvironment":
                    return typeof(GetOctopusEnvironment);
                case "Get-OctopusFeed":
                    return typeof(GetOctopusFeed);
                case "Get-OctopusLifecycle":
                    return typeof(GetOctopusLifecycle);
                case "Get-OctopusMachine":
                    return typeof(GetOctopusMachine);
                case "Get-OctopusProject":
                    return typeof(GetOctopusProject);
                case "Get-OctopusProjectGroup":
                    return typeof(GetOctopusProjectGroup);
                case "Get-OctopusRelease":
                    return typeof(GetOctopusRelease);
                case "Get-OctopusResourceModel":
                    return typeof(GetOctopusResourceModel);
                case "Get-OctopusServerThumbprint":
                    return typeof(GetOctopusServerThumbprint);
                case "Get-OctopusTagSet":
                    return typeof(GetOctopusTagSet);
                case "Get-OctopusTeam":
                    return typeof(GetOctopusTeam);
                case "Get-OctopusTenant":
                    return typeof(GetOctopusTenant);
                case "Get-OctopusToolPath":
                    return typeof(GetOctopusToolPath);
                case "Get-OctopusToolVersion":
                    return typeof(GetOctopusToolVersion);
                case "Get-OctopusToolsFolder":
                    return typeof(GetOctopusToolsFolder);
                case "Get-OctopusUser":
                    return typeof(GetOctopusUser);
                case "Get-OctopusUserRole":
                    return typeof(GetOctopusUserRole);
                case "Get-OctopusVariableSet":
                    return typeof(GetOctopusVariableSet);
                case "Install-OctopusTool":
                    return typeof(InstallOctopusTool);
                case "Set-OctopusReleaseStatus":
                    return typeof(SetOctopusReleaseStatus);
                case "Set-OctopusToolPath":
                    return typeof(SetOctopusToolPath);
                case "Set-OctopusToolsFolder":
                    return typeof(SetOctopusToolsFolder);
                case "Update-OctopusResource":
                    return typeof(UpdateOctopusResource);
                default:
                    return null;
            }
        }
    }
}
