using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Octoposh.Cmdlets;
using Octoposh.Model;

namespace Octoposh.Infrastructure
{
    public abstract class OctoposhConnection : PSCmdlet
    {
        protected OctopusConnection Connection { get; private set; }

        protected override void BeginProcessing()
        {
            Connection = new NewOctopusConnection().Invoke<OctopusConnection>().ToList()[0];
        }
    }
}
