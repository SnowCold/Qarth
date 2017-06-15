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
    [TMonoSingletonAttribute("[Singleton]/Mgr")]
    public class WorldMgr : TMonoSingleton<WorldMgr>
    {
        private WorldRoot m_WorldRoot;

        public WorldRoot worldRoot
        {
            get { return m_WorldRoot; }
        }

        public override void OnSingletonInit()
        {
            if (m_WorldRoot == null)
            {
                WorldRoot root = GameObject.FindObjectOfType<WorldRoot>();
                if (root == null)
                {
                    Log.e("Failed to Find WorldRoot!");
                    return;
                }

                m_WorldRoot = root;
            }
        }
    }
}
