using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{

	public class RuntimeParamFactory : TSingleton<RuntimeParamFactory>
	{
		public delegate IRuntimeParam RuntimeParamCreator();
		private Dictionary<string, RuntimeParamCreator> m_CreatorMap = new Dictionary<string, RuntimeParamCreator>();

		public void RegisterCreator(string name, RuntimeParamCreator creator)
		{
			if (m_CreatorMap.ContainsKey(name))
			{
				Log.w ("Already Add Creator for :" + name);
				return;
			}

			m_CreatorMap.Add (name, creator);
		}


		public IRuntimeParam Create(string name)
		{
			RuntimeParamCreator creator = null;
			if (m_CreatorMap.TryGetValue(name, out creator))
			{
				return creator ();
			}

			return null;
		}
	}
}

