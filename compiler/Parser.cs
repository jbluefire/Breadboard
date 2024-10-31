using System.Collections.Generic;
using System.Text;

namespace Breadboard.Compiler
{
    interface IParser
    {
        bool Parse(string path, out Unit unit);
    }
}
