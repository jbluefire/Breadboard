using System.Collections.Generic;

namespace Breadboard.Compiler
{
    class Unit
    {
        public string BaseName { get; set; }
        public string Namespace { get; set; }

        public List<Reference> References { get; private set; } = [];
        public List<Definition> Definitions { get; private set; } = [];
    }

    class Reference
    {
        public string Target { get; set; }
    }

    abstract class Definition
    {
        public string Name { get; set; }
    }

    class Consts : Definition
    {
        public class Constant
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public string Comment { get; set; }
        }

        public string Type { get; set; }
        public string NativeType { get; set; }
        public List<Constant> Constants { get; private set; } = [];

        public string Comment { get; set; }
    }

    class Cell : Definition
    {
        public class Property
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string SafeName { get; set; }
            public TypeSpec TypeSpec { get; set; }
            public string DefaultValue { get; set; }
            public string NativeName { get; set; }
            public string NativeType { get; set; }

            public string Comment { get; set; }
        }

        public string Base { get; set; }
        public string BaseClass { get; set; }
        public virtual bool IsEvent { get { return false; } }
        public bool IsLocal { get; set; }
        public bool AsIs { get; set; }
        public List<Property> Properties { get; private set; } = [];
        public bool HasProperties { get { return (Properties.Count != 0); } }

        public string Comment { get; set; }
    }

    class Event : Cell
    {
        public string Id { get; set; }
        public override bool IsEvent { get { return true; } }
    }
}
