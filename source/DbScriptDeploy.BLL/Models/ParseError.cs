using System;
using System.Collections.Generic;
using System.Text;

namespace DbScriptDeploy.BLL.Models
{
    public class ParseError
    {
        public ParseError(string message, int number, int offset, int line, int column)
        {
            this.Message = message;
            this.Number = number;
            this.Offset = offset;
            this.Line = line;
            this.Column = column;
        }

        public int Number { get; internal set; }
        public int Offset { get; internal set; }
        public int Line { get; internal set; }
        public int Column { get; internal set; }
        public string Message { get; internal set; }
    }
}
