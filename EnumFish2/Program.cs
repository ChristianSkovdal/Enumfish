using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace EnumFish
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Error("Syntax Error - specify input and output filename");
                return;
            }

            string name = "";
            var infile = args[0];
            var outfile = args[1];
            var ns = args[2];
            if (!File.Exists(infile))
            {
                Error($"The file {infile} cannot be found");
                return;
            }

            try
            {
                Console.WriteLine($"Processing {Path.GetFileName(infile)}");
                var found = false;
                var cslines = File.ReadAllLines(infile);
                var jslines = new StringBuilder();
                foreach (var l in cslines)
                {
                    var line = l.Trim();                    
                    var toks = line.Split(' ');
                    var idxenumkw = Array.IndexOf(toks, "enum");
                    if (idxenumkw>=0 && toks.Length>=2 && !found)
                    {
                        found = true;
                        name = toks[idxenumkw+1];
                        // Ext.define("Aux.Enum.SourceType",
                        jslines.AppendLine($"Ext.define('{ns}.{name}',");
                    }
                    else if (found)
                    {
                        line = line.Replace("=",":");
                        jslines.AppendLine(line);

                        if (line.IndexOf("}") >= 0)
                        {
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(jslines.ToString()))
                {
                    Console.WriteLine("Not a valid input file");
                }
                else
                {
                    jslines.AppendLine(");");
                    // var SourceType = new Ext.create('Aux.Enum.SourceType');
                    jslines.AppendLine($"var {name} = new Ext.create('{ns}.{name}');");

                    var text = jslines.ToString();

                    File.WriteAllText(outfile, text);
                    Console.WriteLine($"Wrote the enum to file {outfile}");
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private static void Error(string v)
        {
            Console.WriteLine(v);
        }
    }
}
