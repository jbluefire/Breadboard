using System;
using System.Collections.Generic;

using System.CommandLine;
using System.Reflection.Metadata.Ecma335;

namespace Breadboard.Compiler
{
    class Compiler
    {
        public bool Recursive { get; set; }

        public void Process(List<string> arguments)
        {
            Console.WriteLine(Recursive);
            foreach (var argument in arguments)
            {
                Console.WriteLine(argument);
            }
        }
    }
}
