using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATServer.Library
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string ToDateString(this string value, string format = "yyyy-MM-dd")
        {
            return Convert.ToDateTime(value).ToString(format);
        }
    }


    public static class ObjectExtensions
    {
        public static string ToDateString(this object row, string format = "dd/MM/yyyy")
        {
            try
            {

                return Convert.ToDateTime(row).ToString(format);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string ToDecimalString(this object row, string format = "0.##")
        {
            return row == DBNull.Value ? null : Convert.ToDecimal(row).ToString(format);
        }
    }
}
