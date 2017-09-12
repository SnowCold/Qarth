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
    public class SpritesData : ScriptableObject
    {
        [SerializeField]
        private string m_AtlasName;
        [SerializeField]
        private List<Sprite> m_SpriteList;
        private Dictionary<string, Sprite> m_SpritesMap;

        public string atlasName
        {
            get { return m_AtlasName; }
            set { m_AtlasName = value; }
        }

        public List<Sprite> spritesList
        {
            get { return m_SpriteList; }
        }

        public void AddSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            if (m_SpritesMap == null)
            {
                m_SpritesMap = new Dictionary<string, Sprite>();
                m_SpriteList = new List<Sprite>();
            }

            if (m_SpritesMap.ContainsKey(sprite.name))
            {
                return;
            }

            m_SpritesMap.Add(sprite.name, sprite);
            m_SpriteList.Add(sprite);
        }

        public void ClearAllSprites()
        {
            if (m_SpritesMap != null)
            {
                m_SpritesMap.Clear();
            }

            if (m_SpriteList != null)
            {
                m_SpriteList.Clear();
            }
        }

        private void BuildMap()
        {
            m_SpritesMap = new Dictionary<string, Sprite>();

            for (int i = m_SpriteList.Count - 1; i >= 0; --i)
            {
                if (m_SpriteList[i] != null)
                {
                    m_SpritesMap.Add(m_SpriteList[i].name, m_SpriteList[i]);
                }
            }
        }

        public Sprite Find(string spriteName)
        {
            if (spriteName == null)
            {
                return null;
            }

            if (m_SpriteList == null)
            {
                return null;
            }

            if (m_SpritesMap == null)
            {
                BuildMap();
            }

            Sprite result = null;
            if (m_SpritesMap.TryGetValue(spriteName, out result))
            {
                return result;
            }
            return null;
        }
    }
}
