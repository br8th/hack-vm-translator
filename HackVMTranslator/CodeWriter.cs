using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HackVMTranslator
{
    // Writes the assembly code that implements the parsed command
    internal class CodeWriter
    {
        private StreamWriter writer;
        private const string SP_PLUS_PLUS = "@SP\nM=M+1\n"; // SP++
        private const string SP_MINUS_MINUS = "@SP\nM=M-1\n"; // SP--
        private static int LOGIC_LABEL_COUNT = 0;

        public CodeWriter(string outputFile)
        {
            this.writer = new StreamWriter(outputFile);
        }

        // Writes to the output file the assembly code that implements the given command.
        // @param command - add, sub, eq, etc
        public void WriteArithmetic(string command)
        {
            writer.WriteLine($"// {command}");

            string operatr;

            switch (command)
            {
                case "add":
                    operatr = "+";
                    writer.WriteLine(WriteAddSub(operatr));
                    break;

                case "sub":
                    operatr = "-";
                    writer.WriteLine(WriteAddSub(operatr));
                    break;
                
                case "eq":
                    operatr = "=";
                    writer.WriteLine(WriteLogicalEq(operatr));
                    break;

                case "and":
                    operatr = "&";
                    writer.WriteLine(WriteAddSub(operatr));
                    break;
                
                case "or":
                    operatr = "|";
                    writer.WriteLine(WriteAddSub(operatr));
                    break;
                
                case "lt":
                    operatr = "<";
                    writer.WriteLine(WriteLogical(operatr));
                    break;

                case "gt":
                    operatr = ">";
                    writer.WriteLine(WriteLogical(operatr));
                    break;
                case "not":
                    operatr = "!";
                    writer.WriteLine(WriteSingleOperand(operatr));
                    break;
                case "neg":
                    operatr = "-";
                    writer.WriteLine(WriteSingleOperand(operatr));
                    break;
            }
        }

        // The assembly to perform the 'not'/ 'neg' vm commands
        private string WriteSingleOperand(string operatr)
        {
            var str = "@SP\n";
            str += "A=M-1\n";
            str += $"M={operatr}M\n"; 

            return str;
        }

        // The assembly to perform the 'eq' vm command
        private string WriteLogicalEq(string operatr)
        {
            var str = SP_MINUS_MINUS;
            str += "A=M\n";
            str += "D=M\n"; 
            str += "A=A-1\n";
            str += "D=D-M\n";

            str += $"@TRUE_{LOGIC_LABEL_COUNT}\n";
            str += "D;JEQ\n";
            str += $"(FALSE_{LOGIC_LABEL_COUNT})\n"; // Label here is for legibility
            str += "@SP\n";
            str += "A=M-1\n";
            str += "M=0\n";
            str += $"@END_{LOGIC_LABEL_COUNT}\n";
            str += "0;JMP\n";

            str += $"(TRUE_{LOGIC_LABEL_COUNT})\n";
            str += "@SP\n";
            str += "A=M-1\n";
            str += "M=-1\n";
            str += $"(END_{LOGIC_LABEL_COUNT})\n";

            LOGIC_LABEL_COUNT++;

            return str;
        }

        // The assembly to perform the 'lt'/ 'gt' vm command
        private string WriteLogical(string operatr = "<")
        {
            var str = SP_MINUS_MINUS;
            str += "A=M\n";
            str += "D=M\n"; 
            str += "A=A-1\n";

            if (operatr == "<")
                str += "D=M-D\n";
            else if (operatr == ">")
                str += "D=D-M\n";

            str += $"@FALSE_{LOGIC_LABEL_COUNT}\n";
            str += "D;JGT\n";
            str += $"@FALSE_{LOGIC_LABEL_COUNT}\n"; // If the operands are equal, return false
            str += "D;JEQ\n";
            str += $"(TRUE_{LOGIC_LABEL_COUNT})\n"; // Label here is for legibility
            str += "@SP\n";
            str += "A=M-1\n";
            str += "M=-1\n";
            str += $"@END_{LOGIC_LABEL_COUNT}\n";
            str += "0;JMP\n";
            str += $"(FALSE_{LOGIC_LABEL_COUNT})\n";
            str += "@SP\n";
            str += "A=M-1\n";
            str += "M=0\n";
            str += $"(END_{LOGIC_LABEL_COUNT})\n";

            LOGIC_LABEL_COUNT++;
            return str;
        }

        // The assembly to perform the 'add'/ 'sub' vm command
        private string WriteAddSub(string operatr = "+")
        {
            var str = "@SP\n";
            str += "A=M-1\n";
            str += "D=M\n"; 
            str += "A=A-1\n";
            str += $"M=M{operatr}D\n";
            str += SP_MINUS_MINUS;
            return str;
        }

        // Writes to the output file the assembly code that implements the given command.
        // Where command is push, pop,
        public void WritePushPop(string command, string segment, int index)
        {
            // push argument 1
            writer.WriteLine($"// {command} {segment} {index}");

            if (command == "push")
            {
                switch (segment) {
                    case "local":
                        writer.WriteLine(WritePushSegment("LCL", index));
                        break;
                    case "argument":
                        writer.WriteLine(WritePushSegment("ARG", index));
                        break;
                    case "this":
                        writer.WriteLine(WritePushSegment("THIS", index));
                        break;
                    case "that":
                        writer.WriteLine(WritePushSegment("THAT", index));
                        break;
                    case "constant":
                        writer.WriteLine(PtrSPEqualsX(index));
                        break;
                }
            }
            
            if (command == "pop")
            {
                switch (segment) {
                    case "local":
                        writer.WriteLine(WritePopSegment("LCL", index));
                        break;
                    case "argument":
                        writer.WriteLine(WritePopSegment("ARG", index));
                        break;
                    case "this":
                        writer.WriteLine(WritePopSegment("THIS", index));
                        break;
                    case "that":
                        writer.WriteLine(WritePopSegment("THAT", index));
                        break;
                }
            }

        }
        
        private string WritePopSegment(string segment, int index)
        {
            var str = $"@{index}\n";
            str += "D=A\n";

            str += $"@{segment}\n";
            str += "A=M+D\n";
            str += "D=M\n";

            str += SP_MINUS_MINUS;

            str += "@SP\n";
            str += "A=M\n";
            str += "M=D\n";

            return str;
        }

        private string WritePushSegment(string segment, int index)
        {
            var str = $"@{index}\n";
            str += "D=A\n";

            str += $"@{segment}\n";
            str += "A=M+D\n";
            str += "D=M\n";

            str += "@SP\n";
            str += "A=M\n";
            str += "M=D\n";
            str += SP_PLUS_PLUS;

            return str;
        }


        // Returns assembly for pseudocode in the syntax *SP = x
        // Where x is a positive integer
        private string PtrSPEqualsX(int x)
        {
            return PtrSegmentEqualsX("SP", x);
        }

        // Returns assembly for pseudocode in the syntax *SP = x
        // Where x is a positive integer
        // segment is one of the memory segments in hack (SP, LCL, ARG, THIS, THAT)
        private string PtrSegmentEqualsX(string segment, int x)
        {
            var str = $"@{x}\n";
            str += "D=A\n";

            str += $"@{segment}\n";
            str += "A=M\n";
            str += "M=D\n";
            str += SP_PLUS_PLUS;

            return str;
        }

        // Closes the output file
        public void Close()
        {
            writer.Dispose(); 
        }

    }
}
