using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
//using Oracle.DataAccess.Client;

namespace DbConns
{
    public class Conns
    {

        string ConnectionString = ConfigurationManager.ConnectionStrings["OracleConnectionString"].ConnectionString;
        OracleConnection con;
        OracleTransaction tr;
        public OracleCommand cmd;

        public void Commit()
        {
            tr.Commit();
            con.Close();
        }

        public void RollBack()
        {
            tr.Rollback();
            con.Close();
        }

        public void OpenConection()
        {
            con = new OracleConnection(ConnectionString);
            con.Open();
            tr = con.BeginTransaction();
        }


        //public void CloseConnection()
        //{

        //    con.Close();
        //}



        public int CheckUserName(string Query_)
        {
            cmd = new OracleCommand(Query_, con,tr);
            int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            return temp;
        }

        public void OpenStoredProcedure()
        {
            con = new OracleConnection(ConnectionString);
            cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                tr = con.BeginTransaction();
                cmd.Transaction = tr;
            }
            catch (Exception ex)
            {
                tr.Rollback();
            }

        }

        public void ExecuteQueries(string Query_)
        {
            OracleCommand cmd = new OracleCommand(Query_, con,tr);
            cmd.ExecuteNonQuery();
        }


        public OracleDataReader DataReader(string Query_)
        {
            cmd = new OracleCommand(Query_, con,tr);
            OracleDataReader dr = cmd.ExecuteReader();   
            return dr;
        }


        public object ShowDataInGridView(string Query_)
        {
            OracleDataAdapter dr = new OracleDataAdapter(Query_, ConnectionString);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            object dataum = ds.Tables[0];
            return dataum;
        }
    }
}
