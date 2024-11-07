using System;
using System.Collections.Generic;

using System.CommandLine;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection.Metadata.Ecma335;

using Breadboard.Compiler.Formatters;
using Breadboard.Compiler.Parsers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Breadboard.Compiler
{
    class Compiler
    {
        public string OutDir { get; set; }
        public bool Recursive { get; set; }
        public string Spec { get; set; } = "cs";

        private static Dictionary<string, Formatter> formatters = new()
        {
            { "cs", new CSharpFormatter() }
        };
        private static Dictionary<string, IParser> parsers = new()
        {
            { ".json", new JsonParser() }
        };

        private Formatter formatter;
        private readonly Stack<string> subDirs = [];

        public void Run(List<string> arguments)
        {
            formatter = formatters["cs"];

            Console.WriteLine(OutDir);
            Console.WriteLine(Recursive);
            foreach (var argument in arguments)
            {
                Process(argument);
            }
        }

        private void Process(string path)
        {
            if (Directory.Exists(path))
            {
                ProcessDir(path, null);
            }
            else if (File.Exists(path))
            {
                ProcessFile(path);
            }
            else
            {
                if (path.IndexOfAny(['*', '?']) >= 0)
                {
                    ProcessDir(".", path);  // TODO
                }
                else
                {
                    Console.Error.WriteLine($"{path} doesn't exist.");
                    Program.IncrementErrorCount();
                }
            }
        }

        private void ProcessDir(string path, string searchPattern)
        {
            Console.WriteLine("Directory {0}", Path.GetFullPath(path));

            var di = new DirectoryInfo(path);
            FileSystemInfo[] entries;
            if (searchPattern is null)
            {
                entries = di.GetFileSystemInfos();
            }
            else
            {
                entries = di.GetFileSystemInfos(searchPattern);
            }
            foreach (var entry in entries)
            {
                var pathname = Path.Combine(path, entry.Name);
                if ((entry.Attributes & FileAttributes.Directory) != 0)
                {
                    if (Recursive)
                    {
                        subDirs.Push(entry.Name);
                        ProcessDir(pathname, searchPattern);
                        subDirs.Pop();
                    }
                }
                else
                {
                    ProcessFile(pathname);
                }
            }
        }

        private void ProcessFile(string path)
        {
            var filename = Path.GetFileName(path);
            var extension = Path.GetExtension(path);
            string outDir;
            if (OutDir is null)
            {
                outDir = Path.GetDirectoryName(path);
            }
            else
            {
                outDir = Path.Combine(OutDir, string.Join(
                    Path.DirectorySeparatorChar.ToString(), subDirs.ToArray()));
            }

            if (!parsers.TryGetValue(extension.ToLower(), out IParser parser))
            {
                return;
            }

            Console.Write(path);

            if (/*!Forced &&*/ formatter.IsUpToDate(path, outDir))
            {
                Console.WriteLine(" up-to-date");
                return;
            }
            else
            {
                Console.WriteLine();
            }

            if (!parser.Parse(path, out Unit unit))
            {
                Program.IncrementErrorCount();
            }
            if (unit is null)
            {
                return;
            }

            unit.BaseName = Path.GetFileNameWithoutExtension(path);

            if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            if (!formatter.Format(unit, outDir))
            {
                Program.IncrementErrorCount();
            }
        }
    }
}
