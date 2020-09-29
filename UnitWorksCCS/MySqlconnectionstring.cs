using System;
using System.Configuration;
using System.Data.SqlClient;

namespace UnitWorksCCS
{
    public  class MysqlConnection : IDisposable
    {
       
        //static String ServerName = @"SRKSDEV001-PC\SQLSERVER17";
        //static String username = "sa";
        //static String password = "srks4$";
        //static String port = "3306";
        //static String DB = "i_facility_tal";

        public static String ServerName = ConfigurationManager.AppSettings["ServerName"];
        public static String username = ConfigurationManager.AppSettings["username"];
        public static String password = ConfigurationManager.AppSettings["password"];
        public static String port = "3306";
        public static String DB = ConfigurationManager.AppSettings["DB"];
        public static String DbName = ConfigurationManager.AppSettings["Db"];
        public static String Host = ConfigurationManager.AppSettings["host"];
        public static int Portmail = 14;

        public static String UserNamemail = ConfigurationManager.AppSettings["UserName"];
        public static String Passwordmail = ConfigurationManager.AppSettings["Password"];
        public static String Domain = ConfigurationManager.AppSettings["Domain"];

        public SqlConnection sqlConnection = new SqlConnection(@"Data Source = " + ServerName + ";User ID = " + username + ";Password = " + password + ";Initial Catalog = " + DB + ";Persist Security Info=True");

        public void open()
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
        }

        public void close()
        {
            sqlConnection.Close();
        }
        
        void IDisposable.Dispose()
        { }

    }
}