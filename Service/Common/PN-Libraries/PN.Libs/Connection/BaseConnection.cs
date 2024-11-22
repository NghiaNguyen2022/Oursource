using PN.SmartLib.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PN.SmartLib.Connection
{
    /// <summary>
    /// This class subcribe the ADO.Net working for any database type
    /// </summary>
    public abstract class BaseConnection : IDisposable
    {
        protected string ConnectionString = string.Empty;
        protected IDbConnection Connection = null;
        protected IDbTransaction Transaction = null;

        /// <summary>
        /// Check Connected Status 
        /// </summary>
        protected bool IsConnected => Connection != null && Connection.State == ConnectionState.Open;
       
        /// <summary>
        /// Open connection to database
        /// </summary>
        public abstract void OpenConnection();

        /// <summary>
        /// Close connectionm to database
        /// </summary>
        public abstract void CloseConnection();

        /// <summary>
        /// Set command to execute
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract IDbCommand SetCommand(string query, IDataParameter[] parameters = null);
        public abstract void FillDataSet(string query, IDataParameter[] parameters, DataSet dataSet);
        public void Dispose()
        {
            if (IsConnected)
            {
                CloseConnection();
            }

            GC.SuppressFinalize(this);
            GC.Collect();
        }
        public Hashtable ExecQueryToHashtable(string query, IDataParameter[] parameters = null)
        {
            Hashtable result = null;
            try
            {
                OpenConnection();
                DataSet dataSet = new DataSet();
                FillDataSet(query, parameters, dataSet);
                DataView defaultView = dataSet.Tables[0].DefaultView;
                result = ((defaultView.Count > 0) ? defaultView[0].ToHashtable() : null);
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                Dispose();
            }

            return result;
        }

       
        public Hashtable[] ExecQueryToArrayHashtable(string query, IDataParameter[] parameters = null)
        {
            Hashtable[] result = new Hashtable[0];
            try
            {
                OpenConnection();
                DataSet dataSet = new DataSet();
                FillDataSet(query, parameters, dataSet);

                result = dataSet.Tables[0].DefaultView.ToHashtableArray();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Dispose();
            }

            return result;
        }
        public int ExecuteWithOpenClose(string query)
        {
            string errorMessage;
            int result = ExecuteWithOpenCloseBase(query, out errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return result;
            }
            return -1;
        }
        public int ExecuteWithOpenCloseBase(string query, out string errorMessage)
        {
            int result = -1;
            errorMessage = string.Empty;
            try
            {
                OpenConnection();
                result = ExecuteBase(query);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                Dispose();
            }

            return result;
        }

        public int ExecuteBase(string query)
        {
            int result = -1;
            using (var command = SetCommand(query))
            {
                result = command.ExecuteNonQuery();
            }

            return result;
        }
    }
}
