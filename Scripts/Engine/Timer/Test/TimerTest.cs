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
    public class TimerTest : MonoBehaviour
    {
        //private TimeItem m_RepeatTimeItem;

        private void Start()
        {
            Log.i(DateTime.Now);
            //m_RepeatTimeItem = Timer.S.Post2Really(OnTimeTick, 1, -1);
            //DateTime time = DateTime.Now;
            //time = time.AddSeconds(5);
            //Timer.S.Post2Really(OnDateTimeTick, time);

            //Time.timeScale = 0.5f;
            //Timer.S.Post2Scale(OnScaleTimeTick, 1, -1);
        }

        private void OnTimeTick(int tick)
        {
            Log.i("TickTick:" + DateTime.Now);
        }

        private void OnDateTimeTick(int tick)
        {
            Log.i("DateTimeTick:" + tick);
        }

        private void OnScaleTimeTick(int tick)
        {
            Log.i("ScaleTickTick:" + tick);
        }
    }
}
