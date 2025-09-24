using System;

namespace WindowsFormsCalculator
{
    /// <summary>
    /// Console demonstration of the calculator functionality
    /// Shows high-precision arithmetic and history features
    /// </summary>
    public class ConsoleDemo
    {
        private CalculationEngine _engine;
        private CalculationHistory _history;

        public ConsoleDemo()
        {
            _engine = new CalculationEngine();
            _history = new CalculationHistory();
        }

        public void RunDemo()
        {
            Console.WriteLine("=== Windows Forms Calculator Console Demo ===");
            Console.WriteLine("Demonstrating high-precision arithmetic and history features");
            Console.WriteLine();

            // Test high-precision decimal arithmetic
            TestHighPrecisionArithmetic();
            
            // Test calculator engine with different modes
            TestCalculatorEngine();
            
            // Test calculation history
            TestCalculationHistory();
            
            Console.WriteLine("\nDemo completed successfully!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private void TestHighPrecisionArithmetic()
        {
            Console.WriteLine("1. Testing High-Precision Decimal Arithmetic:");
            Console.WriteLine("==============================================");

            // Test basic arithmetic with high precision
            var a = new HighPrecisionDecimal("123.456789012345678901234567890");
            var b = new HighPrecisionDecimal("987.654321098765432109876543210");

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"a + b = {a + b}");
            Console.WriteLine($"a - b = {a - b}");
            Console.WriteLine($"a * b = {a * b}");
            Console.WriteLine($"a / b = {a / b}");

            // Test precision preservation
            var precise = new HighPrecisionDecimal("1") / new HighPrecisionDecimal("3");
            Console.WriteLine($"1/3 = {precise.ToString(50)}");
            
            // Test scientific notation
            var scientific = new HighPrecisionDecimal("1.23456789E-15");
            Console.WriteLine($"Scientific notation: {scientific.ToString(20)}");

            Console.WriteLine();
        }

        private void TestCalculatorEngine()
        {
            Console.WriteLine("2. Testing Calculator Engine:");
            Console.WriteLine("=============================");

            // Test standard mode calculations
            _engine.Mode = CalculatorMode.Standard;
            Console.WriteLine("Standard Mode:");
            
            PerformCalculation("2", "+", "3", "=");
            PerformCalculation("10", "÷", "3", "=");
            PerformCalculation("123.456", "×", "789.123", "=");

            // Test scientific mode
            _engine.Mode = CalculatorMode.Scientific;
            Console.WriteLine("\nScientific Mode:");
            
            _engine.SetCurrentValue("9");
            _engine.PerformOperation(OperationType.Sqrt);
            Console.WriteLine($"√9 = {_engine.CurrentDisplay}");

            _engine.SetCurrentValue("100");
            _engine.PerformOperation(OperationType.Log10);
            Console.WriteLine($"log(100) = {_engine.CurrentDisplay}");

            _engine.SetCurrentValue("90");
            _engine.PerformOperation(OperationType.Sin);
            Console.WriteLine($"sin(90) = {_engine.CurrentDisplay}");

            Console.WriteLine();
        }

        private void PerformCalculation(string num1, string op, string num2, string equals)
        {
            _engine.PerformOperation(OperationType.Clear);
            
            // Input first number
            foreach (char c in num1)
            {
                _engine.InputDigit(c.ToString());
            }

            // Input operation
            OperationType operation = op switch
            {
                "+" => OperationType.Add,
                "−" => OperationType.Subtract,
                "×" => OperationType.Multiply,
                "÷" => OperationType.Divide,
                _ => OperationType.None
            };
            _engine.PerformOperation(operation);

            // Input second number
            foreach (char c in num2)
            {
                _engine.InputDigit(c.ToString());
            }

            // Execute calculation
            _engine.PerformOperation(OperationType.Equals);

            var expression = _engine.GetCompleteExpression();
            Console.WriteLine($"  {expression}");

            // Add to history
            _history.AddCalculation(expression, _engine.CurrentDisplay, _engine.Mode);
        }

        private void TestCalculationHistory()
        {
            Console.WriteLine("3. Testing Calculation History:");
            Console.WriteLine("===============================");

            var standardHistory = _history.GetHistory(CalculatorMode.Standard);
            var scientificHistory = _history.GetHistory(CalculatorMode.Scientific);

            Console.WriteLine($"Standard mode history ({standardHistory.Count} items):");
            foreach (var item in standardHistory)
            {
                Console.WriteLine($"  {item.Timestamp:HH:mm:ss} - {item}");
            }

            Console.WriteLine($"\nScientific mode history ({scientificHistory.Count} items):");
            foreach (var item in scientificHistory)
            {
                Console.WriteLine($"  {item.Timestamp:HH:mm:ss} - {item}");
            }

            Console.WriteLine($"\nTotal history items: {_history.GetHistoryCount(CalculatorMode.Standard) + _history.GetHistoryCount(CalculatorMode.Scientific)}");
            Console.WriteLine();
        }

        public static void Main(string[] args)
        {
            // Alternative entry point for console demo
            if (args.Length > 0 && args[0] == "--demo")
            {
                var demo = new ConsoleDemo();
                demo.RunDemo();
            }
            else
            {
                // Normal Windows Forms application entry (commented out due to environment)
                Console.WriteLine("To run the console demo, use: dotnet run -- --demo");
                Console.WriteLine("For the Windows Forms application, open the project in Visual Studio 2010 on Windows.");
            }
        }
    }
}