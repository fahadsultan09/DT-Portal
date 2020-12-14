using System;

namespace Utility.HelperClasses
{
    public static class DataTypeHelper
    {
        public static Int32? TryParseToInt32(this string sourceValue)
        {
            if (Int32.TryParse(sourceValue, out var targetValue))
                return targetValue;
            return null;
        }

        public static string TryParseToString(this string sourceValue)
        {
            if (string.IsNullOrEmpty(sourceValue))
            {
                return "";
            }
            return sourceValue;
        }

        public static Int32? TryParseToInt32(this object sourceValue)
        {
            return TryParseToInt32(sourceValue.ToString());
        }

        public static double? TryParseToDouble(this string sourceValue)
        {
            if (double.TryParse(sourceValue, out var targetValue))
                return targetValue;
            return null;
        }

        public static Decimal? TryParseToDecimal(this string sourceValue)
        {
            if (Decimal.TryParse(sourceValue, out var targetValue))
                return targetValue;
            return null;
        }

        public static DateTime? TryParseToDateTime(this string sourceValue)
        {
            if (DateTime.TryParse(sourceValue, out var targetValue))
                return targetValue;
            return null;
        }

        public static long? TryParseToLong(this string sourceValue)
        {
            if (long.TryParse(sourceValue, out var targetValue))
                return targetValue;
            return null;
        }

        public static bool? TryParseToBool(this string sourceValue)
        {
            if (bool.TryParse(sourceValue, out var targetValue))
                return targetValue;
            return null;
        }

        public static Int32 ParseToInt32(this string sourceValue)
        {
            return TryParseToInt32(sourceValue) ?? 0;
        }

        public static Int32 ParseToInt32(this object sourceValue)
        {
            return TryParseToInt32(sourceValue.ToString()) ?? 0;
        }

        public static double ParseToDouble(this string sourceValue)
        {
            return TryParseToDouble(sourceValue) ?? 0;
        }

        public static Decimal ParseToDecimal(this string sourceValue)
        {
            return TryParseToDecimal(sourceValue) ?? 0;
        }

        public static DateTime ParseToDateTime(this string sourceValue)
        {
            return TryParseToDateTime(sourceValue) ?? DateTime.MinValue;
        }

        public static long ParseToLong(this string sourceValue)
        {
            return TryParseToLong(sourceValue) ?? 0;
        }

        public static bool ParseToBool(this string sourceValue)
        {
            return TryParseToBool(sourceValue) ?? false;
        }
    }
}
