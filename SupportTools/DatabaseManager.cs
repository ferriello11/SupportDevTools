using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportDataBase
    
{
    public class DatabaseManager
    {
        string ConnectionString = "";
        SqlConnection con;
        
        public DatabaseManager(string connStr)
        {
            this.ConnectionString = connStr;
        }

        public void OpenConection()
        {
            con = new SqlConnection(ConnectionString);
            con.Open();
        }


        public void CloseConnection()
        {
            con.Close();
        }


        public void ExecuteNonQueries(string Query_)
        {
            OpenConection();

            SqlCommand cmd = new SqlCommand(Query_, con);
            cmd.ExecuteNonQuery();

            CloseConnection();
        }


        public SqlDataReader DataReader(string Query_)
        {
            SqlCommand cmd = new SqlCommand(Query_, con);
            cmd.CommandTimeout = 999 * 10000;
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }


        public object ShowDataInGridView(string Query_)
        {
            SqlDataAdapter dr = new SqlDataAdapter(Query_, ConnectionString);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            object dataum = ds.Tables[0];
            return dataum;
        }

        public DataTable GetDatable(string Query_)
        {
            OpenConection();

            SqlDataAdapter dr = new SqlDataAdapter(Query_, ConnectionString);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            DataTable dataum = ds.Tables[0];

            CloseConnection();

            return dataum;
        }
               
       

    }
}