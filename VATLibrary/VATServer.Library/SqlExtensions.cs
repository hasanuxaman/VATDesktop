using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace VATServer.Library
{
    public static class SqlExtensions
    {
        public static void AddWithValueAndNullHandle(this SqlParameterCollection collection, string parameterName, object value)
        {
            if (value == null)
            {
                collection.AddWithValue(parameterName, DBNull.Value);
            }
            else
            {
                collection.AddWithValue(parameterName, value);
            }
        }
        public static int WordCount(this string str,string value)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }
        public static bool IsGreaterThan(this int i)
        {
            return true;
        }


        public static void AddWithValueAndParamCheck(this SqlParameterCollection collection, string parameterName, object value)
        {
            if (collection.Contains(parameterName))
            {
                collection[parameterName].Value = value;
            }
            else
            {
                collection.AddWithValue(parameterName, value);
            }
        }
    }


    public static class StringExtentions
    {
        public static string ToUpperFirstLetter(this string source)
        {
            char[] letters = source.ToCharArray();

            try
            {
                if (string.IsNullOrEmpty(source))
                    return string.Empty;
                // convert to char array of the string
                // upper case the first char
                letters[0] = char.ToUpper(letters[0]);
                // return the array made of the new char array
                return new string(letters);
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }


    public static class DataTableExtensions
    {
        public static string GetCommaSeparated(this DataTable source, string columnName)
        {
            var list = source.AsEnumerable().Select(r => r[columnName].ToString());
            string value = string.Join("','", list);

            return "'" + value + "'";
        }


        public static List<T> ToList<T>(this DataTable source)
        {
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(source));
        }

        public static string GetColumns(this DataTable table)
        {
            string columnNames = "";

            foreach (DataColumn tableColumn in table.Columns)
            {
                columnNames += tableColumn.ColumnName + ",";
            }

            return columnNames.Trim(',');
        }
      
    }


}
