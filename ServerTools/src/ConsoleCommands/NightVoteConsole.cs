﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace ServerTools
{
    class NightVoteConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools]- Enable or Disable Night Vote.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. NightVote off\n" +
                   "  2. NightVote on\n" +
                   "1. Turn off night vote\n" +
                   "2. Turn on night vote\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-NightVote", "nightvote" };
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
                    NightVote.IsEnabled = false;
                    LoadConfig.WriteXml();
                    SdtdConsole.Instance.Output(string.Format("Night vote has been set to off"));
                    return;
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    NightVote.IsEnabled = true;
                    LoadConfig.WriteXml();
                    SdtdConsole.Instance.Output(string.Format("Night vote has been set to on"));
                    return;
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("Invalid argument {0}.", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in NightVoteConsole.Run: {0}.", e));
            }
        }
    }
}