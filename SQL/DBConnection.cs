using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQL
{
    class DBConnection
    {
        #region Connection Information
        private string address = string.Empty;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string port = string.Empty;
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        private string username = string.Empty;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        private string database = string.Empty;
        public string Database
        {
            get { return database; }
            set { database = value; }
        }
        #endregion

        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }
        bool connected = false;
        public void OpenConnection()
        {
            if (Connection == null)
            {
                if (string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(Port) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Database))
                {
                    Console.WriteLine("Incomplete database connection info.");
                }
                else
                {
                    try
                    {
                        Console.WriteLine("Attempting connection...");
                        string connstring = string.Format("server={0};port={1};uid={2};pwd={3};database={4}",
                            address, port, username, password, database);
                        connection = new MySqlConnection(connstring);
                        connection.Open();
                        MySqlCommand getVersion = new MySqlCommand("SELECT VERSION()", Connection);
                        Console.WriteLine("Connected. Running version {0}", getVersion.ExecuteScalar());
                        connected = true;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Exception in database connection: {0}", ex.Number);
                    }
                }
            }
            else
            {
                Console.WriteLine("Database connection is already open.");
            }
        }

        public List<string>[] runQuery(string command)
        {
            return runQuery(command, 1);
        }

        private List<string>[] runQuery(string query, int runCount)
        {
            Console.WriteLine("Executing query.");
            //Initializes an array of lists that hold each column and their corresponding data.
            List<string>[] queryInfo;
            //Holds the names of the columns for getting the data from each column.
            List<string> columnNames = new List<string>();
            //Tests whether the connection is open.
            if (connected == true)
            {
                //Creates a new SQL command with the command argument through the existing connection.
                MySqlCommand sqlCommand = new MySqlCommand(query, Connection);
                //Creates a data reader object that holds the data in a better format.
                MySqlDataReader dataReader = sqlCommand.ExecuteReader();
                //Instantiates list with the actual number of columns gotten.
                queryInfo = new List<string>[dataReader.FieldCount];
                //Tests whether or not actual data is in the rows
                if (!dataReader.HasRows)
                {
                    Console.WriteLine("No rows found in query.");
                }
                //Creates the arrays in the return list and gets the column names.
                for (int x = 0; x < dataReader.FieldCount; x++)
                {
                    queryInfo[x] = new List<string>();
                    columnNames.Add(dataReader.GetName(x));
                }
                //For every column, add the information from each column into the list of the respective column.
                if (dataReader.Read())
                {
                    for (int x = 0; x < columnNames.Count; x++)
                    {
                        queryInfo[x].Add(dataReader[columnNames.ElementAt(x)] + "");
                    }
                }
                //Closes the data reader.
                dataReader.Close();
                //Returns the final product.
                return queryInfo;
            }
            else if (runCount <= 3)
            {
                Console.WriteLine("Connection not open. Attempting to establish.");
                OpenConnection();
                runQuery(query, runCount++);
                queryInfo = null;
                return queryInfo;
            }
            else
            {
                Console.WriteLine("Failed to connect after three tries. Giving up on the command :(");
                queryInfo = null;
                return queryInfo;
            }
        }

        public void runCommand(string command)
        {
            Console.WriteLine("Inserting into database.");
            if(connected == true)
            {
                MySqlCommand sqlCommand = new MySqlCommand(command, Connection);
                sqlCommand.ExecuteNonQuery();
                Console.Write(".. Success!");
            }
            else
            {
                Console.WriteLine("Failed to connect.");
            }
        }

        public void Close()
        {
            connected = false;
            connection.Close();
        }
    }
}
