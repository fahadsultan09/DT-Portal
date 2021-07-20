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
                LastYearMonth model = new LastYearMonth
                {
                    MonthName = dateTimeFormat.GetMonthName(iterator.Month),
                    Month = iterator.Month,
                    Year = iterator.Year,
                    LastYear = iterator.AddYears(-1).Year
                };
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
                return string.Format("{0:#,.##}K", amount - 5);

            if (amount < 100000)
                return string.Format("{0:#,.#}K", amount - 50);

            if (amount < 1000000)
                return string.Format("{0:#,.}K", amount - 500);

            if (amount < 10000000)
                return string.Format("{0:#,,.##}M", amount - 5000);

            if (amount < 100000000)
                return string.Format("{0:#,,.#}M", amount - 50000);

            if (amount < 1000000000)
                return string.Format("{0:#,,.}M", amount - 500000);

            return string.Format("{0:#,,,.##}B", amount - 5000000);
        }
        public static string TimeAgo(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return string.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return string.Format("about {0} {1} ago", months, months == 1 ? "month" : "months");
            }
            if (span.Days > 0)
                return string.Format("about {0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return string.Format("about {0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return string.Format("about {0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return string.Format("about {0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;
        }
        public static string ReadTextToFile(string FileName, string folderName)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                string path = @"D:\Arslan\JaredCRM_SVN\JaredCRM\";
                string folderPath = Path.Combine(path, folderName);
                string filePath = Path.Combine(folderPath, FileName);
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public static string WriteTextToFile(string Values, string folderName, string ExistingGuidFileName = null)
        {
            string folderPath = @"D:\Scheduler\Logs\" + folderName + "\\";

            folderPath = Path.Combine(folderPath, folderName);

            string guidValue = Guid.NewGuid().ToString() + ".txt";

            string filePath = Path.Combine(folderPath, guidValue);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!string.IsNullOrEmpty(Values))
            {
                do
                {
                    if (File.Exists(filePath))
                    {
                        guidValue = Guid.NewGuid().ToString() + ".txt";
                        filePath = Path.Combine(folderPath, guidValue);
                    }
                }
                while (File.Exists(filePath));
                File.WriteAllText(filePath, Values);

                if (!string.IsNullOrEmpty(ExistingGuidFileName))
                {
                    if (System.IO.File.Exists(Path.Combine(folderPath, ExistingGuidFileName)))
                    {
                        System.IO.File.Delete(Path.Combine(folderPath, ExistingGuidFileName));
                    }
                }

                return guidValue;
            }
            else
            {
                return null;
            }
        }
        public static void WriteToFile(string Message, string FolderName, string fileName)
        {
            string folderPath = @"D:\Scheduler\Logs\" + FolderName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filepath = folderPath + "\\" + fileName;
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using StreamWriter sw = File.CreateText(filepath);
                sw.WriteLine(Message);
            }
            else
            {
                using StreamWriter sw = File.AppendText(filepath);
                sw.WriteLine(Message);
            }
        }
    }
}
