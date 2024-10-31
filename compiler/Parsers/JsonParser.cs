using System.Collections.Generic;
using System.Text;

namespace Breadboard.Compiler.Parsers
{
    class JsonParser : IParser
    {
        public bool Parse(string path, out Unit unit)
        {
            unit = null;
            return true;
        }
    }
}
