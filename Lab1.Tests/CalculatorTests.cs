using Lab1;

namespace Lab1.Tests
{
    public class CalculatorTests
    {
        // Test 1: all operations of variant 63.
        [Theory]
        [InlineData("7 mod 3", "1")]
        [InlineData("7 div 2", "3")]
        [InlineData("max(3, 10)", "10")]
        [InlineData("min(3, 10)", "3")]
        [InlineData("5 <= 3", "False")]
        [InlineData("5 >= 3", "True")]
        [InlineData("5 <> 5", "False")]
        [InlineData("not 0", "True")]
        [InlineData("1 and 0", "False")]
        [InlineData("1 or 0", "True")]
        public void Evaluate_Variant63Operations_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            object result = Calculator.Evaluate(expression, new Dictionary<string, object>());

            // Assert
            Assert.Equal(expected, result.ToString());
        }

        // Test 2: text instead of an expression gives a clear error message
        [Fact]
        public void Evaluate_PlainText_ThrowsClearMessage()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => Calculator.Evaluate("привіт", new Dictionary<string, object>()));

            // Assert
            Assert.Contains("Недопустимий символ або текст", exception.Message);
        }

        // Test 3: division by zero (div and mod) throws DivideByZeroException
        [Theory]
        [InlineData("7 div 0")]
        [InlineData("7 mod 0")]
        public void Evaluate_DivisionByZero_ThrowsDivideByZeroException(string expression)
        {
            // Act
            var exception = Assert.Throws<DivideByZeroException>(
                () => Calculator.Evaluate(expression, new Dictionary<string, object>()));

            // Assert
            Assert.Contains("Ділення на нуль неможливе.", exception.Message);
        }
    }
}
