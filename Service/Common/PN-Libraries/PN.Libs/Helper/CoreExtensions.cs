using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace PN.SmartLib.Helper
{
    public static class CoreExtensions
    {

        /// <summary>
        /// Read data from datarow to hash table
        /// </summary>
        /// <param name="raw">data row form reader</param>
        /// <returns>hash table with data</returns>
        public static Hashtable ToHashtable(this DataRowView raw)
        {
            var hashtable = new Hashtable();
            foreach (DataColumn col in raw.Row.Table.Columns)
            {
                hashtable[col.ColumnName] = ToString(raw[col.ColumnName]);
            }

            return hashtable;
        }

        /// <summary>
        /// /Read datas from data view to hash tables
        /// </summary>
        /// <param name="dataView">data view form reader</param>
        /// <returns>list hash table with data</returns>
        public static Hashtable[] ToHashtableArray(this DataView dataView)
        {
            return (from DataRowView row in dataView
                    select row.ToHashtable()).ToArray();
        }

        /// <summary>
        /// To string for object
        /// return null or empty object
        /// </summary>
        /// <param name="value">string of object</param>
        /// <returns></returns>
        public static string ToString(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            return value.ToString();
        }
        public static T GetEnumValueByDescription<T>(this string description) where T : Enum
        {
            foreach (Enum enumItem in Enum.GetValues(typeof(T)))
            {
                if (enumItem.GetDescription() == description)
                {
                    return (T)enumItem;
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
        }
        public static string GetDescription(this Enum enumdata)
        {
            return (enumdata.GetType().GetTypeInfo().GetMember(enumdata.ToString())
                .FirstOrDefault(x => x.MemberType == MemberTypes.Field).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)
                .SingleOrDefault() as DescriptionAttribute)?.Description ?? enumdata.ToString();
        }
    }
}
