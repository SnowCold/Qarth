using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{

	public class CommandSequence : CommandNode
	{
		private CommandNode		m_FirstCommand;
		private CommandNode		m_CurrentCommand;
		private CommandNode		m_PreCommand;

		private CommandNode		m_RuntimeCurrentNode;

		public void Append(CommandNode command)
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
		}

		public void Join(CommandNode command)
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
					group = new CommandGroup();
					group.SetCommandNodeEventListener(OnSubCommondComplate);
					if(m_PreCommand != null)
					{
						m_PreCommand.nextCommand = group;
					}
					m_CurrentCommand.SetCommandNodeEventListener(null);
					group.Add(m_CurrentCommand);
					m_CurrentCommand = group;
				}
			}

			group.Add(command);
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