using System.ComponentModel;

namespace System
{
    public static class ObjectExtensions
    {
        public static T ValueOrDefault<T>(this T value)
        {
            return value == null ? default : value;
        }

        public static T ParseOrDefault<T>(this object value, T @default = default)
        {
            if (value == null)
            {
                return @default;
            }

            try
            {
                var castType = typeof(T);
                var convertType = Nullable.GetUnderlyingType(castType) ?? castType;
                return (T)Convert.ChangeType(value, convertType);
            }
            catch
            {
                return @default;
            }
        }

        public static object ChangeType(this object value, Type type)
        {
            if (value == null)
            {
                return default;
            }

            if (value.GetType() == type)
            {
                return value;
            }

            // NOTE: The DateTimeOffset hack is to deal with the QueueTrigger's deserialization
            // of the DateTime objects which is converting them to DateTimeOffset's. Once a 
            // method of fixing the deserialization can be identified this approach can then
            // be undone.
            if (type == typeof(DateTime) && value is DateTimeOffset offset)
            {
                return offset.DateTime;
            }

            var typeOrUnderlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (typeOrUnderlyingType.IsEnum && value.IsWholeNumber())
            {
                return Enum.ToObject(typeOrUnderlyingType, value);
            }

            var converter = TypeDescriptor.GetConverter(type);

            return converter.CanConvertFrom(value.GetType()) ?
                converter.ConvertFrom(value) :
                Convert.ChangeType(value, typeOrUnderlyingType);
        }

        public static T ChangeType<T>(this object value)
        {
            return (T)value.ChangeType(typeof(T));
        }

        public static bool IsNumber(this object value)
        {
            return value.IsWholeNumber() || value.IsRealNumber();
        }

        public static bool IsWholeNumber(this object value)
        {
            return
                value is byte ||
                value is short ||
                value is ushort ||
                value is int ||
                value is uint ||
                value is long ||
                value is ulong;
        }

        public static bool IsRealNumber(this object value)
        {
            return
                value is float ||
                value is double ||
                value is decimal;
        }
    }
}
