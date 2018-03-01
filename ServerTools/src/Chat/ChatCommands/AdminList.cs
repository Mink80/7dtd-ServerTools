﻿using System.Collections.Generic;

namespace ServerTools
{
    class AdminList
    {
        public static bool IsEnabled = false;
        public static int Admin_Level = 0;
        public static int Mod_Level = 1;
        private static List<string> Admins = new List<string>();
        private static List<string> Mods = new List<string>();

        public static void List(ClientInfo _cInfo, bool _announce, string _playerName)
        {
            Admins.Clear();
            Mods.Clear();
            List<ClientInfo> _cInfoList = ConnectionManager.Instance.GetClients();
            foreach (var _cInfoAdmins in _cInfoList)
            {
                if (!AdminChatColor.AdminColorOff.Contains(_cInfoAdmins.playerId))
                {
                    GameManager.Instance.adminTools.IsAdmin(_cInfoAdmins.playerId);
                    AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfoAdmins.playerId);
                    if (Admin.PermissionLevel <= Admin_Level)
                    {
                        Admins.Add(_cInfoAdmins.playerName);
                    }
                    if (Admin.PermissionLevel > Admin_Level & Admin.PermissionLevel <= Mod_Level)
                    {
                        Mods.Add(_cInfoAdmins.playerName);
                    }
                }
            }
            Response(_cInfo, _announce, _playerName);
        }

        public static void Response(ClientInfo _cInfo, bool _announce, string _playerName)
        {
            string _adminList = string.Join(", ", Admins.ToArray());
            string _modList = string.Join(", ", Mods.ToArray());
            if (_announce)
            {
                GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}Server admins in game: [FF8000]{1}[-]", Config.Chat_Response_Color, _adminList), "Server", false, "", false);
                GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}Server mods in game: [FF8000]{1}[-]", Config.Chat_Response_Color, _modList), "Server", false, "", false);
            }
            else
            {
                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Server admins in game: [FF8000]{1}[-]", Config.Chat_Response_Color, _adminList), "Server", false, "", false));
                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Server mods in game: [FF8000]{1}[-]", Config.Chat_Response_Color, _modList), "Server", false, "", false));
            }
        }
    }
}
