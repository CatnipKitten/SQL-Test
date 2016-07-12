using System;
using System.Collections.Generic;

namespace SQL
{
    class Program
    {
        static void Main(string[] args)
        {

            DBConnection database = DBConnection.Instance();

            database.Address = "WIN-2U09UFRCLEE";
            database.Port = "1433";
            database.Username = "db_admin";
            database.Password = "root";
            database.Database = "scouting";

            database.OpenConnection();
            //database.runCommand("CREATE TABLE IF NOT EXIST Matches (MatchNumber INT NOT NULL, Team INT NOT NULL, Score INT NOT NULL, PRIMARY KEY (MatchNumber));");
            //database.runCommand("INSERT INTO Matches(MatchNumber, Team, Score) VALUES (1, 2486, 10);");
            List<string>[] matches = database.runQuery("SELECT * FROM Matches;");
            foreach(List<string> i in matches)
            {
                foreach(string x in i)
                {
                    Console.WriteLine(x);
                }
            }
        }
    }
}
