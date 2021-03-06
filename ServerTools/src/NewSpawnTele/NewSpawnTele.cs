﻿using System.Data;
using UnityEngine;

namespace ServerTools
{
    public class NewSpawnTele
    {
        public static bool IsEnabled = false, Return = false;
        public static string Command29 = "setspawn", Command86 = "ready";
        private static string[] _cmd = { Command29 };
        public static string New_Spawn_Tele_Position = "0,0,0";

        public static void SetNewSpawnTele(ClientInfo _cInfo)
        {
            if (!GameManager.Instance.adminTools.CommandAllowedFor(_cmd, _cInfo.playerId))
            {
                string _phrase107;
                if (!Phrases.Dict.TryGetValue(107, out _phrase107))
                {
                    _phrase107 = " you do not have permissions to use this command.";
                }
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase107 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                Vector3 _position = _player.GetPosition();
                int x = (int)_position.x;
                int y = (int)_position.y;
                int z = (int)_position.z;
                string _sposition = x + "," + y + "," + z;
                New_Spawn_Tele_Position = _sposition;
                string _phrase525;
                if (!Phrases.Dict.TryGetValue(525, out _phrase525))
                {
                    _phrase525 = " you have set the New Spawn position as {NewSpawnTelePosition}.";
                }
                _phrase525 = _phrase525.Replace("{NewSpawnTelePosition}", New_Spawn_Tele_Position);
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase525 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                LoadConfig.WriteXml();
            }
        }

        public static void TeleNewSpawn(ClientInfo _cInfo)
        {
            string _sql = string.Format("SELECT newSpawn FROM Players WHERE steamid = '{0}'", _cInfo.playerId);
            DataTable _result = SQL.TQuery(_sql);
            bool _newSpawn;
            bool.TryParse(_result.Rows[0].ItemArray.GetValue(0).ToString(), out _newSpawn);
            _result.Dispose();
            if (!_newSpawn)
            {
                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                TelePlayer(_cInfo, _player);
            }
        }

        public static void TelePlayer(ClientInfo _cInfo, EntityPlayer _player)
        {
            if (Return)
            {
                Vector3 Vec3 = _player.position;
                string _position = _player.position.x + "," + _player.position.y + "," + _player.position.z;
                string _sql1 = string.Format("UPDATE Players SET newTeleSpawn = '{0}' WHERE steamid = '{1}'", _position, _cInfo.playerId);
                SQL.FastQuery(_sql1, "NewSpawnTele");
            }
            string[] _cords = New_Spawn_Tele_Position.Split(',');
            int x, y, z;
            int.TryParse(_cords[0], out x);
            int.TryParse(_cords[1], out y);
            int.TryParse(_cords[2], out z);
            _cInfo.SendPackage(new NetPackageTeleportPlayer(new Vector3(x, y, z), null, false));
            string _sql2 = string.Format("UPDATE Players SET newSpawn = 'true' WHERE steamid = '{0}'", _cInfo.playerId);
            SQL.FastQuery(_sql2, "NewSpawnTele");
            if (!Return)
            {
                string _phrase526;
                if (!Phrases.Dict.TryGetValue(526, out _phrase526))
                {
                    _phrase526 = " you have been teleported to the new spawn location.";
                }
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase526 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                string _phrase527;
                if (!Phrases.Dict.TryGetValue(527, out _phrase527))
                {
                    _phrase527 = " type {CommandPrivate}{Command86} when you are prepared to leave. You will teleport back to your spawn location.";
                }
                _phrase527 = _phrase527.Replace("{CommandPrivate}", ChatHook.Command_Private);
                _phrase527 = _phrase527.Replace("{Command86}", Command86);
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase527 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void ReturnPlayer(ClientInfo _cInfo)
        {
            string _sql = string.Format("SELECT newTeleSpawn FROM Players WHERE steamid = '{0}'", _cInfo.playerId);
            DataTable _result = SQL.TQuery(_sql);
            string _pos = _result.Rows[0].ItemArray.GetValue(0).ToString();
            _result.Dispose();
            if (_pos != ("Unknown"))
            {
                string[] _cords = { };
                if (New_Spawn_Tele_Position.Contains(","))
                {
                    _cords = New_Spawn_Tele_Position.Split(',');
                }
                else
                {
                    _cords = New_Spawn_Tele_Position.Split(' ');
                }
                int x, y, z;
                int.TryParse(_cords[0], out x);
                int.TryParse(_cords[2], out z);
                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                if ((x - _player.position.x) * (x - _player.position.x) + (z - _player.position.z) * (z - _player.position.z) <= 50 * 50)
                {
                    string[] _oldCords = _pos.Split(',');
                    int.TryParse(_oldCords[0], out x);
                    int.TryParse(_oldCords[1], out y);
                    int.TryParse(_oldCords[2], out z);
                    _cInfo.SendPackage(new NetPackageTeleportPlayer(new Vector3(x, y, z), null, false));
                    _sql = string.Format("UPDATE Players SET newTeleSpawn = 'Unknown' WHERE steamid = '{0}'", _cInfo.playerId);
                    SQL.FastQuery(_sql, "NewSpawnTele");
                    string _phrase530;
                    if (!Phrases.Dict.TryGetValue(530, out _phrase530))
                    {
                        _phrase530 = " you have been sent back to your original spawn location. Good luck.";
                    }
                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase530 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
                else
                {
                    string _phrase529;
                    if (!Phrases.Dict.TryGetValue(529, out _phrase529))
                    {
                        _phrase529 = " you have left the new player area. Return to it before using {CommandPrivate}{Command86}.";
                    }
                    _phrase529 = _phrase529.Replace("{CommandPrivate}", ChatHook.Command_Private);
                    _phrase529 = _phrase529.Replace("{Command86}", Command86);
                    ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase529 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
            else
            {
                string _phrase528;
                if (!Phrases.Dict.TryGetValue(528, out _phrase528))
                {
                    _phrase528 = " you have no saved return point or you have used it.";
                }
                ChatHook.ChatMessage(_cInfo, ChatHook.Player_Name_Color + _cInfo.playerName  + _phrase528 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }
    }
}