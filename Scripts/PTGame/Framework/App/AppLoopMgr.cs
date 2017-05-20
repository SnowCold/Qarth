using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    [TMonoSingletonAttribute("[App]/AppLoopMgr")]
    public class AppLoopMgr : TMonoSingleton<AppLoopMgr>
    {
        public event Action onUpdate;

        private void Update()
        {
            if (onUpdate != null)
            {
                try
                {
                    onUpdate();
                }
                catch (Exception e)
                {
                    Log.e(e);
                }
            }
        }
    }
}
