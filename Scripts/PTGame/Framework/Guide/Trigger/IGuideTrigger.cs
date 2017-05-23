using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public interface ITrigger
    {
        bool isReady { get; }
		void SetParam(string[] param);
        void Start(Action<bool, ITrigger> l);
        void Stop();
    }
}
