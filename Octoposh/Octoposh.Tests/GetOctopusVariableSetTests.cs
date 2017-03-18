using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;
using Octoposh.Cmdlets;
using Octoposh.Model;
using Octopus.Client.Model;

namespace Octoposh.Tests
{
    [TestFixture]
    public class GetOctopusVariableSetTests
    {
        private static readonly string CmdletName = "Get-OctopusVariableSet";
        private static readonly Type CmdletType = typeof(GetOctopusVariableSet);
        private static readonly string Library1 = "VariableSetTests_Library1";
        private static readonly string Library2 = "VariableSetTests_Library2";
        private static readonly string Project1 = "ProjectTests_Project1";
        private static readonly string Project2 = "ProjectTests_Project2";

        [Test]
        public void GetVariableSetBySingleProjectName()
        {
            var projectName = Project1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            Console.WriteLine("Looking for variable set of project [{0}]", projectName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].ProjectName, projectName);

            Console.WriteLine("The variable set found belongs to projet [{0}]", projectName);
        }

        [Test]
        public void GetvariableSetByMultipleProjectNames()
        {
            var name1 = Project1;
            var name2 = Project2;
            var projectNames = new string[] {name1,name2};

            bool isProject1 = false;
            bool isProject2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    MultipleValue = projectNames
                }
            };

            Console.WriteLine("Looking for variable sets of projects [{0}] and [{1}]", name1, name2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(2, results.Count);

            foreach (var variabletSet in results)
            {
                if (variabletSet.ProjectName == name1)
                {
                    isProject1 = true;
                }
                else if (variabletSet.ProjectName == name2)
                {
                    isProject2 = true;
                }
                else
                {
                    Console.WriteLine("Variable set found of unexpected project: [{0}]", variabletSet.ProjectName);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isProject1);
            Assert.IsTrue(isProject2);

            Console.WriteLine("Variable sets of projects [{0}] and [{1}] found", name1, name2);
        }

        [Test]
        public void GetTagSetByProjectNameUsingWildcard()
        {
            var namePattern = "ProjectTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "ProjectName", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.IsTrue(results.Count > 0);
            Console.WriteLine("Resources found: {0}", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Variable set belongs to project: {0}", item.ProjectName);
                Assert.IsTrue(pattern.IsMatch(item.ProjectName));
            }
            Console.WriteLine("Resources found match pattern [{0}]", namePattern);
        }

        [Test]
        public void GetVariableSetBySingleLibraryName()
        {
            var libraryName = Library1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "LibrarySetName",
                    SingleValue = libraryName
                }
            };

            Console.WriteLine("Looking for library variable [{0}]", libraryName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(results[0].LibraryVariableSetName, libraryName);

            Console.WriteLine("Library variable set [{0}] found", libraryName);
        }

        [Test]
        public void GetvariableSetByMultipleLibraryNames()
        {
            var name1 = Library1;
            var name2 = Library2;
            var libraryNames = new string[] { name1, name2};

            bool isLibrary1 = false;
            bool isLibrary2 = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "LibrarySetName",
                    MultipleValue = libraryNames
                }
            };

            Console.WriteLine("Looking for variable sets of projects [{0}] and [{1}]", name1, name2);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(2, results.Count);

            foreach (var variabletSet in results)
            {
                if (variabletSet.LibraryVariableSetName == name1)
                {
                    isLibrary1 = true;
                }
                else if (variabletSet.LibraryVariableSetName == name2)
                {
                    isLibrary2 = true;
                }
                else
                {
                    Console.WriteLine("Unexpected Library Variable set found: [{0}]", variabletSet.LibraryVariableSetName);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isLibrary1);
            Assert.IsTrue(isLibrary2);

            Console.WriteLine("Library Variable sets [{0}] and [{1}] found", name1, name2);
        }

        [Test]
        public void GetVariableSetByLibraryNameUsingWildcard()
        {
            var namePattern = "VariableSetTests_*";

            var parameters = new List<CmdletParameter> {
                new CmdletParameter(){
                    Name = "LibrarySetName", SingleValue = namePattern
                }
            };

            Console.WriteLine("Looking for resources with name pattern: {0}", namePattern);

            var pattern = new WildcardPattern(namePattern);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.IsTrue(results.Count > 0);
            Console.WriteLine("Resources found: {0}", results.Count);

            foreach (var item in results)
            {
                Console.WriteLine("Library Variable set found: {0}", item.LibraryVariableSetName);
                Assert.IsTrue(pattern.IsMatch(item.LibraryVariableSetName));
            }
            Console.WriteLine("Resources found match pattern [{0}]", namePattern);
        }

        [Test]
        public void GetVariableSetByProjectAndLibraryNames()
        {
            var projectName = Project1;
            var libraryName = Library1;
            
            bool isProject = false;
            bool isLibrary = false;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "LibrarySetName",
                    SingleValue = libraryName
                },
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            Console.WriteLine("Looking for variable sets of project [{0}] and library [{1}]", projectName, libraryName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(2, results.Count);

            foreach (var variabletSet in results)
            {
                if (variabletSet.LibraryVariableSetName == libraryName)
                {
                    isLibrary = true;
                }
                else if (variabletSet.ProjectName == projectName)
                {
                    isProject = true;
                }
                else
                {
                    Console.WriteLine("Unexpected Library Variable set found: [{0}]", variabletSet.ID);
                    throw new Exception();
                }
            }

            Assert.IsTrue(isProject);
            Assert.IsTrue(isLibrary);

            Console.WriteLine("Variable sets of project [{0}] and library [{1}] found", projectName, libraryName);
        }

        [Test]
        public void PassingIncludeUsageSwitchReturnsLibraryProjectUsage()
        {
            var libraryName = Library1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "IncludeUsage"
                },
                new CmdletParameter()
                {
                    Name = "LibrarySetName",
                    SingleValue = libraryName
                }
            };

            Console.WriteLine("Looking for library variable set [{0}]", libraryName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(results[0].Usage.Count != 0);

            Console.WriteLine("Project usage list for Library [{0}] is not empty", libraryName);
        }

        [Test]
        public void NotPassingIncludeUsageSwitchDoesNotReturnLibraryProjectUsage()
        {
            var libraryName = Library1;

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "LibrarySetName",
                    SingleValue = libraryName
                }
            };

            Console.WriteLine("Looking for library variable set [{0}]", libraryName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(1, results.Count);

            Assert.IsTrue(results[0].Usage.Count == 0);

            Console.WriteLine("Project usage list for Library [{0}] is empty as expected", libraryName);
        }

        [Test]
        public void VariableSetVariableScopeIsNotEmpty()
        {
            var projectName = Project1;
            var variableName = "var";

            var parameters = new List<CmdletParameter>
            {
                new CmdletParameter()
                {
                    Name = "ProjectName",
                    SingleValue = projectName
                }
            };

            Console.WriteLine("Looking for library variable set [{0}]", projectName);

            var powershell = new CmdletRunspace().CreatePowershellcmdlet(CmdletName, CmdletType, parameters);
            var results = powershell.Invoke<List<OutputOctopusVariableSet>>()[0];

            Assert.AreEqual(1, results.Count);

            var variable = results[0].Variables.FirstOrDefault(v => v.Name == variableName);

            Assert.IsNotNull(variable);

            Assert.IsTrue(variable.Scope.Channels.Count != 0);
            Assert.IsTrue(variable.Scope.Actions.Count != 0);
            Assert.IsTrue(variable.Scope.Environments.Count != 0);
            Assert.IsTrue(variable.Scope.Machines.Count != 0);
            Assert.IsTrue(variable.Scope.Roles.Count != 0);

            Console.WriteLine("Scope of variable [{0}] of project [{1}] is not empty", variable.Name, projectName);
        }
    }
}
