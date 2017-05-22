using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
	public class GuideTriggerFactory : TSingleton<GuideTriggerFactory>
	{
		public delegate ITrigger GuideTriggerCreator();
		private Dictionary<string, GuideTriggerCreator> m_CreatorMap = new Dictionary<string, GuideTriggerCreator>();

		public void RegisterCreator(string name, GuideTriggerCreator creator)
		{
			if (m_CreatorMap.ContainsKey(name))
			{
				Log.w ("Already Add Creator for :" + name);
				return;
			}

			m_CreatorMap.Add (name, creator);
		}


		public ITrigger Create(string name)
		{
			GuideTriggerCreator creator = null;
			if (m_CreatorMap.TryGetValue(name, out creator))
			{
				return creator ();
			}

			return null;
		}
	}
}

