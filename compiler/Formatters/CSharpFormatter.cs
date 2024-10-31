using System.Collections.Generic;
using System.Text;

namespace Breadboard.Compiler.Formatters
{
    class CSharpFormatter : Formatter
    {
        public override string Description { get { return ""; } }

        public override bool Format(Unit unit, string outDir)
        {
            return true;
        }

        public override bool IsUpToDate(string path, string outDir)
        {
            return false;
        }
    }
}
