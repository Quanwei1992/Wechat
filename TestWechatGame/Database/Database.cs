using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace TestWechatGame
{
    public class Database
    {


        private static Database mInsatnce;
        private SQLiteConnection mDBConnection;
        
        public static Database Instance
        {
            get {
                if (mInsatnce == null) {
                    mInsatnce = new Database();
                }
                return mInsatnce;
            }
        }

        private Database()
        {
            Connect();
        }

        private void Connect()
        {
            string dbPath = "./data/pkten.db";
            string connectString = string.Format("Data Source={0}",dbPath);
            mDBConnection = new SQLiteConnection(connectString);
            mDBConnection.Open();

        }


        public SQLiteDataReader ExecuteSQL(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, mDBConnection);
            var reader = cmd.ExecuteReader();
           
            return reader;
        }

        public int ExecuteNoQuery(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, mDBConnection);
            return cmd.ExecuteNonQuery();
        }

    }
}
