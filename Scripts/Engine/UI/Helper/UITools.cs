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
using UnityEngine.UI;

namespace Qarth
{
    public class UITools
    {
        public static void SetCanvasState(GameObject root, bool state)
        {
            Canvas[] childs = root.GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].enabled = state;
            }
        }

        public static void SetGraphicRaycasterState(GameObject root, bool state)
        {
            GraphicRaycaster[] childs = root.GetComponentsInChildren<GraphicRaycaster>(true);
            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].enabled = state;
            }
        }

        public static void SetGameObjectLayer(GameObject root, int layer)
        {
            Transform[] childs = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].gameObject.layer = layer;
            }
        }
    }
}
