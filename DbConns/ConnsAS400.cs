using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.Odbc;
//using Oracle.DataAccess.Client;


namespace DbConns
{
    public class ConnsAS400
    {

        string ConnectionString = ConfigurationManager.ConnectionStrings["ODBCConnectionString"].ConnectionString;
        OdbcConnection con;
        OdbcTransaction tr;
        public OdbcCommand cmd;


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
            con = new OdbcConnection(ConnectionString);
            con.Open();
            tr = con.BeginTransaction();
        }





        public int CheckUserName(string Query_)
        {
            cmd = new OdbcCommand(Query_, con,tr);
            int temp = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            return temp;
        }

        public void OpenStoredProcedure()
        {
            con = new OdbcConnection(ConnectionString);
            cmd = new OdbcCommand();
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
            OdbcCommand cmd = new OdbcCommand(Query_, con,tr);
            cmd.ExecuteNonQuery();
        }


        public OdbcDataReader DataReader(string Query_)
        {
            cmd = new OdbcCommand(Query_, con, tr);
            OdbcDataReader dr = cmd.ExecuteReader();
            return dr;
        }


        public object ShowDataInGridView(string Query_)
        {
            OdbcDataAdapter dr = new OdbcDataAdapter(Query_, ConnectionString);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            object dataum = ds.Tables[0];
            return dataum;
        }
    }
}
