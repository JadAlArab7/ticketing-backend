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

        /// <summary>
        /// Converts a GUID value from the database to a string
        /// </summary>
        public static string GetGuidAsString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            
            if (value is Guid guid)
                return guid.ToString();
            
            return value.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Converts a nullable GUID value from the database to a nullable string
        /// </summary>
        public static string? GetNullableGuidAsString(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            
            if (value is Guid guid)
                return guid.ToString();
            
            return value.ToString();
        }

        // DbDataReader overloads

        public static int GetInt32(DbDataReader reader, string columnName)
        {
            return GetInt32(reader[columnName]);
        }

        public static int? GetNullableInt32(DbDataReader reader, string columnName)
        {
            return GetNullableInt32(reader[columnName]);
        }

        public static long GetInt64(DbDataReader reader, string columnName)
        {
            return GetInt64(reader[columnName]);
        }

        public static long? GetNullableInt64(DbDataReader reader, string columnName)
        {
            return GetNullableInt64(reader[columnName]);
        }

        public static string GetString(DbDataReader reader, string columnName)
        {
            return GetString(reader[columnName]);
        }

        public static string? GetNullableString(DbDataReader reader, string columnName)
        {
            return GetNullableString(reader[columnName]);
        }

        public static bool GetBoolean(DbDataReader reader, string columnName)
        {
            return GetBoolean(reader[columnName]);
        }

        public static bool? GetNullableBoolean(DbDataReader reader, string columnName)
        {
            return GetNullableBoolean(reader[columnName]);
        }

        public static DateTime GetDateTime(DbDataReader reader, string columnName)
        {
            return GetDateTime(reader[columnName]);
        }

        public static DateTime? GetNullableDateTime(DbDataReader reader, string columnName)
        {
            return GetNullableDateTime(reader[columnName]);
        }

        public static Guid GetGuid(DbDataReader reader, string columnName)
        {
            return GetGuid(reader[columnName]);
        }

        public static Guid? GetNullableGuid(DbDataReader reader, string columnName)
        {
            return GetNullableGuid(reader[columnName]);
        }

        public static decimal GetDecimal(DbDataReader reader, string columnName)
        {
            return GetDecimal(reader[columnName]);
        }

        public static decimal? GetNullableDecimal(DbDataReader reader, string columnName)
        {
            return GetNullableDecimal(reader[columnName]);
        }

        public static byte[]? GetByteArray(DbDataReader reader, string columnName)
        {
            return GetByteArray(reader[columnName]);
        }

        public static string GetGuidAsString(DbDataReader reader, string columnName)
        {
            return GetGuidAsString(reader[columnName]);
        }

        public static string? GetNullableGuidAsString(DbDataReader reader, string columnName)
        {
            return GetNullableGuidAsString(reader[columnName]);
        }
    }
}
