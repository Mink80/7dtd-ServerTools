﻿using System;
using System.Collections.Generic;
using System.Timers;

namespace ServerTools
{
    class WeatherVote
    {
        private static int timerInstanceCount = 0;
        public static bool IsEnabled = false;
        public static bool VoteOpen = false;
        public static bool VoteClosed = false;
        public static int Vote_Delay = 30;
        private static string _weather = "";
        public static List<int> clear = new List<int>();
        public static List<int> rain = new List<int>();
        public static List<int> snow = new List<int>();
        private static System.Timers.Timer t1 = new System.Timers.Timer();
        private static System.Timers.Timer t2 = new System.Timers.Timer();

        private static void StartTimerT1()
        {
            timerInstanceCount++;
            if (timerInstanceCount <= 1)
            {
                t1.Interval = 30000;
                t1.Start();
                t1.Elapsed += new ElapsedEventHandler(CallForVote2);
            }
        }

        private static void TimerStopT1()
        {
            t1.Stop();
        }

        public static void CallForVote1()
        {
            string _phrase611;
            if (!Phrases.Dict.TryGetValue(611, out _phrase611))
            {
                _phrase611 = "A vote to change the weather has begun and will close in 30 seconds.";
            }
            GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase611), "Server", false, "", false);
            string _phrase615;
            if (!Phrases.Dict.TryGetValue(615, out _phrase615))
            {
                _phrase615 = "Type /clear, /rain or /snow to cast your vote.";
            }
            GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase615), "Server", false, "", false);
            VoteOpen = true;
            StartTimerT1();
        }

        private static void CallForVote2(object sender, ElapsedEventArgs e)
        {
            TimerStopT1();
            VoteOpen = false;
            if (clear.Count > rain.Count & clear.Count > snow.Count)
            {
                GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}Clear skies ahead", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                SdtdConsole.Instance.ExecuteSync("weather rain 0", (ClientInfo)null);
                SdtdConsole.Instance.ExecuteSync("weather wet 0", (ClientInfo)null);
                SdtdConsole.Instance.ExecuteSync("weather snow 0", (ClientInfo)null);
                SdtdConsole.Instance.ExecuteSync("weather snowfall 0", (ClientInfo)null);
                VoteClosed = true;
                WeatherTimerStart();
                _weather = "clear";
            }
            if (rain.Count > clear.Count & rain.Count > snow.Count)
            {
                Random rnd = new Random();
                int _rndWeather = rnd.Next(1, 3);
                if (_rndWeather == 1)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}Light rain has started", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                    SdtdConsole.Instance.ExecuteSync("weather rain 0.2", (ClientInfo)null);
                }
                if (_rndWeather == 2)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}A rain storm has started", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                    SdtdConsole.Instance.ExecuteSync("weather rain 0.6", (ClientInfo)null);
                    SdtdConsole.Instance.ExecuteSync("weather wet 1", (ClientInfo)null);
                }
                if (_rndWeather == 3)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}A heavy rain storm has started", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                    SdtdConsole.Instance.ExecuteSync("weather rain 1", (ClientInfo)null);
                    SdtdConsole.Instance.ExecuteSync("weather wet 1", (ClientInfo)null);
                }
                VoteClosed = true;
                WeatherTimerStart();
                _weather = "rain";               
            }
            if (snow.Count > clear.Count & snow.Count > rain.Count)
            {
                Random rnd = new Random();
                int _rndWeather = rnd.Next(1, 3);
                if (_rndWeather == 1)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}Light snow has started", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                    SdtdConsole.Instance.ExecuteSync("weather snowfall 0.2", (ClientInfo)null);
                }
                if (_rndWeather == 2)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}A snow storm has started", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                    SdtdConsole.Instance.ExecuteSync("weather snowfall 0.6", (ClientInfo)null);
                    SdtdConsole.Instance.ExecuteSync("weather snow 0.6", (ClientInfo)null);
                }
                if (_rndWeather == 3)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}A heavy snow storm has started", Config.Chat_Response_Color), "Server", false, "ServerTools", true);
                    SdtdConsole.Instance.ExecuteSync("weather snowfall 1", (ClientInfo)null);
                    SdtdConsole.Instance.ExecuteSync("weather snow 1", (ClientInfo)null);
                }
                VoteClosed = true;
                WeatherTimerStart();
                _weather = "snow";
            }
            if (clear.Count == 0 & rain.Count == 0 & snow.Count == 0)
            {
                string _phrase612;
                if (!Phrases.Dict.TryGetValue(612, out _phrase612))
                {
                    _phrase612 = "Weather vote complete, but no votes were cast. No changes were made.";
                }
                GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase612), "Server", false, "", false);
                clear.Clear(); rain.Clear(); snow.Clear();
                _weather = "";
            }
            else
            {
                if (_weather != "")
                {
                    string _phrase613;
                    if (!Phrases.Dict.TryGetValue(613, out _phrase613))
                    {
                        _phrase613 = "Weather vote complete. Most votes went to {weather}. The next weather vote can be started in {VoteDelay} minutes.";
                    }
                    _phrase613 = _phrase613.Replace("{weather}", _weather.ToString());
                    _phrase613 = _phrase613.Replace("{VoteDelay}", Vote_Delay.ToString());
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase613), "Server", false, "", false);
                    clear.Clear(); rain.Clear(); snow.Clear();
                    _weather = "";
                }
                else if (_weather != "" & clear.Count > 0 || _weather != "" & rain.Count > 0 || _weather != "" & snow.Count > 0)
                {
                    string _phrase614;
                    if (!Phrases.Dict.TryGetValue(614, out _phrase614))
                    {
                        _phrase614 = "Weather vote was a tie. No changes were made.";
                    }
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase614), "Server", false, "", false);
                    clear.Clear(); rain.Clear(); snow.Clear();
                    _weather = "";
                }
            }
        }

        private static void WeatherTimerStart()
        {
            timerInstanceCount++;
            if (timerInstanceCount <= 1)
            {
                int d = Vote_Delay * 60000;
                t2.Interval = d;
                t2.Start();
                t2.Elapsed += new ElapsedEventHandler(WeatherTimerStop);
            }
        }

        private static void WeatherTimerStop(object sender, ElapsedEventArgs e)
        {
            t2.Stop();
            SdtdConsole.Instance.ExecuteSync("weather defaults", (ClientInfo)null);
            VoteClosed = false;
        }
    }
}
