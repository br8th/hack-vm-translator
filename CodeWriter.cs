using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VMTranslator
{
    // Writes the assembly code that implements the parsed command
    internal class CodeWriter
    {
        private StreamWriter writer;
        private StringBuilder sb;
        private string outputFileName;
        private const string SP_PLUS_PLUS = "@SP\nM=M+1\n"; // SP++
        private const string SP_MINUS_MINUS = "@SP\nM=M-1\n"; // SP--
        private static int LOGIC_LABEL_COUNT = 0;
        private static int FUNCTION_CALL_LABEL_COUNT = 0;
        private static int FUNCTION_RETURN_LABEL_COUNT = 0;

        public CodeWriter(string outputFile)
        {
            this.writer = new StreamWriter(outputFile);
            // TODO: Clean up
            this.outputFileName = Path.GetFileNameWithoutExtension(outputFile);
            this.sb = new StringBuilder();
        }

        // Informs the code writer that the translation of a new vm file has started
        public void SetFileName(string fileName)
        {
            writer.WriteLine($"// Transalation of file: {fileName} has started.");
            writer.WriteLine("// Updating name for static variables..");
            this.outputFileName = Path.GetFileNameWithoutExtension(fileName);
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
                    case "static":
                        writer.WriteLine(WritePushStatic(index));
                        break;
                    case "temp":
                        writer.WriteLine(WritePushTemp(index));
                        break;
                    case "pointer":
                        writer.WriteLine(WritePushPointer(index));
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
                    case "static":
                        writer.WriteLine(WritePopStatic(index));
                        break;
                    case "temp":
                        writer.WriteLine(WritePopTemp(index));
                        break;
                    case "pointer":
                        writer.WriteLine(WritePopPointer(index));
                        break;
                }
            }

        }

        private string WritePopStatic(int index)
        {
            var str = SP_MINUS_MINUS;

            str += "A=M\n";
            str += "D=M\n";

            str += $"@{outputFileName}.{index}\n";
            str += "M=D\n";

            return str;
        }
        private string WritePushStatic(int index)
        {
            var str = $"@{outputFileName}.{index}\n";
            str += "D=M\n";

            str += "@SP\n";
            str += "A=M\n";
            str += "M=D\n";
            str += SP_PLUS_PLUS;

            return str;
        }
        private string WritePushTemp(int index)
        {
            var str = $"@{index}\n";
            str += "D=A\n";

            str += $"@5\n";
            str += "A=A+D\n";
            str += "D=M\n";

            str += "@SP\n";
            str += "A=M\n";
            str += "M=D\n";
            str += SP_PLUS_PLUS;

            return str;
        }
        private string WritePopTemp(int index)
        {
            var str = $"@{index}\n";
            str += "D=A\n";

            str += $"@5\n";
            str += "D=D+A\n";

            str += $"@temp_temp_{index}\n";
            str += "M=D\n";

            str += SP_MINUS_MINUS;

            str += "A=M\n";
            str += "D=M\n";

            str += $"@temp_temp_{index}\n";
            str += "A=M\n";
            str += "M=D\n";

            return str;
        }
        private string WritePopPointer(int index)
        {
            if (index != 0 && index != 1)
                throw new ArgumentException("Invalid param. Value should be either 0 or 1");

            // SP--, THIS/THAT = *SP
            var str = SP_MINUS_MINUS;
            str += "A=M\n";
            str += "D=M\n";

            str += (index == 0) ? "@THIS\n" : "@THAT\n";

            str += "M=D\n";

            return str;
        }
        private string WritePushPointer(int index)
        {
            if (index != 0 && index != 1)
                throw new ArgumentException("Invalid param. Value should be either 0 or 1");

            // *SP=THIS/THAT, SP++

            var str = (index == 0) ? "@THIS\n" : "@THAT\n";
            str += "D=M\n";

            str += "@SP\n";
            str += "A=M\n";
            str += "M=D\n";

            str += SP_PLUS_PLUS;

            return str;
        }

        private string WritePopSegment(string segment, int index)
        {
            var str = $"@{index}\n";
            str += "D=A\n";

            str += $"@{segment}\n";
            str += "D=D+M\n";

            str += $"@temp_{segment}_{index}\n";
            str += "M=D\n";

            str += SP_MINUS_MINUS;

            str += "A=M\n";
            str += "D=M\n";

            str += $"@temp_{segment}_{index}\n";
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

        // Returns assembly for pseudocode in the syntax *SP = x, SP++
        // Where x is a positive integer
        // segment is one of the memory segments in hack (SP, LCL, ARG, THIS, THAT)
        private string PtrSegmentEqualsX(string segment, int x)
        {
            // TODO: Something odd about the online course grader. If I change the first few lines
            // to windows style line endings, it works.

            //sb.Clear();
            //sb.AppendLine($"@{x}\r\n");
            //sb.AppendLine("D=A");
            //sb.AppendLine($"@{segment}");
            //sb.AppendLine("A=M");
            //sb.AppendLine("M=D");

            //sb.AppendLine(SP_PLUS_PLUS);

            //return sb.ToString();
            var str = $"@{x}\r\n";
            str += "D=A\r\n";

            str += $"@{segment}\r\n";
            str += "A=M\r\n";
            str += "M=D\r\n";
            str += SP_PLUS_PLUS;

            return str;
        }

        // Returns assembly for pseudocode in the syntax *SP = *Segment, SP++
        // Where Segment is one of the memory segments in hack (LCL, ARG, THIS, THAT)
        private string PtrSPEqualsPtrSegment(string segment)
        {
            sb.Clear();

            sb.AppendLine($"@{segment}");
            sb.AppendLine("D=M");
            sb.AppendLine($"@SP");
            sb.AppendLine("A=M");
            sb.AppendLine("M=D");

            return sb.ToString();
        }


        // The assembly that effects the if-goto command
        public void WriteIf(string label)
        {
            var labelName = $"{outputFileName.ToUpper()}${label}";

            sb.Clear();
            sb.AppendLine($"// if-goto {labelName}");
            sb.AppendLine($"@SP");
            sb.AppendLine($"AM=M-1");
            sb.AppendLine($"D=M");

            sb.AppendLine($"@{labelName}");
            sb.AppendLine("D;JNE");

            writer.WriteLine(sb.ToString());
        }

        // The assembly that effects the goto command
        public void WriteGoto(string label)
        {
            var labelName = $"{outputFileName.ToUpper()}${label}";

            writer.WriteLine($"// goto {labelName}"); // debug
            writer.WriteLine($"@{labelName}");
            writer.WriteLine("0;JMP");
        }

        // The assembly that effects the label command
        public void WriteLabel(string label)
        {
            var labelName = $"{outputFileName.ToUpper()}${label}";
            writer.WriteLine($"// label {labelName}"); // debug
            writer.WriteLine($"({labelName})");
        }

        // The assembly that effects the call command
        // call functionName nArgs
        public void WriteCall(string functionName, int numArgs)
        {
            //var functionName = $"{outputFileName}.{name}";
            var lblReturnAddr = $"{functionName}$ret.{FUNCTION_CALL_LABEL_COUNT}";

            writer.WriteLine($"// call {functionName} numArgs{numArgs}"); // debug

            // Assembly to save the caller's state
            // 1. Push returnAddress (using label declared below)
            writer.WriteLine($"@{lblReturnAddr}");
            writer.WriteLine("D=A");
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");
            writer.WriteLine(SP_PLUS_PLUS);

            // 2. Push LCL
            writer.WriteLine(PtrSPEqualsPtrSegment("LCL"));
            writer.WriteLine(SP_PLUS_PLUS);
            // 3. Push ARG
            writer.WriteLine(PtrSPEqualsPtrSegment("ARG"));
            writer.WriteLine(SP_PLUS_PLUS);
            // 4. Push THIS
            writer.WriteLine(PtrSPEqualsPtrSegment("THIS"));
            writer.WriteLine(SP_PLUS_PLUS);
            // 5. Push THAT
            writer.WriteLine(PtrSPEqualsPtrSegment("THAT"));
            writer.WriteLine(SP_PLUS_PLUS); 

            // Setup for the function call
            // 6. Reposition ARG (To the called function's args) ARG = SP - nArgs - 5
            writer.WriteLine("@5");
            writer.WriteLine("D=A");

            writer.WriteLine($"@{numArgs}");
            writer.WriteLine($"D=A+D");

            writer.WriteLine("@SP");
            writer.WriteLine("D=M-D"); // SP - (nArgs + 5)

            writer.WriteLine("@ARG");
            writer.WriteLine("M=D");

            // 7. Reposition LCL (For the called function) LCL = SP
            writer.WriteLine("@SP");
            writer.WriteLine("D=M");

            writer.WriteLine("@LCL");
            writer.WriteLine("M=D");

            // 9. Go to Bar.Mult (called function name)
            writer.WriteLine($"@{functionName}");
            writer.WriteLine($"0;JMP");

            // 10. (returnAddress) Foo$ret.1
            writer.WriteLine($"({lblReturnAddr})");
            writer.WriteLine();
            FUNCTION_CALL_LABEL_COUNT++;
        }

        // The assembly that effects the call command
        // function Bar.mult 2
        public void WriteFunction(string functionName, int numLocalVars)
        {
            //var functionName = $"{outputFileName}.{name}";

            sb.Clear();
            sb.AppendLine($"// function {functionName} {numLocalVars}"); // debug

            // 1. (Function.Name)
            sb.AppendLine($"({functionName})");

            for (int i = 0; i < numLocalVars; i++)
            {
                //  push constant 0 
                sb.AppendLine(PtrSegmentEqualsX("SP", 0));
            }

            writer.WriteLine(sb.ToString());
        }

        // The assembly that effects the return command
        public void WriteReturn()
        {
            var frame = $"frame.{FUNCTION_RETURN_LABEL_COUNT}";
            var returnAddress = $"return-address.{FUNCTION_RETURN_LABEL_COUNT}";

            writer.WriteLine($"// return"); // debug

            // 1. Moves return value to the caller
            // 2. Reinstates caller's state
            // - endFrame = LCL
            writer.WriteLine("@LCL");
            writer.WriteLine("D=M");
            writer.WriteLine($"@{frame}");
            writer.WriteLine("M=D");

            // - returnAddress = *(endFrame - 5): Review line
            writer.WriteLine("@5"); 
            writer.WriteLine("D=A");

            writer.WriteLine($"@{frame}");
            writer.WriteLine("A=M-D");
            writer.WriteLine("D=M");

            writer.WriteLine($"@{returnAddress}");
            writer.WriteLine("M=D");

            // - *ARG = pop()
            writer.WriteLine(SP_MINUS_MINUS);
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("D=M");

            writer.WriteLine("@ARG");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");

            // - SP = ARG + 1
            writer.WriteLine("@ARG");
            writer.WriteLine("D=M+1");
            writer.WriteLine("@SP");
            writer.WriteLine("M=D");

            // - THAT = *(endFrame - 1)
            writer.WriteLine($"@{frame}");
            writer.WriteLine("A=M-1");
            writer.WriteLine("D=M");

            writer.WriteLine("@THAT");
            writer.WriteLine("M=D");

            // - THIS = *(endFrame - 2)
            writer.WriteLine("@2"); 
            writer.WriteLine("D=A");

            writer.WriteLine($"@{frame}");
            writer.WriteLine("A=M-D");
            writer.WriteLine("D=M");

            writer.WriteLine("@THIS");
            writer.WriteLine("M=D");

            // - ARG = *(endFrame - 3)
            writer.WriteLine("@3"); 
            writer.WriteLine("D=A");

            writer.WriteLine($"@{frame}");
            writer.WriteLine("A=M-D");
            writer.WriteLine("D=M");

            writer.WriteLine("@ARG");
            writer.WriteLine("M=D");

            // - LCL = *(endFrame - 4)
            writer.WriteLine("@4"); 
            writer.WriteLine("D=A");

            writer.WriteLine($"@{frame}");
            writer.WriteLine("A=M-D");
            writer.WriteLine("D=M");

            writer.WriteLine("@LCL");
            writer.WriteLine("M=D");

            // - goto returnAddress popped from stack
            writer.WriteLine($"@{returnAddress}");
            writer.WriteLine("A=M");
            writer.WriteLine("0;JMP");

            FUNCTION_RETURN_LABEL_COUNT++;
        }

        // The bootstrap code
        public void WriteInit()
        {
            // We need windows style line endings on the first line for the autograder.

            // SP = 256
            sb.Clear();
            sb.AppendLine($"// init\r\n"); // debug
            sb.AppendLine($"@256");
            sb.AppendLine($"D=A");
            sb.AppendLine($"@SP");
            sb.AppendLine($"M=D");

            writer.WriteLine(sb.ToString());

            // Call Sys.Init
            this.WriteCall("Sys.init", 0);
        }

        // Closes the output file
        public void Close()
        {
            writer.Dispose(); 
        }

    }
}
