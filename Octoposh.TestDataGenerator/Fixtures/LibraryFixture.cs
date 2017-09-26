using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octoposh.TestDataGenerator.Fixtures
{
    /// <summary>
    /// This fixture is in charge of creating/setting up most of the things that can be found in the "Library" section of the Octopus portal, such as External feeds, Script modules, Library Variable Sets,etc
    /// </summary>
    class LibraryFixture
    {
        private static IOctopusAsyncRepository _repository;

        private static readonly string[] FeedNames = { "Local Folder", "FeedTests_Feed1", "FeedTests_Feed2", "unmodified_TestFeed" };
        private static readonly string[] ScriptModuleNames = { "ScriptModuleTests_Module1" };
        private static readonly string[] LibraryVariableSetNames = { "VariableSetTests_Library1", "VariableSetTests_Library2", "unmodified_TestLibraryVariableSet" };

        /// <summary>
        /// Run the fixture
        /// </summary>
        public static void Run()
        {
            _repository = OctopusRepository.GetRepository().Result;

            Log.Logger.Information("**Running Library Fixture**");
            
            CreateLibraryStuff();
        }

        #region Library Stuff

        private static void CreateLibraryStuff()
        {
            CreateFeeds();
            CreateScriptModules();
            CreateLibraryVariableSets();
        }

        private static void CreateFeeds()
        {
            foreach (var feedName in FeedNames)
            {
                var feed = _repository.Feeds.FindByName(feedName).Result ?? new NuGetFeedResource();

                feed.Name = feedName;
                feed.FeedUri = "C:\\packages";

                try
                {
                    Log.Information($"Creating/Modifying Feed [{feed.Name}]");

                    if (feed.Id != null)
                        _repository.Feeds.Modify(feed).Wait();
                    else
                        _repository.Feeds.Create(feed).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static void CreateScriptModules()
        {
            foreach (var scriptModuleName in ScriptModuleNames)
            {
                var scriptModule = _repository.LibraryVariableSets.FindByName(scriptModuleName).Result ??
                                   new LibraryVariableSetResource();

                scriptModule.Name = scriptModuleName;
                scriptModule.Description = GeneralResourceProperty.ResourceDescription;
                scriptModule.ContentType = VariableSetContentType.ScriptModule;

                try
                {
                    Log.Information($"Creating/Modifying Script Module [{scriptModule.Name}]");

                    if (scriptModule.Id != null)
                        _repository.LibraryVariableSets.Modify(scriptModule).Wait();
                    else
                        _repository.LibraryVariableSets.Create(scriptModule).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static void CreateLibraryVariableSets()
        {
            foreach (var libraryVariableSetName in LibraryVariableSetNames)
            {
                var libraryVariableSet = _repository.LibraryVariableSets.FindByName(libraryVariableSetName).Result ??
                                   new LibraryVariableSetResource();

                libraryVariableSet.Name = libraryVariableSetName;
                libraryVariableSet.Description = GeneralResourceProperty.ResourceDescription;
                libraryVariableSet.ContentType = VariableSetContentType.Variables;

                try
                {
                    Log.Information($"Creating/Modifying Library Variable Set [{libraryVariableSet.Name}]");

                    libraryVariableSet = libraryVariableSet.Id != null ? _repository.LibraryVariableSets.Modify(libraryVariableSet).Result : _repository.LibraryVariableSets.Create(libraryVariableSet).Result;

                    Log.Information($"Adding variables to Library Variable Set [{libraryVariableSet.Name}]");
                    var variableSet = _repository.VariableSets.Get(libraryVariableSet.VariableSetId).Result;

                    variableSet.Variables.Clear();
                    variableSet.AddOrUpdateVariableValue("foo", "bar");

                    _repository.VariableSets.Modify(variableSet);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        #endregion
    }
}
