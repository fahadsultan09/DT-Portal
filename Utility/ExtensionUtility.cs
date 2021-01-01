using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Utility.Constant;

namespace Utility
{
    public static class ExtensionUtility
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable ConvertToDataTable(string fullpath, string[] Columns, DataTable tbl)
        {
            var sr = new StreamReader(fullpath);
            string fullFile = sr.ReadToEnd().Trim();
            sr.Close();
            fullFile = fullFile.Replace("\r\n", "~");
            fullFile = fullFile.Replace("#$#", "|").Replace("#&#", "|");

            foreach (string line in fullFile.Split('~'))
            {
                var cols = line.Split("|".ToCharArray());
                if ((Columns.Length - 1) == cols.Length)
                {
                    DataRow dr = tbl.NewRow();

                    for (int cIndex = 0; cIndex < cols.Length; cIndex++)
                    {
                        dr[cIndex] = cols[cIndex].Trim();
                    }

                    tbl.Rows.Add(dr);
                }

            }
            return tbl;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
        public static Tuple<List<LastYearMonth>> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;
            List<LastYearMonth> list = new List<LastYearMonth>();
            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator < limit)
            {
                LastYearMonth model = new LastYearMonth();
                model.MonthName = dateTimeFormat.GetMonthName(iterator.Month);
                model.Month = iterator.Month;
                model.Year = iterator.Year;
                model.LastYear = iterator.AddYears(-1).Year;
                list.Add(model);
                iterator = iterator.AddMonths(1);
            }
            return Tuple.Create(list);
        }
        public static string FormatNumberAmount(double amount)
        {
            if (amount < 1000)
                return amount.ToString();

            if (amount < 10000)
                return String.Format("{0:#,.##}K", amount - 5);

            if (amount < 100000)
                return String.Format("{0:#,.#}K", amount - 50);

            if (amount < 1000000)
                return String.Format("{0:#,.}K", amount - 500);

            if (amount < 10000000)
                return String.Format("{0:#,,.##}M", amount - 5000);

            if (amount < 100000000)
                return String.Format("{0:#,,.#}M", amount - 50000);

            if (amount < 1000000000)
                return String.Format("{0:#,,.}M", amount - 500000);

            return String.Format("{0:#,,,.##}B", amount - 5000000);
        }
    }
}
