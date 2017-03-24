using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class AbstractApplicationMgr<T> : TMonoSingleton<T> where T : TMonoSingleton<T>
    {
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
    }
}
