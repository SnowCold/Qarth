using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTGame.Framework;

namespace PTGame.PaiLogic
{
    public class GuideFactory
    {
        public static GuideCommand CreateCommand(TDGuideCommand command)
        {

            return null;
        }

        public static Guide CreateGuide(TDGuide data)
        {
            if (data == null)
            {
                return null;
            }

            List<TDGuideCommand> commandList = TDGuideCommandTable.GetCommandByGuideID(data.id);
            if (commandList == null || commandList.Count == 0)
            {
                return null;
            }

            CommandSequence sequence = new CommandSequence();
            int preGroupID = -1;

            for (int i = 0; i < commandList.Count; ++i)
            {
                TDGuideCommand commandData = commandList[i];

                GuideCommand command = CreateCommand(commandData);

                if (command == null)
                {
                    continue;
                }

                if (commandData.groupId == preGroupID)
                {
                    sequence.Join(command);
                }
                else
                {
                    sequence.Append(command);
                    preGroupID = commandData.groupId;
                }
            }

            return null;
        }

    }
}
