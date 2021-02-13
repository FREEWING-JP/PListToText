/**
 * macOS config.plist to Normal Text formater program
 * PListToText.exe
 * Copyright (c)2021 FREE WING, Y.Sakamoto
 * 
 * Build and Work Tested
 * Windows 10 builtin C# compiler
 * Windows 10 and Visual Studio 2017
 * macOS Big Sur and Visual Studio 2019 ver. 8.8.8
 * 
 * Windows 10
 * echo search C# compiler from Windows 10
 * cd /d C:\Windows
 * dir /b /s | findstr csc.exe$
 * 
 * echo build
 * C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe PListToText.cs
 * 
 * PListToText.exe config.plist
 * 
 * macOS
 * dotnet PListToText.dll config.plist
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PListToText
{
    class Program
    {
        static bool isMaskValue = true;
        static string[] maskKeyList = {
            // for OpenCore
            "SystemSerialNumber", "MLB", "SystemUUID", "ROM",
            // for Clover
            "SerialNumber", "BoardSerialNumber", "SmUUID", "Board-ID"
        };

        static void Main(string[] args)
        {
            string filename = @"config.plist";
            if (args.Length > 0)
            {
                filename = args[0];
            }
            if (args.Length > 1)
            {
                isMaskValue = false;
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine("File not Found . {0}", filename);
                Environment.Exit(0);
            }

            Data.PList plist = new Data.PList();
            plist.Load(filename);

            Console.WriteLine("plist file = {0}", filename);
            PrintPList(plist);
        }


        static public void PrintIndent(int level)
        {
            for (int i = 0; i < level * 2; ++i)
            {
                Console.Write(" ");
            }
        }

        static public void PrintPList(Data.PList plist, int level = 0)
        {
            if (plist.Count == 0)
            {
                PrintIndent(level);
                Console.WriteLine("{0}", "{ }");
                return;
            }

            PrintIndent(level);
            Console.WriteLine("{0}", "{");
            ++level;

            foreach (KeyValuePair<string, dynamic> kvp in plist)
            {
                if (kvp.Value is Data.PList)
                {
                    PrintIndent(level);
                    Console.WriteLine("{0}", kvp.Key);
                    PrintPList(kvp.Value, level);
                    continue;
                }

                if (kvp.Value is List<dynamic>)
                {
                    PrintIndent(level);
                    Console.WriteLine("{0}", kvp.Key);
                    if (kvp.Value.Count == 0)
                    {
                        PrintIndent(level);
                        Console.WriteLine("{0}", "[ ]");
                        continue;
                    }

                    PrintIndent(level);
                    Console.WriteLine("{0}", "[");
                    foreach (dynamic item in kvp.Value)
                    {
                        if (item is Data.PList)
                        {
                            PrintPList(item, level + 1);
                            continue;
                        }

                        PrintIndent(level + 1);
                        Console.WriteLine("{0}", item);
                    }
                    PrintIndent(level);
                    Console.WriteLine("{0}", "]");
                    continue;
                }

                string value = "";
                if (kvp.Value is string)
                {
                    value = '"' + kvp.Value + '"';
                }
                else
                if (kvp.Value is bool)
                {
                    value = kvp.Value ? "true" : "false";
                }
                else
                if (kvp.Value is byte[])
                {
                    foreach (byte b in kvp.Value)
                    {
                        value += string.Format("{0:X2}", b);
                    }
                    value += " (";
                    value += Convert.ToBase64String(kvp.Value);
                    value += ")";
                }
                else
                {
                    PrintIndent(level);
                    Console.WriteLine("{0} = {1}", kvp.Key, kvp.Value);
                    continue;
                }

                if (isMaskValue)
                {
                    if (maskKeyList.Contains(kvp.Key))
                    {
                        value = "/** masked **/";
                    }
                }

                PrintIndent(level);
                Console.WriteLine("{0} = {1}", kvp.Key, value);
            }

            --level;
            PrintIndent(level);
            Console.WriteLine("{0}", "}");
        }
    }
}

// A Simple PList Parser in C#
// https://www.codeproject.com/Tips/406235/A-Simple-PList-Parser-in-Csharp
namespace Data
{
    public class PList : Dictionary<string, dynamic>
    {
        public PList()
        {
        }

        public PList(string file)
        {
            Load(file);
        }

        public void Load(string file)
        {
            Clear();

            XDocument doc = XDocument.Load(file);
            XElement plist = doc.Element("plist");
            XElement dict = plist.Element("dict");

            var dictElements = dict.Elements();
            Parse(this, dictElements);
        }

        private void Parse(PList dict, IEnumerable<XElement> elements)
        {
            for (int i = 0; i < elements.Count(); i += 2)
            {
                XElement key = elements.ElementAt(i);
                XElement val = elements.ElementAt(i + 1);

                dict[key.Value] = ParseValue(val);
            }
        }

        private List<dynamic> ParseArray(IEnumerable<XElement> elements)
        {
            List<dynamic> list = new List<dynamic>();
            foreach (XElement e in elements)
            {
                dynamic one = ParseValue(e);
                list.Add(one);
            }

            return list;
        }

        private dynamic ParseValue(XElement val)
        {
            switch (val.Name.ToString())
            {
                case "string":
                    return val.Value;

                case "integer":
                    return long.Parse(val.Value);

                case "real":
                    return float.Parse(val.Value);

                case "true":
                    return true;

                case "false":
                    return false;

                case "dict":
                    PList plist = new PList();
                    Parse(plist, val.Elements());
                    return plist;

                case "array":
                    List<dynamic> list = ParseArray(val.Elements());
                    return list;

                case "data":
                    byte[] bytes = Convert.FromBase64String(val.Value);
                    return bytes;

                default:
                    throw new ArgumentException("Unsupported");
            }
        }
    }
}
