using System;
using System.IO;

namespace VMTranslator
{
    // Takes File.vm as input, produces File.asm as output
    // Takes Dir as input, produces Dir.asm as output
    internal class VMTranslator
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            bool isDirectory = File.GetAttributes(filePath).HasFlag(FileAttributes.Directory);

            // If the path is a directory, outputfile should be directory.asm (inside the directory)
            string outputFileName;

            if (isDirectory)
            {
                string filePathTrimmed = Path.TrimEndingDirectorySeparator(filePath);
                DirectoryInfo directoryInfo = new DirectoryInfo(filePathTrimmed);
                bool hasParent = directoryInfo.Parent.Exists; // No subdirectories

                if (!hasParent)
                    outputFileName = directoryInfo.Name + ".asm";
                else
                    outputFileName = Path.Combine(Path.GetDirectoryName(filePathTrimmed), directoryInfo.Name, directoryInfo.Name)+ ".asm";

            } else
            {
                outputFileName = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath)) + ".asm";
            }
            
            CodeWriter codeWriter = new CodeWriter(outputFileName);

            string[] filesInDir = { filePath };

            if (isDirectory)
            {
                codeWriter.WriteInit();
                filesInDir = Directory.GetFiles(filePath);
            }

            foreach (string fileName in filesInDir)
            {
                if (Path.GetExtension(fileName) == ".vm")
                {
                    Console.WriteLine(fileName);
                    Parser parser = new Parser(fileName);
                    codeWriter.SetFileName(fileName);

                    while (parser.HasMoreCommands())
                    {
                        switch (parser.GetCommandType())
                        {
                            case Parser.CommandType.C_ARITHMETIC:
                                codeWriter.WriteArithmetic(parser.arg1());
                                break;
                            case Parser.CommandType.C_PUSH:
                                codeWriter.WritePushPop("push", parser.arg1(), Convert.ToInt32(parser.arg2()));
                                break;
                            case Parser.CommandType.C_POP:
                                codeWriter.WritePushPop("pop", parser.arg1(), Convert.ToInt32(parser.arg2()));
                                break;
                            case Parser.CommandType.C_LABEL:
                                codeWriter.WriteLabel(parser.arg1());
                                break;
                            case Parser.CommandType.C_GOTO:
                                codeWriter.WriteGoto(parser.arg1());
                                break;
                            case Parser.CommandType.C_IF:
                                codeWriter.WriteIf(parser.arg1());
                                break;
                            case Parser.CommandType.C_CALL:
                                codeWriter.WriteCall(parser.arg1(), Convert.ToInt32(parser.arg2()));
                                break;
                            case Parser.CommandType.C_FUNCTION:
                                codeWriter.WriteFunction(parser.arg1(), Convert.ToInt32(parser.arg2()));
                                break;
                            case Parser.CommandType.C_RETURN:
                                codeWriter.WriteReturn();
                                break;
                        }

                        parser.Advance();
                    }
                }
            }

            codeWriter.Close();
        }
    }
}
