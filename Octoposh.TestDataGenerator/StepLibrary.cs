using System;
using System.Collections.Generic;
using System.Text;
using Octopus.Client.Model;

namespace Octoposh.TestDataGenerator
{
    /// <summary>
    /// Static class that exposes baked in steps ready to be added to Deployment Processes
    /// </summary>
    public static class StepLibrary
    {
        /// <summary>
        /// A simple script step that prints "Hello World" using Powershell.
        /// </summary>
        /// <returns>DeploymentStepResource</returns>
        public static DeploymentStepResource GetSimpleScriptStep()
        {
            var step = new DeploymentStepResource()
            {
                Name = "Script Step"
            };
            step.Actions.Add(new DeploymentActionResource()
            {
                Name = "Script Step",
                ActionType = "Octopus.Script"
            });

            step.Actions[0].Properties.Clear();
            step.Actions[0].Properties["Octopus.Action.Script.ScriptSource"] = "Inline";
            step.Actions[0].Properties["Octopus.Action.RunOnServer"] = "true";
            step.Actions[0].Properties["Octopus.Action.Script.ScriptBody"] = "'Hello World'";
            step.Properties["Octopus.Action.TargetRoles"] = "default-role";
            return step;
        }
    }
}
