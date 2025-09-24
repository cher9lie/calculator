namespace WindowsFormsCalculator
{
    /// <summary>
    /// Enumeration for calculator modes, matching the original C++ implementation
    /// </summary>
    public enum CalculatorMode
    {
        Standard = 0,
        Scientific = 1
    }

    /// <summary>
    /// Precision settings for different calculator modes
    /// </summary>
    public static class CalculatorPrecision
    {
        public const int StandardModePrecision = 16;
        public const int ScientificModePrecision = 32;
    }

    /// <summary>
    /// Operation types for the calculator
    /// </summary>
    public enum OperationType
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide,
        Equals,
        Clear,
        ClearEntry,
        // Scientific operations
        Sin,
        Cos,
        Tan,
        Log,
        Log10,
        Exp,
        Power,
        Sqrt,
        Reciprocal,
        Percent,
        ChangeSign
    }
}