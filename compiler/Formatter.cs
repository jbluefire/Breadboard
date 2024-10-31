using System.Collections.Generic;
using System.Text;

namespace Breadboard.Compiler
{
    abstract class Formatter
    {
        public abstract string Description { get; }

        public abstract bool Format(Unit unit, string outDir);

        public abstract bool IsUpToDate(string path, string outDir);
    }
}
