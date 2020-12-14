using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Utility.HelperClasses
{
    public static class MYSQLDB
    {

        #region Properties
        public static string getConnectionString { get { return ConfigurationManager.ConnectionStrings["FMSDbContext"].ConnectionString; } }


        private static MySqlDataAdapter DataAdapter { set; get; }
        public static MySqlConnection MySqlConnection { set; get; }

        #endregion

        #region Constructor
        static MYSQLDB()
        {
            DataAdapter = new MySqlDataAdapter();
            // MySqlConnection = new MySqlConnection(getConnectionString);
        }


        #endregion

        #region Build Connection String
        private static MySqlConnection MakeConnectionOpen()
        {

            if (MySqlConnection.State == ConnectionState.Closed || MySqlConnection.State == ConnectionState.Broken)
            {
                MySqlConnection.Open();
            }
            return MySqlConnection;

        }

        #endregion 

        #region Insert by Query
        public static bool ExecuteInsertQuery(string runQuery, MySqlParameter[] parameters)
        {
            var isExecuted = true;
            var MySqlCommand = new MySqlCommand();
            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();
                MySqlCommand.CommandText = runQuery;
                MySqlCommand.Parameters.Clear();
                if (parameters != null)
                {
                    MySqlCommand.Parameters.AddRange(parameters);
                }

                DataAdapter.InsertCommand = MySqlCommand;
                MySqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                isExecuted = false;
            }
            return isExecuted;
        }

        public static bool ExecuteInsertQuery(string RunQuery)
        {
            return ExecuteInsertQuery(RunQuery, null);
        }
        #endregion

        #region Insert by SP's
        public static bool ExecuteInsertSP(string spName, MySqlParameter[] parameters)
        {
            var isExecuted = false;
            var command = new MySqlCommand();
            try
            {
                command.CommandText = spName;
                command.Connection = MakeConnectionOpen();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
                isExecuted = true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            return isExecuted;
        }

        public static bool ExecuteInsertSP(string SPName)
        {
            return ExecuteInsertSP(SPName, null);
        }
        #endregion

        #region Delete by Query
        public static bool ExecuteDeleteQuery(string runQuery, MySqlParameter[] parameters)
        {
            var isExecuted = true;
            var MySqlCommand = new MySqlCommand();
            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();
                MySqlCommand.CommandText = runQuery;
                if (parameters != null) { MySqlCommand.Parameters.AddRange(parameters); }

                DataAdapter.DeleteCommand = MySqlCommand;
                MySqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException) { isExecuted = false; }
            return isExecuted;
        }

        public static bool ExecuteDeleteQuery(string RunQuery)
        {
            return ExecuteDeleteQuery(RunQuery, null);
        }
        #endregion

        #region Delete by SP's
        public static bool ExecuteDeleteSP(string spName, MySqlParameter[] parameters)
        {
            var isExecuted = true;
            var MySqlCommand = new MySqlCommand();
            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();

                MySqlCommand.CommandText = spName;
                MySqlCommand.CommandType = CommandType.StoredProcedure;

                MySqlCommand.Parameters.AddRange(parameters);

                MySqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException) { isExecuted = false; }
            return isExecuted;
        }

        public static bool ExecuteDeleteSP(string SPName)
        {
            return ExecuteDeleteSP(SPName, null);
        }
        #endregion 

        #region Update by Query
        public static bool ExecuteUpdateQuery(string runQuery, MySqlParameter[] parameters)
        {
            var isExecuted = true;
            var MySqlCommand = new MySqlCommand();
            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();
                MySqlCommand.CommandText = runQuery;
                MySqlCommand.Parameters.Clear();
                if (parameters != null)
                {
                    MySqlCommand.Parameters.AddRange(parameters);
                }

                DataAdapter.UpdateCommand = MySqlCommand;
                MySqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException) { isExecuted = false; }
            return isExecuted;
        }

        public static bool ExecuteUpdateQuery(string RunQuery)
        {
            return ExecuteUpdateQuery(RunQuery, null);
        }
        #endregion

        #region Update by SP's
        public static bool ExecuteUpdateSP(string spName, MySqlParameter[] parameters)
        {
            var isExecuted = true;
            var MySqlCommand = new MySqlCommand();
            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();

                MySqlCommand.CommandText = spName;
                MySqlCommand.CommandType = CommandType.StoredProcedure;

                MySqlCommand.Parameters.AddRange(parameters);

                MySqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException) { isExecuted = false; }
            return isExecuted;
        }

        public static bool ExecuteUpdateSP(string spName)
        {
            return ExecuteUpdateSP(spName, null);
        }
        #endregion

        #region Return DataSet


        public static DataSet ReturnDataSet(string RunQuery, MySqlParameter[] Parameters)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet DataSets = new DataSet();
            try
            {
                command.Connection = MakeConnectionOpen();
                command.CommandText = RunQuery;
                if (Parameters != null && Parameters.Any())
                {
                    command.Parameters.AddRange(Parameters);
                }
                DataAdapter.SelectCommand = command;
                DataAdapter.Fill(DataSets);
            }

            catch (MySqlException)
            {

            }

            return DataSets;
        }


        public static DataSet ReturnDataSet(string RunQuery)
        {
            return ReturnDataSet(RunQuery, null);
        }


        #endregion

        #region Return DataSet BY SP


        public static DataSet ReturnDataSetSP(string sp, MySqlParameter[] Parameters)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet DataSets = new DataSet();
            try
            {
                command.Connection = MakeConnectionOpen();
                command.CommandText = sp;
                command.CommandType = CommandType.StoredProcedure;
                if (Parameters != null && Parameters.Any())
                {
                    command.Parameters.AddRange(Parameters);
                }
                DataAdapter.SelectCommand = command;
                DataAdapter.Fill(DataSets);
            }

            catch (MySqlException)
            {

            }

            return DataSets;
        }


        public static DataSet ReturnDataSetSP(string RunQuery)
        {
            return ReturnDataSet(RunQuery, null);
        }


        #endregion

        #region Return Data table by Query

        public static DataTable ReturnDataTabebyQuery(string runQuery, MySqlParameter[] parameters)
        {
            MySqlCommand MySqlCommand = new MySqlCommand();
            DataTable Datatable = new DataTable();

            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();
                MySqlCommand.CommandText = runQuery;
                MySqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Any())
                {
                    MySqlCommand.Parameters.AddRange(parameters);
                }
                DataAdapter.SelectCommand = MySqlCommand;
                DataAdapter.Fill(Datatable);
            }

