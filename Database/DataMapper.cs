using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Linq;

namespace FI.Foundation
{
    /// <summary>
    /// DataMapper helps you to easily work with ADO.NET and overcome some performance issues
    /// that you might face when you want to read columns from DataReader using column names.
    /// Also if you are dealing with results from several tables and you want to read columns
    /// using a prefix in the name, you can use this.
    /// <example>
    /// var mapper = new DataMapper(dr);
    /// dr.SetPrefix('User');
    /// var uid = dr.GetGuid("Id"); // it will read column "User_Id"
    /// </example>
    /// </summary>
    public class DataMapper: IDisposable
    {
        IDictionary<string, int> dictionary = null;
        System.Data.SqlClient.SqlDataReader _dr = null;
        string _Prefix = null;

        public void SetPrefix(string p)
        {
            _Prefix = p;
        }

        public void ResetPrefix()
        {
            _Prefix = null;
        }


        public string[] DefinedColumns
        {
            get
            {
                if (dictionary != null)
                {
                    return dictionary.Keys.ToArray();
                }
                return null;
            }
        }

        public DataMapper(SqlDataReader dr)
        {
            dictionary = new SortedDictionary<string, int>();
            _dr = dr;
            for (int i = 0, ii = dr.FieldCount; i < ii; i++)
            {
                dictionary.Add(dr.GetName(i), i);
            }
        }

        int GetColumnIndex(string name)
        {
            var k = _Prefix == null ? name : _Prefix + name;
            if (dictionary.ContainsKey(k))
                return dictionary[k];
            return -1;
        }

        public bool IsColumnDefined(string name)
        {
            return GetColumnIndex(name) > -1;
        }

        #region INT
        public int GetInt(string name)
        {
            return GetInt(name, 0);
        }
        public int GetInt(string name, int defaultValue)
        {
            return GetIntNullable(name, null) ?? defaultValue;
        }
        public int? GetIntNullable(string name)
        {
            return GetIntNullable(name, null);
        }
        public int? GetIntNullable(string name, int? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlInt32(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region LONG
        public long GetLong(string name)
        {
            return GetLong(name, 0);
        }
        public long GetLong(string name, long defaultValue)
        {
            return GetLongNullable(name, null) ?? defaultValue;
        }
        public long? GetLongNullable(string name)
        {
            return GetLongNullable(name, null);
        }
        public long? GetLongNullable(string name, long? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlInt64(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region String
        public string GetString(string name)
        {
            return GetString(name, null);
        }
        public string GetString(string name, string defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlString(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region DateTime
        public DateTime GetDateTime(string name)
        {
            return GetDateTime(name, DateTime.MinValue);
        }
        public DateTime GetDateTime(string name, DateTime defaultValue)
        {
            return GetDateTimeNullable(name, null) ?? defaultValue;
        }
        public DateTime? GetDateTimeNullable(string name)
        {
            return GetDateTimeNullable(name, null);
        }
        public DateTime? GetDateTimeNullable(string name, DateTime? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlDateTime(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region Guid
        public Guid GetGuid(string name)
        {
            return GetGuid(name, Guid.Empty);
        }
        public Guid GetGuid(string name, Guid defaultValue)
        {
            return GetGuidNullable(name, null) ?? defaultValue;
        }
        public Guid? GetGuidNullable(string name)
        {
            return GetGuidNullable(name, null);
        }
        public Guid? GetGuidNullable(string name, Guid? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlGuid(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region Boolean
        public bool GetBoolean(string name)
        {
            return GetBoolean(name, false);
        }
        public bool GetBoolean(string name, bool defaultValue)
        {
            return GetBooleanNullable(name, null) ?? defaultValue;
        }
        public bool? GetBooleanNullable(string name)
        {
            return GetBooleanNullable(name, null);
        }
        public bool? GetBooleanNullable(string name,bool? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlBoolean(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region Char
        public char GetChar(string name)
        {
            return GetChar(name, '\0');
        }
        public char GetChar(string name, char defaultValue)
        {
            return GetCharNullable(name, null) ?? defaultValue;
        }
        public char? GetCharNullable(string name)
        {
            return GetCharNullable(name, null);
        }
        public char? GetCharNullable(string name, char? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlChars(index);
                if (!v.IsNull && v.Length == 1) return v.Value[0];
            }
            return defaultValue;
        }
        #endregion

        #region Decimal
        public decimal GetDecimal(string name)
        {
            return GetDecimal(name, 0);
        }
        public decimal GetDecimal(string name, decimal defaultValue)
        {
            return GetDecimalNullable(name, null) ?? defaultValue;
        }
        public decimal? GetDecimalNullable(string name)
        {
            return GetDecimalNullable(name, null);
        }
        public decimal? GetDecimalNullable(string name, decimal? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlDecimal(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region Double
        public double GetDouble(string name)
        {
            return GetDouble(name,0);
        }
        public double GetDouble(string name, double defaultValue)
        {
            return GetDoubleNullable(name, null) ?? defaultValue;
        }
        public double? GetDoubleNullable(string name)
        {
            return GetDoubleNullable(name, null);
        }
        public double? GetDoubleNullable(string name, double? defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlDouble(index);
                if (!v.IsNull) return v.Value;
            }
            return defaultValue;
        }
        #endregion

        #region XML
        public XElement GetXml(string name)
        {
            return GetXml(name, true, null);
        }
        public XElement GetXml(string name, bool includeRoot)
        {
            return GetXml(name, includeRoot, null);
        }
        public XElement GetXml(string name, bool includeRoot, XElement defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlXml(index);
                if (!v.IsNull)
                {
                    var xml = v.Value;
                    if (includeRoot)
                    {
                        xml = string.Concat("<Root>", xml, "</Root>");
                    }
                    return XElement.Parse(xml);
                }
            }
            return defaultValue;
        }
        #endregion

        #region Object
        public object GetObject(string name)
        {
            return GetObject(name, null);
        }
        public object GetObject(string name, object defaultValue)
        {
            int index = GetColumnIndex(name);
            if (index >= 0)
            {
                var v = _dr.GetSqlValue(index);
                if (v != null)
                {
                    return v;
                }
            }
            return defaultValue;
        }
        #endregion

        public void Dispose()
        {
            if (_dr != null) _dr = null;
            if (dictionary != null) { dictionary.Clear(); dictionary = null; }
        }
    }
}
