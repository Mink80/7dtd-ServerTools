﻿using MySql.Data.MySqlClient;
using System.Data;

namespace ServerTools
{
    public class MySqlDatabase
    {
        private static MySqlConnection connection;
        private static MySqlCommand cmd;
        public static string Server = "localhost";
        public static int Port = 3306;
        public static string Database = "ServerTools";
        public static string UserName = "UserName";
        public static string Password = "ChangeMe";

        public static void SetConnection()
        {
            string _connectionString;
            _connectionString = string.Format("SERVER={0};PORT={1};DATABASE={2};UID={3};PASSWORD={4};", Server, Port, Database, UserName, Password);
            connection = new MySqlConnection(_connectionString);
            try
            {
                Log.Out("[ServerTools] Connecting to MySql Database.");
                connection.Open();
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        Log.Out("[ServerTools] MySqlException in MySqlDatabase.SetConnection: Cannot connect to server.");
                        break;

                    case 1045:
                        Log.Out("[ServerTools] MySqlException in MySqlDatabase.SetConnection: Invalid username/password, please try again.");
                        break;
                }
                return;
            }
            connection.Close();
            CreateTables();
        }

        private static void CreateTables()
        {
            FastQuery("CREATE TABLE IF NOT EXISTS Players (" +
                "steamid VARCHAR(50) NOT NULL, " +
                "playername VARCHAR(50) DEFAULT 'Unknown', " +
                "last_joined VARCHAR(50) DEFAULT 'Never', " +
                "pingimmunity VARCHAR(10) DEFAULT 'false', " +
                "last_gimme VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastkillme VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "bank INT DEFAULT 0, " +
                "wallet INT DEFAULT 0, " +
                "playerSpentCoins INT DEFAULT 0, " +
                "hardcoreSessionTime INT DEFAULT 0, " +
                "hardcoreKills INT DEFAULT 0, " +
                "hardcoreZKills INT DEFAULT 0, " +
                "hardcoreScore INT DEFAULT 0, " +
                "hardcoreDeaths INT DEFAULT 0, " +
                "hardcoreName VARCHAR(50) DEFAULT 'Unknown', " +
                "bounty INT DEFAULT 0, " +
                "bountyHunter INT DEFAULT 0, " +
                "sessionTime INT DEFAULT 0, " +
                "bikeId INT DEFAULT 0, " +
                "lastBike VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "jailTime INT DEFAULT 0, " +
                "jailName VARCHAR(50) DEFAULT 'Unknown', " +
                "jailDate VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "muteTime INT DEFAULT 0, " +
                "muteName VARCHAR(50) DEFAULT 'Unknown', " +
                "muteDate VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "zkills INT DEFAULT 0, " +
                "kills INT DEFAULT 0, " +
                "deaths INT DEFAULT 0, " +
                "eventReturn VARCHAR(50) DEFAULT 'Unknown', " +
                "marketReturn VARCHAR(50) DEFAULT 'Unknown', " +
                "lobbyReturn VARCHAR(50) DEFAULT 'Unknown', " +
                "newTeleSpawn VARCHAR(50) DEFAULT 'Unknown', " +
                "homeposition VARCHAR(50) DEFAULT 'Unknown', " +
                "homeposition2 VARCHAR(50) DEFAULT 'Unknown', " +
                "lastsethome VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastwhisper VARCHAR(50) DEFAULT 'Unknown', " +
                "lastWaypoint VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastMarket VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastStuck VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastLobby VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastLog VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastDied VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastFriendTele VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "respawnTime VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastTravel VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastAnimals VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "lastVoteReward VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "firstClaim VARCHAR(10) DEFAULT 'false', " +
                "ismuted VARCHAR(10) DEFAULT 'false', " +
                "isjailed VARCHAR(10) DEFAULT 'false', " +
                "startingItems VARCHAR(10) DEFAULT 'false', " +
                "clanname VARCHAR(50) DEFAULT 'Unknown', " +
                "invitedtoclan VARCHAR(50) DEFAULT 'Unknown', " +
                "isclanowner VARCHAR(10) DEFAULT 'false', " +
                "isclanofficer VARCHAR(10) DEFAULT 'false', " +
                "customCommand1 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand2 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand3 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand4 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand5 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand6 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand7 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand8 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand9 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "customCommand10 VARCHAR(50) DEFAULT '10/29/2000 7:30:00 AM', " +
                "PRIMARY KEY (steamid)) ENGINE = InnoDB;");
            FastQuery("CREATE TABLE IF NOT EXISTS Config (sql_version INTEGER) ENGINE = InnoDB;");
            DataTable _result = SQL.TQuery("SELECT sql_version FROM Config");
            if (_result.Rows.Count == 0)
            {
                string _sql = string.Format("INSERT INTO Config (sql_version) VALUES ({0})", SQL.Sql_version);
                SQL.FastQuery(_sql);
            }
            else
            {
                int _version;
                int.TryParse(_result.Rows[0].ItemArray.GetValue(0).ToString(), out _version);
                if (_version != SQL.Sql_version)
                {
                    SQL.UpdateSQL(_version);
                }
            }
        }

        public static void FastQuery(string _sql)
        {
            try
            {
                connection.Open();
                cmd = new MySqlCommand(_sql, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException e)
            {
                Log.Out(string.Format("[ServerTools] MySqlException in MySqlException.FastQuery: {0}", e));
            }
        }

        public static DataTable TQuery(string _sql)
        {
            DataTable dt = new DataTable();
            try
            {
                connection.Open();
                cmd = new MySqlCommand(_sql, connection);
                MySqlDataReader _reader = cmd.ExecuteReader();
                dt.Load(_reader);
                _reader.Close();
                connection.Close();
            }
            catch (MySqlException e)
            {
                Log.Out(string.Format("[ServerTools] MySqlException in MySqlException.TQuery: {0}", e));
            }
            return dt;
        }

        public static string EscapeString(string _string)
        {
            string _str = MySqlHelper.EscapeString(_string);
            return _str;
        }
    }
}
