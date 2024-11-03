using System;
using System.Collections.Generic;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace Breadboard.Compiler
{
    class Program
    {
        private static int errorCount;

        public static void IncrementErrorCount()
        {
            Interlocked.Increment(ref errorCount);
        }

        static int Main(string[] args)
        {
            var arguments = new Argument<List<string>>(
                name: "argument",
                description: "File or directory",
                getDefaultValue: () => { return [ "." ]; });

            var recursiveOption = new Option<bool>(
                aliases: [ "-r", "--recursive" ],
                description: "Process subdirectories recursively");

            var rootCommand = new RootCommand("Breadboard definition compiler");
            rootCommand.AddArgument(arguments);
            rootCommand.AddOption(recursiveOption);

            rootCommand.SetHandler((arguments, recursive) =>
            {
                var compiler = new Compiler {
                    Recursive = recursive
                };

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                compiler.Process(arguments);

                stopWatch.Stop();
                Console.WriteLine(errorCount == 0 ? "Ok" : $"{errorCount} error(s)");
                Console.WriteLine($"Elapsed {stopWatch.ElapsedMilliseconds} ms");
            }
            , arguments, recursiveOption);

            int exitCode = rootCommand.Invoke(args);
            return exitCode != 0 ? exitCode : errorCount;
        }
    }
}
