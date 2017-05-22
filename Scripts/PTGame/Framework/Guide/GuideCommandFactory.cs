using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
	public class GuideCommandFactory : TSingleton<GuideCommandFactory>
    {
		public delegate GuideCommand GuideCommandCreator();
		private Dictionary<string, GuideCommandCreator> m_CreatorMap = new Dictionary<string, GuideCommandCreator>();

		public void RegisterCreator(string name, GuideCommandCreator creator)
		{
			if (m_CreatorMap.ContainsKey(name))
			{
				Log.w ("Already Add Creator for :" + name);
				return;
			}

			m_CreatorMap.Add (name, creator);
		}


		public GuideCommand Create(string name)
		{
			GuideCommandCreator creator = null;
			if (m_CreatorMap.TryGetValue(name, out creator))
			{
				return creator ();
			}

			return null;
		}
    }
}
