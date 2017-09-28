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

namespace Qarth.Editor
{
    public class AssetFileFilter
    {
        public static bool IsAsset(string fileName)
        {
            if (fileName.EndsWith(".meta") || fileName.EndsWith(".gaf") || fileName.EndsWith(".DS_Store"))
            {
                return false;
            }
            return true;
        }

		public static bool IsAssetBundle(string fileName)
		{
			if (fileName.EndsWith(".bundle"))
			{
				return true;
			}
			return false;
		}

        public static bool IsConfigTable(string fileName)
        {
            if (fileName.EndsWith(".txt"))
            {
                return true;
            }
            return false;
        }
    }
}
