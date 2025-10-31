using System.Data.Common;

namespace TicketingBE.DAL.Helpers
{
    /// <summary>
    /// Helper class for safe type conversion from database reader
    /// </summary>
    public static class TypeHelper
    {
        public static int GetInt32(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            
            return Convert.ToInt32(value);
        }

        public static int? GetNullableInt32(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return Convert.ToInt32(value);
        }

        public static long GetInt64(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            
            return Convert.ToInt64(value);
        }

        public static long? GetNullableInt64(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return Convert.ToInt64(value);
        }

        public static string GetString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            
            return value.ToString() ?? string.Empty;
        }

        public static string? GetNullableString(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return value.ToString();
        }

        public static bool GetBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;
            
            return Convert.ToBoolean(value);
        }

        public static bool? GetNullableBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return Convert.ToBoolean(value);
        }

        public static DateTime GetDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return DateTime.MinValue;
            
            return Convert.ToDateTime(value);
        }

        public static DateTime? GetNullableDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return Convert.ToDateTime(value);
        }

        public static Guid GetGuid(object value)
        {
            if (value == null || value == DBNull.Value)
                return Guid.Empty;
            
            if (value is Guid guid)
                return guid;
            
            return Guid.Parse(value.ToString() ?? string.Empty);
        }

        public static Guid? GetNullableGuid(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            if (value is Guid guid)
                return guid;
            
            return Guid.Parse(value.ToString() ?? string.Empty);
        }

        public static decimal GetDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            
            return Convert.ToDecimal(value);
        }

        public static decimal? GetNullableDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return Convert.ToDecimal(value);
        }

        public static byte[]? GetByteArray(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            return (byte[])value;
        }
    }
}
