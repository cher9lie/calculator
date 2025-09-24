using System;
using System.Globalization;
using System.Numerics;

namespace WindowsFormsCalculator
{
    /// <summary>
    /// High-precision decimal implementation using BigInteger for lossless arithmetic
    /// Preserves precision for all basic calculations, inspired by the C++ Rational class
    /// </summary>
    public struct HighPrecisionDecimal : IEquatable<HighPrecisionDecimal>, IComparable<HighPrecisionDecimal>
    {
        private readonly BigInteger _numerator;
        private readonly BigInteger _denominator;
        private const int DEFAULT_PRECISION = 32;

        public HighPrecisionDecimal(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException("Denominator cannot be zero");

            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            var gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
            _numerator = numerator / gcd;
            _denominator = denominator / gcd;
        }

        public HighPrecisionDecimal(decimal value) : this(value.ToString(CultureInfo.InvariantCulture))
        {
        }

        public HighPrecisionDecimal(double value) : this(value.ToString("R", CultureInfo.InvariantCulture))
        {
        }

        public HighPrecisionDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or empty");

            value = value.Trim();

            // Handle scientific notation
            if (value.Contains("E") || value.Contains("e"))
            {
                var parts = value.Split(new[] { 'E', 'e' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    throw new FormatException("Invalid scientific notation format");

                var mantissa = new HighPrecisionDecimal(parts[0]);
                var exponent = int.Parse(parts[1]);
                var power = BigInteger.Pow(10, Math.Abs(exponent));

                if (exponent >= 0)
                {
                    _numerator = mantissa._numerator * power;
                    _denominator = mantissa._denominator;
                }
                else
                {
                    _numerator = mantissa._numerator;
                    _denominator = mantissa._denominator * power;
                }

                var gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(_numerator), _denominator);
                _numerator /= gcd;
                _denominator /= gcd;
                return;
            }

            if (value.Contains("."))
            {
                var decimalParts = value.Split('.');
                if (decimalParts.Length != 2)
                    throw new FormatException("Invalid decimal format");

                var wholePart = BigInteger.Parse(decimalParts[0]);
                var fractionalPart = decimalParts[1];
                var denominator = BigInteger.Pow(10, fractionalPart.Length);
                var fractionalValue = BigInteger.Parse(fractionalPart);

                _numerator = wholePart * denominator + (wholePart < 0 ? -fractionalValue : fractionalValue);
                _denominator = denominator;
            }
            else
            {
                _numerator = BigInteger.Parse(value);
                _denominator = BigInteger.One;
            }

            var finalGcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(_numerator), _denominator);
            _numerator /= finalGcd;
            _denominator /= finalGcd;
        }

        public static HighPrecisionDecimal Zero => new HighPrecisionDecimal(BigInteger.Zero, BigInteger.One);
        public static HighPrecisionDecimal One => new HighPrecisionDecimal(BigInteger.One, BigInteger.One);

        public bool IsZero => _numerator == 0;
        public bool IsOne => _numerator == _denominator;

        // Arithmetic operators
        public static HighPrecisionDecimal operator +(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            var numerator = left._numerator * right._denominator + right._numerator * left._denominator;
            var denominator = left._denominator * right._denominator;
            return new HighPrecisionDecimal(numerator, denominator);
        }

        public static HighPrecisionDecimal operator -(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            var numerator = left._numerator * right._denominator - right._numerator * left._denominator;
            var denominator = left._denominator * right._denominator;
            return new HighPrecisionDecimal(numerator, denominator);
        }

        public static HighPrecisionDecimal operator *(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return new HighPrecisionDecimal(left._numerator * right._numerator, left._denominator * right._denominator);
        }

        public static HighPrecisionDecimal operator /(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            if (right.IsZero)
                throw new DivideByZeroException("Cannot divide by zero");

            return new HighPrecisionDecimal(left._numerator * right._denominator, left._denominator * right._numerator);
        }

        public static HighPrecisionDecimal operator -(HighPrecisionDecimal value)
        {
            return new HighPrecisionDecimal(-value._numerator, value._denominator);
        }

        // Comparison operators
        public static bool operator ==(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return left._numerator * right._denominator == right._numerator * left._denominator;
        }

        public static bool operator !=(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return !(left == right);
        }

        public static bool operator <(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return left._numerator * right._denominator < right._numerator * left._denominator;
        }

        public static bool operator >(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return left._numerator * right._denominator > right._numerator * left._denominator;
        }

        public static bool operator <=(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return left._numerator * right._denominator <= right._numerator * left._denominator;
        }

        public static bool operator >=(HighPrecisionDecimal left, HighPrecisionDecimal right)
        {
            return left._numerator * right._denominator >= right._numerator * left._denominator;
        }

        // Conversion operators
        public static implicit operator HighPrecisionDecimal(int value)
        {
            return new HighPrecisionDecimal(value, 1);
        }

        public static implicit operator HighPrecisionDecimal(decimal value)
        {
            return new HighPrecisionDecimal(value);
        }

        public static implicit operator HighPrecisionDecimal(double value)
        {
            return new HighPrecisionDecimal(value);
        }

        public static explicit operator decimal(HighPrecisionDecimal value)
        {
            return (decimal)((double)value._numerator / (double)value._denominator);
        }

        public static explicit operator double(HighPrecisionDecimal value)
        {
            return (double)value._numerator / (double)value._denominator;
        }

        // Mathematical functions
        public HighPrecisionDecimal Abs()
        {
            return new HighPrecisionDecimal(BigInteger.Abs(_numerator), _denominator);
        }

        public HighPrecisionDecimal Sqrt()
        {
            if (_numerator < 0)
                throw new ArgumentException("Cannot compute square root of negative number");

            if (IsZero)
                return Zero;

            // Newton's method for square root with high precision
            var x = new HighPrecisionDecimal(Math.Sqrt((double)this));
            var two = new HighPrecisionDecimal(2);

            for (int i = 0; i < 50; i++) // More iterations for higher precision
            {
                var newX = (x + this / x) / two;
                if ((newX - x).Abs() < new HighPrecisionDecimal(1E-30))
                    break;
                x = newX;
            }

            return x;
        }

        public override string ToString()
        {
            return ToString(DEFAULT_PRECISION);
        }

        public string ToString(int precision)
        {
            if (_denominator == 1)
                return _numerator.ToString();

            if (precision <= 0)
                precision = DEFAULT_PRECISION;

            // Perform long division for specified precision
            var quotient = _numerator / _denominator;
            var remainder = BigInteger.Abs(_numerator % _denominator);

            if (remainder == 0)
                return quotient.ToString();

            var result = quotient.ToString();
            result += ".";

            for (int i = 0; i < precision && remainder != 0; i++)
            {
                remainder *= 10;
                var digit = remainder / _denominator;
                result += digit.ToString();
                remainder %= _denominator;
            }

            // Remove trailing zeros
            return result.TrimEnd('0').TrimEnd('.');
        }

        public bool Equals(HighPrecisionDecimal other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is HighPrecisionDecimal other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _numerator.GetHashCode() ^ _denominator.GetHashCode();
        }

        public int CompareTo(HighPrecisionDecimal other)
        {
            if (this < other) return -1;
            if (this > other) return 1;
            return 0;
        }
    }
}