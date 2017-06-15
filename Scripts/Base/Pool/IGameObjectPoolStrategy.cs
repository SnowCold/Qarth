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
    public interface IGameObjectPoolStrategy
    {
        void ProcessContainer(GameObject container);
        void OnAllocate(GameObject result);

        void OnRecycle(GameObject result);
    }

    public class DefaultPoolStrategy : TSingleton<DefaultPoolStrategy>, IGameObjectPoolStrategy
    {
        public void ProcessContainer(GameObject container)
        {
            container.SetActive(false);
        }

        public void OnAllocate(GameObject result)
        {

        }

        public void OnRecycle(GameObject result)
        {

        }
    }

    public class UIPoolStrategy : TSingleton<UIPoolStrategy>, IGameObjectPoolStrategy
    {
        public void ProcessContainer(GameObject container)
        {
            UITools.SetGameObjectLayer(container, LayerDefine.LAYER_HIDE_UI);
        }

        public void OnAllocate(GameObject result)
        {
            UITools.SetGameObjectLayer(result, LayerDefine.LAYER_UI);
        }

        public void OnRecycle(GameObject result)
        {
            UITools.SetGameObjectLayer(result, LayerDefine.LAYER_HIDE_UI);
        }
    }
}
