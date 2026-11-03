using System.Numerics;

namespace Lab1
{
    internal class LabCalculatorVisitor : LabCalculatorBaseVisitor<object>
    {
        // Dictionary<cellname, result>
        public Dictionary<string, object> exprList = new();

        public override object VisitNumExpr(LabCalculatorParser.NumExprContext context)
        {
            return BigInteger.Parse(context.GetText());
        }

        public override object VisitCellExpr(LabCalculatorParser.CellExprContext context)
        {
            string cellName = context.GetText();
            if (!exprList.ContainsKey(cellName))
            {
                throw new ArgumentException($"Клітинка {cellName} порожня або не існує.");
            }
            object value = exprList[cellName];

            // "" (not calculated yet) or "#ПОМИЛКА"
            if (value is string)
            {
                throw new ArgumentException($"Клітинка {cellName} порожня або містить помилку.");
            }
            return value;
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
            BigInteger left = ToNumber(Visit(context.left));
            BigInteger right = ToNumber(Visit(context.right));
            if (right == 0)
            {
                throw new DivideByZeroException("Ділення на нуль неможливе.");
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
            BigInteger left = ToNumber(Visit(context.left));
            BigInteger right = ToNumber(Visit(context.right));
            return BigInteger.Max(left, right);
        }

        public override object VisitMinExpr(LabCalculatorParser.MinExprContext context)
        {
            BigInteger left = ToNumber(Visit(context.left));
            BigInteger right = ToNumber(Visit(context.right));
            return BigInteger.Min(left, right);
        }

        public override object VisitCompareExpr(LabCalculatorParser.CompareExprContext context)
        {
            BigInteger left = ToNumber(Visit(context.left));
            BigInteger right = ToNumber(Visit(context.right));
            switch (context.operatorToken.Type)
            {
                case LabCalculatorParser.LE: return left <= right;
                case LabCalculatorParser.GE: return left >= right;
                case LabCalculatorParser.NE: return left != right;
                default: throw new ArgumentException("Введені дані неправильні.");
            }
        }

        public override object VisitAndExpr(LabCalculatorParser.AndExprContext context)
        {
            bool left = ToBool(Visit(context.left));
            bool right = ToBool(Visit(context.right));
            return left && right;
        }

        public override object VisitOrExpr(LabCalculatorParser.OrExprContext context)
        {
            bool left = ToBool(Visit(context.left));
            bool right = ToBool(Visit(context.right));
            return left || right;
        }

        public override object VisitNotExpr(LabCalculatorParser.NotExprContext context)
        {
            return !ToBool(Visit(context.expression()));
        }

        private static BigInteger ToNumber(object value)
        {
            if (value is BigInteger number) return number;
            if (value is bool flag) return flag ? 1 : 0;
            throw new ArgumentException($"Значення \"{value}\" не є числом.");
        }

        private static bool ToBool(object value)
        {
            if (value is bool flag) return flag;
            if (value is BigInteger number) return number != 0;
            throw new ArgumentException($"Значення \"{value}\" не є логічним.");
        }
    }
}
