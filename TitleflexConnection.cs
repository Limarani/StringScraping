using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace ScrapMaricopa
{
    public class TitleflexConnection
    {
        string mysqlConnection = ConfigurationManager.ConnectionStrings["TitleFlexConnectionString"].ConnectionString;

        MySqlConnection mConnection;
        MySqlDataAdapter mDa;
        MySqlCommand mCmd;
        MySqlDataReader mDr;

        public MySqlConnection openConnection()
        {
            mConnection = new MySqlConnection(mysqlConnection);
            if (mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
            mConnection.Open();
            return mConnection;
        }
        public void closeConnection()
        {
            mConnection = new MySqlConnection(mysqlConnection);
            if (mConnection.State == ConnectionState.Open)
            {
                mConnection.Close();
            }
            mConnection.Dispose();
        }


        public int ExecuteSPNonQuery(string Query, bool isProcedure, MySqlParameter[] myParams)
        {
            int result;
            mCmd = new MySqlCommand(Query, openConnection());

            if (isProcedure)
            {
                mCmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (myParams != null)
                {
                    foreach (MySqlParameter param in myParams)
                    {
                        mCmd.Parameters.Add(param);
                    }
                }
            }
            try
            {
                result = mCmd.ExecuteNonQuery();
            }
            catch (MySqlException mye)
            {
                mConnection.Close();
                mConnection.Dispose();
                return -1;
            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }
            return result;
        }
        public int ExecuteSPNonQuery1(string Query, bool isProcedure, MySqlParameter[] myParams)
        {
            int result;
            mCmd = new MySqlCommand(Query, openConnection());

            if (isProcedure)
            {
                mCmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (myParams != null)
                {
                    foreach (MySqlParameter param in myParams)
                    {
                        mCmd.Parameters.Add(param);
                    }
                }
            }
            try
            {
                result = mCmd.ExecuteNonQuery();
            }
            catch (MySqlException mye)
            {
                mConnection.Close();
                mConnection.Dispose();
                return -1;
            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }
            return result;
        }
        public string ExecuteScalar(string Query, bool isProcedure, MySqlParameter[] myParams)
        {
            string result;
            openConnection();
            mCmd = new MySqlCommand(Query, mConnection);

            if (isProcedure)
            {
                mCmd.CommandType = CommandType.StoredProcedure;
                if (myParams != null)
                {
                    foreach (MySqlParameter param in myParams)
                    {
                        mCmd.Parameters.Add(param);
                    }
                }
            }
            try
            {
                result = Convert.ToString(mCmd.ExecuteScalar());
            }
            catch (MySqlException mye)
            {
                if (mye.Number == 1062)
                {
                    //SessionHandler.ErrMsg = "Duplicate Entry: Name already found.";
                }
                else
                {
                    //SessionHandler.ErrMsg = mye.Number + " " + mye.Message;
                }
                mConnection.Close();
                mConnection.Dispose();
                return "";

            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }
            return result;

        }
        public string ExecuteScalar(string Query)
        {
            openConnection();
            mCmd = new MySqlCommand(Query, mConnection);

            try
            {
                return Convert.ToString(mCmd.ExecuteScalar());
            }
            catch (MySqlException mye)
            {
                mConnection.Close();
                mConnection.Dispose();
                return "";
            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }
        }
        public MySqlDataReader ExecuteSPReader(string query, bool isProcedure, MySqlParameter[] myParams)
        {
            openConnection();
            mCmd = new MySqlCommand(query, mConnection);

            if (isProcedure)
            {
                mCmd.CommandType = CommandType.StoredProcedure;
                if (myParams != null)
                {
                    foreach (MySqlParameter param in myParams)
                    {
                        mCmd.Parameters.Add(param);
                    }
                }
            }
            try
            {
                mDr = mCmd.ExecuteReader(CommandBehavior.CloseConnection);
                return mDr;
            }
            catch (MySqlException mye)
            {

                return mDr;
            }
            finally
            {

            }
        }
        public MySqlDataAdapter ExecuteSPAdapter(string query, bool isProcedure, MySqlParameter[] myParams)
        {
            openConnection();
            mCmd = new MySqlCommand(query, mConnection);

            if (isProcedure)
            {
                mCmd.CommandType = CommandType.StoredProcedure;
                if (myParams != null)
                {
                    foreach (MySqlParameter param in myParams)
                    {
                        mCmd.Parameters.Add(param);
                    }
                }
            }
            try
            {
                mDa = new MySqlDataAdapter(mCmd);
                return mDa;
            }
            catch (MySqlException mye)
            {
                mConnection.Close();
                mConnection.Dispose();
                return mDa;
            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }
        }
        //Query Returns Dataset
        public DataSet ExecuteQuery(string Query)
        {
            DataSet ds;
            openConnection();
            mCmd = new MySqlCommand(Query, mConnection);
            ds = new DataSet();
            mDa = new MySqlDataAdapter(mCmd);

            mDa.Fill(ds);
            mConnection.Close();
            mConnection.Dispose();
            return ds;

        }


        //SP Returns, Dataset
        public DataSet Executedataset(string Query, bool isProcedure, MySqlParameter[] myParams)
        {
            DataSet dss = new DataSet();
            openConnection();
            mCmd = new MySqlCommand(Query, mConnection);

            if (isProcedure)
            {
                mCmd.CommandType = CommandType.StoredProcedure;
                if (myParams != null)
                {
                    foreach (MySqlParameter param in myParams)
                    {
                        mCmd.Parameters.Add(param);
                    }
                }
            }
            mDa = new MySqlDataAdapter(mCmd);
            mDa.Fill(dss);
            mConnection.Close();
            mConnection.Dispose();
            return dss;
        }
        //Excecute Query array
        public DataSet ExecuteTables(string[] Querys)
        {
            DataSet ds = new DataSet();
            openConnection();
            mCmd = new MySqlCommand();
            mCmd.Connection = mConnection;
            mDa = new MySqlDataAdapter();
            mDa.SelectCommand = mCmd;
            try
            {
                int count = 0;
                foreach (string query in Querys)
                {
                    mDa.SelectCommand.CommandText = query;
                    mDa.Fill(ds, "Table" + count);
                    count++;
                }
                mConnection.Close();
                mConnection.Dispose();
                return ds;
            }
            catch (MySqlException mye)
            {
                //SessionHandler.ErrMsg = mye.Number + " " + mye.Message;
                mConnection.Close();
                mConnection.Dispose();
                return ds;
            }
            finally
            {
                mConnection.Close();
                mConnection.Dispose();
            }

        }

    }
}