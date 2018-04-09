﻿using System;
using UnityEngine;

namespace ServerTools
{
    public class TeleportHome
    {
        public static bool IsEnabled = false, Set_Home2_Enabled = false, Set_Home2_Donor_Only = false;
        public static int Delay_Between_Uses = 60;

        public static void SetHome(ClientInfo _cInfo, string _playerName, bool _announce)
        {
            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            Vector3 _position = _player.GetPosition();
            string x = _position.x.ToString();
            string y = _position.y.ToString();
            string z = _position.z.ToString();
            string _sposition = x + "," + y + "," + z;
            PersistentContainer.Instance.Players[_cInfo.playerId, true].HomePosition = _sposition;
            PersistentContainer.Instance.Save();
            string _phrase10;
            if (!Phrases.Dict.TryGetValue(10, out _phrase10))
            {
                _phrase10 = "{PlayerName} your home has been saved.";
            }
            _phrase10 = _phrase10.Replace("{PlayerName}", _cInfo.playerName);
            if (_announce)
            {
                GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase10, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
            }
            else
            {
                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase10, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
            }
        }

        public static void TeleHome(ClientInfo _cInfo, string _playerName, bool _announce)
        {
            Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
            if (p == null || p.HomePosition == null)
            {
                string _phrase11;
                if (!Phrases.Dict.TryGetValue(11, out _phrase11))
                {
                    _phrase11 = "{PlayerName} you do not have a home saved.";
                }
                _phrase11 = _phrase11.Replace("{PlayerName}", _cInfo.playerName);
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase11, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase11, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                }
            }
            else
            {
                bool _donator = false;
                if (Delay_Between_Uses < 1)
                {
                    Home(_cInfo, p.HomePosition, _announce);
                }
                else
                {
                    if (p.LastSetHome == null)
                    {
                        Home(_cInfo, p.HomePosition, _announce);
                    }
                    else
                    {
                        TimeSpan varTime = DateTime.Now - p.LastSetHome;
                        double fractionalMinutes = varTime.TotalMinutes;
                        int _timepassed = (int)fractionalMinutes;
                        if (ReservedSlots.IsEnabled && ReservedSlots.Reduced_Delay)
                        {
                            if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _donator = true;
                                    int _newDelay = Delay_Between_Uses / 2;
                                    if (_timepassed >= _newDelay)
                                    {
                                        Home(_cInfo, p.HomePosition, _announce);
                                    }
                                    else
                                    {
                                        int _timeleft = _newDelay - _timepassed;
                                        string _phrase13;
                                        if (!Phrases.Dict.TryGetValue(13, out _phrase13))
                                        {
                                            _phrase13 = "{PlayerName} you can only use /home or /home2 once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                                        }
                                        _phrase13 = _phrase13.Replace("{PlayerName}", _cInfo.playerName);
                                        _phrase13 = _phrase13.Replace("{DelayBetweenUses}", _newDelay.ToString());
                                        _phrase13 = _phrase13.Replace("{TimeRemaining}", _timeleft.ToString());
                                        if (_announce)
                                        {
                                            GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase13, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                        }
                                        else
                                        {
                                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase13, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                                        }
                                    }
                                }
                            }
                        }
                        if (!_donator)
                        {
                            if (_timepassed >= Delay_Between_Uses)
                            {
                                Home(_cInfo, p.HomePosition, _announce);
                            }
                            else
                            {
                                int _timeleft = Delay_Between_Uses - _timepassed;
                                string _phrase13;
                                if (!Phrases.Dict.TryGetValue(13, out _phrase13))
                                {
                                    _phrase13 = "{PlayerName} you can only use /home or /home2 once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                                }
                                _phrase13 = _phrase13.Replace("{PlayerName}", _cInfo.playerName);
                                _phrase13 = _phrase13.Replace("{DelayBetweenUses}", Delay_Between_Uses.ToString());
                                _phrase13 = _phrase13.Replace("{TimeRemaining}", _timeleft.ToString());
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase13, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase13, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                                }
                            }
                        }
                    }
                }
            }
        }       


        private static void Home(ClientInfo _cInfo, string _home, bool _announce)
        {
            int x, y, z;
            string[] _cords = _home.Split(',');
            int.TryParse(_cords[0], out x);
            int.TryParse(_cords[1], out y);
            int.TryParse(_cords[2], out z);
            _cInfo.SendPackage(new NetPackageTeleportPlayer(new Vector3(x, y, z), false));
            PersistentContainer.Instance.Players[_cInfo.playerId, true].LastSetHome = DateTime.Now;
            PersistentContainer.Instance.Save();
        }

        public static void DelHome(ClientInfo _cInfo, string _playerName, bool _announce)
        {
            Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
            if (p.HomePosition != null)
            {
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1} deleted home.[-]", Config.Chat_Response_Color, _playerName), _playerName, false, "ServerTools", true);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} deleted home.[-]", Config.Chat_Response_Color, _playerName), Config.Server_Response_Name, false, "ServerTools", false));
                }
                PersistentContainer.Instance.Players[_cInfo.playerId, true].HomePosition = null;
                PersistentContainer.Instance.Save();
            }
            else
            {
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1} you have no home to delete.[-]", Config.Chat_Response_Color, _playerName), _playerName, false, "ServerTools", true);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} you have no home to delete.[-]", Config.Chat_Response_Color, _playerName), Config.Server_Response_Name, false, "ServerTools", false));
                }
            }
        }

        public static void SetHome2(ClientInfo _cInfo, string _playerName, bool _announce)
        {
            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            Vector3 _position = _player.GetPosition();
            string x = _position.x.ToString();
            string y = _position.y.ToString();
            string z = _position.z.ToString();
            string _sposition = x + "," + y + "," + z;
            PersistentContainer.Instance.Players[_cInfo.playerId, true].HomePosition2 = _sposition;
            PersistentContainer.Instance.Save();
            string _phrase607;
            if (!Phrases.Dict.TryGetValue(607, out _phrase607))
            {
                _phrase607 = "{PlayerName} your home2 has been saved.";
            }
            _phrase607 = _phrase607.Replace("{PlayerName}", _cInfo.playerName);
            if (_announce)
            {
                GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase607, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
            }
            else
            {
                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase607, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
            }
        }

        public static void TeleHome2(ClientInfo _cInfo, string _playerName, bool _announce)
        {
            Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
            if (p == null || p.HomePosition2 == null)
            {
                string _phrase608;
                if (!Phrases.Dict.TryGetValue(608, out _phrase608))
                {
                    _phrase608 = "{PlayerName} you do not have a home2 saved.";
                }
                _phrase608 = _phrase608.Replace("{PlayerName}", _cInfo.playerName);
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase608, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase608, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                }
            }
            else
            {
                bool _donator = false;
                if (Delay_Between_Uses < 1)
                {
                    Home2(_cInfo, p.HomePosition2, _playerName, _announce);
                }
                else
                {
                    if (p.LastSetHome == null)
                    {
                        Home2(_cInfo, p.HomePosition2, _playerName, _announce);
                    }
                    else
                    {
                        TimeSpan varTime = DateTime.Now - p.LastSetHome;
                        double fractionalMinutes = varTime.TotalMinutes;
                        int _timepassed = (int)fractionalMinutes;
                        if (ReservedSlots.IsEnabled && ReservedSlots.Reduced_Delay)
                        {
                            if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _donator = true;
                                    int _newDelay = Delay_Between_Uses / 2;
                                    if (_timepassed >= _newDelay)
                                    {
                                        Home2(_cInfo, p.HomePosition2, _playerName, _announce);
                                    }
                                    else
                                    {
                                        int _timeleft = _newDelay - _timepassed;
                                        string _phrase609;
                                        if (!Phrases.Dict.TryGetValue(609, out _phrase609))
                                        {
                                            _phrase609 = "{PlayerName} you can only use /home or /home2 once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                                        }
                                        _phrase609 = _phrase609.Replace("{PlayerName}", _cInfo.playerName);
                                        _phrase609 = _phrase609.Replace("{DelayBetweenUses}", _newDelay.ToString());
                                        _phrase609 = _phrase609.Replace("{TimeRemaining}", _timeleft.ToString());
                                        if (_announce)
                                        {
                                            GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase609, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                        }
                                        else
                                        {
                                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase609, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                                        }
                                    }
                                }
                            }
                        }
                        if (!_donator)
                        {
                            if (_timepassed >= Delay_Between_Uses)
                            {
                                Home2(_cInfo, p.HomePosition2, _playerName, _announce);
                            }
                            else
                            {
                                int _timeleft = Delay_Between_Uses - _timepassed;
                                string _phrase609;
                                if (!Phrases.Dict.TryGetValue(609, out _phrase609))
                                {
                                    _phrase609 = "{PlayerName} you can only use /home or /home2 once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                                }
                                _phrase609 = _phrase609.Replace("{PlayerName}", _cInfo.playerName);
                                _phrase609 = _phrase609.Replace("{DelayBetweenUses}", Delay_Between_Uses.ToString());
                                _phrase609 = _phrase609.Replace("{TimeRemaining}", _timeleft.ToString());
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase609, Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{1}{0}[-]", _phrase609, Config.Chat_Response_Color), Config.Server_Response_Name, false, "ServerTools", false));
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void Home2(ClientInfo _cInfo, string _home2, string _playerName, bool _announce)
        {
            int x, y, z;
            string[] _cords = _home2.Split(',');
            int.TryParse(_cords[0], out x);
            int.TryParse(_cords[1], out y);
            int.TryParse(_cords[2], out z);
            _cInfo.SendPackage(new NetPackageTeleportPlayer(new Vector3(x, y, z), false));
            PersistentContainer.Instance.Players[_cInfo.playerId, true].LastSetHome = DateTime.Now;
            PersistentContainer.Instance.Save();
        }

        public static void DelHome2(ClientInfo _cInfo, string _playerName, bool _announce)
        {
            Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
            if (p.HomePosition2 != null)
            {
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1} deleted home2.[-]", Config.Chat_Response_Color, _playerName), _playerName, false, "ServerTools", true);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} deleted home2.[-]", Config.Chat_Response_Color, _playerName), Config.Server_Response_Name, false, "ServerTools", false));
                }
                PersistentContainer.Instance.Players[_cInfo.playerId, true].HomePosition2 = null;
                PersistentContainer.Instance.Save();
            }
            else
            {
                if (_announce)
                {
                    GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1} you have no home2 to delete.[-]", Config.Chat_Response_Color, _playerName), _playerName, false, "ServerTools", true);
                }
                else
                {
                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1} you have no home2 to delete.[-]", Config.Chat_Response_Color, _playerName), Config.Server_Response_Name, false, "ServerTools", false));
                }
            }
        }
    }
}