#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (MySqlException ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
            }
            return Datatable;
        }


        public static DataSet ReturnDatasetbyQuery(string runQuery, MySqlParameter[] parameters)
        {
            MySqlCommand MySqlCommand = new MySqlCommand();
            DataSet Datatable = new DataSet();

            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();
                MySqlCommand.CommandText = runQuery;
                MySqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Any())
                {
                    MySqlCommand.Parameters.AddRange(parameters);
                }
                DataAdapter.SelectCommand = MySqlCommand;
                DataAdapter.TableMappings.Add("dual", "dual");
                DataAdapter.Fill(Datatable);
            }

#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (MySqlException ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
            }
            return Datatable;
        }



        public static DataTable ReturnDataTabebyQuery(string runQuery)
        {
            return ReturnDataTabebyQuery(runQuery, null);
        }

        public static DataSet ReturnDatasetbyQuery(string runQuery)
        {
            return ReturnDatasetbyQuery(runQuery, null);
        }

        #endregion 

        #region Return Data table by SP

        public static DataTable ReturnDataTabebySP(string sp, MySqlParameter[] parameters)
        {
            MySqlCommand MySqlCommand = new MySqlCommand();
            DataTable Datatable = new DataTable();

            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();

                MySqlCommand.CommandText = sp;
                MySqlCommand.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Any())
                {
                    MySqlCommand.Parameters.AddRange(parameters);
                }

                DataAdapter.SelectCommand = MySqlCommand;
                DataAdapter.Fill(Datatable);
            }

            catch (MySqlException)
            {

            }
            return Datatable;
        }

        public static DataTable ReturnDataTabebySP(string sp)
        {
            return ReturnDataTabebySP(sp, null);

        }
        #endregion

        #region Return One Value
        public static string ReturnOneValue(string runQuery, MySqlParameter[] parameters)
        {
            string strValue = "";
            MySqlCommand MySqlCommand = new MySqlCommand();
            try
            {
                MySqlCommand.Connection = MakeConnectionOpen();
                MySqlCommand.CommandText = runQuery;
                if (parameters != null && parameters.Any())
                {
                    MySqlCommand.Parameters.AddRange(parameters);
                }
                strValue = MySqlCommand.ExecuteScalar().ToString();
            }
            catch (System.Exception)
            {

            }
            return strValue;
        }
        public static string ReturnOneValue(string runQuery)
        {
            return ReturnOneValue(runQuery, null);
        }


        #endregion

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

        public static DataTable ConvertToDataTable(string fullFile, string[] Columns)
        {
            DataTable tbl = new DataTable();
            for (int col = 0; col < Columns.Length; col++)
            {
                tbl.Columns.Add(new DataColumn(Columns[col]));
            }

            foreach (string line in fullFile.Split('~'))
            {
                var cols = line.Split("|".ToCharArray());
                DataRow dr = tbl.NewRow();

                for (int cIndex = 0; cIndex < cols.Length; cIndex++)
                {
                    dr[cIndex] = cols[cIndex];
                }

                tbl.Rows.Add(dr);
            }
            return tbl;
        }

        public static MySqlParameter[] GetParameters(string ParamName, string ParamValue)
        {
            MySqlParameter[] mySqlParameters = { new MySqlParameter() { ParameterName = ParamName, Value = ParamValue } };
            return mySqlParameters;
        }
    }
}