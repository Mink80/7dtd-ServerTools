﻿using System.Collections.Generic;
using System;

namespace ServerTools
{
    public static class AutoShutdown
    {
        public static bool IsEnabled = false, Alert_On_Login = false, Bloodmoon = false;
        public static int Countdown_Timer = 2, Days_Until_Horde = 7;
        public static List<DateTime> timerStart = new List<DateTime>();
        private static int _duskTime = (int)SkyManager.GetDuskTime();

        public static void ShutdownList()
        {
            timerStart.Clear();
            timerStart.Add(DateTime.Now);
        }

        public static void CheckBloodmoon()
        {
            ulong _worldTime = GameManager.Instance.World.worldTime;
            int _daysUntilHorde = Days_Until_Horde - GameUtils.WorldTimeToDays(_worldTime) % Days_Until_Horde;
            int _daysUntilHorde1 = (Days_Until_Horde + 1) - GameUtils.WorldTimeToDays(_worldTime) % (Days_Until_Horde + 1);
            int _worldHours = (int)(_worldTime / 1000UL) % 24;
            if (_daysUntilHorde == Days_Until_Horde && (_worldHours >= _duskTime - 3) || _daysUntilHorde1 == (Days_Until_Horde + 1) && (_worldHours < _duskTime - 17) && GameManager.Instance.World.IsDaytime())
            {
                Bloodmoon = true;
            }
            else
            {
                if (Lottery.OpenLotto)
                {
                    Lottery.StartLotto();
                }
                Lottery.ShuttingDown = true;
                Bloodmoon = false;
                Auto_Shutdown();
            }
        }

        public static void Auto_Shutdown()
        {
            Log.Out("[SERVERTOOLS] Running auto shutdown.");
            GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, ("[FF0000]Auto shutdown initiated[-]"), Config.Server_Response_Name, false, "ServerTools", false);
            SdtdConsole.Instance.ExecuteSync(string.Format("stopserver {0}", Countdown_Timer), (ClientInfo)null);
        }

        public static void CheckNextShutdown(ClientInfo _cInfo, bool _announce)
        {
            if (!Bloodmoon)
            {
                DateTime _timeStart = timerStart[0];
                TimeSpan varTime = DateTime.Now - _timeStart;
                double fractionalMinutes = varTime.TotalMinutes;
                int _timeMinutes = (int)fractionalMinutes;
                int _timeleftMinutes = Timers.Shutdown_Delay - _timeMinutes;
                if (_timeleftMinutes > 0)
                {
                    string TimeLeft;
                    TimeLeft = string.Format("{0:00} H : {1:00} M", _timeleftMinutes / 60, _timeleftMinutes % 60);
                    if (_announce)
                    {
                        string _phrase730;
                        if (!Phrases.Dict.TryGetValue(730, out _phrase730))
                        {
                            _phrase730 = "The next auto shutdown is in [FF8000]{TimeLeft}.";
                        }
                        _phrase730 = _phrase730.Replace("{TimeLeft}", TimeLeft);
                        GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase730), Config.Server_Response_Name, false, "ServerTools", false);
                    }
                    else
                    {
                        string _phrase730;
                        if (!Phrases.Dict.TryGetValue(730, out _phrase730))
                        {
                            _phrase730 = "The next auto shutdown is in [FF8000]{TimeLeft}.";
                        }
                        _phrase730 = _phrase730.Replace("{TimeLeft}", TimeLeft);
                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.Chat_Response_Color, _phrase730), Config.Server_Response_Name, false, "ServerTools", false));
                    }
                }
            }
            else
            {
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}A bloodmoon is currently active. The server is set to shutdown after it finishes.[-]", Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}A bloodmoon is currently active. The server is set to shutdown after it finishes.[-]", Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                }
            }
        }
    }
}