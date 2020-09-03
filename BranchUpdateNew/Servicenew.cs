using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DbConns;
//using Oracle.DataAccess.Client;
using System.Data.Odbc;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.Services.Client;
using System.Threading;




namespace BranchUpdateNew
{
    public partial class Servicenew : ServiceBase
    {

        Thread thread = null;
        bool IsStarted = false;

        FileStream fs = null;


        public Servicenew()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            thread = new Thread(new ThreadStart(DailyRun));
            thread.Start();
            IsStarted = true;
        }

        private void DailyRun()
        {

            Conns con = new Conns();

            ConnsAS400 con400 = new ConnsAS400();

            fs = new FileStream(ConfigurationSettings.AppSettings["logFilePath"].ToString(), FileMode.OpenOrCreate, FileAccess.Write); ;


            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);

            m_streamWriter.WriteLine(Environment.NewLine + "Branch Update service Started: start date & time :" + DateTime.Now.ToString());
            m_streamWriter.Flush();


            while (IsStarted)
            {
                if (DateTime.Now.Hour.Equals(int.Parse(ConfigurationSettings.AppSettings["StartHour"].ToString())))
                {

                    try
                    {
                        con.OpenStoredProcedure();
                        con.cmd.CommandText = "INTERNET.GET_EACH_COUNT_OF_EPF";
                        con.cmd.ExecuteNonQuery();

                        m_streamWriter.WriteLine(Environment.NewLine + "Successfully run the procedure" + DateTime.Now.ToString());
                        m_streamWriter.Flush();
                    //}
                    //catch (Exception ex)
                    //{
                    //    m_streamWriter.WriteLine("Error: Unable to run the procedure. " + ex.Message);
                    //    m_streamWriter.WriteLine("");
                    //    m_streamWriter.Flush();

                    //}







                    //m_streamWriter.WriteLine(Environment.NewLine + "Branch Update service Started: start date & time :" + DateTime.Now.ToString());
                    //m_streamWriter.Flush();

                    //try
                    //{
                        //Conns con = new Conns();

                        //ConnsAS400 con400 = new ConnsAS400();

                        con.OpenConection();
                        con400.OpenConection();

                        m_streamWriter.WriteLine(Environment.NewLine + "Database connection open" + DateTime.Now.ToString());
                        m_streamWriter.Flush();




                        //string getuser = "SELECT USER_ID FROM GENPAY.TRACK_PERFORM WHERE  (trunc(TRANS_DATE) = TRUNC(sysdate) )";
                        string getuser = "SELECT USER_ID FROM SLIC_MTRACK.TRACK_PERFORM";
                        OracleDataReader reader1 = con.DataReader(getuser);

                        while (reader1.Read())
                        {
                            string id = reader1["USER_ID"].ToString();
                            string branch = "SELECT BRNACH FROM INTRANET.INTUSR WHERE USERID = '" + id + "' ";
                            OdbcDataReader reader2 = con400.DataReader(branch);

                            while (reader2.Read())
                            {
                                string branch1 = reader2["BRNACH"].ToString();
                                string update = "UPDATE SLIC_MTRACK.TRACK_PERFORM SET BRANCH = '" + branch1 + "' WHERE USER_ID = '" + id + "'";
                                con.ExecuteQueries(update);
                            }

                            //if ()
                            //{
                            //    reader["Pwd"].ToString();

                            //}
                            //else
                            //{


                            //}
                        }
                        con.Commit();
                        con400.Commit();

                       // IsStarted = false;

                        m_streamWriter.WriteLine(Environment.NewLine + "SuccessFully Updated the branch.." + DateTime.Now.ToString());
                        m_streamWriter.Flush();

                        Thread.Sleep(1000 * 60 * 60 * 10);
                    }
                    catch (Exception ex)
                    {
                        m_streamWriter.WriteLine("Error: " + ex.Message);
                        m_streamWriter.WriteLine("");
                        m_streamWriter.Flush();
                        IsStarted = false;

                    }
                }
            }
            m_streamWriter.WriteLine(Environment.NewLine + "Branch Update service Finished: finish date & time :" + DateTime.Now.ToString());
            m_streamWriter.Flush();

            //con.CloseConnection();
            //con400.CloseConnection();
        }



        protected override void OnStop()
        {
            IsStarted = false;
            thread.Abort();
        }
    }
}
