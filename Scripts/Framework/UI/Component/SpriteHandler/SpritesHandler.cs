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
    public class SpritesHandler
    {
        private SpritesData[] m_SpritesData;

        public void SetData(SpritesData[] data)
        {
            m_SpritesData = data;
        }

        public Sprite FindSprite(string spriteName)
        {
            if (m_SpritesData == null || m_SpritesData.Length == 0)
            {
                //Log.w("Nor Find SpriteName:" + spriteName);
                return null;
            }

            Sprite result = null;
            for (int i = m_SpritesData.Length - 1; i >= 0; --i)
            {
                if (m_SpritesData[i] == null)
                {
                    continue;
                }

                result = m_SpritesData[i].Find(spriteName);
                if (result != null)
                {
                    return result;
                }
            }
            //Log.w("Nor Find SpriteName:" + spriteName);
            return null;
        }
    }
}
