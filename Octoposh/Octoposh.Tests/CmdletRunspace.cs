using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
                    if (parameter.MultipleValue != null)
                    {
                        command.Parameters.Add(new CommandParameter(parameter.Name, parameter.MultipleValue));
                    }
                    else if (parameter.SingleValue != null)
                    {
                        command.Parameters.Add(new CommandParameter(parameter.Name, parameter.SingleValue));
                    }
                    else if (parameter.Resource != null)
                    {
                        command.Parameters.Add(new CommandParameter(parameter.Name, parameter.Resource));
                    }
                    else
                    {
                        command.Parameters.Add(new CommandParameter(parameter.Name));
                    }
                }

                powershell.Commands.AddCommand(command);

                var octopusUrl = string.Concat("http://localhost:", ConfigurationManager.AppSettings["OctopusBindingPort"]);
                var octopusApiKey = ConfigurationManager.AppSettings["OctopusAPIKey"];

                //todo: figure out what to do with the URL and API Key
                Environment.SetEnvironmentVariable("OctopusURL", octopusUrl);
                Environment.SetEnvironmentVariable("OctopusAPIKey", octopusApiKey);

                return powershell;
        }
        
    }
}