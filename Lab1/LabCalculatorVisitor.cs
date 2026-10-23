using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    internal class LabCalculatorVisitor : LabCalculatorBaseVisitor<object>
    {
        // Dictionary<cellname, result>
        public Dictionary<string, object> exprList = new();
        public override object VisitNumExpr(LabCalculatorParser.NumExprContext context)
        {
            return int.Parse(context.GetText());
        }
        public override object VisitCellExpr(LabCalculatorParser.CellExprContext context)
        {
            string cellName = context.GetText();
            if (!exprList.ContainsKey(cellName)){
                throw new ArgumentException("Cell does not exist or is empty.");
            }
            return exprList[cellName];
        }
        public override object VisitParenthesizedExpr(LabCalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }
        public override object VisitCompileUnit(LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override object VisitArithmExpr(LabCalculatorParser.ArithmExprContext context)
        {
            int left = Convert.ToInt32(Visit(context.left));
            int right = Convert.ToInt32(Visit(context.right));
            if (right == 0)
            {
                throw new DivideByZeroException("You cannot divide by zero.");
            }
            if (context.operatorToken.Type == LabCalculatorParser.MOD)
            {
                return left % right;
            }
            else
            {
                return left / right;
            }
        }
        public override object VisitMaxExpr(LabCalculatorParser.MaxExprContext context)
        {
            int left = Convert.ToInt32(Visit(context.left));
            int right = Convert.ToInt32(Visit(context.right));
            return Math.Max(left, right);
        }
        public override object VisitMinExpr(LabCalculatorParser.MinExprContext context)
        {
            int left = Convert.ToInt32(Visit(context.left));
            int right = Convert.ToInt32(Visit(context.right));
            return Math.Min(left, right);
        }
        public override object VisitCompareExpr(LabCalculatorParser.CompareExprContext context)
        {
            int left = Convert.ToInt32(Visit(context.left));
            int right = Convert.ToInt32(Visit(context.right));
            switch (context.operatorToken.Type)
            {
                case LabCalculatorParser.LE: return left <= right;
                case LabCalculatorParser.GE: return left >= right;
                case LabCalculatorParser.NE: return left != right;
                default: throw new ArgumentException("Incorrect input.");
            }
        }
        public override object VisitAndExpr(LabCalculatorParser.AndExprContext context)
        {
            bool left = Convert.ToBoolean(Visit(context.left));
            bool right = Convert.ToBoolean(Visit(context.right));
            return left && right;
        }
        public override object VisitOrExpr(LabCalculatorParser.OrExprContext context)
        {
            bool left = Convert.ToBoolean(Visit(context.left));
            bool right = Convert.ToBoolean(Visit(context.right));
            return left || right;
        }
        public override object VisitNotExpr(LabCalculatorParser.NotExprContext context)
        {
            return !(Convert.ToBoolean(Visit(context.expression())));
        }
    }
}
