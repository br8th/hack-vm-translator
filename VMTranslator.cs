using System;
using System.IO;

namespace VMTranslator
{
    // Takes File.vm as input, produces File.asm as output
    internal class VMTranslator
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            var outputFileName = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath)) + ".asm";

            Parser parser = new Parser(filePath);
            CodeWriter codeWriter = new CodeWriter(outputFileName);

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
                }

                parser.Advance();
            }

            codeWriter.Close();
        }
    }
}
