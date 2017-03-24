using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
	//命令模式:后面可以优化下组织形式，全部通过Event来实现的话可能效率上有点低
	public class CommandNode
	{
		public delegate void CommandNodeEvent(CommandNode commond);

		private CommandNodeEvent m_OnCommandComplate;

		private CommandNode	m_NextCommand;
		
		public CommandNode nextCommand
		{
			get { return m_NextCommand; }
			set { m_NextCommand = value; }
		}

        public void SetCommandNodeEventListener(CommandNodeEvent c)
        {
            m_OnCommandComplate = c;
        }

        public virtual void Start()
		{

		}

        protected void FinishCommand()
        {
            if (m_OnCommandComplate != null)
            {
                m_OnCommandComplate(this);
            }
        }

    }
}


















