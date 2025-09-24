using System;
using System.Globalization;
using System.Text;

namespace WindowsFormsCalculator
{
    /// <summary>
    /// Main calculation engine that handles all arithmetic operations with high precision
    /// Inspired by the C++ CCalcEngine class but adapted for .NET Framework 4.0
    /// </summary>
    public class CalculationEngine
    {
        private HighPrecisionDecimal _currentValue;
        private HighPrecisionDecimal _previousValue;
        private OperationType _pendingOperation;
        private readonly StringBuilder _currentExpression;
        private bool _isNewCalculation;
        private bool _hasError;
        private string _errorMessage;
        private CalculatorMode _mode;
        private int _precision;

        public CalculationEngine()
        {
            _currentValue = HighPrecisionDecimal.Zero;
            _previousValue = HighPrecisionDecimal.Zero;
            _pendingOperation = OperationType.None;
            _currentExpression = new StringBuilder();
            _isNewCalculation = true;
            _hasError = false;
            _mode = CalculatorMode.Standard;
            _precision = CalculatorPrecision.StandardModePrecision;
        }

        public string CurrentDisplay
        {
            get
            {
                if (_hasError)
                    return _errorMessage;

                return _currentValue.ToString(_precision);
            }
        }

        public string CurrentExpression => _currentExpression.ToString();

        public bool HasError => _hasError;

