using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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
