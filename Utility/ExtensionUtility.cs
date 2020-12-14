using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

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
    }
}