        public CalculatorMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                _precision = value == CalculatorMode.Standard 
                    ? CalculatorPrecision.StandardModePrecision 
                    : CalculatorPrecision.ScientificModePrecision;
            }
        }

        /// <summary>
        /// Input a digit or decimal point
        /// </summary>
        public void InputDigit(string digit)
        {
            if (_hasError)
                return;

            if (_isNewCalculation)
            {
                _currentExpression.Clear();
                _isNewCalculation = false;
            }

            // Handle decimal point
            if (digit == "." && CurrentDisplay.Contains("."))
                return;

            var currentDisplay = CurrentDisplay;
            if (currentDisplay == "0" && digit != ".")
            {
                currentDisplay = digit;
            }
            else
            {
                currentDisplay += digit;
            }

            try
            {
                _currentValue = new HighPrecisionDecimal(currentDisplay);
            }
            catch (Exception)
            {
                SetError("Invalid input");
            }
        }

        /// <summary>
        /// Perform an operation
        /// </summary>
        public void PerformOperation(OperationType operation)
        {
            if (_hasError && operation != OperationType.Clear && operation != OperationType.ClearEntry)
                return;

            try
            {
                switch (operation)
                {
                    case OperationType.Clear:
                        Clear();
                        break;

                    case OperationType.ClearEntry:
                        ClearEntry();
                        break;

                    case OperationType.ChangeSign:
                        _currentValue = -_currentValue;
                        break;

                    case OperationType.Percent:
                        _currentValue = _currentValue / 100;
                        AppendToExpression("%");
                        break;

                    case OperationType.Sqrt:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            _currentValue = _currentValue.Sqrt();
                            AppendToExpression("sqrt(" + _currentValue.ToString(_precision) + ")");
                        }
                        break;

                    case OperationType.Reciprocal:
                        if (!_currentValue.IsZero)
                        {
                            _currentValue = HighPrecisionDecimal.One / _currentValue;
                            AppendToExpression("1/(" + _currentValue.ToString(_precision) + ")");
                        }
                        else
                        {
                            SetError("Cannot divide by zero");
                        }
                        break;

                    case OperationType.Sin:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            var value = Math.Sin((double)_currentValue);
                            _currentValue = new HighPrecisionDecimal(value);
                            AppendToExpression("sin(" + _currentValue.ToString(_precision) + ")");
                        }
                        break;

                    case OperationType.Cos:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            var value = Math.Cos((double)_currentValue);
                            _currentValue = new HighPrecisionDecimal(value);
                            AppendToExpression("cos(" + _currentValue.ToString(_precision) + ")");
                        }
                        break;

                    case OperationType.Tan:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            var value = Math.Tan((double)_currentValue);
                            _currentValue = new HighPrecisionDecimal(value);
                            AppendToExpression("tan(" + _currentValue.ToString(_precision) + ")");
                        }
                        break;

                    case OperationType.Log:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            if (_currentValue > HighPrecisionDecimal.Zero)
                            {
                                var value = Math.Log((double)_currentValue);
                                _currentValue = new HighPrecisionDecimal(value);
                                AppendToExpression("ln(" + _currentValue.ToString(_precision) + ")");
                            }
                            else
                            {
                                SetError("Invalid input for logarithm");
                            }
                        }
                        break;

                    case OperationType.Log10:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            if (_currentValue > HighPrecisionDecimal.Zero)
                            {
                                var value = Math.Log10((double)_currentValue);
                                _currentValue = new HighPrecisionDecimal(value);
                                AppendToExpression("log(" + _currentValue.ToString(_precision) + ")");
                            }
                            else
                            {
                                SetError("Invalid input for logarithm");
                            }
                        }
                        break;

                    case OperationType.Exp:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            var value = Math.Exp((double)_currentValue);
                            _currentValue = new HighPrecisionDecimal(value);
                            AppendToExpression("exp(" + _currentValue.ToString(_precision) + ")");
                        }
                        break;

                    case OperationType.Add:
                    case OperationType.Subtract:
                    case OperationType.Multiply:
                    case OperationType.Divide:
                    case OperationType.Power:
                        HandleBinaryOperation(operation);
                        break;

                    case OperationType.Equals:
                        CalculateResult();
                        break;
                }
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
        }

        private void HandleBinaryOperation(OperationType operation)
        {
            if (_pendingOperation != OperationType.None)
            {
                CalculateResult();
            }

            _previousValue = _currentValue;
            _pendingOperation = operation;
            
            // Append operator to expression
            string operatorSymbol = GetOperatorSymbol(operation);
            AppendToExpression(_currentValue.ToString(_precision) + " " + operatorSymbol + " ");
            
            _isNewCalculation = true;
        }

        private void CalculateResult()
        {
            if (_pendingOperation == OperationType.None)
                return;

            try
            {
                HighPrecisionDecimal result;

                switch (_pendingOperation)
                {
                    case OperationType.Add:
                        result = _previousValue + _currentValue;
                        break;

                    case OperationType.Subtract:
                        result = _previousValue - _currentValue;
                        break;

                    case OperationType.Multiply:
                        result = _previousValue * _currentValue;
                        break;

                    case OperationType.Divide:
                        if (_currentValue.IsZero)
                        {
                            SetError("Cannot divide by zero");
                            return;
                        }
                        result = _previousValue / _currentValue;
                        break;

                    case OperationType.Power:
                        if (_mode == CalculatorMode.Scientific)
                        {
                            var baseValue = (double)_previousValue;
                            var exponent = (double)_currentValue;
                            var value = Math.Pow(baseValue, exponent);
                            result = new HighPrecisionDecimal(value);
                        }
                        else
                        {
                            return;
                        }
                        break;

                    default:
                        return;
                }

                AppendToExpression(_currentValue.ToString(_precision));
                _currentValue = result;
                _pendingOperation = OperationType.None;
                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
        }

        private void Clear()
        {
            _currentValue = HighPrecisionDecimal.Zero;
            _previousValue = HighPrecisionDecimal.Zero;
            _pendingOperation = OperationType.None;
            _currentExpression.Clear();
            _isNewCalculation = true;
            _hasError = false;
            _errorMessage = null;
        }

        private void ClearEntry()
        {
            _currentValue = HighPrecisionDecimal.Zero;
            _hasError = false;
            _errorMessage = null;
        }

        private void SetError(string message)
        {
            _hasError = true;
            _errorMessage = message;
        }

        private void AppendToExpression(string text)
        {
            _currentExpression.Append(text);
        }

        private string GetOperatorSymbol(OperationType operation)
        {
            switch (operation)
            {
                case OperationType.Add: return "+";
                case OperationType.Subtract: return "−";
                case OperationType.Multiply: return "×";
                case OperationType.Divide: return "÷";
                case OperationType.Power: return "^";
                default: return "";
            }
        }

        /// <summary>
        /// Get the complete calculation expression with result for history
        /// </summary>
        public string GetCompleteExpression()
        {
            if (_currentExpression.Length == 0)
                return _currentValue.ToString(_precision);

            var expression = _currentExpression.ToString();
            if (!expression.EndsWith("="))
            {
                expression += " = " + _currentValue.ToString(_precision);
            }
            return expression;
        }

        /// <summary>
        /// Set the current value directly (for loading from history)
        /// </summary>
        public void SetCurrentValue(string value)
        {
            try
            {
                _currentValue = new HighPrecisionDecimal(value);
                _currentExpression.Clear();
                _currentExpression.Append(value);
                _isNewCalculation = true;
                _hasError = false;
                _errorMessage = null;
            }
            catch (Exception)
            {
                SetError("Invalid value format");
            }
        }
    }
}