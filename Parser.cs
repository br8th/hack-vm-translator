using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VMTranslator
{
    // Parses each VM command into its lexical elements
    // Handles parsing of a single .vm file
    internal class Parser
    {
        private StreamReader reader;
        private string currentLine;

        public enum CommandType
        {
            C_ARITHMETIC = 0,
            C_PUSH = 1,
            C_POP = 2,
            C_LABEL = 3,
            C_GOTO = 4,
            C_IF = 5,
            C_FUNCTION = 6,
            C_RETURN = 7,
            C_CALL = 8,
        }

        public Parser(string inputFilePath)
        {
            this.reader = new StreamReader(inputFilePath);
            this.currentLine = this.GetLine();
        }
        
        // Returns the first arg of the current command. In the case of C_ARITHMETIC, the command itself is returned (sub, add, etc)
        public string arg1()
        {
            if (GetCommandType() == CommandType.C_ARITHMETIC)
            {
                return ParseAlpha(currentLine.ToCharArray());
            }

            return currentLine.Split(' ')[1];
        }
        
        // Returns the second arg of the current command. Should be called only if the current command is C_PUSH, C_POP, C_FUNCTION or C_CALL
        public string arg2()
        {
            var currentCommand = GetCommandType();

            switch (currentCommand)
            {
                case CommandType.C_PUSH:
                case CommandType.C_POP:
                case CommandType.C_FUNCTION:
                case CommandType.C_CALL:
                    return ParseNumber(currentLine.Split(' ')[2].ToCharArray());
                default:
                    return null;
            }
        }

        // To handle comments after the 1st arg in logical commands e.g.
        // lt    // checks if n<2
        private string ParseAlpha(char[] str)
        {
            if (str.Length == 0)
                return null;

            int i = 0;

            while (i < str.Length && Char.IsLetter(str[i]))
            {
                i++;
            }

            ArraySegment<char> number = new ArraySegment<char>(str, 0, i);
            return new string(number);
        }

        // push constant 4000	// test THIS and THAT context save
        // to handle comments after the 2nd arg,
        // 400\t// test THIS and THAT context save
        private string ParseNumber(char[] str)
        {
            if (str.Length == 0)
                return null;

            int i = 0;

            while (i < str.Length && Char.IsDigit(str[i]))
            {
                i++;
            }

            ArraySegment<char> number = new ArraySegment<char>(str, 0, i);
            return new string(number);
        }

        // Returns a constant representing the type of the current command
        public CommandType GetCommandType()
        {
            var firstWord = currentLine.Split(' ')[0];

            switch(firstWord)
            {
                case "pop":
                    return CommandType.C_POP;
                case "push":
                    return CommandType.C_PUSH;
                case "label":
                    return CommandType.C_LABEL;
                case "call":
                    return CommandType.C_CALL;
                case "function":
                    return CommandType.C_FUNCTION;
                case "return":
                    return CommandType.C_RETURN;
                case "goto":
                    return CommandType.C_GOTO;
                case "if-goto":
                    return CommandType.C_IF;
                // sub, add, lt, eq, gt 
                default:
                    return CommandType.C_ARITHMETIC;
            }
        }

        public string GetCurrentLine()
        {
            return currentLine;
        }

        // Are there more commands in the input?
        public bool HasMoreCommands()
        {
            // Console.WriteLine("Reader.Peek(): {0}", this.reader.Peek());
            //return this.reader.Peek() > -1;
            return currentLine != null;
        }

        // Reads next command from the input and makes it the current command
        public void Advance()
        {
            currentLine = GetLine();
        }

        private string GetLine()
        {
            var line = this.reader.ReadLine();

            // EOF
            if (line == null)
            {
                return null;
            }

            if (isBlank(line) || isComment(line))
                line = GetLine();

            return line;
        }

        private static bool isBlank(string line)
        {
            return line.Trim().Length == 0;
        }

        private static bool isComment(string line)
        {
            return line.StartsWith("//");
        }
    }
}
