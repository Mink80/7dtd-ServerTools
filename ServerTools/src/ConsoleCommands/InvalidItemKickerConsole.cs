﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace ServerTools
{
    class InvalidItemKickerConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools]- Enable or Disable Invalid Item Kicker.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. InvalidItemKicker off\n" +
                   "  2. InvalidItemKicker on\n" +
                   "1. Turn off invalid item kicker\n" +
                   "2. Turn on invalid item kicker\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-InvalidItemKicker", "invaliditemkicker" };
        }
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 1, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    InventoryCheck.IsEnabled = false;
                    LoadConfig.WriteXml();
                    SdtdConsole.Instance.Output(string.Format("Invalid Item Kicker has been set to off"));
                    return;
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    InventoryCheck.IsEnabled = true;
                    LoadConfig.WriteXml();
                    SdtdConsole.Instance.Output(string.Format("Invalid Item Kicker has been set to on"));
                    return;
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("Invalid argument {0}.", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in InvalidItemKickerConsole.Run: {0}.", e));
            }
        }
    }
}