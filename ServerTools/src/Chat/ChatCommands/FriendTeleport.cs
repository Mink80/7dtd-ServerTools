﻿using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace ServerTools
{
    class FriendTeleport
    {
        public static bool IsEnabled = false, PvP_Check = false, Zombie_Check = false;
        public static int Delay_Between_Uses = 60, Command_Cost = 0;
        public static string Command59 = "friend", Command60 = "accept";
        public static Dictionary<int, int> Dict = new Dictionary<int, int>();
        public static Dictionary<int, DateTime> Dict1 = new Dictionary<int, DateTime>();

        public static void ListFriends(ClientInfo _cInfo, string _message)
        {
            bool _found = false;
            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            List<EntityPlayer> _playerList = GameManager.Instance.World.Players.list;
            for (int i = 0; i < _playerList.Count; i++)
            {
                EntityPlayer _player2 = _playerList[i];
                if (_player2 != null)
                {
                    if (_player != _player2 && _player.IsFriendsWith(_player2))
                    {
                        _found = true;
                        ClientInfo _cInfo2 = ConnectionManager.Instance.Clients.ForEntityId(_player2.entityId);
                        string _phrase625;
                        if (!Phrases.Dict.TryGetValue(625, out _phrase625))
                        {
                            _phrase625 = "Friend = {FriendName}, Id = {EntityId}.";
                        }
                        _phrase625 = _phrase625.Replace("{FriendName}", _cInfo2.playerName);
                        _phrase625 = _phrase625.Replace("{EntityId}", _cInfo2.entityId.ToString());
                        ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase625 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
            }
            if (!_found)
            {
                string _phrase632;
                if (!Phrases.Dict.TryGetValue(632, out _phrase632))
                {
                    _phrase632 = "No friends found online.";
                }
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase632 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void Checks(ClientInfo _cInfo, string _message, bool _announce)
        {
            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            if (PvP_Check)
            {
                if (Teleportation.PCheck(_cInfo, _player))
                {
                    return;
                }
            }
            if (Zombie_Check)
            {
                if (Teleportation.ZCheck(_cInfo, _player))
                {
                    return;
                }
            }
            ClientInfo _friend = ConsoleHelper.ParseParamIdOrName(_message);
            if (_friend != null)
            {
                EntityPlayer _friendPlayer = GameManager.Instance.World.Players.dict[_friend.entityId];
                if (_player.IsFriendsWith(_friendPlayer))
                {
                    bool _donator = false;
                    string _sql = string.Format("SELECT lastFriendTele FROM Players WHERE steamid = '{0}'", _cInfo.playerId);
                    DataTable _result = SQL.TQuery(_sql);
                    DateTime _lastFriendTele;
                    DateTime.TryParse(_result.Rows[0].ItemArray.GetValue(0).ToString(), out _lastFriendTele);
                    _result.Dispose();
                    if (Delay_Between_Uses < 1 || _lastFriendTele.ToString() == "10/29/2000 7:30:00 AM")
                    {
                        if (Wallet.IsEnabled && Command_Cost >= 1)
                        {
                            CommandCost(_cInfo, _message);
                        }
                        else
                        {
                            MessageFriend(_cInfo, _message);
                        }
                    }
                    else
                    {
                        TimeSpan varTime = DateTime.Now - _lastFriendTele;
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
                                        if (Wallet.IsEnabled && Command_Cost >= 1)
                                        {
                                            CommandCost(_cInfo, _message);
                                        }
                                        else
                                        {
                                            MessageFriend(_cInfo, _message);
                                        }
                                    }
                                    else
                                    {
                                        int _timeleft = _newDelay - _timepassed;
                                        string _phrase630;
                                        if (!Phrases.Dict.TryGetValue(630, out _phrase630))
                                        {
                                            _phrase630 = " you can only teleport to a friend once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                                        }
                                        _phrase630 = _phrase630.Replace("{DelayBetweenUses}", _newDelay.ToString());
                                        _phrase630 = _phrase630.Replace("{TimeRemaining}", _timeleft.ToString());
                                        if (_announce)
                                        {
                                            ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase630 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Global, null);
                                        }
                                        else
                                        {
                                            ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase630 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                                        }
                                    }
                                }
                            }
                        }
                        if (!_donator)
                        {
                            if (_timepassed >= Delay_Between_Uses)
                            {
                                if (Wallet.IsEnabled && Command_Cost >= 1)
                                {
                                    CommandCost(_cInfo, _message);
                                }
                                else
                                {
                                    MessageFriend(_cInfo, _message);
                                }
                            }
                            else
                            {
                                int _timeleft = Delay_Between_Uses - _timepassed;
                                string _phrase630;
                                if (!Phrases.Dict.TryGetValue(630, out _phrase630))
                                {
                                    _phrase630 = " you can only teleport to a friend once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                                }
                                _phrase630 = _phrase630.Replace("{DelayBetweenUses}", Delay_Between_Uses.ToString());
                                _phrase630 = _phrase630.Replace("{TimeRemaining}", _timeleft.ToString());
                                if (_announce)
                                {
                                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase630 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Global, null);
                                }
                                else
                                {
                                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase630 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string _phrase633;
                    if (!Phrases.Dict.TryGetValue(633, out _phrase633))
                    {
                        _phrase633 = "This player is not your friend. You can not request teleport to them.";
                    }
                    ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase633 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
        }

        public static void CommandCost(ClientInfo _cInfo, string _message)
        {
            int _currentCoins = Wallet.GetcurrentCoins(_cInfo);
            if (Command_Cost >= 1)
            {
                if (_currentCoins >= Command_Cost)
                {
                    MessageFriend(_cInfo, _message);
                }
                else
                {
                    string _phrase814;
                    if (!Phrases.Dict.TryGetValue(814, out _phrase814))
                    {
                        _phrase814 = " you do not have enough {WalletCoinName} in your wallet to run this command.";
                    }
                    _phrase814 = _phrase814.Replace("{WalletCoinName}", Wallet.Coin_Name);
                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase814 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
            else
            {
                MessageFriend(_cInfo, _message);
            }
        }

        public static void MessageFriend(ClientInfo _cInfo, string _message)
        {
            int _Id;
            if (!int.TryParse(_message, out _Id))
            {
                string _phrase626;
                if (!Phrases.Dict.TryGetValue(626, out _phrase626))
                {
                    _phrase626 = " this {EntityId} is not valid. Only integers accepted.";
                }
                _phrase626 = _phrase626.Replace("{EntityId}", _Id.ToString());
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase626 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                return;
            }
            ClientInfo _cInfo3 = ConnectionManager.Instance.Clients.ForEntityId(_Id);
            if (_cInfo3 != null)
            {
                string _phrase627;
                if (!Phrases.Dict.TryGetValue(627, out _phrase627))
                {
                    _phrase627 = " sent your friend {PlayerName} a teleport request.";
                }
                _phrase627 = _phrase627.Replace("{PlayerName}", _cInfo3.playerName);
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase627 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                string _phrase628;
                if (!Phrases.Dict.TryGetValue(628, out _phrase628))
                {
                    _phrase628 = " would like to teleport to you. Type {CommandPrivate}{Command60} in chat to accept the request.";
                }
                _phrase628 = _phrase628.Replace("{PlayerName}", _cInfo.playerName);
                _phrase628 = _phrase628.Replace("{CommandPrivate}", ChatHook.Command_Private);
                _phrase628 = _phrase628.Replace("{Command60}", Command60);
                ChatHook.ChatMessage(_cInfo3, ChatHook.Player_Name_Color + _cInfo3.playerName + LoadConfig.Chat_Response_Color + _phrase628 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                if (Dict.ContainsKey(_cInfo3.entityId))
                {
                    Dict.Remove(_cInfo3.entityId);
                    Dict1.Remove(_cInfo3.entityId);
                    Dict.Add(_cInfo3.entityId, _cInfo.entityId);
                    Dict1.Add(_cInfo3.entityId, DateTime.Now);
                }
                else
                {
                    Dict.Add(_cInfo3.entityId, _cInfo.entityId);
                    Dict1.Add(_cInfo3.entityId, DateTime.Now);
                }
            }
            else
            {
                string _phrase629;
                if (!Phrases.Dict.TryGetValue(629, out _phrase629))
                {
                    _phrase629 = " did not find EntityId {EntityId}. No teleport request sent.";
                }
                _phrase629 = _phrase629.Replace("{EntityId}", _Id.ToString());
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName + LoadConfig.Chat_Response_Color + _phrase629 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void TeleFriend(ClientInfo _cInfo, int _friendToTele)
        {
            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            ClientInfo _cInfo2 = ConnectionManager.Instance.Clients.ForEntityId(_friendToTele);
            if (_cInfo2 != null)
            {
                _cInfo2.SendPackage(new NetPackageTeleportPlayer(new Vector3((int)_player.position.x, (int)_player.position.y, (int)_player.position.z), null, false));
                string _sql;
                if (Wallet.IsEnabled && Command_Cost >= 1)
                {
                    Wallet.SubtractCoinsFromWallet(_cInfo.playerId, Command_Cost);
                }
                _sql = string.Format("UPDATE Players SET lastFriendTele = '{0}' WHERE steamid = '{1}'", DateTime.Now, _cInfo2.playerId);
                SQL.FastQuery(_sql, "FriendTeleport");
                string _phrase631;
                if (!Phrases.Dict.TryGetValue(631, out _phrase631))
                {
                    _phrase631 = " your request was accepted. Teleporting you to your friend.";
                }
                ChatHook.ChatMessage(_cInfo2, ChatHook.Player_Name_Color + _cInfo2.playerName + LoadConfig.Chat_Response_Color + _phrase631 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }
    }
}
