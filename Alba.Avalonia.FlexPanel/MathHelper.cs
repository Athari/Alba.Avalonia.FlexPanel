namespace Alba.Avalonia.FlexPanel;

internal static class MathHelper
{
    internal const double DoubleEpsilon = 2.2204460492503131e-016;

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public static bool AreClose(double value1, double value2) => value1 == value2 || IsVerySmall(value1 - value2);
    public static double Lerp(double x, double y, double alpha) => x * (1.0 - alpha) + y * alpha;
    public static bool IsVerySmall(double value) => Math.Abs(value) < 1E-06;
    public static bool IsZero(double value) => Math.Abs(value) < 10.0 * DoubleEpsilon;
    public static bool IsFiniteDouble(double x) => !double.IsInfinity(x) && !double.IsNaN(x);
    public static double DoubleFromMantissaAndExponent(double x, int exp) => x * Math.Pow(2.0, exp);
    public static bool GreaterThan(double value1, double value2) => value1 > value2 && !AreClose(value1, value2);
    public static bool GreaterThanOrClose(double value1, double value2) => !(value1 <= value2) || AreClose(value1, value2);
    public static double Hypotenuse(double x, double y) => Math.Sqrt(x * x + y * y);
    public static bool LessThan(double value1, double value2) => value1 < value2 && !AreClose(value1, value2);
    public static bool LessThanOrClose(double value1, double value2) => !(value1 >= value2) || AreClose(value1, value2);
    public static double SafeDivide(double lhs, double rhs, double fallback) => !IsVerySmall(rhs) ? lhs / rhs : fallback;
    public static double EnsureRange(double value, double? min, double? max) => value < min ? min.Value : value > max ? max.Value : value;
}