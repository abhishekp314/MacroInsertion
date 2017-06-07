using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroInserter
{
    class Program
    {
        //Inputs : 
        //Filename pattern
        //Path where the source files are located. Look for both .h/.cpp
        //Marco name to insert
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                //Get the path of the directory
                var fileNamePattern = args[0];
                var path = args[1];
                var macroName = args[2];

                Run(fileNamePattern, path, macroName);
            }
            else
            {
                Debug.Assert(false);
            }

        }

        static void Run(string fileNamePattern, string path, string marcoName)
        {
            List<string> filePaths = new List<string>();
            Traverse(fileNamePattern, path, ref filePaths);

            //For every filepath run the check
            foreach (string filePath in filePaths)
            {
                InsertMacro(filePath, marcoName);
            }
        }

        static void Traverse(string fileNamePattern, string path, ref List<string> filePaths)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetFileNameWithoutExtension(file).Contains(fileNamePattern) && (Path.GetExtension(file).Equals(".h") || Path.GetExtension(file).Equals(".cpp")))
                {
                    filePaths.Add(file);
                }
            }

            foreach (string dirPath in Directory.GetDirectories(path))
            {
                Traverse(fileNamePattern, dirPath, ref filePaths);
            }
        }

        static void InsertMacro(string file, string marcoName)
        {
            List<string> contents = File.ReadAllLines(file).ToList();
            int isHeaderFile = -1;

            if (Path.GetExtension(file).Equals(".h"))
                isHeaderFile = 1;
            else if (Path.GetExtension(file).Equals(".cpp"))
                isHeaderFile = 0;

            bool found = false;

            if (isHeaderFile != -1)
            {
                //Look for first #ifdef ... or #if defined ... combined with #define ...
                for (int index = 0; index < contents.Count; ++index)
                {
                    if (isHeaderFile == 1)
                    {
                        if (((contents[index].Contains("#ifndef") || contents[index].Contains("#if !defined")) && contents[++index].Contains("#define"))
                            || (contents[index].Contains("#pragma once")))
                        {
                            //test the next line
                            if (AddMacro(ref contents, ++index, marcoName))
                            {
                                found = true;
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (contents[index].Contains("PCH.h") || contents[index].Contains("pch.h"))
                        {
                            if (AddMacro(ref contents, ++index, marcoName))
                            {
                                found = true;
                            }
                            break;
                        }
                    }
                }

                if (found)
                {
                    Console.WriteLine("Writing to the file");
                    File.WriteAllLines(file, contents.ToArray());
                }
            }
        }

        static bool AddMacro(ref List<string> contents, int index, string macroName)
        {
            //Make sure it doesn't already have a macro
            if (!contents[index].Contains(macroName))
            {
                contents.Insert(index, macroName);
                contents.Add("#endif");
                return true;
            }
            return false;
        }
    }
}
