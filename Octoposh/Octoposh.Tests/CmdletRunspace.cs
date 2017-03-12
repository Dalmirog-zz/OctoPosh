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
        /// <summary>
        /// Creates a Powershell cmdlet with parameters
        /// </summary>
        /// <param name="cmdletName">Name of the cmdlet to invoke</param>
        /// <param name="cmdletType">Class type of the cmdlet to invoke</param>
        /// <param name="parameters">Parameters to pass to the cmdlet</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a Powershell cmdlet without parameters
        /// </summary>
        /// <param name="cmdletName">Name of the cmdlet to invoke</param>
        /// <param name="cmdletType">Class type of the cmdlet to invoke</param>
        /// <returns></returns>
        public PowerShell CreatePowershellcmdlet(string cmdletName, Type cmdletType)
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