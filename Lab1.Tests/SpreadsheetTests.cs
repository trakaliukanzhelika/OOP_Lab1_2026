using System.Numerics;
using Lab1;

namespace Lab1.Tests
{
    public class SpreadsheetTests
    {
        // Test 4: if a cell changes, the cells that refer to it are recalculated
        [Fact]
        public void SetCell_ChangeReferencedCell_DependentCellIsRecalculated()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCell("A1", "10");
            spreadsheet.SetCell("B1", "A1 div 2");

            // Act
            spreadsheet.SetCell("A1", "20");

            // Assert
            Assert.Equal(new BigInteger(10), spreadsheet.Cells["B1"].Value);
        }

        // Test 5: an error in one cell does not stop the calculation of other cells
        [Fact]
        public void EvaluateAll_ErrorInOneCell_OtherCellsAreStillCalculated()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCell("A1", "5");

            // Act
            var exception = Assert.Throws<Exception>(() => spreadsheet.SetCell("B1", "1 div 0"));

            // Assert
            Assert.Contains("B1", exception.Message);
            Assert.Equal("#ПОМИЛКА", spreadsheet.Cells["B1"].Value);
            Assert.Equal(new BigInteger(5), spreadsheet.Cells["A1"].Value);
        }

    }
}
