//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{

	public class CommandSequence : CommandNode
	{
		private CommandNode		m_FirstCommand;
		private CommandNode		m_CurrentCommand;
		private CommandNode		m_PreCommand;

		private CommandNode		m_RuntimeCurrentNode;

		public CommandSequence Append(CommandNode command)
		{
			if(m_CurrentCommand == null)
			{
				m_CurrentCommand = command;
				m_FirstCommand = m_CurrentCommand;
			}
			else
			{
				m_CurrentCommand.nextCommand = command;
				m_PreCommand = m_CurrentCommand;
				m_CurrentCommand = command;
			}
			m_CurrentCommand.SetCommandNodeEventListener(OnSubCommondComplate);

            return this;
		}

		public CommandSequence Join(CommandNode command)
		{
			CommandGroup group;
			if(m_CurrentCommand == null)
			{
				group = new CommandGroup();
				m_CurrentCommand.SetCommandNodeEventListener(OnSubCommondComplate);
				m_CurrentCommand = group;
				m_FirstCommand = m_CurrentCommand;
			}
			else
			{
				if(m_CurrentCommand.GetType() == typeof(CommandGroup))
				{
					group = m_CurrentCommand as CommandGroup;
				}
				else
				{
                    if (m_FirstCommand == m_CurrentCommand)
                    {
                        m_FirstCommand = null;
                    }

					group = new CommandGroup();
					group.SetCommandNodeEventListener(OnSubCommondComplate);
					if(m_PreCommand != null)
					{
						m_PreCommand.nextCommand = group;
					}
					m_CurrentCommand.SetCommandNodeEventListener(null);
					group.Add(m_CurrentCommand);
					m_CurrentCommand = group;

                    if (m_FirstCommand == null)
                    {
                        m_FirstCommand = m_CurrentCommand;
                    }
				}
			}

			group.Add(command);

            return this;
		}

		public override void Start()
		{
			if(m_FirstCommand != null)
			{
				m_RuntimeCurrentNode = m_FirstCommand;
				m_RuntimeCurrentNode.Start();
			}
			else
			{
				FinishCommand();
			}
		}

		private void OnSubCommondComplate(CommandNode command)
		{
			m_RuntimeCurrentNode = command.nextCommand;
			if(m_RuntimeCurrentNode == null)
			{
				FinishCommand();
			}
			else
			{
				m_RuntimeCurrentNode.Start();
			}

		}

	}

}
