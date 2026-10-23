using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace Lab1
{
    public class ThrowExceptionErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        public override void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ArgumentException($"Invalid Expression: {msg}", e);
        }
        public void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ArgumentException($"Invalid Expression: {msg}", e);
        }
    }
    public static class Calculator
    {

        public static object Evaluate(string expression, Dictionary<string, object> variables)
        {
            var lexer = new LabCalculatorLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new LabCalculatorParser(tokens);

            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowExceptionErrorListener());

            var tree = parser.compileUnit();
            var visitor = new LabCalculatorVisitor { exprList = variables };
            return visitor.Visit(tree);
        }
    }
}
