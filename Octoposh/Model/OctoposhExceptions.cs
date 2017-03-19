using System;

namespace Octoposh.Model
{
    internal class OctoposhExceptions
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
    }
}