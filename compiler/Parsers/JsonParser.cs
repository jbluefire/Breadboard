using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json.Linq;
using static Breadboard.Compiler.Consts;

namespace Breadboard.Compiler.Parsers
{
    class JsonParser : IParser
    {
        public bool Parse(string path, out Unit unit)
        {
            unit = null;
            try
            {
                JObject json = JObject.Parse(File.ReadAllText(path));
                unit = new Unit();
                var ns = json["namespace"];
                if (ns is not null)
                {
                    unit.Namespace = ns.ToString().Trim();
                }
                var refs = json["refs"];
                if (refs is not null && refs.Type == JTokenType.Array)
                {
                    foreach (var item in refs)
                    {
                        var reference = new Reference();
                        var type = item["type"];
                        if (type is not null)
                        {
                            if (type.ToString().Trim().ToLower() == "file")
                            {
                                reference.Type = ReferenceType.File;
                            }
                        }
                        reference.Target = item["target"].ToString().Trim();
                        unit.References.Add(reference);
                    }
                }
                var consts = json["consts"];
                if (consts is not null && consts.Type == JTokenType.Array)
                {
                    foreach (var item in consts)
                    {
                        var constants = new Consts();
                        constants.Name = item["name"].ToString().Trim();
                        if (item["desc"] is not null)
                        {
                            constants.Comment = item["desc"].ToString();
                        }
                        constants.Type = item["type"].ToString().Trim();
                        foreach (var value in item["values"])
                        {
                            var constant = new Consts.Constant();
                            constant.Name = value["name"].ToString().Trim();
                            constant.Value = value["value"].ToString().Trim();
                            if (value["desc"] is not null)
                            {
                                constant.Comment = value["desc"].ToString();
                            }
                            constants.Constants.Add(constant);
                        }
                        unit.Definitions.Add(constants);
                    }
                }
                var types = json["types"];
                if (types is not null && types.Type == JTokenType.Array)
                {
                    foreach (var item in types)
                    {
                        var type = item["type"];
                        bool isEvent = type.ToString().Trim().ToLower() == "event";
                        Cell cell = isEvent ? new Event() : new Cell();
                        cell.Name = item["name"].ToString().Trim();
                        if (item["desc"] is not null)
                        {
                            cell.Comment = item["desc"].ToString();
                        }
                        if (item["base"] is not null)
                        {
                            cell.Base = item["base"].ToString().Trim();
                        }
                        if (isEvent)
                        {
                            ((Event)cell).Id = item["id"].ToString().Trim();
                        }
                        var props = item["props"];
                        foreach (var prop in props)
                        {
                            var property = new Cell.Property();
                            property.Name = prop["name"].ToString().Trim();
                            property.TypeSpec = Types.Parse(prop["type"].ToString().Trim());
                            if (prop["desc"] is not null)
                            {
                                property.Comment = prop["desc"].ToString();
                            }
                            cell.Properties.Add(property);
                        }
                        unit.Definitions.Add(cell);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Program.IncrementErrorCount();
                Console.Error.WriteLine(e);
                return false;
            }
        }
    }
}
