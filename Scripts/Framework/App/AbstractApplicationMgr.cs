//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public class AbstractApplicationMgr<T> : TMonoSingleton<T> where T : TMonoSingleton<T>
    {
        public static Action s_OnApplicationQuit = null;
        public static Action<bool> s_OnApplicationPause = null;
        public static Action<bool> s_OnApplicationFocus = null;
        public static Action s_OnApplicationUpdate = null;
        public static Action s_OnApplicationOnGUI = null;

        protected void Start()
        {
            StartApp();
        }

        protected void StartApp()
        {
            InitThirdLibConfig();
            InitAppEnvironment();
            StartGame();
        }

        #region 子类实现

        protected virtual void InitThirdLibConfig()
        {

        }

        protected virtual void InitAppEnvironment()
        {

        }

        protected virtual void StartGame()
        {

        }

        #endregion

        void OnApplicationQuit()
        {
            MonoSingleton.isApplicationQuit = true;

            if (s_OnApplicationQuit != null)
            {
                try
                {
                    s_OnApplicationQuit();
                }
                catch (Exception e)
                {
                    Log.e(e);
                }
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (s_OnApplicationPause != null)
            {
                try
                {
                    s_OnApplicationPause(pauseStatus);
                }
                catch (Exception e)
                {
                    Log.e(e);
                }
            }
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (s_OnApplicationFocus != null)
            {
                try
                {
                    s_OnApplicationFocus(focusStatus);
                }
                catch (Exception e)
                {
                    Log.e(e);
                }
            }
        }

        void Update()
        {
            if (s_OnApplicationUpdate != null)
            {
                s_OnApplicationUpdate();
            }
        }

        void OnGUI()
        {
            if (s_OnApplicationOnGUI != null)
            {
                s_OnApplicationOnGUI();
            } 
        }
    }
}
