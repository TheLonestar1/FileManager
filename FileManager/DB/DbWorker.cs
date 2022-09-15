using System.Data.SQLite;
using System;
using System.Windows;
using System.IO;

namespace FileManager.DB
{


    public class DbWorker
    {
        SQLiteConnection _connection;
        SQLiteCommand _command;

        
        public DbWorker(string dbFileName){
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                _connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                _connection.Open();
                _command = new SQLiteCommand(_connection);

                _command.CommandText = "CREATE TABLE IF NOT EXISTS Record (id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    " name TEXT," +
                    " time TIMESTAMP DEFAULT CURRENT_TIMESTAMP)";
                _command.ExecuteNonQuery();

                
            }
            catch (SQLiteException ex)
            {
                
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void AddRecord(string name)
        {
            try
            {
                _command.CommandText = $"INSERT INTO Record (name) values ('{name}')";
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
