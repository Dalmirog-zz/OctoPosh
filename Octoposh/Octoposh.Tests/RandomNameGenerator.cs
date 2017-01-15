using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octoposh.Tests
{
    public static class RandomNameGenerator
    {

        public static string Generate()
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, 15)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            
        }
    }
}
