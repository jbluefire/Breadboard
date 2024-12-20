﻿using System.Collections.Generic;
using System.Text;

namespace Breadboard.Compiler
{
    class TypeSpec
    {
        public string Type { get; set; }
        public List<TypeSpec> Details { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(Type);
            if (Details is not null && Details.Count != 0)
            {
                sb.Append('(');
                for (int i = 0, count = Details.Count; i < count; ++i)
                {
                    if (i != 0) { sb.Append(", "); }
                    sb.Append(Details[i].ToString());
                }
                sb.Append(')');
            }
            return sb.ToString();
        }
    }

    class TypeProperty
    {
        public bool IsPrimitive { get; set; }
        public bool IsCollective { get; set; }
        public bool DetailRequired { get; set; }
    }

    static class Types
    {
        private static readonly Dictionary<string, TypeProperty> types = [];

        static Types()
        {
            // Primitive types
            types.Add("bool", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("byte", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("int8", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("int16", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("int32", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("int64", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("float32", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("float64", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("decimal", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("string", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });
            types.Add("datetime", new TypeProperty
            {
                IsPrimitive = true,
                DetailRequired = false
            });

            // Collective types
            types.Add("bytes", new TypeProperty
            {
                IsPrimitive = false,
                IsCollective = true,
                DetailRequired = false
            });
            types.Add("list", new TypeProperty
            {
                IsPrimitive = false,
                IsCollective = true,
                DetailRequired = true
            });
            types.Add("map", new TypeProperty
            {
                IsPrimitive = false,
                IsCollective = true,
                DetailRequired = true
            });
        }

        public static bool IsBuiltin(string type)
        {
            return types.ContainsKey(type);
        }

        public static bool IsCollective(string type)
        {
            return types.TryGetValue(type, out TypeProperty typeProperty) &&
                typeProperty.IsCollective;
        }

        public static bool IsPrimitive(string type)
        {
            return types.TryGetValue(type, out TypeProperty typeProperty) &&
                typeProperty.IsPrimitive;
        }

        public static TypeSpec Parse(string s)
        {
            int index = 0;
            return ParseTypeSpec(s, ref index);
        }

        private static TypeSpec ParseTypeSpec(string s, ref int index)
        {
            string type = null;
            List<TypeSpec> details = null;

            int backMargin = 0;
            int start = index;
            for (; index < s.Length; ++index)
            {
                var c = s[index];
                if (c == '(' && index < (s.Length - 1))
                {
                    type = s.Substring(start, index - start).Trim();
                    ++index;
                    details = ParseDetails(s, ref index);
                    backMargin = 1;
                    break;
                }
                else if (c == ',')
                {
                    ++index;
                    backMargin = 1;
                    break;
                }
                else if (c == ')')
                {
                    break;
                }
            }
            if (type is null)
            {
                type = s.Substring(start, index - start - backMargin).Trim();
            }
            return type.Length == 0 ? null
                : new TypeSpec { Type = type, Details = details };
        }

        private static List<TypeSpec> ParseDetails(string s, ref int index)
        {
            List<TypeSpec> details = [];

            for (; index < s.Length; ++index)
            {
                var c = s[index];
                if (c == ',')
                {
                    continue;
                }
                if (c == ')')
                {
                    ++index;
                    break;
                }
                else
                {
                    var detail = ParseTypeSpec(s, ref index);
                    if (detail is not null)
                    {
                        details.Add(detail);
                        --index;
                    }
                }
            }
            return details.Count == 0 ? null : details;
        }
    }
}
