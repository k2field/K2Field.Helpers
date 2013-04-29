using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace K2Field.Helpers.Core.Code
{

    public class SQLHelper : IDisposable
    {
        string _sqlConnstring = "Data Source={0};Initial Catalog={1};Integrated Security=True";

        SqlConnection conn = null;
        SqlCommand cmd = null;
        DataTable dt = null;

        public SQLHelper(string sqlServerName, string sqlServerCatalog)
        {
            _sqlConnstring = string.Format(_sqlConnstring, sqlServerName, sqlServerCatalog);
        }

        public System.Data.DataTable ExecuteSQL(string sql)
        {
            try
            {

                //Create connection
                conn = new SqlConnection(_sqlConnstring);
                cmd = new SqlCommand();
                cmd.Connection = conn;

                //Opportunities for a company
                cmd.CommandText = sql;

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                dt = new DataTable();
                da.Fill(dt);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public void Dispose()
        {
            conn = null;
            cmd = null;
            GC.Collect();
        }
    }
}
