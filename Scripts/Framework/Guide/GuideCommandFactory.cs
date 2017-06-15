//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
	public class GuideCommandFactory : TSingleton<GuideCommandFactory>
    {
		public delegate AbstractGuideCommand GuideCommandCreator();
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


		public AbstractGuideCommand Create(string name)
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
