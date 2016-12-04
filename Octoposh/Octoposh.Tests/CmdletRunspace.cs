using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Cmdlets;

namespace Octoposh.Tests
{
    internal class CmdletRunspace
    {
        public PowerShell CreatePowershellcmdlet(string cmdletName, Type cmdletType, List<CmdletParameter> parameters)
        {
            var initialsessionState =
                InitialSessionState.CreateDefault();

            initialsessionState.Commands.Add(
                new SessionStateCmdletEntry(
                    cmdletName, cmdletType, null)
            );

            var runspace = RunspaceFactory.CreateRunspace(initialsessionState);
            
            runspace.Open();

            var powershell = PowerShell.Create();
            
                powershell.Runspace = runspace;

                var command = new Command(cmdletName);

                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(new CommandParameter(parameter.Name,parameter.Value));
                }

                powershell.Commands.AddCommand(command);

                //todo: figure out what to do with the URL and API Key
                Environment.SetEnvironmentVariable("OctopusURL", "http://Devbox:81");
                Environment.SetEnvironmentVariable("OctopusAPIKey", "API-B3ZK7BTFAKSKRTCHQFKAZNPT5Y");

                return powershell;
        }
        
    }
}