using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;

using TrackTracker.Services.Interfaces;



namespace TrackTracker.Services
{
    /*
    Class: SQLiteService
    Description:
        Implements database persistence.
        Uses SQLite provider and locally stored DB file.
    */
    public class SQLiteService : IDatabaseService
    {
        //TODO: implement try-catch error handling and argument checking
        //TODO: more commenting
        private static string databaseFileName = "UserData.sqlite";
        private static string password = "0-TrackTrackerPassword-1"; //just because why not, will be changed later

        private string connString;

        public SQLiteService()
        {
            connString = @"Data Source=" + databaseFileName + ";Version=3;Password=" + password;
        }

        public bool DatabaseExists //returns true if the SQLite database file already exists
        {
            get => File.Exists(databaseFileName); //if the file does not exist, it is the first run, and vice versa
        }
        public void CreateDatabase() //creates a new database file if none created yet
        {
            if (!DatabaseExists) //to be sure, no accidental overwrite happens
            {
                SQLiteConnection.CreateFile(databaseFileName);
            }
        }
        public void FormatNewDatabase() //formats a new database from a 0-byte file: creates tables, keys, indexes
        {
            if (DatabaseExists) //to avoid exceptions
            {
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = @"CREATE TABLE LocalMediaPacks
                                (RootPath NVARCHAR2 PRIMARY KEY NOT NULL UNIQUE,
                                BaseExtension NVARCHAR2,
                                IsResultOfDriveSearch INT,
                                FilePaths NVARCHAR2 NOT NULL)";
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    command.ExecuteNonQuery();
                }
            }
            //TODO: store Tracks, MetaData and AppConfig as well
        }

        public string[] GetRowByID(string table, string IDColumnName, string ID) //selects exactly 1 row by ID
        {
            string[] columns = null;

            if (DatabaseExists) //to avoid exceptions
            {
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "SELECT * FROM TABLE " + table + " WHERE " + IDColumnName + " = " + ID;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            columns = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns[i] = reader[i].ToString();
                            }
                        }
                    }
                }
            }

            return columns;
        }
        public List<string[]> GetRowsByWhere(string table, string whereExpression) //selects multiple rows where an expression is true
        {
            List<string[]> rows = new List<string[]>();

            if (DatabaseExists) //to avoid exceptions
            {
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "SELECT * FROM " + table + " WHERE " + whereExpression;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] columns = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns[i] = reader[i].ToString();
                            }
                            rows.Add(columns);
                        }
                    }
                }
            }

            return rows;
        }
        public List<string[]> GetAllRows(string table) //selects all rows in a given table
        {
            List<string[]> rows = new List<string[]>();

            if (DatabaseExists) //to avoid exceptions
            {
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "SELECT * FROM " + table;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] columns = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns[i] = reader[i].ToString();
                            }
                            rows.Add(columns);
                        }
                    }
                }
            }

            return rows;
        }

        public int InsertInto(string table, string[] values) //inserts a new row into a given table
        {
            int affected = -1;

            if (DatabaseExists) //to avoid exceptions
            {
                StringBuilder valuesString = new StringBuilder();
                valuesString.Append("(");
                for (int i = 0; i < values.Length; i++)
                {
                    valuesString.Append("\"");
                    valuesString.Append(values[i]);
                    valuesString.Append("\"");
                    if((i + 1) < values.Length) valuesString.Append(", ");
                }
                valuesString.Append(")");
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "INSERT INTO " + table + " VALUES " + valuesString.ToString();
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    affected = command.ExecuteNonQuery();
                }
            }

            return affected;
        }

        public int UpdateRowByID(string table, string IDColumnName, string ID, string[] columnNames, string[] values) //updates excatly 1 row by ID
        {
            int affected = -1;

            if (DatabaseExists) //to avoid exceptions
            {
                StringBuilder valuesString = new StringBuilder();
                for (int i = 0; i < values.Length; i++)
                {
                    valuesString.Append(columnNames[i]);
                    valuesString.Append(" = ");
                    valuesString.Append("\"");
                    valuesString.Append(values[i]);
                    valuesString.Append("\"");
                    if ((i + 1) < values.Length) valuesString.Append(", ");
                }
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "UPDATE " + table + " SET " + valuesString.ToString() + " WHERE " + IDColumnName + " = " + ID;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    affected = command.ExecuteNonQuery();
                }
            }

            return affected;
        }
        public int UpdateRowsByWhere(string table, string whereExpression, string[] columnNames, string[] values) //updates multiple rows where an expression is true
        {
            int affected = -1;

            if (DatabaseExists) //to avoid exceptions
            {
                StringBuilder valuesString = new StringBuilder();
                for (int i = 0; i < values.Length; i++)
                {
                    valuesString.Append(columnNames[i]);
                    valuesString.Append(" = ");
                    valuesString.Append("\"");
                    valuesString.Append(values[i]);
                    valuesString.Append("\"");
                    if ((i + 1) < values.Length) valuesString.Append(", ");
                }
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "UPDATE " + table + " SET " + valuesString.ToString() + " WHERE " + whereExpression;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    affected = command.ExecuteNonQuery();
                }
            }

            return affected;
        }

        public int DeleteRowByID(string table, string IDColumnName, string ID) //deletes exaclty 1 row by ID
        {
            int affected = -1;

            if (DatabaseExists) //to avoid exceptions
            {
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "DELETE FROM " + table + " WHERE " + IDColumnName + " = " + ID;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    affected = command.ExecuteNonQuery();
                }
            }

            return affected;
        }
        public int DeleteRowsByWhere(string table, string whereExpression) //deletes multiple rows where an expression is true
        {
            int affected = -1;

            if (DatabaseExists) //to avoid exceptions
            {
                using (SQLiteConnection connection = new SQLiteConnection(connString))
                {
                    connection.Open();
                    string SQL = "DELETE FROM " + table + " WHERE " + whereExpression;
                    SQLiteCommand command = new SQLiteCommand(SQL, connection);
                    affected = command.ExecuteNonQuery();
                }
            }

            return affected;
        }

        public bool ExecuteSQLCommand(string SQL) //executes any direct SQL command, returning true if no exceptions were thrown
        {
            try
            {
                if (DatabaseExists) //to avoid exceptions
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connString))
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand(SQL, connection);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
        }
    }
}
